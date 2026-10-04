using Compiller.ASM.Optimizators;
using Kernel.Common;
using Kernel.Contracts;

namespace VMApplication.Project;

public class PeepholeOptimizationLogs(PeepholeLog log) : IReadOnlyLogOptimization
{
    public TypeOptimization Id => TypeOptimization.Peephole;
    public string GetLogs() => log.ToString();
}
