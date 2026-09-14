using Kernel.Common;

namespace Compiller.C;

// ============================================================
// БАЗОВЫЙ КЛАСС И РЕАЛИЗАЦИИ УЗЛОВ
// ============================================================

public abstract class ASTNode : IReadOnlyASTNode;

public class ProgramNode : ASTNode, IReadOnlyProgramNode
{
    public List<StructDeclNode> StructNodes { get; set; } = [];
    public List<FunctionNode> FunctionNodes { get; set; } = [];
    public List<VariableNode> GlobalVarNodes { get; set; } = [];
    public List<string> IncludesList { get; set; } = [];

    IReadOnlyList<IReadOnlyFunctionNode> IReadOnlyProgramNode.Functions => FunctionNodes;
    IReadOnlyList<IReadOnlyVariableNode> IReadOnlyProgramNode.Globals => GlobalVarNodes;
    IReadOnlyList<string> IReadOnlyProgramNode.Includes => IncludesList;
    IReadOnlyList<IReadOnlyStructDeclNode> IReadOnlyProgramNode.Structs => StructNodes;
}

public class FunctionNode(string name, string returnType, BlockNode body) : ASTNode, IReadOnlyFunctionNode
{
    public string Name { get; set; } = name;
    public string ReturnType { get; set; } = returnType;
    public List<ParameterNode> Parameters { get; set; } = [];
    public BlockNode Body { get; set; } = body;
    public int LocalSize { get; set; }
    public bool IsExternal { get; set; }

    // Явная реализация для свойств с отличными типами
    IReadOnlyList<IReadOnlyParameterNode> IReadOnlyFunctionNode.Parameters => Parameters;
    IReadOnlyBlockNode IReadOnlyFunctionNode.Body => Body;
}

public class ParameterNode(string type, string name) : ASTNode, IReadOnlyParameterNode
{
    public string Type { get; set; } = type;
    public string Name { get; set; } = name;
    public bool IsPointer { get; set; }
    public string? PointedType { get; set; }
}

public class BlockNode : ASTNode, IReadOnlyBlockNode
{
    public List<ASTNode> Statements { get; set; } = [];

    IReadOnlyList<IReadOnlyASTNode> IReadOnlyBlockNode.Statements => Statements;
}

public class VariableNode(string type, string name, ASTNode? initializer = null) : ASTNode, IReadOnlyVariableNode
{
    public string? StructTypeName { get; set; }
    public string Type { get; set; } = type;
    public string Name { get; set; } = name;
    public ASTNode? Initializer { get; set; } = initializer;
    public bool IsPointer { get; set; }
    public string? PointedType { get; set; }
    public bool IsArray { get; set; }
    public int ArraySize { get; set; }

    IReadOnlyASTNode? IReadOnlyVariableNode.Initializer => Initializer;
}

public class AssignmentNode(string name, ASTNode value, ASTNode? indexExpr = null) : ASTNode, IReadOnlyAssignmentNode
{
    public string Name { get; set; } = name;
    public ASTNode Value { get; set; } = value;
    public ASTNode? IndexExpr { get; set; } = indexExpr;
    public ASTNode? LValue { get; set; }

    IReadOnlyASTNode IReadOnlyAssignmentNode.Value => Value;
    IReadOnlyASTNode? IReadOnlyAssignmentNode.IndexExpr => IndexExpr;
    IReadOnlyASTNode? IReadOnlyAssignmentNode.LValue => LValue;
}

public class BinaryOpNode(string op, ASTNode left, ASTNode right) : ASTNode, IReadOnlyBinaryOpNode
{
    public string Operator { get; set; } = op;
    public ASTNode Left { get; set; } = left;
    public ASTNode Right { get; set; } = right;

    IReadOnlyASTNode IReadOnlyBinaryOpNode.Left => Left;
    IReadOnlyASTNode IReadOnlyBinaryOpNode.Right => Right;
}

public class UnaryOpNode(string op, ASTNode operand) : ASTNode, IReadOnlyUnaryOpNode
{
    public string Operator { get; set; } = op;
    public ASTNode Operand { get; set; } = operand;

    IReadOnlyASTNode IReadOnlyUnaryOpNode.Operand => Operand;
}

public class NumberNode(long value) : ASTNode, IReadOnlyNumberNode
{
    public long Value { get; set; } = value;
}

