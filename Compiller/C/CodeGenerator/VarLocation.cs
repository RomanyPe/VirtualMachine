using static Kernel.ProcessorSystem.Processor;

namespace Compiller.C.CodeGenerator;

// ============================================================
// 4. ГЕНЕРАТОР КОДА
// ============================================================

public class VarLocation
{
    public bool IsRegister;
    public RegType Register;
    public int StackOffset;
    public OpCodeSize TypeSize;
    public bool IsArray;
    public int ArraySize;
    public bool IsPointer;
    public string? PointedType;
}
