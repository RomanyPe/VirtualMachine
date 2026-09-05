using System.Threading.Tasks.Sources;

namespace Kernel.Common;

/// <summary>
/// Абстракция для вывода сообщений (без привязки к конкретному UI).
/// </summary>
public interface ILogger
{
    void CharOutPut(char c);
    void Info(string message);
    void Warning(string message);
    void Error(string message);
    void Clear();
}

public interface IReadOnlyLogOptimization
{
    TypeOptimization Id { get; }
    string GetLogs();
}

public enum TypeOptimization
{
    Peephole,
    ASTNodeRemovedBeforeInline,
    ASTNodeInlinedFunc,
    ASTNodeConstPropagate,
    ASTNodeConstFold,
    ASTNodeRemovedAfterInline
}