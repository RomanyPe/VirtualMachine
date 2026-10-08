namespace Kernel.Contracts;

public interface IReadOnlyLogOptimization
{
    OptimizationId Id { get; }
    string GetLogs();
}
