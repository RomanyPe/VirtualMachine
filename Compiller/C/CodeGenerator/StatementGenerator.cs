using Compiller.ASM;
using static Kernel.ProcessorSystem.Processor;

namespace Compiller.C.CodeGenerator;

// ============================================================
// StatementGenerator – генерация инструкций
// ============================================================
public class StatementGenerator(Assembler asm, ExpressionGenerator exprGen, FunctionContext funcCtx, Func<string> getLabel, GlobalMemoryManager globalMem)
{
    private readonly GlobalMemoryManager _globalMem = globalMem;
    private readonly Assembler _asm = asm;
    private readonly ExpressionGenerator _exprGen = exprGen;
    private readonly FunctionContext _funcCtx = funcCtx;
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
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.MOV, (uint)RegType.r1, (uint)RegType.r0));
                _exprGen.GenerateExpression(deref.Operand);
                OpCodeSize size = GetPointedSize(deref.Operand);
                _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, (uint)RegType.r0, (uint)size));
                return;
            }
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
                    _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.INC, (uint)xReg));
                else if (num.Value == 1 && binop.Operator == "-")
                    _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.DEC, (uint)xReg));
                else
                {
                    _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), (ulong)num.Value);
                    if (binop.Operator == "+")
                        _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.ADD, (uint)xReg, (uint)RegType.r0));
                    else
                        _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.SUB, (uint)xReg, (uint)RegType.r0));
                }
                return;
            }
            else if (binop.Right is IdentifierNode)
            {
                // Загружаем значение y в r0
                _exprGen.GenerateExpression(binop.Right);
                // Теперь r0 содержит y, напрямую делаем ADD/SUB с xReg
                if (binop.Operator == "+")
                    _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.ADD, (uint)xReg, (uint)RegType.r0));
                else
                    _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.SUB, (uint)xReg, (uint)RegType.r0));
                return;
            }
            else
            {
                _exprGen.GenerateExpression(binop.Right);
                if (binop.Operator == "+")
                    _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.ADD, (uint)xReg, (uint)RegType.r0));
                else
                    _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.SUB, (uint)xReg, (uint)RegType.r0));
                return;
            }
        }

        // Обычное скалярное присваивание
        _exprGen.GenerateExpression(assign.Value);
        _exprGen.StoreR0ToVariable(assign.Name);
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
            _asm.EmitJump(InstructionEncoder.EncodeJ((uint)OpCode.JMP), endLabel);
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
        _asm.EmitJump(InstructionEncoder.EncodeJ((uint)OpCode.JMP), startLabel);
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
        _asm.EmitJump(InstructionEncoder.EncodeJ((uint)OpCode.JMP), startLabel);
        _asm.MarkLabel(endLabel);
    }

    private void GenerateReturn(ReturnNode ret)
    {
        if (ret.Value != null)
            _exprGen.GenerateExpression(ret.Value);
        if (_funcCtx.FunctionName == "main")
            _asm.EmitInstruction(InstructionEncoder.EncodeHALT());
        else
            _asm.EmitJump(InstructionEncoder.EncodeJ((uint)OpCode.JMP), _funcCtx.EpilogueLabel);
    }
}
