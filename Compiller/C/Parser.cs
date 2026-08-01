namespace Compiller.C;

// ============================================================
// 3. ПАРСЕР (без изменений)
// ============================================================
public class Parser(List<Token> tokens)
{
    private readonly List<Token> _tokens = tokens;
    private int _position;
    private Token Current => _tokens[_position];
    public Token CurrentToken => Current;

    public ProgramNode Parse()
    {
        var program = new ProgramNode();
        while (Current.Type != TokenType.EOF)
        {
            if (IsTypeSpecifier())
            {
                string type = ParseType();
                string name = Expect(TokenType.Identifier).Value;
                if (Current.Type == TokenType.Punctuation && Current.Value == "(")
                {
                    var func = ParseFunction(type, name);
                    program.Functions.Add(func);
                }
                else
                {
                    var varNode = ParseVariable(type, name);
                    program.Globals.Add(varNode);
                    Expect(TokenType.Punctuation, ";");
                }
            }
            else
            {
                throw new Exception($"Unexpected token: {Current}");
            }
        }
        return program;
    }

    private bool IsTypeSpecifier()
    {
        return Current.Type == TokenType.Keyword &&
               (Current.Value == "int" || Current.Value == "char" || Current.Value == "void" ||
                Current.Value == "byte" || Current.Value == "ushort" || Current.Value == "ulong");
    }
    private string ParseType() => Expect(TokenType.Keyword).Value;

    private FunctionNode ParseFunction(string returnType, string name)
    {
        Expect(TokenType.Punctuation, "(");
        var parameters = new List<ParameterNode>();
        if (Current.Value != ")")
        {
            while (true)
            {
                string paramType = ParseType();
                string paramName = Expect(TokenType.Identifier).Value;
                parameters.Add(new ParameterNode(paramType, paramName));
                if (Current.Value != ",") break;
                Expect(TokenType.Punctuation, ",");
            }
        }
        Expect(TokenType.Punctuation, ")");
        Expect(TokenType.Punctuation, "{");
        var body = ParseBlock();
        var func = new FunctionNode(name, returnType, body)
        {
            Parameters = parameters // Ключевая строка!
        };
        return func;
    }
    private VariableNode ParseVariable(string type, string name)
    {
        ASTNode? initializer = null;
        if (Current.Value == "=")
        {
            Expect(TokenType.Operator, "=");
            initializer = ParseExpression();
        }
        return new VariableNode(type, name, initializer);
    }

    private BlockNode ParseBlock()
    {
        BlockNode block = new();
        while (Current.Value != "}")
        {
            if (IsTypeSpecifier())
            {
                string type = ParseType();
                string name = Expect(TokenType.Identifier).Value;
                var varNode = ParseVariable(type, name);
                block.Statements.Add(varNode);
                Expect(TokenType.Punctuation, ";");
            }
            else
            {
                block.Statements.Add(ParseBlockType(Current.Value));
            }
        }
        Expect(TokenType.Punctuation, "}");
        return block;
    }

    private ASTNode ParseBlockType(string text) => text switch
    {
        "if" => ParseIf(),
        "while" => ParseWhile(),
        "for" => ParseFor(),
        "return" => ParseReturn(),
        _ => ParseStatement(),
    };

    private IfNode ParseIf()
    {
        Expect(TokenType.Keyword, "if");
        Expect(TokenType.Punctuation, "(");
        ASTNode condition = ParseExpression();
        Expect(TokenType.Punctuation, ")");
        Expect(TokenType.Punctuation, "{");
        var thenBlock = ParseBlock();
        BlockNode? elseBlock = null;
        if (Current.Value == "else")
        {
            Expect(TokenType.Keyword, "else");
            Expect(TokenType.Punctuation, "{");
            elseBlock = ParseBlock();
        }
        return new IfNode(condition, thenBlock, elseBlock);
    }

    private WhileNode ParseWhile()
    {
        Expect(TokenType.Keyword, "while");
        Expect(TokenType.Punctuation, "(");
        ASTNode condition = ParseExpression();
        Expect(TokenType.Punctuation, ")");
        Expect(TokenType.Punctuation, "{");
        var body = ParseBlock();
        return new WhileNode(condition, body);
    }

    private ForNode ParseFor()
    {
        Expect(TokenType.Keyword, "for");
        Expect(TokenType.Punctuation, "(");
        ASTNode? init = null;
        if (Current.Value != ";") init = ParseExpression();
        Expect(TokenType.Punctuation, ";");
        ASTNode? condition = null;
        if (Current.Value != ";") condition = ParseExpression();
        Expect(TokenType.Punctuation, ";");
        ASTNode? increment = null;
        if (Current.Value != ")") increment = ParseExpression();
        Expect(TokenType.Punctuation, ")");
        Expect(TokenType.Punctuation, "{");
        var body = ParseBlock();
        return new ForNode(init, condition, increment, body);
    }

    private ReturnNode ParseReturn()
    {
        Expect(TokenType.Keyword, "return");
        ASTNode? value = null;
        if (Current.Value != ";") value = ParseExpression();
        Expect(TokenType.Punctuation, ";");
        return new ReturnNode(value);
    }

    private ASTNode ParseStatement()
    {
        ASTNode expr = ParseExpression();
        Expect(TokenType.Punctuation, ";");
        return expr;
    }

    private ASTNode ParseExpression() => ParseAssignment();

