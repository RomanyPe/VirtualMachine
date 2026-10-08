using Compiller.ASM.Optimizators;
using Compiller.C.Optimizators;

namespace VMApplication.Project;

public sealed class VMCompilerOptimizator
{
    internal AstOptimizer AstOptimizer { get; }
    internal PeepholeOptimizer PeepholeOptimizer { get; }
    public VMPeepholeOptimizer VMPeepholeOptimizer { get; }
    public VMAstOptimizer VMAstOptimizer { get; }

    public VMCompilerOptimizator()
    {
        AstOptimizer = new();
        PeepholeOptimizer = new();
        VMAstOptimizer = new(AstOptimizer);
        VMPeepholeOptimizer = new(PeepholeOptimizer);
    }
}