public class IdentifierNode(string name) : ASTNode, IReadOnlyIdentifierNode
{
    public string Name { get; set; } = name;
}

public class IfNode(ASTNode condition, BlockNode thenBlock, BlockNode? elseBlock = null) : ASTNode, IReadOnlyIfNode
{
    public ASTNode Condition { get; set; } = condition;
    public BlockNode ThenBlock { get; set; } = thenBlock;
    public BlockNode? ElseBlock { get; set; } = elseBlock;

    IReadOnlyASTNode IReadOnlyIfNode.Condition => Condition;
    IReadOnlyBlockNode IReadOnlyIfNode.ThenBlock => ThenBlock;
    IReadOnlyBlockNode? IReadOnlyIfNode.ElseBlock => ElseBlock;
}

public class WhileNode(ASTNode condition, BlockNode body) : ASTNode, IReadOnlyWhileNode
{
    public ASTNode Condition { get; set; } = condition;
    public BlockNode Body { get; set; } = body;

    IReadOnlyASTNode IReadOnlyWhileNode.Condition => Condition;
    IReadOnlyBlockNode IReadOnlyWhileNode.Body => Body;
}

public class ForNode(ASTNode? init, ASTNode? condition, ASTNode? increment, BlockNode body) : ASTNode, IReadOnlyForNode
{
    public ASTNode? Init { get; set; } = init;
    public ASTNode? Condition { get; set; } = condition;
    public ASTNode? Increment { get; set; } = increment;
    public BlockNode Body { get; set; } = body;

    IReadOnlyASTNode? IReadOnlyForNode.Init => Init;
    IReadOnlyASTNode? IReadOnlyForNode.Condition => Condition;
    IReadOnlyASTNode? IReadOnlyForNode.Increment => Increment;
    IReadOnlyBlockNode IReadOnlyForNode.Body => Body;
}

public class ReturnNode(ASTNode? value = null) : ASTNode, IReadOnlyReturnNode
{
    public ASTNode? Value { get; set; } = value;

    IReadOnlyASTNode? IReadOnlyReturnNode.Value => Value;
}

public class FunctionCallNode(string name) : ASTNode, IReadOnlyFunctionCallNode
{
    public string Name { get; set; } = name;
    public List<ASTNode> Arguments { get; set; } = [];

    IReadOnlyList<IReadOnlyASTNode> IReadOnlyFunctionCallNode.Arguments => Arguments;
}

public class ArrayAccessNode(string name, ASTNode index) : ASTNode, IReadOnlyArrayAccessNode
{
    public string ArrayName { get; set; } = name;
    public ASTNode Index { get; set; } = index;

    IReadOnlyASTNode IReadOnlyArrayAccessNode.Index => Index;
}

public class AddressOfNode(ASTNode operand) : ASTNode, IReadOnlyAddressOfNode
{
    public ASTNode Operand { get; set; } = operand;

    IReadOnlyASTNode IReadOnlyAddressOfNode.Operand => Operand;
}

public class DereferenceNode(ASTNode operand) : ASTNode, IReadOnlyDereferenceNode
{
    public ASTNode Operand { get; set; } = operand;

    IReadOnlyASTNode IReadOnlyDereferenceNode.Operand => Operand;
}

public class NewArrayNode(string type, ASTNode size) : ASTNode, IReadOnlyNewArrayNode
{
    public string Type { get; set; } = type;
    public ASTNode Size { get; set; } = size;

    IReadOnlyASTNode IReadOnlyNewArrayNode.Size => Size;
}

public class InlineAsmNode(string asmCode) : ASTNode, IReadOnlyInlineAsmNode
{
    public string AsmCode { get; set; } = asmCode;
}

public class StructDeclNode(string name, List<(string type, string fieldName)> fields) : ASTNode, IReadOnlyStructDeclNode
{
    public string Name { get; set; } = name;
    public List<(string type, string fieldName)> Fields { get; set; } = fields;

    IReadOnlyList<(string type, string fieldName)> IReadOnlyStructDeclNode.Fields => Fields;
}

public class MemberAccessNode(ASTNode obj, string fieldName, bool isArrow) : ASTNode, IReadOnlyMemberAccessNode
{
    public ASTNode Object { get; set; } = obj;
    public string FieldName { get; set; } = fieldName;
    public bool IsArrow { get; set; } = isArrow;

    IReadOnlyASTNode IReadOnlyMemberAccessNode.Object => Object;
}