using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadMetrics;

namespace ThreadingSystem.ThreadControl;

public class MonitoringThreader
{
    private readonly MetricsCollector _metricsTime = new();
    private readonly StringBuilder _log;
    private readonly ContextScheduler _contextScheduler;
    private readonly Stopwatch _sw = new();
    private readonly int _id;
    private readonly Thread _thread;
    private TaskData? _nextTask = null;
    private int _countHighTaskDone = 0;
    private int _countNormalTaskDone = 0;
    private int _countLowTaskDone = 0;
    private bool _isBusy = false;
    private bool _haveNextTask = false;

    public bool IsBusy => Volatile.Read(ref _isBusy);
    public MetricsCollector Metrics => _metricsTime;
    public int Id => _id;
    public int СountHighTaskDone => _countHighTaskDone;
    public int CountNormalTaskDone => _countNormalTaskDone;
    public int CountLowTaskDone => _countLowTaskDone;
    public MonitoringThreader(int id, StringBuilder log, ContextScheduler contextScheduler)
    {
        _log = log;
        _contextScheduler = contextScheduler;
        _id = id;
        _thread = new(WorkerLoop)
        {
            IsBackground = true
        };
    }
    public void StartThread() => _thread.Start();
    public bool TryJoin(int millisecundesOnJoin = 1000) => _thread.Join(millisecundesOnJoin);
    public void Join() => _thread.Join();

    public bool TrySetNextTask(TaskData task)
    {
        bool haveTask = Volatile.Read(ref _haveNextTask);
        if (!haveTask)
        {
            Volatile.Write(ref _haveNextTask, true);
            _nextTask = task;
            return true;
        }
        return false;
    }

    private void WorkerLoop()
    {
        while (true)
        {
            _contextScheduler.WorkSignal.Wait();

            if (_contextScheduler.IsStopping)
                break;

            if (_contextScheduler.IsPaused)
            {
                _contextScheduler.ResumeSignal.Wait();
                if (_contextScheduler.IsStopping) break;
            }

            _contextScheduler.WorkSignal.Reset();

            bool haveTask = Volatile.Read(ref _haveNextTask);
            if (haveTask && _nextTask != null)
            {
                ExecuteActionItem(_nextTask.Value);
                Volatile.Write(ref _haveNextTask, false);
                _nextTask = null;
            }

            TaskData item;
            while (_contextScheduler.TryDequeueAny(out item))
            {
                ExecuteActionItem(item);
            }

            _contextScheduler.WorkSignal.Reset();
            if (_contextScheduler.TryDequeueAny(out item))
            {
                ExecuteActionItem(item);
            }
        }

        _contextScheduler.FlushLog(_log);
    }

    private void ExecuteActionItem(TaskData item)
    {
        Volatile.Write(ref _isBusy, true);
        _sw.Restart();
        _contextScheduler.ExecuteItem(item, _log);
        _sw.Stop();
        _metricsTime.Add(_sw.Elapsed);
        SetCountOverTheEntirePeriod(item.TaskAction.Priority);
        item.DoneEvent.Set();
        Volatile.Write(ref _isBusy, false);
    }


    private void SetCountOverTheEntirePeriod(TaskPriority priority)
    {
        switch (priority)
        {
            case TaskPriority.High: _countHighTaskDone++; break;
            case TaskPriority.Normal: _countNormalTaskDone++; break;
            case TaskPriority.Low: _countLowTaskDone++; break;
        }
    }
}