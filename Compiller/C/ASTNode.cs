namespace Compiller.C;

// ============================================================
// 2. АБСТРАКТНОЕ СИНТАКСИЧЕСКОЕ ДЕРЕВО (AST)
// ============================================================
public abstract class ASTNode;

public class ProgramNode : ASTNode
{
    public List<StructDeclNode> Structs { get; set; } = [];
    public List<FunctionNode> Functions { get; set; } = [];
    public List<VariableNode> Globals { get; set; } = [];
    public List<string> Includes { get; set; } = [];   // <-- новое
}

public class FunctionNode(string name, string returnType, BlockNode body) : ASTNode
{
    public string Name { get; set; } = name;
    public string ReturnType { get; set; } = returnType;
    public List<ParameterNode> Parameters { get; set; } = [];
    public BlockNode Body { get; set; } = body;
    public int LocalSize { get; set; }
    public bool IsExternal { get; set; } = false;  // новое поле
}

public class ParameterNode(string type, string name) : ASTNode
{
    public string Type { get; set; } = type;
    public string Name { get; set; } = name;
    public bool IsPointer { get; set; }
    public string? PointedType { get; set; }
}

public class BlockNode : ASTNode
{
    public List<ASTNode> Statements { get; set; } = [];
}

public class VariableNode(string type, string name, ASTNode? initializer = null) : ASTNode
{
    public string? StructTypeName { get; set; } = null;
    public string Type { get; set; } = type;
    public string Name { get; set; } = name;
    public ASTNode? Initializer { get; set; } = initializer;
    public bool IsPointer { get; set; } = false;
    public string? PointedType { get; set; } // например "int"
    public bool IsArray { get; set; } = false;
    public int ArraySize { get; set; } = 0;   // количество элементов
}

public class AssignmentNode(string name, ASTNode value, ASTNode? indexExpr = null) : ASTNode
{
    public string Name = name;
    public ASTNode Value = value;
    public ASTNode? IndexExpr = indexExpr; // null если присваивание скаляру, иначе индекс для массива
    public ASTNode? LValue { get; set; }
}
public class BinaryOpNode(string op, ASTNode left, ASTNode right) : ASTNode
{
    public string Operator { get; set; } = op;
    public ASTNode Left { get; set; } = left;
    public ASTNode Right { get; set; } = right;
}

public class UnaryOpNode(string op, ASTNode operand) : ASTNode
{
    public string Operator { get; set; } = op; public ASTNode Operand { get; set; } = operand;
}

public class NumberNode(long value) : ASTNode
{
    public long Value { get; set; } = value;
}

public class IdentifierNode(string name) : ASTNode
{
    public string Name { get; set; } = name;
}

public class IfNode(ASTNode condition, BlockNode thenBlock, BlockNode? elseBlock = null) : ASTNode
{
    public ASTNode Condition { get; set; } = condition;
    public BlockNode ThenBlock { get; set; } = thenBlock;
    public BlockNode? ElseBlock { get; set; } = elseBlock;
}

public class WhileNode(ASTNode condition, BlockNode body) : ASTNode
{
    public ASTNode Condition { get; set; } = condition; public BlockNode Body { get; set; } = body;
}

public class ForNode(ASTNode? init, ASTNode? condition, ASTNode? increment, BlockNode body) : ASTNode
{
    public ASTNode? Init { get; set; } = init;
    public ASTNode? Condition { get; set; } = condition;
    public ASTNode? Increment { get; set; } = increment;
    public BlockNode Body { get; set; } = body;
}

public class ReturnNode(ASTNode? value = null) : ASTNode
{
    public ASTNode? Value { get; set; } = value;
}

public class FunctionCallNode(string name) : ASTNode
{
    public string Name { get; set; } = name;
    public List<ASTNode> Arguments { get; set; } = [];
}

public class ArrayAccessNode(string name, ASTNode index) : ASTNode
{
    public string ArrayName = name;       // имя переменной-массива
    public ASTNode Index = index;          // выражение для индекса
}

// Узел взятия адреса: &expr
public class AddressOfNode(ASTNode operand) : ASTNode
{
    public ASTNode Operand = operand;
}

// Узел разыменования: *expr
public class DereferenceNode(ASTNode operand) : ASTNode
{
    public ASTNode Operand = operand;
}

public class NewArrayNode(string type, ASTNode size) : ASTNode
{
    public string Type = type;      // "int", "byte" и т.д.
    public ASTNode Size = size;     // выражение, задающее количество элементов
}

public class InlineAsmNode(string asmCode) : ASTNode
{
    public string AsmCode = asmCode;   // текст между { и }
}

public class StructDeclNode(string name, List<(string type, string fieldName)> fields) : ASTNode
{
    public string Name = name;
    public List<(string type, string fieldName)> Fields = fields;
}

public class MemberAccessNode(ASTNode @object, string fieldName, bool isArrow) : ASTNode
{
    public ASTNode Object = @object;       // выражение, дающее объект (или указатель)
    public string FieldName = fieldName;
    public bool IsArrow = isArrow;         // true: ->, false: .
}