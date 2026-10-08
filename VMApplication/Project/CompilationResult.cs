using Kernel.Common;
using Kernel.Contracts;

namespace VMApplication.Project;

// Результат компиляции
public readonly struct CompilationResult(byte[]? program, ulong startAdress, IReadOnlyList<string>? errors, OptimizationResultLog? log)
{
    public byte[]? Program { get; } = program;
    public ulong StartAdress { get; } = startAdress;
    public OptimizationResultLog? OptimizationResultLog { get; } = log;
    public IReadOnlyList<string>? Errors { get; } = errors;
    public bool Success => Errors == null || Errors.Count == 0;
}

public class OptimizationResultLog
{
    public Dictionary<OptimizationId, IReadOnlyLogOptimization> Logs { get; private set; } = [];

    public bool AddLog(IReadOnlyLogOptimization log) => Logs.TryAdd(log.Id, log);
    public void AddLog(IEnumerable<IReadOnlyLogOptimization> log)
    {
        foreach (var logItem in log)
        {
            Logs.TryAdd(logItem.Id, logItem);
        }
    }
}

