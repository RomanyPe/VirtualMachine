using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Kernel.RamSystem;

internal static class MemoryBusHelpers
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RAMResultInt16 GenerateInt16Le(ReadOnlySpan<byte> span)
    {
        return new RAMResultInt16(BinaryPrimitives.ReadUInt16LittleEndian(span));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RAMResultInt32 GenerateInt32Le(ReadOnlySpan<byte> span)
    {
        return new RAMResultInt32(BinaryPrimitives.ReadUInt32LittleEndian(span));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RAMResultInt64 GenerateInt64Le(ReadOnlySpan<byte> span)
    {
        return new RAMResultInt64(BinaryPrimitives.ReadUInt64LittleEndian(span));
    }
}