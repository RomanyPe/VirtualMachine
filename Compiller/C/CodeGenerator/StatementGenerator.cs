using Compiller.ASM;
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// StatementGenerator – генерация инструкций
// ============================================================
public class StatementGenerator(Assembler asm, ExpressionGenerator exprGen, FunctionContext funcCtx, Func<string> getLabel, GlobalMemoryManager globalMem, Dictionary<string, StructLayout> structTable)
{
    private readonly GlobalMemoryManager _globalMem = globalMem;
    private readonly Assembler _asm = asm;
    private readonly ExpressionGenerator _exprGen = exprGen;
    private readonly FunctionContext _funcCtx = funcCtx;
    private readonly Dictionary<string, StructLayout> _structTable = structTable;
    private readonly Func<string> _getLabel = getLabel;

    public void GenerateBlock(BlockNode block)
    {
        foreach (var stmt in block.Statements)
        {
            switch (stmt)
            {
                case VariableNode var:
                    if (var.Initializer != null)
                    {
                        _exprGen.GenerateExpression(var.Initializer);
                        _exprGen.StoreR0ToVariable(var.Name);
                    }
                    break;
                case AssignmentNode assign:
                    GenerateAssignment(assign);
                    break;
                case BinaryOpNode binop:
                    _exprGen.GenerateExpression(binop);   // выражение как инструкция
                    break;
                case UnaryOpNode unop:
                    _exprGen.GenerateExpression(unop);
                    break;
                case IfNode ifNode:
                    GenerateIf(ifNode);
                    break;
                case WhileNode whileNode:
                    GenerateWhile(whileNode);
                    break;
                case ForNode forNode:
                    GenerateFor(forNode);
                    break;
                case ReturnNode ret:
                    GenerateReturn(ret);
                    break;
                case FunctionCallNode call:
                    _exprGen.GenerateExpression(call);
                    break;
                case InlineAsmNode asm:
                    GenerateInlineAsm(asm);
                    break;
            }
        }
    }

    private void GenerateInlineAsm(InlineAsmNode asm)
    {
        // Парсим asm-строку и вставляем в текущий Assembler
        var asmParser = new AssemblerParser();
        asmParser.Assemble(asm.AsmCode, _asm);   // _asm должен быть доступен
    }

