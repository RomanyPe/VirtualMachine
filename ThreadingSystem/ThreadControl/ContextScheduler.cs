using System.Collections.Concurrent;
using System.Text;
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadPool;

namespace ThreadingSystem.ThreadControl;

public class ContextScheduler(ManualResetEventSlimPool slimPool, SynchronizationContext uiContext, Action<string>? logger = null)
{
    public readonly ConcurrentQueue<TaskData> HighQueue = new();
    public readonly ConcurrentQueue<TaskData> NormalQueue = new();
    public readonly ConcurrentQueue<TaskData> LowQueue = new();

    public readonly ManualResetEventSlimPool SlimPool = slimPool;

    public readonly ManualResetEventSlim WorkSignal = new(false);
    public readonly ManualResetEventSlim ResumeSignal = new(true);

    public volatile bool IsStopping = false;
    public volatile bool IsPaused = false;

    public readonly SynchronizationContext UiContext = uiContext;
    public readonly Action<string>? Logger = logger; // внешний логгер (опционально)

    public bool TryDequeueAny(out TaskData item)
    {
        return HighQueue.TryDequeue(out item) || NormalQueue.TryDequeue(out item) || LowQueue.TryDequeue(out item);
    }

    public void ExecuteItem(TaskData item, StringBuilder log)
    {

        log.AppendLine($"Start {item.TaskAction?.Priority} task");
        try
        {
            item.TaskAction?.Token.ThrowIfCancellationRequested();
            item.TaskAction?.Execute();
            log.AppendLine($"Task with priority: {item.TaskAction?.Priority} completed");
        }
        catch (OperationCanceledException)
        {
            log.AppendLine("Task cancelled");
        }
        catch (Exception ex)
        {
            log.AppendLine($"Exception: {ex.Message}");
            Logger?.Invoke($"Error in {item.TaskAction?.Priority} task: {ex}");
        }
        finally
        {
            if (item.TaskAction?.Priority == TaskPriority.High)
            {
                UiContext?.Post(_ => FlushLog(log) , null);
            }
        }
    }

    public void FlushLog(StringBuilder log)
    {
        if (log.Length == 0) return;
        string text = log.ToString();
        log.Clear();
        Logger?.Invoke(text);
        UiContext?.Post(_ => { /* обновление UI с текстом */ }, null);
    }
}
