namespace Kernel.Contracts;

// удалить эту хуйню и заменить на структуры с строками
public enum TypeOptimization
{
    Peephole,
    ASTNodeRemovedBeforeInline,
    ASTNodeInlinedFunc,
    ASTNodeConstPropagate,
    ASTNodeConstFold,
    ASTNodeRemovedAfterInline
}
