using Compiller.ASM.Optimizators.Rules;

namespace Compiller.ASM.Optimizators;


public static class PeepholeOptimizer
{
    private static readonly HashSet<IPeepholeRule> _rules = [
        new RemoveNoopMovRule(),
        new RemovePushPopPairRule(),
        new RemoveSwapMovPairRule(),
        new RemoveJumpToNextLabelRule(),
        new ConstantFoldingRule(),
        new RemoveMultiEndPairRule(),
    ];
    public static bool AddRule(IPeepholeRule rule) => _rules.Add(rule);
    public static void RemoveRule(IPeepholeRule rule) => _rules.Remove(rule);
    public static IReadOnlySet<IPeepholeRule> Rules => _rules;
    public static string Optimize(List<AsmItem> items, PeepholeLog log, int maxPasses = 5)
    {
        bool changed;
        int passCount = 0;

        do
        {
            changed = false;
            int i = 0;
            while (i < items.Count)
            {
                if (items[i] is AsmLabel)
                {
                    i++;
                    continue;
                }

                bool ruleApplied = false;
                foreach (var rule in _rules)
                {
                    if (rule.TryOptimize(items, i, log, out int nextIndex))
                    {
                        if (nextIndex < 0)
                            nextIndex = 0;
                        changed = true;
                        ruleApplied = true;
                        i = nextIndex;
                        break;
                    }
                }

                if (!ruleApplied)
                {
                    i++;
                }
            }

            passCount++;
        }
        while (changed && passCount < maxPasses);
        return log.ToString();
    }
}