    private void GenerateAssignment(AssignmentNode assign)
    {
        // Присваивание элементу массива
        if (assign.IndexExpr != null)
        {
            _exprGen.GenerateExpression(assign.Value);
            _exprGen.StoreR0ToArrayElement(assign.Name, assign.IndexExpr);
            return;
        }

        if (assign.LValue != null)
        {
            if (assign.LValue is DereferenceNode deref)
            {
                _exprGen.GenerateExpression(assign.Value);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));
                _exprGen.GenerateExpression(deref.Operand);
                OpCodeSize size = GetPointedSize(deref.Operand);
                _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, (uint)RegType.r0, (uint)size));
                return;
            }
        }

        if (assign.LValue is MemberAccessNode memberAccess)
        {
            // Генерируем значение правой части в r0, затем сохраняем в поле
            _exprGen.GenerateExpression(assign.Value);   // r0 = значение
                                                         // Сохраняем значение во временный регистр r1
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));

            // Вычисляем адрес поля
            _exprGen.GenerateMemberAddress(memberAccess); // нужно написать метод, возвращающий адрес в r0
                                                          // Сохраняем
            OpCodeSize size = GetFieldSize(memberAccess);
            _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, (uint)RegType.r0, size.Uint));
            return;
        }

        // Оптимизация x = x +/- ...
        if (assign.Value is BinaryOpNode binop &&
            (binop.Operator == "+" || binop.Operator == "-") &&
            binop.Left is IdentifierNode leftId &&
            leftId.Name == assign.Name &&
            _funcCtx.VarMap.TryGetValue(assign.Name, out var xLoc) &&
            xLoc.IsRegister)
        {
            RegType xReg = xLoc.Register;
            if (binop.Right is NumberNode num)
            {
                if (num.Value == 1 && binop.Operator == "+")
                    _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.INC.Uint, (uint)xReg));
                else if (num.Value == 1 && binop.Operator == "-")
                    _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.DEC.Uint, (uint)xReg));
                else
                {
                    _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), (ulong)num.Value);
                    if (binop.Operator == "+")
                        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)xReg, (uint)RegType.r0));
                    else
                        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)xReg, (uint)RegType.r0));
                }
                return;
            }
            else if (binop.Right is IdentifierNode)
            {
                // Загружаем значение y в r0
                _exprGen.GenerateExpression(binop.Right);
                // Теперь r0 содержит y, напрямую делаем ADD/SUB с xReg
                if (binop.Operator == "+")
                    _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)xReg, (uint)RegType.r0));
                else
                    _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)xReg, (uint)RegType.r0));
                return;
            }
            else
            {
                _exprGen.GenerateExpression(binop.Right);
                if (binop.Operator == "+")
                    _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)xReg, (uint)RegType.r0));
                else
                    _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)xReg, (uint)RegType.r0));
                return;
            }
        }

        // Обычное скалярное присваивание
        _exprGen.GenerateExpression(assign.Value);
        _exprGen.StoreR0ToVariable(assign.Name);
    }


    private OpCodeSize GetFieldSize(MemberAccessNode node)
    {
        var (_, field, _) = ResolveFieldInfo(node);
        return CodeGenUtils.GetSizeForType(field.Type);
    }

    private (StructLayout layout, FieldInfo field, string fieldType) ResolveFieldInfo(MemberAccessNode node)
    {
        // 1. Собираем цепочку полей от корневого объекта до самого вложенного
        var chain = new List<string>();
        MemberAccessNode? current = node;
        ASTNode? rootObject = null;

        while (current != null)
        {
            chain.Add(current.FieldName);
            if (current.Object is MemberAccessNode inner)
            {
                current = inner;
            }
            else
            {
                rootObject = current.Object;
                break;
            }
        }
        chain.Reverse(); // от внешнего к внутреннему

        // 2. Определяем тип корневого объекта
        string? rootStructType = null;
        if (rootObject is IdentifierNode id)
        {
            if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
            {
                if (loc.StructTypeName != null)
                    rootStructType = loc.StructTypeName;
                else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
                    rootStructType = loc.PointedType;
            }
            else if (_globalMem.Contains(id.Name))
            {
                var gInfo = _globalMem.GetInfo(id.Name)!.Value;
                if (_structTable.ContainsKey(gInfo.Type))
                    rootStructType = gInfo.Type;
                else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
                    rootStructType = gInfo.PointedType;
            }
        }
        else if (rootObject is ArrayAccessNode arrAcc)
        {
            rootStructType = _exprGen.ResolveArrayStructType(arrAcc.ArrayName);
        }

        if (rootStructType == null)
            throw new Exception("Cannot determine struct type for member access");

        // 3. Проходим по цепочке, получая раскладку и поле на каждом уровне
        StructLayout? currentLayout = null;
        FieldInfo? currentField = null;
        string currentType = rootStructType;

        foreach (var fieldName in chain)
        {
            if (!_structTable.TryGetValue(currentType, out var layout))
                throw new Exception($"Unknown struct type '{currentType}'");
            currentLayout = layout;
            currentField = layout.GetField(fieldName)
                           ?? throw new Exception($"Field '{fieldName}' not found in struct '{currentType}'");
            currentType = currentField.Type;
        }

        return (currentLayout!, currentField!, currentType);
    }
    // Вспомогательный метод – определение типа структуры для данного MemberAccessNode
    private string? ResolveStructType(MemberAccessNode node)
    {
        // Собираем цепочку полей: для o.y.a -> ["y", "a"]
        var chain = new List<string>();
        MemberAccessNode? current = node;
        ASTNode? rootObject = null;

        // Раскручиваем цепочку MemberAccessNode до корневого объекта
        while (current != null)
        {
            chain.Add(current.FieldName);
            if (current.Object is MemberAccessNode inner)
            {
                current = inner;
            }
            else
            {
                rootObject = current.Object;
                break;
            }
        }
        chain.Reverse(); // теперь от корня к последнему полю

        // Определяем тип корневого объекта
        string? structType = null;
        if (rootObject is IdentifierNode id)
        {
            if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
            {
                if (loc.StructTypeName != null)
                    structType = loc.StructTypeName;
                else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
                    structType = loc.PointedType;
            }
            else if (_globalMem.Contains(id.Name))
            {
                var gInfo = _globalMem.GetInfo(id.Name)!.Value;
                if (_structTable.ContainsKey(gInfo.Type))
                    structType = gInfo.Type;
                else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
                    structType = gInfo.PointedType;
            }
        }
        else if (rootObject is ArrayAccessNode arrAcc)
        {
            structType = _exprGen.ResolveArrayStructType(arrAcc.ArrayName);
        }

        if (structType == null)
            return null;

        // Проходим по цепочке полей, обновляя тип
        foreach (var fieldName in chain)
        {
            if (!_structTable.TryGetValue(structType, out var layout))
                return null;
            var field = layout.GetField(fieldName);
            if (field == null)
                return null;
            structType = field.Type;
        }

        return structType;
    }

    private OpCodeSize GetPointedSize(ASTNode expr)
    {
        if (expr is IdentifierNode id)
        {
            if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc) && loc.IsPointer)
                return CodeGenUtils.GetSizeForType(loc.PointedType!);
            if (_globalMem.Contains(id.Name))
            {
                var gInfo = _globalMem.GetInfo(id.Name)!.Value;
                if (gInfo.IsPointer)
                    return CodeGenUtils.GetSizeForType(gInfo.PointedType!);
            }
        }
        throw new Exception("Cannot determine pointed type");
    }
    private void GenerateIf(IfNode ifNode)
    {
        string elseLabel = _getLabel();
        string endLabel = ifNode.ElseBlock != null ? _getLabel() : elseLabel;

        _exprGen.GenerateCondition(ifNode.Condition, null, elseLabel);
        GenerateBlock(ifNode.ThenBlock);

        if (ifNode.ElseBlock != null)
        {
            _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
            _asm.MarkLabel(elseLabel);
            GenerateBlock(ifNode.ElseBlock);
            _asm.MarkLabel(endLabel);
        }
        else
        {
            _asm.MarkLabel(elseLabel);
        }
    }

    private void GenerateWhile(WhileNode whileNode)
    {
        string startLabel = _getLabel();
        string endLabel = _getLabel();

        _asm.MarkLabel(startLabel);
        _exprGen.GenerateCondition(whileNode.Condition, null, endLabel);
        GenerateBlock(whileNode.Body);
        _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), startLabel);
        _asm.MarkLabel(endLabel);
    }

    private void GenerateFor(ForNode forNode)
    {
        if (forNode.Init != null)
        {
            if (forNode.Init is VariableNode varInit)
            {
                if (varInit.Initializer != null)
                {
                    _exprGen.GenerateExpression(varInit.Initializer);
                    _exprGen.StoreR0ToVariable(varInit.Name);
                }
            }
            else if (forNode.Init is AssignmentNode assignInit)
            {
                GenerateAssignment(assignInit);
            }
            else
            {
                _exprGen.GenerateExpression(forNode.Init);
            }
        }

        string startLabel = _getLabel();
        string endLabel = _getLabel();

        _asm.MarkLabel(startLabel);
        if (forNode.Condition != null)
            _exprGen.GenerateCondition(forNode.Condition, null, endLabel);
        GenerateBlock(forNode.Body);
        if (forNode.Increment != null)
        {
            if (forNode.Increment is AssignmentNode assignIncr)
                GenerateAssignment(assignIncr);
            else
                _exprGen.GenerateExpression(forNode.Increment);
        }
        _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), startLabel);
        _asm.MarkLabel(endLabel);
    }

    private void GenerateReturn(ReturnNode ret)
    {
        if (ret.Value != null)
            _exprGen.GenerateExpression(ret.Value);
        if (_funcCtx.FunctionName == "main")
            _asm.EmitInstruction(InstructionEncoder.EncodeEND());
        else
            _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), _funcCtx.EpilogueLabel);
    }
}
