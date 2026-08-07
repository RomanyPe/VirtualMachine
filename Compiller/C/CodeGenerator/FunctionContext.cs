using static Kernel.ProcessorSystem.Processor;

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


    public static FunctionContext Create(FunctionNode func, Dictionary<string, VariableNode> localVarNodes)
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
            var size = CodeGenUtils.GetSizeForType(type);
            int varSize;

            if (isArray)
            {
                varSize = CodeGenUtils.GetSizeInBytes(size) * arraySize;
                varSize = (varSize + 7) & ~7;   // выравниваем размер массива до 8
            }
            else
            {
                varSize = 8;                     // скаляр/указатель
            }

            int myOffset = stackOffset;          // начало текущей переменной
            stackOffset += varSize;              // сдвигаем для следующей

            if (nextReg <= 22 && !isArray)
            {
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
            else
            {
                varMap[name] = new VarLocation
                {
                    IsRegister = false,
                    StackOffset = myOffset,        // ← ВОТ ЗДЕСЬ правильное смещение
                    TypeSize = isPointer ? OpCodeSize.S64 : size,
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
