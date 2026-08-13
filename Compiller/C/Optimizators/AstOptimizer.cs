using System.Text;

namespace Compiller.C.Optimizators;

public readonly struct OutPutOptimizeText(StringBuilder removedNodes, StringBuilder inlinedFunc)
{
    public readonly StringBuilder RemovedNodes = removedNodes;
    public readonly StringBuilder InlinedFunc = inlinedFunc;
}

public static class AstOptimizer
{
    private const int MaxWeightInlineSize = 10;
    public static OutPutOptimizeText Optimize(ProgramNode program)
    {
        var removedNodes = new StringBuilder(1024);
        var inlinedFunc = new StringBuilder(1024);

        RemoveUnreachableCode(program, removedNodes);
        InlineSmallVoidFunctions(program, MaxWeightInlineSize, inlinedFunc);
        RemoveUnreachableCode(program, removedNodes); // после инлайнинга

        return new OutPutOptimizeText(removedNodes, inlinedFunc);
    }

    // ------------------------------------------------------------
    // УДАЛЕНИЕ НЕДОСТИЖИМОГО КОДА
    // ------------------------------------------------------------
    private static void RemoveUnreachableCode(ProgramNode program, StringBuilder removedNodesLog)
    {
        foreach (var func in program.Functions)
        {
            if (func.Body != null)
                OptimizeBlock(func.Body, removedNodesLog);
        }

        // Также обрабатываем глобальные инициализаторы, если они были в блоках (у нас их нет, но на будущее)
        foreach (var global in program.Globals)
        {
            if (global.Initializer is BlockNode block)
                OptimizeBlock(block, removedNodesLog);
        }
    }

    private static void OptimizeBlock(BlockNode block, StringBuilder removedNodesLog)
    {
        var newStatements = new List<ASTNode>();
        bool unreachable = false;

        foreach (var stmt in block.Statements)
        {
            if (unreachable)
            {
                // Логируем все оставшиеся как недостижимые
                for (int i = block.Statements.IndexOf(stmt); i < block.Statements.Count; i++)
                {
                    removedNodesLog.AppendLine($"Unreachable node removed: {block.Statements[i].GetType().Name}");
                }
                break;
            }

            // Рекурсивно оптимизируем вложенные блоки
            switch (stmt)
            {
                case BlockNode nested:
                    OptimizeBlock(nested, removedNodesLog);
                    newStatements.Add(nested);
                    break;

                case IfNode ifNode:
                    OptimizeBlock(ifNode.ThenBlock, removedNodesLog);
                    if (ifNode.ElseBlock != null) OptimizeBlock(ifNode.ElseBlock, removedNodesLog);
                    newStatements.Add(ifNode);
                    break;

                case WhileNode whileNode:
                    OptimizeBlock(whileNode.Body, removedNodesLog);
                    newStatements.Add(whileNode);
                    break;

                case ForNode forNode:
                    if (forNode.Body != null) OptimizeBlock(forNode.Body, removedNodesLog);
                    newStatements.Add(forNode);
                    break;

                case ReturnNode:
                    newStatements.Add(stmt);
                    unreachable = true;
                    break;

                default:
                    newStatements.Add(stmt);
                    break;
            }
        }

        block.Statements.Clear();
        block.Statements.AddRange(newStatements);

        // Упрощение if (false) / while (false) / for(;false;)
        SimplifyControlFlow(block, removedNodesLog);
    }

