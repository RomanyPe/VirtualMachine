using Kernel.Common;
using System.Collections.Concurrent;

namespace Kernel.RamSystem;

public static class NativeMemoryPool
{
    private static readonly ConcurrentDictionary<RamSize, ConcurrentBag<NativeMemoryBuffer>> _pools = new();

    public static NativeMemoryBuffer Rent(RamSize size)
    {
        var bag = _pools.GetOrAdd(size, _ => []);
        if (bag.TryTake(out var buffer))
            return buffer;
        return new NativeMemoryBuffer(size);
    }

    public static void Return(NativeMemoryBuffer buffer)
    {
        if (buffer == null) return;
        // опционально: очистить память
        buffer.AsSpan().Clear();
        var bag = _pools.GetOrAdd((RamSize)buffer.Length, _ => []);
        bag.Add(buffer);
    }
}
