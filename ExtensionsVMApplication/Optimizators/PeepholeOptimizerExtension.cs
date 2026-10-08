using Compiller.ASM.Optimizators;
using ExtensionsVMApplication.Optimizators.IRAsmRules;

namespace ExtensionsVMApplication.Optimizators;

public static class PeepholeOptimizerExtension
{
    extension(PeepholeOptimizer peephole)
    {
        public PeepholeOptimizer AddStd()
        {
            peephole.AddRule(new RemoveNoopMovRule());
            peephole.AddRule(new RemovePushPopPairRule());
            peephole.AddRule(new RemoveSwapMovPairRule());
            peephole.AddRule(new RemoveJumpToNextLabelRule());
            peephole.AddRule(new ConstantFoldingRule());
            peephole.AddRule(new RemoveMultiEndPairRule());
            return peephole;
        }
    }
}
