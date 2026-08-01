using Compiller.ASM;
using Kernel.ProcessorSystem;

namespace Compiller.C;

// ============================================================
// 4. ГЕНЕРАТОР КОДА
// ============================================================
public class CodeGenerator(Assembler asm)
{
    private class VarLocation
    {
        public bool IsRegister;              // true – в регистре, false – в стеке
        public Processor.RegType Register;   // если IsRegister
        public int StackOffset;              // если !IsRegister (смещение от rSP)
        public Processor.OpCodeSize TypeSize;
    }

    private Dictionary<string, VarLocation> _currentVarMap = null!;
    private int _totalLocalSize;             // размер области локальных переменных в стеке

    private readonly Assembler _asm = asm;
    private readonly Dictionary<string, (ulong Address, string Type)> _globalAddresses = [];
    private ulong _nextGlobalAddress = 0x1000;
    private int _labelCounter;
    private string _currentFunction = "";
    private readonly Dictionary<string, FunctionNode> _functionTable = [];

    private string? _currentEpilogueLabel;

    private const uint TMP_REG = (uint)Processor.RegType.r2;

    public void Generate(ProgramNode program)
    {
        // 1. Глобальные переменные
        foreach (var global in program.Globals)
        {
            _globalAddresses[global.Name] = (_nextGlobalAddress, global.Type);
            _nextGlobalAddress += 8;
        }

        // 2. Таблица функций
        _functionTable.Clear();
        foreach (var func in program.Functions)
            _functionTable[func.Name] = func;

        // 3. Выделяем псевдо‑глобальные адреса для параметров всех функций
        foreach (var func in program.Functions)
            AllocatePseudoGlobals(func);

        // 4. Генерация кода функций
        foreach (var func in program.Functions)
            GenerateFunction(func);

        _asm.EmitInstruction(InstructionEncoder.EncodeHALT());
    }

    private void AllocatePseudoGlobals(FunctionNode func)
    {
        foreach (var param in func.Parameters)
        {
            string globalName = $"__param_{func.Name}_{param.Name}";
            _globalAddresses[globalName] = (_nextGlobalAddress, param.Type);
            _nextGlobalAddress += 8;
        }
    }

    private static bool IsComparisonOperator(string op) =>
    op is "==" or "!=" or "<" or ">" or "<=" or ">=";