    private static void SimplifyControlFlow(BlockNode block, StringBuilder removedNodesLog)
    {
        var simplified = new List<ASTNode>();
        foreach (var stmt in block.Statements)
        {
            if (stmt is IfNode ifNode)
            {
                if (IsConstantFalse(ifNode.Condition))
                {
                    if (ifNode.ElseBlock != null)
                    {
                        removedNodesLog.AppendLine($"IfNode removed (then-block discarded, else-block kept): {ifNode.GetType().Name}");
                        simplified.AddRange(ifNode.ElseBlock.Statements);
                    }
                    else
                    {
                        removedNodesLog.AppendLine($"IfNode removed (no else): {ifNode.GetType().Name}");
                    }
                }
                else if (IsConstantTrue(ifNode.Condition))
                {
                    removedNodesLog.AppendLine($"IfNode removed (condition always true, else-block discarded): {ifNode.GetType().Name}");
                    simplified.AddRange(ifNode.ThenBlock.Statements);
                }
                else
                {
                    simplified.Add(ifNode);
                }
            }
            else if (stmt is WhileNode whileNode && IsConstantFalse(whileNode.Condition))
            {
                removedNodesLog.AppendLine($"WhileNode removed (condition false): {whileNode.GetType().Name}");
            }
            else if (stmt is ForNode forNode && forNode.Condition != null && IsConstantFalse(forNode.Condition))
            {
                removedNodesLog.AppendLine($"ForNode removed (condition false): {forNode.GetType().Name}");
            }
            else
            {
                simplified.Add(stmt);
            }
        }
        block.Statements.Clear();
        block.Statements.AddRange(simplified);
    }

    private static bool IsConstantFalse(ASTNode node) =>
        node is NumberNode num && num.Value == 0;

    private static bool IsConstantTrue(ASTNode node) =>
        node is NumberNode num && num.Value != 0;

    // ------------------------------------------------------------
    // ИНЛАЙНИНГ МАЛЕНЬКИХ VOID-ФУНКЦИЙ
    // ------------------------------------------------------------
    private static void InlineSmallVoidFunctions(ProgramNode program, int maxBodySize, StringBuilder inlinedFunc)
    {
        // Собираем кандидатов: void-функции, не extern, не main, не рекурсивные, с маленьким телом
        var candidates = program.Functions
            .Where(f => f.ReturnType == "void" &&
                        !f.IsExternal &&
                        f.Name != "main" &&
                        f.Body != null &&
                        CountStatements(f.Body) <= maxBodySize)
            .ToList();

        if (candidates.Count == 0) return;

        var candidateNames = new HashSet<string>(candidates.Select(f => f.Name));

        // Проверяем рекурсию
        candidates = [.. candidates.Where(f => !ContainsCallTo(f.Body, f.Name))];

        var inlined = new HashSet<string>();

        foreach (var func in program.Functions)
        {
            if (func.Body != null && func.Name != "main")
            {
                InlineCallsInBlock(func.Body, candidates.ToDictionary(f => f.Name), inlined, inlinedFunc);
            }
        }

        // Можно удалить инлайнированные функции, если они больше не вызываются,
        // но для простоты оставим их в программе – они будут генерироваться, но не использоваться.
    }

    private static void InlineCallsInBlock(BlockNode block,
                                            Dictionary<string, FunctionNode> candidateMap,
                                            HashSet<string> inlined,
                                            StringBuilder inlinedFunc)
    {
        var newStatements = new List<ASTNode>();

        foreach (var stmt in block.Statements)
        {
            switch (stmt)
            {
                case FunctionCallNode call when candidateMap.TryGetValue(call.Name, out var target) &&
                                                target.ReturnType == "void" &&
                                                target.Parameters.Count == call.Arguments.Count:
                    // Заменяем вызов на inline-блок
                    var inlineBlock = CreateInlineBlock(call, target);
                    newStatements.AddRange(inlineBlock.Statements);
                    inlined.Add(target.Name);
                    // Логируем факт инлайнинга
                    inlinedFunc.AppendLine($"Inlined call to '{call.Name}' (replaced with inline block)");
                    break;

                case IfNode ifNode:
                    InlineCallsInBlock(ifNode.ThenBlock, candidateMap, inlined, inlinedFunc);
                    if (ifNode.ElseBlock != null) InlineCallsInBlock(ifNode.ElseBlock, candidateMap, inlined, inlinedFunc);
                    newStatements.Add(ifNode);
                    break;

                case WhileNode whileNode:
                    InlineCallsInBlock(whileNode.Body, candidateMap, inlined, inlinedFunc);
                    newStatements.Add(whileNode);
                    break;

                case ForNode forNode:
                    if (forNode.Init is BlockNode initBlock) InlineCallsInBlock(initBlock, candidateMap, inlined, inlinedFunc);
                    if (forNode.Increment is BlockNode incBlock) InlineCallsInBlock(incBlock, candidateMap, inlined, inlinedFunc);
                    InlineCallsInBlock(forNode.Body, candidateMap, inlined, inlinedFunc);
                    newStatements.Add(forNode);
                    break;

                default:
                    newStatements.Add(stmt);
                    break;
            }
        }

        block.Statements.Clear();
        block.Statements.AddRange(newStatements);
    }

