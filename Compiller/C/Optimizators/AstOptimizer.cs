using Compiller.C.Optimizators.Rules;
using Kernel.Common;
using System.Text;

namespace Compiller.C.Optimizators;


public class ASTNodesOptimization(StringBuilder logs, TypeOptimization type) : ILogOptimization
{
    public TypeOptimization Id => type;

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
    TypeOptimization Type { get; }
    /// <summary>
    /// Применяет оптимизацию к программе.
    /// Возвращает true, если были внесены изменения.
    /// </summary>
    IReadOnlyLogOptimization Optimize(ProgramNode program);
}


public static class AstOptimizer
{
    private static readonly List<IAstOptimizationRule> _rules =
        [
            new PropagateConstantsRule(),
            new FoldConstantsRule(),
            new RemoveUnusedVariablesRule(),
            new RemoveUnreachableCodeRuleBeforeInline(),
            new InlineSmallVoidFunctionsRule(),
            new RemoveUnreachableCodeRuleAfterInline()
        ];

    //public static int Count => _rules.Count;
    //public static void Insert(int index, IAstOptimizationRule item) => _rules.Insert(index, item);
    //public static void Remove(IAstOptimizationRule item) => _rules.Remove(item);
    //public static void RemoveAt(int index) => _rules.RemoveAt(index);
    //public static TypeOptimization GetType(int i) => _rules[i].Type;
    //public static TypeOptimization GetType(IAstOptimizationRule rule) => _rules.FirstOrDefault(rule).Type;

    public static IEnumerable<IReadOnlyLogOptimization> Optimize(ProgramNode program)
    {
        return [.. _rules.Select(rule => rule.Optimize(program))];
    }
}