    private ASTNode ParseAssignment()
    {
        ASTNode left = ParseLogicalOr();
        if (Current.Type == TokenType.Operator && Current.Value == "=")
        {
            Advance();
            ASTNode right = ParseAssignment();
            if (left is IdentifierNode id)
                return new AssignmentNode(id.Name, right);
            throw new Exception("Invalid assignment target");
        }
        return left;
    }

    private ASTNode ParseLogicalOr()
    {
        ASTNode left = ParseLogicalAnd();
        while (Current.Type == TokenType.Operator && Current.Value == "||")
        {
            string op = Current.Value;
            Advance();
            ASTNode right = ParseLogicalAnd();
            left = new BinaryOpNode(op, left, right);
        }
        return left;
    }


    private ASTNode ParseLogicalAnd()
    {
        ASTNode left = ParseEquality();
        while (Current.Type == TokenType.Operator && Current.Value == "&&")
        {
            string op = Current.Value;
            Advance();
            ASTNode right = ParseEquality();
            left = new BinaryOpNode(op, left, right);
        }
        return left;
    }

    private ASTNode ParseEquality()
    {
        ASTNode left = ParseRelational();
        while (Current.Type == TokenType.Operator && (Current.Value == "==" || Current.Value == "!="))
        {
            string op = Current.Value;
            Advance();
            ASTNode right = ParseRelational();
            left = new BinaryOpNode(op, left, right);
        }
        return left;
    }

    private ASTNode ParseRelational()
    {
        ASTNode left = ParseAdditive();
        while (Current.Type == TokenType.Operator && (Current.Value == "<" || Current.Value == ">" ||
                                                     Current.Value == "<=" || Current.Value == ">="))
        {
            string op = Current.Value;
            Advance();
            ASTNode right = ParseAdditive();
            left = new BinaryOpNode(op, left, right);
        }
        return left;
    }

    private ASTNode ParseAdditive()
    {
        ASTNode left = ParseMultiplicative();
        while (Current.Type == TokenType.Operator && (Current.Value == "+" || Current.Value == "-"))
        {
            string op = Current.Value;
            Advance();
            ASTNode right = ParseMultiplicative();
            left = new BinaryOpNode(op, left, right);
        }
        return left;
    }

    private ASTNode ParseMultiplicative()
    {
        ASTNode left = ParseUnary();
        while (Current.Type == TokenType.Operator && (Current.Value == "*" || Current.Value == "/" || Current.Value == "%"))
        {
            string op = Current.Value;
            Advance();
            ASTNode right = ParseUnary();
            left = new BinaryOpNode(op, left, right);
        }
        return left;
    }

    private ASTNode ParseUnary()
    {
        if (Current.Type == TokenType.Operator && (Current.Value == "+" || Current.Value == "-" ||
                                                   Current.Value == "!" || Current.Value == "~" ||
                                                   Current.Value == "*" || Current.Value == "&"))
        {
            string op = Current.Value;
            Advance();
            ASTNode operand = ParseUnary();
            return new UnaryOpNode(op, operand);
        }
        return ParsePrimary();
    }

    private ASTNode ParsePrimary()
    {
        if (Current.Type == TokenType.Number)
        {
            long value = long.Parse(Current.Value);
            Advance();
            return new NumberNode(value);
        }
        if (Current.Type == TokenType.Identifier)
        {
            string name = Current.Value;
            Advance();
            if (Current.Type == TokenType.Punctuation && Current.Value == "(")
            {
                // Проверяем встроенные функции ввода-вывода
                if (name == "_in_port")
                {
                    Expect(TokenType.Punctuation, "(");
                    ASTNode port = ParseExpression();
                    Expect(TokenType.Punctuation, ",");
                    // Ожидаем идентификатор (переменную)
                    if (Current.Type != TokenType.Identifier)
                        throw new Exception("Expected identifier as second argument of _in_port");
                    IdentifierNode dataVar = new(Current.Value);
                    Advance();
                    Expect(TokenType.Punctuation, ")");
                    return new InPortNode(port, dataVar);
                }
                else if (name == "_out_port")
                {
                    Expect(TokenType.Punctuation, "(");
                    ASTNode port = ParseExpression();
                    Expect(TokenType.Punctuation, ",");
                    ASTNode value = ParseExpression();
                    Expect(TokenType.Punctuation, ")");
                    return new OutPortNode(port, value);
                }

                // Обычный вызов функции
                var call = new FunctionCallNode(name);
                Expect(TokenType.Punctuation, "(");
                if (Current.Value != ")")
                {
                    while (true)
                    {
                        call.Arguments.Add(ParseExpression());
                        if (Current.Value != ",") break;
                        Expect(TokenType.Punctuation, ",");
                    }
                }
                Expect(TokenType.Punctuation, ")");
                return call;
            }
            return new IdentifierNode(name);
        }
        if (Current.Value == "(")
        {
            Advance();
            ASTNode expr = ParseExpression();
            Expect(TokenType.Punctuation, ")");
            return expr;
        }
        throw new Exception($"Unexpected token: {Current}");
    }
    private Token Expect(TokenType type, string? value = null)
    {
        if (Current.Type != type)
            throw new Exception($"Expected {type}, got {Current.Type} at {Current.Line}:{Current.Column}");
        if (value != null && Current.Value != value)
            throw new Exception($"Expected '{value}', got '{Current.Value}' at {Current.Line}:{Current.Column}");
        Token token = Current;
        Advance();
        return token;
    }

    private void Advance() => _position++;
}
