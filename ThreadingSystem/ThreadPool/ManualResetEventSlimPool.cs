using Microsoft.Extensions.ObjectPool;

namespace ThreadingSystem.ThreadPool;

public class ManualResetEventSlimPool
{
    private readonly ObjectPool<ManualResetEventSlim> _pool;

    public ManualResetEventSlimPool(int maximumRetained = 100)
    {
        var policy = new ManualResetEventSlimPooledObjectPolicy();

        var provider = new DefaultObjectPoolProvider
        {
            MaximumRetained = maximumRetained
        };

        _pool = provider.Create(policy);
    }

    public ManualResetEventSlim Get() => _pool.Get();

    public void Return(ManualResetEventSlim obj) => _pool.Return(obj);
}
