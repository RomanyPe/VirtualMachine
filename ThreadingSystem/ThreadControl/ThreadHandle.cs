using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadPool;

namespace ThreadingSystem.ThreadControl;

public readonly struct ThreadHandle(TaskData slot, ManualResetEventSlimPool pool) : IDisposable
{
    private readonly TaskData _slot = slot;
    private readonly ManualResetEventSlimPool _pool = pool;

    public bool Wait(TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(1);
        return _slot.DoneEvent.Wait(timeout.Value);
    }
    public void Wait() => _slot.DoneEvent.Wait();

    public void Dispose() => _pool.Return(_slot.DoneEvent);
}
