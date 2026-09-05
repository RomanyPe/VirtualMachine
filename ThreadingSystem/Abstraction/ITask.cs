namespace ThreadingSystem.Abstraction;

public interface ITask
{
    TaskPriority Priority { get; }
    CancellationToken Token { get; }
    void Execute();
}
