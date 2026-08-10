using Kernel.Common;
using Kernel.ControllersData;
using Kernel.RamSystem;
using System.Collections.Concurrent;

namespace Kernel.ProcessorSystem;

public static class ProcessorPoolEmulator
{
    private const int MaxPoolSize = 32;
    private static readonly ConcurrentQueue<Processor> _poolProcessors = [];

    public static Processor Rent(MemoryBus ram, NameDeviceToken nameDeviceToken, PortBus portBus, Lock regLock, ReadOnlySpan<char> name)
    {
        if (_poolProcessors.TryDequeue(out Processor? processor))
        {
            if (processor != null && processor.TryInitInPool(ram, nameDeviceToken, portBus, regLock, name))
                return processor;
        }

        return new Processor(ram, nameDeviceToken, portBus, regLock, name);
    }

    public static void Return(Processor proc)
    {
        if (proc == null) return;

        proc.Reset();
        if (_poolProcessors.Count < MaxPoolSize)
            _poolProcessors.Enqueue(proc);
    }
}
