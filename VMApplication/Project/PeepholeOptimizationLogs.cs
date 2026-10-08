using Compiller.ASM.Optimizators;
using Kernel.Common;
using Kernel.Contracts;

namespace VMApplication.Project;

public class PeepholeOptimizationLogs(PeepholeLog log) : IReadOnlyLogOptimization
{
    public OptimizationId Id => OptimizationId.Peephole;
    public string GetLogs() => log.ToString();
}
