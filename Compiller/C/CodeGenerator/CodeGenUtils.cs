using Compiller.ASM;
using static Kernel.ProcessorSystem.Processor;

namespace Compiller.C.CodeGenerator;

// ============================================================
// CodeGenUtils – статические утилиты
// ============================================================

// ============================================================
// CodeGenUtils – статические утилиты
// ============================================================
public static class CodeGenUtils
{
    public const uint TMP_REG = (uint)RegType.r2;

    public static OpCodeSize GetSizeForType(string? type) => type switch
    {
        "byte" or "char" => OpCodeSize.S8,
        "ushort" => OpCodeSize.S16,
        "ulong" => OpCodeSize.S64,
        "int"  => OpCodeSize.S32,
        _ => throw new Exception($"Unknown type '{type}' for memory size")
    };

    public static int GetSizeInBytes(OpCodeSize size) => size switch
    {
        OpCodeSize.S8 => 1,
        OpCodeSize.S16 => 2,
        OpCodeSize.S32 => 4,
        OpCodeSize.S64 => 8,
        _ => throw new Exception("Unknown size")
    };

    public static void EmitMultiplyByConstant(Assembler asm, uint reg, int multiplier)
    {
        for (int i = 1; i < multiplier; i *= 2)
        {
            asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.ADD, reg, reg));
        }
    }

    public static bool IsComparisonOperator(string op) => op is "==" or "!=" or "<" or ">" or "<=" or ">=";
}
