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

        // Собрать все переменные
        var allVars = new List<(string name, string type, bool isArray, int arraySize, bool isPointer, string? pointedType)>();
        foreach (var param in func.Parameters)
            allVars.Add((param.Name, param.Type, false, 0, param.IsPointer, param.PointedType));

        foreach (var kv in localVarNodes)
        {
            var varDecl = kv.Value;
            if (!allVars.Any(v => v.name == kv.Key))
                allVars.Add((kv.Key, varDecl.Type, varDecl.IsArray, varDecl.ArraySize, varDecl.IsPointer, varDecl.PointedType));
        }

        // Распределение регистров и стека
        var varMap = new Dictionary<string, VarLocation>();
        int nextReg = 4;
        int stackOffset = 0;

        foreach (var (name, type, isArray, arraySize, isPointer, pointedType) in allVars)
        {
            StructLayout layout = null!;
            bool isStruct = !isPointer && structTable.TryGetValue(type, out layout!);
            int varSize;
            OpCodeSize size = OpCodeSize.S64;

            if (isPointer)
            {
                varSize = 8;
                size = OpCodeSize.S64;
            }
            else if (isStruct)
            {
                varSize = layout.Size;
            }
            else
            {
                // Обычные типы (int, char, …)
                size = CodeGenUtils.GetSizeForType(type);
                if (isArray)
                {
                    varSize = CodeGenUtils.GetSizeInBytes(size) * arraySize;
                    varSize = (varSize + 7) & ~7;   // выравнивание
                }
                else
                {
                    varSize = 8; // скалярное значение на стеке/регистре занимает 8 байт (выравнивание)
                }
            }

            int myOffset = stackOffset;
            stackOffset += varSize;

            if (isArray && structTable.TryGetValue(type, out StructLayout? layoutArr))
            {
                varSize = layoutArr.Size * arraySize;
                varSize = (varSize + 7) & ~7;
            }

            // Выделение регистра или стека
            if (nextReg <= 21 && !isArray && !isStruct)
            {
                // Только для простых типов (не массивы и не структуры)
                varMap[name] = new VarLocation
                {
                    IsRegister = true,
                    Register = (RegType)nextReg,
                    TypeSize = size,
                    IsArray = false,
                    ArraySize = 0,
                    IsPointer = isPointer,
                    PointedType = pointedType
                };
                nextReg++;
            }
            else if (isStruct)
            {
                varMap[name] = new VarLocation
                {
                    IsRegister = false,
                    StackOffset = myOffset,
                    TypeSize = OpCodeSize.S64,
                    StructTypeName = type,
                    IsArray = false,
                    IsPointer = false
                };
            }
            else
            {
                varMap[name] = new VarLocation
                {
                    IsRegister = false,
                    StackOffset = myOffset,
                    TypeSize = isPointer ? OpCodeSize.S64 : size,
                    StructTypeName = null,
                    IsArray = isArray,
                    ArraySize = arraySize,
                    IsPointer = isPointer,
                    PointedType = pointedType
                };
            }
        }
        ctx.VarMap = varMap;
        ctx.TotalLocalSize = stackOffset;
        ctx.UsedRegisters = [.. varMap.Values.Where(v => v.IsRegister).Select(v => v.Register)];
        return ctx;
    }
}
