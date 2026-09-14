using System.Collections.Concurrent;
using System.Text;
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadControl;
using ThreadingSystem.ThreadMetrics;

namespace ThreadingSystem;

public class ThreadScheduler
{
    private class ActionTask(Action action, TaskPriority priority, CancellationToken token) : ITask
    {
        private readonly Action _action = action;
        public TaskPriority Priority { get; } = priority;
        public CancellationToken Token { get; } = token;

        public void Execute() => _action();
    }

    private readonly MonitoringThreader[] _workers;
    private readonly ContextScheduler _contextScheduler;
    private readonly int _workerCount;
    public bool IsStopping => _contextScheduler.IsStopping;
    public bool IsPaused => _contextScheduler.IsPaused;
    public int ThreadCount => _workerCount;

    public ThreadScheduler(int? workerCount = null, Action<string>? logger = null)
    {
        int effectiveWorkerCount = ThreadSchedulerHelper.DetermineWorkerCount(workerCount);

        SynchronizationContext uiContext = ThreadSchedulerHelper.GetRequiredSynchronizationContext();

        _workerCount = effectiveWorkerCount;
        _workers = new MonitoringThreader[effectiveWorkerCount];
        _contextScheduler = ThreadSchedulerHelper.CreateContextScheduler(effectiveWorkerCount, uiContext, logger);

        InitializeWorkers(effectiveWorkerCount, _contextScheduler);
    }

    public static ITask CreateAsTask(Action action,TaskPriority priority = TaskPriority.Low, CancellationToken token = default) 
        => new ActionTask(action, priority, token);


    private void InitializeWorkers(int workerCount, ContextScheduler contextScheduler)
    {
        for (int i = 0; i < workerCount; i++)
        {
            var log = new StringBuilder();
            var worker = new MonitoringThreader(i, log, contextScheduler);
            _workers[i] = worker;
            worker.StartThread();
        }
    }

    public ThreadHandle Schedule(ITask task)
    {
        TaskData item = new(task, _contextScheduler.SlimPool.Get());
        GetQueue(task.Priority).Enqueue(item);
        _contextScheduler.WorkSignal.Set();
        return new ThreadHandle(item, _contextScheduler.SlimPool);
    }

    public ThreadHandle Schedule(Action action, TaskPriority priority = TaskPriority.Normal, CancellationToken token = default)
        => Schedule(new ActionTask(action, priority, token));

    public void Stop()
    {
        _contextScheduler.IsStopping = true;
        _contextScheduler.WorkSignal.Set(); // разбудить все потоки, чтобы они вышли
        _contextScheduler.ResumeSignal.Set(); // если были в паузе – выйти
        foreach (var t in _workers)
        {
            t.Join(); // дождаться завершения
        }
    }

    public void Pause()
    {
        _contextScheduler.IsPaused = true;
        _contextScheduler.ResumeSignal.Reset(); // дальнейшие ожидания будут блокироваться
    }

    public void Resume()
    {
        _contextScheduler.IsPaused = false;
        _contextScheduler.ResumeSignal.Set(); // разбудить потоки в паузе
    }

    public bool TryAddNewTaskToWorker(int id, ITask task, out ThreadHandle handle)
    {
        handle = default;
        if (id < 0 || id >= _workerCount) return false;

        ManualResetEventSlim poolItem = _contextScheduler.SlimPool.Get();
        TaskData taskData = new(task, poolItem);
        if (_workers[id].TrySetNextTask(taskData))
        {
            handle = new ThreadHandle(taskData, _contextScheduler.SlimPool);
            return true;
        }
        else
        {
            _contextScheduler.SlimPool.Return(poolItem);
            return false;
        }
    }

    public int? FoundFreeWorkerId()
    {
        for (int i = 0; i < _workers.Length; i++)
        {
            if (!_workers[i].IsBusy) return i;
        }
        return null;
    }
    public MetricsCollector GetThreadMetric(int id) => _workers[id].Metrics;

    private ConcurrentQueue<TaskData> GetQueue(TaskPriority priority) => priority switch
    {
        TaskPriority.High => _contextScheduler.HighQueue,
        TaskPriority.Low => _contextScheduler.LowQueue,
        _ => _contextScheduler.NormalQueue
    };
}
