namespace Kernel.Contracts;

public interface IReadOnlyLogOptimization
{
    TypeOptimization Id { get; }
    string GetLogs();
}
