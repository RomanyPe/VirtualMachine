namespace Compiller.C.CodeGenerator;

// ============================================================
// GlobalMemoryManager – управление глобальной памятью
// ============================================================
public class GlobalMemoryManager(Dictionary<string, StructLayout> structTable)
{
    private readonly Dictionary<string, GlobalInfo> _globalAddresses = [];
    private ulong _nextGlobalAddress = 0x1000;
    private readonly Dictionary<string, StructLayout> _structTable = structTable; // <-- добавить

    public IReadOnlyDictionary<string, GlobalInfo> Addresses => _globalAddresses;

    public GlobalInfo Allocate(string name, string type, bool isArray = false, bool isPointer = false, string? pointedType = null, int arraySize = 0)
    {
        int size;
        string realType = type;
        if (CodeGenUtils.IsStructType(type, _structTable))
        {
            size = _structTable[type].Size;
            realType = type; // сохраняем имя структуры
        }
        else if (isPointer)
            size = 8;
        else
            size = CodeGenUtils.GetSizeInBytes(CodeGenUtils.GetSizeForType(type));

        if (isArray)
            size *= arraySize;

        var info = new GlobalInfo(_nextGlobalAddress, realType, isArray, isPointer, pointedType);
        _globalAddresses[name] = info;
        _nextGlobalAddress += (ulong)size;
        _nextGlobalAddress = (_nextGlobalAddress + 7) & ~7UL;
        return info;
    }
    public void AllocatePseudoGlobals(FunctionNode func)
    {
        foreach (var param in func.Parameters)
        {
            string globalName = $"__param_{func.Name}_{param.Name}";
            Allocate(globalName, param.Type);
        }
    }

    public void AllocateLocalGlobals(BlockNode block, string funcName)
    {
        foreach (var stmt in block.Statements)
        {
            if (stmt is VariableNode var)
            {
                string globalName = $"__local_{funcName}_{var.Name}";
                Allocate(globalName, var.Type, var.IsArray, var.IsPointer, var.PointedType, var.ArraySize);
            }
            else if (stmt is BlockNode nested)
                AllocateLocalGlobals(nested, funcName);
        }
    }

    public bool TryGetAddress(string name, out ulong address)
    {
        if (_globalAddresses.TryGetValue(name, out var info))
        {
            address = info.Address;
            return true;
        }
        address = 0;
        return false;
    }

    public GlobalInfo? GetInfo(string name)
    {
        return _globalAddresses.TryGetValue(name, out var info) ? info : null;
    }

    public bool Contains(string name) => _globalAddresses.ContainsKey(name);
}
