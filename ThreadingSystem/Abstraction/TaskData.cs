namespace ThreadingSystem.Abstraction;

public readonly struct TaskData(ITask taskAction, ManualResetEventSlim doneEvent)
{
    public readonly ITask TaskAction = taskAction;
    public readonly ManualResetEventSlim DoneEvent = doneEvent;
}
