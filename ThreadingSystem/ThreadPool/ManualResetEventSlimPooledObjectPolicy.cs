using Microsoft.Extensions.ObjectPool;

namespace ThreadingSystem.ThreadPool;

public class ManualResetEventSlimPooledObjectPolicy : IPooledObjectPolicy<ManualResetEventSlim>
{
    public ManualResetEventSlim Create()
    {
        return new ManualResetEventSlim(initialState: false);
    }

    public bool Return(ManualResetEventSlim obj)
    {
        if (obj == null) return false;

        obj.Set();
        obj.Reset();

        return true;
    }
}
