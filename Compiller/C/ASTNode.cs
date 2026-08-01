namespace Compiller.C;

// ============================================================
// 2. АБСТРАКТНОЕ СИНТАКСИЧЕСКОЕ ДЕРЕВО (AST)
// ============================================================
public abstract class ASTNode { }

public class ProgramNode : ASTNode
{
    public List<FunctionNode> Functions { get; } = [];
    public List<VariableNode> Globals { get; } = [];
}

public class FunctionNode(string name, string returnType, BlockNode body) : ASTNode
{
    public string Name { get; } = name;
    public string ReturnType { get; } = returnType;
    public List<ParameterNode> Parameters { get; set; } = [];
    public BlockNode Body { get; } = body;
    public int LocalSize { get; set; }
}

public class ParameterNode(string type, string name) : ASTNode
{
    public string Type { get; } = type; public string Name { get; } = name;
}

public class BlockNode : ASTNode
{
    public List<ASTNode> Statements { get; } = [];
}

public class VariableNode(string type, string name, ASTNode? initializer = null) : ASTNode
{
    public string Type { get; } = type;
    public string Name { get; } = name;
    public ASTNode? Initializer { get; } = initializer;
    public bool IsArray { get; set; } = false;
    public int ArraySize { get; set; } = 0;   // количество элементов
}

public class AssignmentNode(string name, ASTNode value) : ASTNode
{
    public string Name { get; } = name; public ASTNode Value { get; } = value;
}

public class BinaryOpNode(string op, ASTNode left, ASTNode right) : ASTNode
{
    public string Operator { get; } = op;
    public ASTNode Left { get; } = left;
    public ASTNode Right { get; } = right;
}

public class UnaryOpNode(string op, ASTNode operand) : ASTNode
{
    public string Operator { get; } = op; public ASTNode Operand { get; } = operand;
}

public class NumberNode(long value) : ASTNode
{
    public long Value { get; } = value;
}

public class IdentifierNode(string name) : ASTNode
{
    public string Name { get; } = name;
}

public class IfNode(ASTNode condition, BlockNode thenBlock, BlockNode? elseBlock = null) : ASTNode
{
    public ASTNode Condition { get; } = condition;
    public BlockNode ThenBlock { get; } = thenBlock;
    public BlockNode? ElseBlock { get; } = elseBlock;
}

public class WhileNode(ASTNode condition, BlockNode body) : ASTNode
{
    public ASTNode Condition { get; } = condition; public BlockNode Body { get; } = body;
}

public class ForNode(ASTNode? init, ASTNode? condition, ASTNode? increment, BlockNode body) : ASTNode
{
    public ASTNode? Init { get; } = init;
    public ASTNode? Condition { get; } = condition;
    public ASTNode? Increment { get; } = increment;
    public BlockNode Body { get; } = body;
}

public class ReturnNode(ASTNode? value = null) : ASTNode
{
    public ASTNode? Value { get; } = value;
}

public class FunctionCallNode(string name) : ASTNode
{
    public string Name { get; } = name;
    public List<ASTNode> Arguments { get; } = [];
}

// Входной порт: _in_port(порт, переменная_назначения)
public class InPortNode(ASTNode port, IdentifierNode dataVar) : ASTNode
{
    public ASTNode Port { get; } = port;
    public IdentifierNode DataVar { get; } = dataVar;
}

// Выходной порт: _out_port(порт, значение)
public class OutPortNode(ASTNode port, ASTNode value) : ASTNode
{
    public ASTNode Port { get; } = port;
    public ASTNode Value { get; } = value;
}

public class ArrayAccessNode(string name, ASTNode index) : ASTNode
{
    public string ArrayName = name;       // имя переменной-массива
    public ASTNode Index = index;          // выражение для индекса
}