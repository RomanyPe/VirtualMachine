using Compiller.ASM.Optimizators;
using Kernel.Contracts;
using System.Text;

namespace Compiller.C.Optimizators;


public class ASTNodesOptimization(StringBuilder logs, OptimizationId type) : ILogOptimization
{
    public OptimizationId Id => type;

    public void Append(string text) => logs.Append(text);

    public void AppendLine(string text) => logs.AppendLine(text);

    public string GetLogs() => logs.ToString();
}


public interface ILogOptimization : IReadOnlyLogOptimization
{
    void Append(string text);
    void AppendLine(string text);

}


public interface IAstOptimizationRule
{
    OptimizationId Type { get; }
    /// <summary>
    /// Применяет оптимизацию к программе.
    /// Возвращает true, если были внесены изменения.
    /// </summary>
    IReadOnlyLogOptimization Optimize(ProgramNode program);
}


public class AstOptimizer
{
    private readonly List<IAstOptimizationRule> _rules = [];
    public void Add(IAstOptimizationRule rule) => _rules.Add(rule);
    public void Insert(int index, IAstOptimizationRule item) => _rules.Insert(index, item);
    public void Remove(IAstOptimizationRule item) => _rules.Remove(item);
    public void RemoveAt(int index) => _rules.RemoveAt(index);
    public int Count => _rules.Count;

    public OptimizationId GetType(int i) => _rules[i].Type;

    public IEnumerable<IReadOnlyLogOptimization> Optimize(ProgramNode program)
        => _rules.Select(rule => rule.Optimize(program));
}