    // Загрузить значение переменной в r0
    private void LoadVariableToR0(string varName)
    {
        if (_currentVarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
            {
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.MOV,
                    (uint)Processor.RegType.r0,
                    (uint)loc.Register));
            }
            else
            {
                // Загружаем смещение в rTmp, прибавляем rSP, затем LOAD_IND
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(
                    TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.ADD,
                    TMP_REG,
                    (uint)Processor.RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND(
                    (uint)Processor.RegType.r0,
                    TMP_REG,
                    (uint)loc.TypeSize));
            }
        }
        else if (_globalAddresses.TryGetValue(varName, out var gInfo))
        {
            var opSize = GetSizeForType(gInfo.Type);
            _asm.EmitInstruction64(InstructionEncoder.EncodeLOAD(
                (uint)Processor.RegType.r0, (uint)opSize), gInfo.Address);
        }
        else
            throw new Exception($"Undefined variable: {varName}");
    }

    // Сохранить значение из r0 в переменную
    private void StoreR0ToVariable(string varName)
    {
        if (_currentVarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
            {
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.MOV,
                    (uint)loc.Register,
                    (uint)Processor.RegType.r0));
            }
            else
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(
                    TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.ADD,
                    TMP_REG,
                    (uint)Processor.RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND(
                    (uint)Processor.RegType.r0,
                    TMP_REG,
                    (uint)loc.TypeSize));
            }
        }
        else if (_globalAddresses.TryGetValue(varName, out var gInfo))
        {
            var opSize = GetSizeForType(gInfo.Type);
            _asm.EmitInstruction64(InstructionEncoder.EncodeSTORE(
                (uint)Processor.RegType.r0, (uint)opSize), gInfo.Address);
        }
        else
            throw new Exception($"Undefined variable: {varName}");
    }

    private static Processor.OpCodeSize GetSizeForType(string type) => type switch
    {
        "byte" => Processor.OpCodeSize.S8,
        "ushort" => Processor.OpCodeSize.S16,
        "ulong" => Processor.OpCodeSize.S64,
        "int" or "char" => Processor.OpCodeSize.S32,   // int 32 бита, char тоже читаем как 32 (младший байт)
        _ => throw new Exception($"Unknown type '{type}' for memory size")
    };

    private void AllocateLocalVariables(BlockNode block, string funcName)
    {
        foreach (var stmt in block.Statements)
        {
            if (stmt is VariableNode var)
            {
                string globalName = $"__local_{funcName}_{var.Name}";
                _globalAddresses[globalName] = (_nextGlobalAddress, var.Type);
                _nextGlobalAddress += 8;
            }
            else if (stmt is BlockNode nested)
                AllocateLocalVariables(nested, funcName);
        }
    }


    private void GenerateFunction(FunctionNode func)
    {
        _asm.MarkLabel($"func_{func.Name}");
        _currentFunction = func.Name;
        _currentEpilogueLabel = $"__epilogue_{func.Name}";

        // ---- 1. Собираем все переменные (параметры + локальные) ----
        var allVars = new List<(string name, string type)>();
        foreach (var param in func.Parameters)
            allVars.Add((param.Name, param.Type));

        // Локальные переменные
        var localMap = new Dictionary<string, string>();
        void CollectLocalVars(BlockNode block)
        {
            foreach (var stmt in block.Statements)
            {
                if (stmt is VariableNode var)
                    localMap[var.Name] = var.Type;
                else if (stmt is BlockNode nested) CollectLocalVars(nested);
                else if (stmt is IfNode ifn) { CollectLocalVars(ifn.ThenBlock); if (ifn.ElseBlock != null) CollectLocalVars(ifn.ElseBlock); }
                else if (stmt is WhileNode wh) CollectLocalVars(wh.Body);
                else if (stmt is ForNode fr) CollectLocalVars(fr.Body);
            }
        }
        CollectLocalVars(func.Body);

        // Добавляем локальные переменные в общий список
        foreach (var kv in localMap)
            if (!allVars.Any(v => v.name == kv.Key))
                allVars.Add((kv.Key, kv.Value));

        // ---- 2. Распределение регистров и стека ----
        var varMap = new Dictionary<string, VarLocation>();
        int nextReg = 4;          // начинаем с r4
        int stackOffset = 0;      // смещение для стековых переменных (все выравниваем по 8)

        foreach (var (name, type) in allVars)
        {
            var size = GetSizeForType(type);
            if (nextReg <= 22)
            {
                varMap[name] = new VarLocation
                {
                    IsRegister = true,
                    Register = (Processor.RegType)nextReg,
                    TypeSize = size
                };
                nextReg++;
            }
            else
            {
                varMap[name] = new VarLocation
                {
                    IsRegister = false,
                    StackOffset = stackOffset,
                    TypeSize = size
                };
                stackOffset += 8;   // все данные выравниваем по 8 байт
            }
        }
        _currentVarMap = varMap;
        _totalLocalSize = stackOffset;

        // Список регистров, используемых для переменных (для PUSH/POP)
        var usedRegs = varMap.Values.Where(v => v.IsRegister).Select(v => v.Register).ToList();

        bool isMain = func.Name == "main";

        // ---- 3. Пролог ----
        if (!isMain)
        {
            // Сохраняем используемые регистры
            foreach (var reg in usedRegs.OrderBy(r => (int)r))
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)Processor.OpCode.PUSH, (uint)reg));

            // Загружаем параметры из глобальной памяти в регистры
            foreach (var param in func.Parameters)
            {
                if (!varMap.TryGetValue(param.Name, out var loc) || !loc.IsRegister)
                    throw new Exception($"Parameter '{param.Name}' not allocated to a register");
                string globalName = $"__param_{func.Name}_{param.Name}";
                var (addr, _) = _globalAddresses[globalName];
                var size = GetSizeForType(param.Type);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLOAD((uint)loc.Register, (uint)size), addr);
            }

            // Выделяем место для локальных переменных в стеке
            if (_totalLocalSize > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(TMP_REG), (ulong)_totalLocalSize);
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.SUB,
                    (uint)Processor.RegType.rSP, TMP_REG));
            }
        }
        else
        {
            // Для main – тоже выделяем стек для локальных
            if (_totalLocalSize > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(TMP_REG), (ulong)_totalLocalSize);
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.SUB,
                    (uint)Processor.RegType.rSP, TMP_REG));
            }
        }

        // ---- 4. Тело функции ----
        GenerateBlock(func.Body);

        // ---- 5. Эпилог ----
        if (isMain)
        {
            _asm.EmitInstruction(InstructionEncoder.EncodeHALT());
        }
        else
        {
            _asm.MarkLabel(_currentEpilogueLabel!);

            // Освобождаем стек локальных переменных
            if (_totalLocalSize > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(TMP_REG), (ulong)_totalLocalSize);
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.ADD,
                    (uint)Processor.RegType.rSP, TMP_REG));
            }

            // Восстанавливаем регистры в обратном порядке
            for (int i = usedRegs.Count - 1; i >= 0; i--)
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)Processor.OpCode.POP, (uint)usedRegs[i]));

            _asm.EmitInstruction(InstructionEncoder.EncodeRET());
        }
    }
    private static void BuildLocalMap(BlockNode block, string funcName, Dictionary<string, (string GlobalName, string Type)> localMap)
    {
        foreach (var stmt in block.Statements)
        {
            if (stmt is VariableNode var)
            {
                string globalName = $"__local_{funcName}_{var.Name}";
                localMap[var.Name] = (globalName, var.Type);
            }
            else if (stmt is BlockNode nested)
                BuildLocalMap(nested, funcName, localMap);
        }
    }

    private void GenerateBlock(BlockNode block)
    {
        foreach (var stmt in block.Statements)
        {
            switch (stmt)
            {
                case VariableNode var:
                    if (var.Initializer != null)
                    {
                        GenerateExpression(var.Initializer);
                        StoreR0ToVariable(var.Name);
                    }
                    break;
                case AssignmentNode assign:
                    GenerateAssignment(assign);
                    break;
                case BinaryOpNode binop:
                    GenerateBinaryOp(binop);
                    break;
                case UnaryOpNode unop:
                    GenerateUnaryOp(unop);
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
                    GenerateFunctionCall(call);
                    break;
                case InPortNode inPort:
                    GenerateInPort(inPort);
                    break;
                case OutPortNode outPort:
                    GenerateOutPort(outPort);
                    break;
                default:
                    // Игнорируем прочие (например, просто выражение)
                    break;
            }
        }
    }

    private void GenerateAssignment(AssignmentNode assign)
    {
        // === ОПТИМИЗАЦИЯ: x = x +/- y / x = x +/- 1 (только если x в регистре) ===
        if (assign.Value is BinaryOpNode binop &&
            (binop.Operator == "+" || binop.Operator == "-") &&
            binop.Left is IdentifierNode leftId &&
            leftId.Name == assign.Name &&
            _currentVarMap.TryGetValue(assign.Name, out var xLoc) &&
            xLoc.IsRegister) // проверяем, что x находится в регистре
        {
            Processor.RegType xReg = xLoc.Register;
            // Случай x = x + 1 или x = x - 1
            if (binop.Right is NumberNode num)
            {
                if (num.Value == 1 && binop.Operator == "+")
                    _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)Processor.OpCode.INC, (uint)xReg));
                else if (num.Value == 1 && binop.Operator == "-")
                    _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)Processor.OpCode.DEC, (uint)xReg));
                else
                {
                    _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), (ulong)num.Value);
                    if (binop.Operator == "+")
                        _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.ADD, (uint)xReg, (uint)Processor.RegType.r0));
                    else
                        _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.SUB, (uint)xReg, (uint)Processor.RegType.r0));
                }
                return;
            }
            // Случай x = x + y (y - переменная)
            else if (binop.Right is IdentifierNode rightId)
            {
                // Загружаем значение y в r0 (используем вспомогательный метод)
                LoadVariableToR0(rightId.Name);
                if (binop.Operator == "+")
                    _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.ADD, (uint)xReg, (uint)Processor.RegType.r0));
                else
                    _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.SUB, (uint)xReg, (uint)Processor.RegType.r0));
                return;
            }
            // Общий случай: сложное выражение справа
            else
            {
                GenerateExpression(binop.Right);
                if (binop.Operator == "+")
                    _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.ADD, (uint)xReg, (uint)Processor.RegType.r0));
                else
                    _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.SUB, (uint)xReg, (uint)Processor.RegType.r0));
                return;
            }
        }

        // === Обычное присваивание ===
        GenerateExpression(assign.Value);
        StoreR0ToVariable(assign.Name);
    }

    private void GenerateExpression(ASTNode expr)
    {
        switch (expr)
        {
            case NumberNode num:
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), (ulong)num.Value);
                break;
            case IdentifierNode id:
                LoadVariableToR0(id.Name);
                break;

            case BinaryOpNode binop:
                GenerateBinaryOp(binop);
                break;

            case UnaryOpNode unop:
                GenerateUnaryOp(unop);
                break;

            case FunctionCallNode call:
                GenerateFunctionCall(call);
                break;

            default:
                throw new Exception($"Unsupported expression: {expr.GetType()}");
        }
    }
    private void GenerateBinaryOp(BinaryOpNode binop)
    {
        GenerateExpression(binop.Left);
        _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.MOV, (uint)Processor.RegType.r1, (uint)Processor.RegType.r0));
        GenerateExpression(binop.Right);

        switch (binop.Operator)
        {
            case "+":
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.ADD, (uint)Processor.RegType.r1, (uint)Processor.RegType.r0));
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.MOV, (uint)Processor.RegType.r0, (uint)Processor.RegType.r1));
                break;
            case "-":
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.SUB, (uint)Processor.RegType.r1, (uint)Processor.RegType.r0));
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)Processor.OpCode.MOV, (uint)Processor.RegType.r0, (uint)Processor.RegType.r1));
                break;
            case "*":
            case "/":
                throw new NotImplementedException($"Operator '{binop.Operator}' not implemented");
            case "<":
            case ">":
            case "<=":
            case ">=":
            case "==":
            case "!=":
                GenerateComparison(binop.Operator);
                break;
            case "&&":
            case "||":
                // Заглушка
                break;
            default:
                throw new Exception($"Unsupported operator: {binop.Operator}");
        }
    }
    // Генерация кода для условия, которое НЕ ВОЗВРАЩАЕТ значение,
    // а сразу переходит на trueLabel/falseLabel
    private void GenerateCondition(ASTNode condition, string? trueLabel, string? falseLabel)
    {
        if (condition is BinaryOpNode binop && IsComparisonOperator(binop.Operator))
        {
            // Вычисляем левый операнд -> r0, сохраняем в r1
            GenerateExpression(binop.Left);
            _asm.EmitInstruction(InstructionEncoder.EncodeR(
                (uint)Processor.OpCode.MOV, (uint)Processor.RegType.r1, (uint)Processor.RegType.r0));
            // Правый операнд -> r0
            GenerateExpression(binop.Right);
            // Вычитание r1 = r1 - r0 (флаги установятся)
            _asm.EmitInstruction(InstructionEncoder.EncodeR(
                (uint)Processor.OpCode.SUB, (uint)Processor.RegType.r1, (uint)Processor.RegType.r0));

            // Переходы по флагам
            GenerateComparisonJump(binop.Operator, trueLabel, falseLabel);
        }
        else
        {
            // Обычное выражение: вычисляем в r0, проверяем на 0
            GenerateExpression(condition);
            if (trueLabel != null && falseLabel != null)
            {
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JNZ), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), falseLabel);
            }
            else if (trueLabel != null)
            {
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JNZ), trueLabel);
            }
            else if (falseLabel != null)
            {
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JZ), falseLabel);
            }
            // если обе метки null – ничего не делаем
        }
    }
    // Генерация переходов на основе флагов после SUB r1,r0
    private void GenerateComparisonJump(string op, string? trueLabel, string? falseLabel)
    {
        // Вспомогательные локальные функции для эмиссии переходов
        void EmitJmp(string label) =>
            _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), label);
        void EmitCond(Processor.OpCode jmpOp, string label) =>
            _asm.EmitJump(InstructionEncoder.EncodeJ((uint)jmpOp), label);

        // Если нужна только истина (при лжи – провалиться)
        if (trueLabel != null && falseLabel == null)
        {
            string skip = GetLabel(); // сюда перейдём, если условие ложно
            GenerateComparisonJump(op, trueLabel, skip); // полная форма
            _asm.MarkLabel(skip);
            return;
        }
        // Если нужна только ложь (при истине – провалиться)
        if (falseLabel != null && trueLabel == null)
        {
            string skip = GetLabel();
            GenerateComparisonJump(op, skip, falseLabel);
            _asm.MarkLabel(skip);
            return;
        }
        // Если обе метки null – ничего не делаем
        if (trueLabel == null && falseLabel == null)
            return;

        // Обе метки заданы
        switch (op)
        {
            case "==": EmitCond(Processor.OpCode.JZ, trueLabel!); EmitJmp(falseLabel!); break;
            case "!=": EmitCond(Processor.OpCode.JNZ, trueLabel!); EmitJmp(falseLabel!); break;
            case "<": EmitCond(Processor.OpCode.JL, trueLabel!); EmitJmp(falseLabel!); break;
            case ">":
                // > : не (Zero или Negative) → если Z или N, то ложно
                EmitCond(Processor.OpCode.JL, falseLabel!);
                EmitCond(Processor.OpCode.JZ, falseLabel!);
                EmitJmp(trueLabel!);
                break;
            case "<=":
                // <= : Zero или Negative → истина
                EmitCond(Processor.OpCode.JL, trueLabel!);
                EmitCond(Processor.OpCode.JZ, trueLabel!);
                EmitJmp(falseLabel!);
                break;
            case ">=":
                // >= : Negative == 0 → если Negative, то ложно
                EmitCond(Processor.OpCode.JL, falseLabel!);
                EmitJmp(trueLabel!);
                break;
        }
    }
    private void GenerateComparison(string op)
    {
        // r1 = левый, r0 = правый; вычисляем r1 - r0
        _asm.EmitInstruction(InstructionEncoder.EncodeR(
            (uint)Processor.OpCode.SUB,
            (uint)Processor.RegType.r1,
            (uint)Processor.RegType.r0));
        _asm.EmitInstruction(InstructionEncoder.EncodeR(
            (uint)Processor.OpCode.MOV,
            (uint)Processor.RegType.r0,
            (uint)Processor.RegType.r1));

        string trueLabel = GetLabel();
        string endLabel = GetLabel();

        // По умолчанию r0 = 0 (ложь)
        _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 0);

        switch (op)
        {
            case "<":
                // negative flag (bit 1)
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.MOV,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r2), 2);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.AND,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JNZ), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;

            case ">":
                // not zero and not negative
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.MOV,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r2), 3);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.AND,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JZ), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;

            case "<=":
                // zero or negative
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.MOV,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r2), 3);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.AND,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JNZ), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;

            case ">=":
                // not negative
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.MOV,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r2), 2);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.AND,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JZ), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;

            case "==":
                // zero flag
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.MOV,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r2), 1);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.AND,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JNZ), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;

            case "!=":
                // zero == 0
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.MOV,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r2), 1);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.AND,
                    (uint)Processor.RegType.r1,
                    (uint)Processor.RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JZ), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;

            default:
                throw new NotImplementedException($"Comparison '{op}' not implemented");
        }
    }
    private void GenerateUnaryOp(UnaryOpNode unop)
    {
        switch (unop.Operator)
        {
            case "-":
                GenerateExpression(unop.Operand);
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)Processor.OpCode.NOT, (uint)Processor.RegType.r0));
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)Processor.OpCode.INC, (uint)Processor.RegType.r0));
                break;

            case "!":
                GenerateExpression(unop.Operand);
                string trueLabel = GetLabel();
                string endLabel = GetLabel();
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 1);
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JNZ), trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 0);
                _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)Processor.RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;

            case "~":
                GenerateExpression(unop.Operand);
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)Processor.OpCode.NOT, (uint)Processor.RegType.r0));
                break;

            default:
                throw new Exception($"Unsupported unary operator: {unop.Operator}");
        }
    }

    private void GenerateIf(IfNode ifNode)
    {
        string elseLabel = GetLabel();
        string endLabel = ifNode.ElseBlock != null ? GetLabel() : elseLabel;

        // Если условие ложно – перейти на elseLabel; иначе провалиться в then
        GenerateCondition(ifNode.Condition, null, elseLabel);

        GenerateBlock(ifNode.ThenBlock);

        if (ifNode.ElseBlock != null)
        {
            _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), endLabel);
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
        string startLabel = GetLabel();
        string endLabel = GetLabel();

        _asm.MarkLabel(startLabel);
        GenerateCondition(whileNode.Condition, null, endLabel); // ложно → конец цикла
        GenerateBlock(whileNode.Body);
        _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), startLabel);
        _asm.MarkLabel(endLabel);
    }
    private void GenerateFor(ForNode forNode)
    {
        if (forNode.Init != null)
            GenerateExpression(forNode.Init);

        string startLabel = GetLabel();
        string endLabel = GetLabel();

        _asm.MarkLabel(startLabel);
        if (forNode.Condition != null)
            GenerateCondition(forNode.Condition, null, endLabel); // ложно → конец

        GenerateBlock(forNode.Body);

        if (forNode.Increment != null)
            GenerateExpression(forNode.Increment);

        _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), startLabel);
        _asm.MarkLabel(endLabel);
    }

    private void GenerateReturn(ReturnNode ret)
    {
        if (ret.Value != null)
            GenerateExpression(ret.Value);
        if (_currentFunction == "main")
            _asm.EmitInstruction(InstructionEncoder.EncodeHALT());
        else
            _asm.EmitJump(InstructionEncoder.EncodeJ((uint)Processor.OpCode.JMP), _currentEpilogueLabel!);
    }

    private void GenerateFunctionCall(FunctionCallNode call)
    {
        if (!_functionTable.TryGetValue(call.Name, out var targetFunc))
            throw new Exception($"Function '{call.Name}' not found");

        if (call.Arguments.Count != targetFunc.Parameters.Count)
            throw new Exception($"Argument count mismatch for function '{call.Name}'");

        // Передаём аргументы в соответствии с типами параметров
        for (int i = 0; i < call.Arguments.Count; i++)
        {
            GenerateExpression(call.Arguments[i]); // результат в r0
            var param = targetFunc.Parameters[i];
            string paramType = param.Type;
            string globalName = $"__param_{targetFunc.Name}_{param.Name}";
            if (!_globalAddresses.TryGetValue(globalName, out var paramInfo))
                throw new Exception($"Parameter global not found: {globalName}");

            var opSize = GetSizeForType(paramType);
            _asm.EmitInstruction64(
                InstructionEncoder.EncodeSTORE((uint)Processor.RegType.r0, (uint)opSize),
                paramInfo.Address);
        }

        _asm.EmitJump(InstructionEncoder.EncodeCALL(), $"func_{call.Name}");
    }

    private void GenerateInPort(InPortNode inPort)
    {
        // Порт -> r0
        GenerateExpression(inPort.Port);
        // IN r1, r0  (прочитает из порта r0 в r1)
        _asm.EmitInstruction(InstructionEncoder.EncodeIN(
            (uint)Processor.RegType.r1, (uint)Processor.RegType.r0));
        // Сохранить r1 в переменную, указанную вторым аргументом
        StoreToVariable(inPort.DataVar.Name, Processor.RegType.r1);
    }

    private void GenerateOutPort(OutPortNode outPort)
    {
        // Вычисляем значение -> r1
        GenerateExpression(outPort.Value);
        _asm.EmitInstruction(InstructionEncoder.EncodeR(
            (uint)Processor.OpCode.MOV, (uint)Processor.RegType.r1, (uint)Processor.RegType.r0));
        // Порт -> r0
        GenerateExpression(outPort.Port);
        // OUT r1, r0  (запишет из r1 в порт r0)
        _asm.EmitInstruction(InstructionEncoder.EncodeOUT(
            (uint)Processor.RegType.r1, (uint)Processor.RegType.r0));
    }

    private void StoreToVariable(string varName, Processor.RegType srcReg)
    {
        // Если переменная в регистре – просто MOV
        if (_currentVarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
            {
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.MOV, (uint)loc.Register, (uint)srcReg));
            }
            else
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(
                    TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    (uint)Processor.OpCode.ADD, TMP_REG, (uint)Processor.RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND(
                    (uint)srcReg, TMP_REG, (uint)loc.TypeSize));
            }
        }
        else if (_globalAddresses.TryGetValue(varName, out var gInfo))
        {
            var opSize = GetSizeForType(gInfo.Type);
            _asm.EmitInstruction64(
                InstructionEncoder.EncodeSTORE((uint)srcReg, (uint)opSize),
                gInfo.Address);
        }
        else
            throw new Exception($"Undefined variable: {varName}");
    }

    private string GetLabel() => $"L{_labelCounter++}";
}
