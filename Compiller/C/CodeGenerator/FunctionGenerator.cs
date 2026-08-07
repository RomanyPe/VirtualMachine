using Compiller.ASM;
using static Kernel.ProcessorSystem.Processor;

namespace Compiller.C.CodeGenerator;

// ============================================================
// FunctionGenerator – генерация программы и функций
// ============================================================
public class FunctionGenerator(Assembler asm)
{
    private readonly Assembler _asm = asm;
    private readonly GlobalMemoryManager _globalMem = new();
    private readonly Dictionary<string, FunctionNode> _functionTable = [];
    private int _labelCounter;

    public void Generate(ProgramNode program)
    {
        // 1. Глобальные переменные
        foreach (var global in program.Globals)
        {
            _globalMem.Allocate(global.Name, global.Type, global.IsArray, global.IsPointer, global.PointedType, global.ArraySize);
        }

        // 2. Таблица функций
        _functionTable.Clear();
        foreach (var func in program.Functions)
            _functionTable[func.Name] = func;

        // 3. Псевдо‑глобальные адреса параметров
        foreach (var func in program.Functions)
            _globalMem.AllocatePseudoGlobals(func);

        // 4. Глобальная инициализация
        GenerateGlobalInit(program);

        // 5. Генерация кода функций (main уже первый)
        foreach (var func in program.Functions)
        {
            if (func.IsExternal) continue; // не генерируем тело
            GenerateFunction(func);
        }
    }

    private void GenerateGlobalInit(ProgramNode program)
    {
        // Временный контекст для вычисления глобальных инициализаторов
        var dummyCtx = new FunctionContext(); // не используется для varMap
        var getLabel = GetLabel;
        var exprGen = new ExpressionGenerator(_asm, _globalMem, dummyCtx, _functionTable, getLabel);

        foreach (var global in program.Globals)
        {
            if (global.Initializer != null)
            {
                exprGen.GenerateExpression(global.Initializer);
                if (_globalMem.TryGetAddress(global.Name, out var addr))
                {
                    var gInfo = _globalMem.GetInfo(global.Name)!.Value;
                    var opSize = global.IsPointer ? OpCodeSize.S64 : CodeGenUtils.GetSizeForType(global.Type);
                    _asm.EmitInstruction64(InstructionEncoder.EncodeSTORE((uint)RegType.r0, (uint)opSize), addr);
                }
            }
        }
    }

    private void GenerateFunction(FunctionNode func)
    {
        _asm.MarkLabel($"func_{func.Name}");

        // Сбор локальных переменных
        var localVarNodes = new Dictionary<string, VariableNode>();
        CollectLocalVars(func.Body, localVarNodes);

        // Выделение псевдо‑глобальных адресов для локальных переменных
        _globalMem.AllocateLocalGlobals(func.Body, func.Name);

        // Создание контекста функции
        var funcCtx = FunctionContext.Create(func, localVarNodes);

        // Генераторы для тела функции
        var getLabel = GetLabel;
        var exprGen = new ExpressionGenerator(_asm, _globalMem, funcCtx, _functionTable, getLabel);
        var stmtGen = new StatementGenerator(_asm, exprGen, funcCtx, getLabel, _globalMem);

        bool isMain = func.Name == "main";

        // Пролог
        if (!isMain)
        {
            foreach (var reg in funcCtx.UsedRegisters.OrderBy(r => (int)r))
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.PUSH, (uint)reg));

            foreach (var param in func.Parameters)
            {
                if (!funcCtx.VarMap.TryGetValue(param.Name, out var loc) || !loc.IsRegister)
                    throw new Exception($"Parameter '{param.Name}' not allocated to a register");
                string globalName = $"__param_{func.Name}_{param.Name}";
                var addr = _globalMem.GetInfo(globalName)!.Value.Address;
                var size = param.IsPointer ? OpCodeSize.S64 : CodeGenUtils.GetSizeForType(param.Type);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLOAD((uint)loc.Register, (uint)size), addr);
            }

            if (funcCtx.TotalLocalSize > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)funcCtx.TotalLocalSize);
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.SUB, (uint)RegType.rSP, CodeGenUtils.TMP_REG));
            }
        }
        else
        {
            if (funcCtx.TotalLocalSize > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)funcCtx.TotalLocalSize);
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.SUB, (uint)RegType.rSP, CodeGenUtils.TMP_REG));
            }
        }

        // Тело
        stmtGen.GenerateBlock(func.Body);

        // Эпилог
        if (isMain)
        {
            _asm.EmitInstruction(InstructionEncoder.EncodeHALT());
        }
        else
        {
            _asm.MarkLabel(funcCtx.EpilogueLabel);
            if (funcCtx.TotalLocalSize > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)funcCtx.TotalLocalSize);
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.ADD, (uint)RegType.rSP, CodeGenUtils.TMP_REG));
            }
            for (int i = funcCtx.UsedRegisters.Count - 1; i >= 0; i--)
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.POP, (uint)funcCtx.UsedRegisters[i]));
            _asm.EmitInstruction(InstructionEncoder.EncodeRET());
        }
    }

    private static void CollectLocalVars(BlockNode block, Dictionary<string, VariableNode> map)
    {
        foreach (var stmt in block.Statements)
        {
            if (stmt is VariableNode varNode)
                map[varNode.Name] = varNode;
            else if (stmt is BlockNode nested) CollectLocalVars(nested, map);
            else if (stmt is IfNode ifn) { CollectLocalVars(ifn.ThenBlock, map); if (ifn.ElseBlock != null) CollectLocalVars(ifn.ElseBlock, map); }
            else if (stmt is WhileNode wh) CollectLocalVars(wh.Body, map);
            else if (stmt is ForNode fr)
            {
                if (fr.Init is VariableNode forVar)
                    map[forVar.Name] = forVar;
                CollectLocalVars(fr.Body, map);
            }
        }
    }

    private string GetLabel() => $"L{_labelCounter++}";
}