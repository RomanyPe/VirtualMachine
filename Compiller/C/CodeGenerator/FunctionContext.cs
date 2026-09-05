using Compiller.ASM;
using Compiller.ASM.Optimizators;
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// FunctionContext – контекст текущей функции
// ============================================================
public class FunctionContext
{
    public Dictionary<string, VarLocation> VarMap { get; private set; } = null!;
    public int TotalLocalSize { get; private set; }
    public string FunctionName { get; private set; } = null!;
    public string EpilogueLabel { get; private set; } = null!;
    public List<RegType> UsedRegisters { get; private set; } = [];


    public static FunctionContext Create(FunctionNode func, Dictionary<string, VariableNode> localVarNodes, Dictionary<string, StructLayout> structTable)
    {
        var ctx = new FunctionContext
        {
            FunctionName = func.Name,
            EpilogueLabel = $"__epilogue_{func.Name}"
        };

        var allVars = new List<(string name, string type, bool isArray, int arraySize, bool isPointer, string? pointedType)>();
        foreach (var param in func.Parameters)
            allVars.Add((param.Name, param.Type, false, 0, param.IsPointer, param.PointedType));

        foreach (var kv in localVarNodes)
        {
            var varDecl = kv.Value;
            if (!allVars.Any(v => v.name == kv.Key))
                allVars.Add((kv.Key, varDecl.Type, varDecl.IsArray, varDecl.ArraySize, varDecl.IsPointer, varDecl.PointedType));
        }
        // Создаём словарь всех переменных функции (параметры + локальные)
        var allVarNodes = new Dictionary<string, VariableNode>();

        foreach (var param in func.Parameters)
        {
            allVarNodes[param.Name] = new VariableNode(param.Type, param.Name)
            {
                IsPointer = param.IsPointer,
                PointedType = param.PointedType,
                IsArray = false,
                ArraySize = 0
            };
        }

        foreach (var kv in localVarNodes)
        {
            if (!allVarNodes.ContainsKey(kv.Key))
                allVarNodes[kv.Key] = kv.Value;
        }

        RegisterAllocationResult allocation = RegisterAllocator.AllocateRegisters(func, allVarNodes, structTable);
        var varMap = new Dictionary<string, VarLocation>();
        int stackOffset = 0;

        foreach (var (name, type, isArray, arraySize, isPointer, pointedType) in allVars)
        {
            StructLayout? layout = null;
            bool isStruct = !isPointer && structTable.TryGetValue(type, out layout);
            OpCodeSize size = OpCodeSize.S64;
            int varSize;

            // Определяем размер переменной (для стекового размещения)
            if (isPointer)
            {
                varSize = 8;
                size = OpCodeSize.S64;
            }
            else if (isStruct)
            {
                varSize = layout!.Size;
            }
            else
            {
                size = CodeGenUtils.GetSizeForType(type);
                varSize = isArray ? CodeGenUtils.GetSizeInBytes(size) * arraySize : 8;
            }

            // Для массива структур пересчитываем размер
            if (isArray && isStruct)
            {
                varSize = layout!.Size * arraySize;
            }
            varSize = (varSize + 7) & ~7; // выравнивание на 8

            // Может ли переменная находиться в регистре?
            bool canUseRegister = !isArray && !structTable.ContainsKey(type) && !isPointer;

            if (canUseRegister && allocation.VarToRegister.TryGetValue(name, out var reg) && reg.HasValue)
            {
                varMap[name] = new VarLocation
                {
                    IsRegister = true,
                    Register = reg.Value,
                    TypeSize = CodeGenUtils.GetSizeForType(type),
                    IsArray = false,
                    ArraySize = 0,
                    IsPointer = isPointer,
                    PointedType = pointedType
                };
            }
            else
            {
                varMap[name] = new VarLocation
                {
                    IsRegister = false,
                    StackOffset = stackOffset,
                    TypeSize = size,
                    StructTypeName = structTable.ContainsKey(type) ? type : null,
                    IsArray = isArray,
                    ArraySize = arraySize,
                    IsPointer = isPointer,
                    PointedType = pointedType
                };
                stackOffset += varSize;
            }
        }

        ctx.VarMap = varMap;
        ctx.TotalLocalSize = stackOffset;   // теперь только для стековых переменных
        ctx.UsedRegisters = [.. varMap.Values.Where(v => v.IsRegister).Select(v => v.Register)];
        return ctx;
    }
}


public class VariableAccessor(AssemblerBase asm, FunctionContext funcCtx, GlobalMemoryManager globalMem)
{
    private readonly AssemblerBase _asm = asm;
    private readonly FunctionContext _funcCtx = funcCtx;
    private readonly GlobalMemoryManager _globalMem = globalMem;

    /// <summary>
    /// Загружает значение переменной в указанный регистр.
    /// Если переменная уже в регистре, делает MOV; иначе загружает из стека/глобальной памяти.
    /// </summary>
    public void LoadToRegister(string varName, RegType targetReg)
    {
        if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
            {
                if (loc.Register != targetReg)
                    _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)targetReg, (uint)loc.Register));
            }
            else
            {
                // Загрузка из стека
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)targetReg, CodeGenUtils.TMP_REG, (uint)loc.TypeSize));
            }
        }
        else if (_globalMem.TryGetAddress(varName, out var addr))
        {
            var gInfo = _globalMem.GetInfo(varName)!.Value;
            var size = CodeGenUtils.GetSizeForType(gInfo.Type);
            _asm.EmitInstruction64(InstructionEncoder.EncodeLOAD((uint)targetReg, (uint)size), addr);
        }
        else
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, varName);
    }

    /// <summary>
    /// Сохраняет значение из регистра в переменную.
    /// </summary>
    public void StoreFromRegister(string varName, RegType sourceReg)
    {
        // Аналогично LoadToRegister, но с сохранением
    }

    /// <summary>
    /// Загружает адрес переменной в r0 (для операций с указателями).
    /// </summary>
    public void LoadAddressToR0(string varName)
    {
        // ...
    }
}