    private static BlockNode CreateInlineBlock(FunctionCallNode call, FunctionNode targetFunc)
    {
        var inlineBlock = new BlockNode();
        var renameMap = new Dictionary<string, string>();
        int uniqueId = Guid.NewGuid().GetHashCode() & 0xFFFF;

        // Присваивание аргументов параметрам (параметры переименовываются)
        for (int i = 0; i < targetFunc.Parameters.Count; i++)
        {
            var param = targetFunc.Parameters[i];
            var arg = call.Arguments[i];
            string newParamName = $"{param.Name}_inl_{uniqueId}";
            renameMap[param.Name] = newParamName;

            // Создаём локальную переменную для параметра (она будет вставлена в FunctionContext)
            var paramVar = new VariableNode(param.Type, newParamName, null)
            {
                IsPointer = param.IsPointer,
                PointedType = param.PointedType
            };
            inlineBlock.Statements.Add(paramVar);

            // Присваиваем значение аргумента
            var assign = new AssignmentNode(newParamName, arg) { LValue = new IdentifierNode(newParamName) };
            inlineBlock.Statements.Add(assign);
        }

        // Копируем тело функции с заменой имён переменных
        foreach (var stmt in targetFunc.Body.Statements)
        {
            var cloned = CloneStatementWithRename(stmt, renameMap);
            inlineBlock.Statements.Add(cloned);
        }

        return inlineBlock;
    }

    private static ASTNode CloneStatementWithRename(ASTNode node, Dictionary<string, string> renameMap)
    {
        switch (node)
        {
            case BlockNode block:
                var newBlock = new BlockNode();
                foreach (var s in block.Statements)
                    newBlock.Statements.Add(CloneStatementWithRename(s, renameMap));
                return newBlock;

            case VariableNode varNode:
                string newName = varNode.Name;
                if (renameMap.TryGetValue(varNode.Name, out var mapped)) newName = mapped;
                return new VariableNode(varNode.Type, newName, CloneExpression(varNode.Initializer, renameMap))
                {
                    IsArray = varNode.IsArray,
                    ArraySize = varNode.ArraySize,
                    IsPointer = varNode.IsPointer,
                    PointedType = varNode.PointedType
                };

            case AssignmentNode assign:
                string assignName = assign.Name;
                if (renameMap.TryGetValue(assign.Name, out var mappedAssign)) assignName = mappedAssign;
                return new AssignmentNode(assignName, CloneExpression(assign.Value, renameMap)!, CloneExpression(assign.IndexExpr, renameMap))
                {
                    LValue = CloneExpression(assign.LValue, renameMap)
                };

            case ReturnNode ret:
                return new ReturnNode(CloneExpression(ret.Value, renameMap));

            case IfNode ifNode:
                var newIf = new IfNode(CloneExpression(ifNode.Condition, renameMap)!,
                                       (BlockNode)CloneStatementWithRename(ifNode.ThenBlock, renameMap),
                                       ifNode.ElseBlock != null ? (BlockNode)CloneStatementWithRename(ifNode.ElseBlock, renameMap) : null);
                return newIf;

            case WhileNode whileNode:
                return new WhileNode(CloneExpression(whileNode.Condition, renameMap)!,
                                     (BlockNode)CloneStatementWithRename(whileNode.Body, renameMap));

            case ForNode forNode:
                var newFor = new ForNode(CloneExpression(forNode.Init, renameMap),
                                         CloneExpression(forNode.Condition, renameMap),
                                         CloneExpression(forNode.Increment, renameMap),
                                         (BlockNode)CloneStatementWithRename(forNode.Body, renameMap));
                return newFor;

            case FunctionCallNode call:
                var newCall = new FunctionCallNode(call.Name);
                foreach (var arg in call.Arguments)
                    newCall.Arguments.Add(CloneExpression(arg, renameMap)!);
                return newCall;

            case BinaryOpNode bin:
                return new BinaryOpNode(bin.Operator,
                                        CloneExpression(bin.Left, renameMap)!,
                                        CloneExpression(bin.Right, renameMap)!);

            case UnaryOpNode un:
                return new UnaryOpNode(un.Operator, CloneExpression(un.Operand, renameMap)!);

            case IdentifierNode id:
                if (renameMap.TryGetValue(id.Name, out var idMapped))
                    return new IdentifierNode(idMapped);
                return new IdentifierNode(id.Name);

            case ArrayAccessNode arr:
                return new ArrayAccessNode(
                    renameMap.TryGetValue(arr.ArrayName, out var arrMapped) ? arrMapped : arr.ArrayName,
                    CloneExpression(arr.Index, renameMap)!);

            case MemberAccessNode member:
                return new MemberAccessNode(CloneExpression(member.Object, renameMap)!,
                                            member.FieldName,
                                            member.IsArrow);

            case AddressOfNode addrOf:
                return new AddressOfNode(CloneExpression(addrOf.Operand, renameMap)!);

            case DereferenceNode deref:
                return new DereferenceNode(CloneExpression(deref.Operand, renameMap)!);

            case NewArrayNode newArr:
                return new NewArrayNode(newArr.Type, CloneExpression(newArr.Size, renameMap)!);

            default:
                // Числа и прочие листовые узлы не содержат имён переменных
                return node;
        }
    }

