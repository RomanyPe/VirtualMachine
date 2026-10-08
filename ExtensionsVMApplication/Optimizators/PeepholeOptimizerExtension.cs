using Compiller.ASM.Optimizators;
using ExtensionsVMApplication.Optimizators.IRAsmRules;
using VMApplication.Project;

namespace ExtensionsVMApplication.Optimizators;

public static class PeepholeOptimizerExtension
{
    extension(VMPeepholeOptimizer peephole)
    {
        public VMPeepholeOptimizer AddStdRules()
        {
            peephole.Add(new RemoveNoopMovRule());
            peephole.Add(new RemovePushPopPairRule());
            peephole.Add(new RemoveSwapMovPairRule());
            peephole.Add(new RemoveJumpToNextLabelRule());
            peephole.Add(new ConstantFoldingRule());
            peephole.Add(new RemoveMultiEndPairRule());
            return peephole;
        }
    }
}
