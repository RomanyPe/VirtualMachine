using Compiller.ASM.Optimizators;
using Kernel.Common;

namespace VMApplication.Project;

public class PeepholeOptimizationLogs(PeepholeLog log) : IReadOnlyLogOptimization
{
    public TypeOptimization Id => TypeOptimization.Peephole;
    public string GetLogs() => log.ToString();
}
