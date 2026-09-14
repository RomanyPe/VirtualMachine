namespace Kernel.Common;

public interface IReadOnlyASTNode;

// Интерфейсы только для чтения для каждого узла
public interface IReadOnlyFunctionNode : IReadOnlyASTNode
{
    string Name { get; }
    string ReturnType { get; }
    IReadOnlyList<IReadOnlyParameterNode> Parameters { get; }
    IReadOnlyBlockNode Body { get; }
    int LocalSize { get; }
    bool IsExternal { get; }
}

public interface IReadOnlyVariableNode : IReadOnlyASTNode
{
    string? StructTypeName { get; }
    string Type { get; }
    string Name { get; }
    IReadOnlyASTNode? Initializer { get; }
    bool IsPointer { get; }
    string? PointedType { get; }
    bool IsArray { get; }
    int ArraySize { get; }
}

public interface IReadOnlyStructDeclNode : IReadOnlyASTNode
{
    string Name { get; }
    IReadOnlyList<(string type, string fieldName)> Fields { get; }
}

public interface IReadOnlyParameterNode : IReadOnlyASTNode
{
    string Type { get; }
    string Name { get; }
    bool IsPointer { get; }
    string? PointedType { get; }
}

public interface IReadOnlyBlockNode : IReadOnlyASTNode
{
    IReadOnlyList<IReadOnlyASTNode> Statements { get; }
}

public interface IReadOnlyAssignmentNode : IReadOnlyASTNode
{
    string Name { get; }
    IReadOnlyASTNode Value { get; }
    IReadOnlyASTNode? IndexExpr { get; }
    IReadOnlyASTNode? LValue { get; }
}

public interface IReadOnlyBinaryOpNode : IReadOnlyASTNode
{
    string Operator { get; }
    IReadOnlyASTNode Left { get; }
    IReadOnlyASTNode Right { get; }
}

public interface IReadOnlyUnaryOpNode : IReadOnlyASTNode
{
    string Operator { get; }
    IReadOnlyASTNode Operand { get; }
}

public interface IReadOnlyNumberNode : IReadOnlyASTNode
{
    long Value { get; }
}

public interface IReadOnlyIdentifierNode : IReadOnlyASTNode
{
    string Name { get; }
}

public interface IReadOnlyIfNode : IReadOnlyASTNode
{
    IReadOnlyASTNode Condition { get; }
    IReadOnlyBlockNode ThenBlock { get; }
    IReadOnlyBlockNode? ElseBlock { get; }
}

public interface IReadOnlyWhileNode : IReadOnlyASTNode
{
    IReadOnlyASTNode Condition { get; }
    IReadOnlyBlockNode Body { get; }
}

public interface IReadOnlyForNode : IReadOnlyASTNode
{
    IReadOnlyASTNode? Init { get; }
    IReadOnlyASTNode? Condition { get; }
    IReadOnlyASTNode? Increment { get; }
    IReadOnlyBlockNode Body { get; }
}

public interface IReadOnlyReturnNode : IReadOnlyASTNode
{
    IReadOnlyASTNode? Value { get; }
}

public interface IReadOnlyFunctionCallNode : IReadOnlyASTNode
{
    string Name { get; }
    IReadOnlyList<IReadOnlyASTNode> Arguments { get; }
}

public interface IReadOnlyArrayAccessNode : IReadOnlyASTNode
{
    string ArrayName { get; }
    IReadOnlyASTNode Index { get; }
}

public interface IReadOnlyAddressOfNode : IReadOnlyASTNode
{
    IReadOnlyASTNode Operand { get; }
}

public interface IReadOnlyDereferenceNode : IReadOnlyASTNode
{
    IReadOnlyASTNode Operand { get; }
}

public interface IReadOnlyNewArrayNode : IReadOnlyASTNode
{
    string Type { get; }
    IReadOnlyASTNode Size { get; }
}

public interface IReadOnlyInlineAsmNode : IReadOnlyASTNode
{
    string AsmCode { get; }
}

public interface IReadOnlyMemberAccessNode : IReadOnlyASTNode
{
    IReadOnlyASTNode Object { get; }
    string FieldName { get; }
    bool IsArrow { get; }
}

public interface IReadOnlyProgramNode
{
    IReadOnlyList<IReadOnlyFunctionNode> Functions { get; }
    IReadOnlyList<IReadOnlyVariableNode> Globals { get; }
    IReadOnlyList<string> Includes { get; }
    IReadOnlyList<IReadOnlyStructDeclNode> Structs { get; }
}
