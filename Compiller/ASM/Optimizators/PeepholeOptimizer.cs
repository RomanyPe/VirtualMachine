namespace Compiller.ASM.Optimizators;


public class PeepholeOptimizer()
{
    private readonly List<IPeepholeRule> _rules = [];
    public void AddRule(IPeepholeRule rule) => _rules.Add(rule);
    public void RemoveRule(IPeepholeRule rule) => _rules.Remove(rule);
    public IReadOnlyList<IPeepholeRule> Rules => _rules;
    public string Optimize(List<AsmItem> items, PeepholeLog log, int maxPasses = 5)
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

