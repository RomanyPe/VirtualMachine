using Compiller.C.Optimizators;
using Kernel.Contracts;

namespace VMApplication.Project;

public sealed class VMAstOptimizer
{
    private readonly AstOptimizer _opt;

    internal VMAstOptimizer(AstOptimizer astOptimizer)
    {
        _opt = astOptimizer;
    }
    public int Count => _opt.Count;
    public void Add(IAstOptimizationRule rule) => _opt.Add(rule);
    public void Insert(int index, IAstOptimizationRule item) => _opt.Insert(index, item);
    public void Remove(IAstOptimizationRule item) => _opt.Remove(item);
    public void RemoveAt(int index) => _opt.RemoveAt(index);
    public OptimizationId GetType(int i) => _opt.GetType(i);

}
