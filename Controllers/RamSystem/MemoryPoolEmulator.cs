using Kernel.Common;
using System.Collections.Concurrent;

namespace Kernel.RamSystem;

public static class MemoryPoolEmulator
{
    private static readonly ConcurrentDictionary<RamSize, ConcurrentBag<byte[]>> _pools = new();

    public static byte[] Rent(RamSize size)
    {
        var bag = _pools.GetOrAdd(size, _ => []);
        if (bag.TryTake(out byte[]? array) && array != null)
            return array;
        return new byte[(ulong)size];
    }

    public static void Return(byte[] array, RamSize size)
    {
        if (array == null) return;
        Array.Clear(array, 0, array.Length); // очищаем перед возвратом
        var bag = _pools.GetOrAdd(size, _ => []);
        bag.Add(array);
    }
}