    private static ASTNode? CloneExpression(ASTNode? node, Dictionary<string, string> renameMap)
    {
        if (node == null) return null;
        return CloneStatementWithRename(node, renameMap);
    }

    private static int CountStatements(BlockNode block)
    {
        int count = 0;
        foreach (var stmt in block.Statements)
        {
            count++;
            switch (stmt)
            {
                case IfNode ifNode:
                    count += CountStatements(ifNode.ThenBlock) + (ifNode.ElseBlock != null ? CountStatements(ifNode.ElseBlock) : 0);
                    break;
                case WhileNode whileNode:
                    count += CountStatements(whileNode.Body);
                    break;
                case ForNode forNode:
                    count += CountStatements(forNode.Body);
                    break;
            }
        }
        return count;
    }

    private static bool ContainsCallTo(BlockNode block, string funcName)
    {
        foreach (var stmt in block.Statements)
        {
            if (ContainsCallToStatement(stmt, funcName))
                return true;
        }
        return false;
    }

    private static bool ContainsCallToStatement(ASTNode node, string funcName) => node switch
    {
        FunctionCallNode call => FunkCallContains(funcName, call),
        BinaryOpNode bin => ContainsCallToStatement(bin.Left, funcName) || ContainsCallToStatement(bin.Right, funcName),
        UnaryOpNode un => ContainsCallToStatement(un.Operand, funcName),
        IfNode ifNode => ContainsCallTo(ifNode.ThenBlock, funcName) ||
                               (ifNode.ElseBlock != null && ContainsCallTo(ifNode.ElseBlock, funcName)),
        WhileNode whileNode => ContainsCallTo(whileNode.Body, funcName),
        ForNode forNode => ContainsCallTo(forNode.Body, funcName),
        BlockNode block => ContainsCallTo(block, funcName),
        _ => false,
    };

    private static bool FunkCallContains(string funcName, FunctionCallNode call)
    {
        if (call.Name == funcName) return true;
        foreach (var arg in call.Arguments)
            if (ContainsCallToStatement(arg, funcName)) return true;
        return false;
    }
}
