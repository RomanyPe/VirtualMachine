using System.Text;

namespace VMApplication.Project;

// Результат компиляции
public readonly struct CompilationResult(byte[]? program,ulong startAdress, IReadOnlyList<string>? errors, OptimizationResultLog? log)
{
    public byte[]? Program { get; } = program;
    public ulong StartAdress { get; } = startAdress;
    public OptimizationResultLog? OptimizationResultLog { get; } = log;
    public IReadOnlyList<string>? Errors { get; } = errors;
    public bool Success => Errors == null || Errors.Count == 0;
}

public readonly struct OptimizationResultLog(StringBuilder inlinedFunc, StringBuilder removedNodes)
{
    public readonly StringBuilder InlinedFunc = inlinedFunc;
    public readonly StringBuilder RemovedNodes = removedNodes;
}
