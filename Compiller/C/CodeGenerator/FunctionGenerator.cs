using Compiller.ASM;
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// FunctionGenerator – генерация программы и функций
// ============================================================
public class FunctionGenerator(Assembler asm, Dictionary<string, StructLayout> structTable)
{
    private readonly Assembler _asm = asm;
    private GlobalMemoryManager _globalMem = null!;
    private readonly Dictionary<string, FunctionNode> _functionTable = [];
    private readonly Dictionary<string, StructLayout> _structTable = structTable;
    private int _labelCounter;

    public void Generate(ProgramNode program)
    {
        _globalMem = new GlobalMemoryManager(_structTable);

        //foreach (var s in program.Structs)
        //{
        //    _structTable[s.Name] = new(s.Name, s.Fields, _structTable);
        //}

        foreach (var global in program.Globals)
        {
            _globalMem.Allocate(global.Name, global.Type, global.IsArray, global.IsPointer, global.PointedType, global.ArraySize);
        }


        _functionTable.Clear();
        foreach (var func in program.Functions)
            _functionTable[func.Name] = func;

        foreach (var func in program.Functions)
            _globalMem.AllocatePseudoGlobals(func);

        GenerateGlobalInit(program);

        foreach (var func in program.Functions)
        {
            if (func.IsExternal) continue;
            GenerateFunction(func);
        }
    }

    private void GenerateGlobalInit(ProgramNode program)
    {
        // Временный контекст для вычисления глобальных инициализаторов
        var dummyCtx = new FunctionContext(); // не используется для varMap
        var getLabel = GetLabel;
        var exprGen = new ExpressionGenerator(_asm, _globalMem, dummyCtx, _functionTable, getLabel, _structTable);

        foreach (var global in program.Globals)
        {
            if (global.Initializer != null)
            {
                // Пропускаем структуры – их инициализация пока не поддерживается
                if (CodeGenUtils.IsStructType(global.Type, _structTable))
                    continue;

                exprGen.GenerateExpression(global.Initializer);
                if (_globalMem.TryGetAddress(global.Name, out var addr))
                {
                    var gInfo = _globalMem.GetInfo(global.Name)!.Value;
                    var opSize = global.IsPointer ? OpCodeSize.S64 : CodeGenUtils.GetSizeForType(global.Type);
                    _asm.EmitInstruction64(InstructionEncoder.EncodeSTORE((uint)RegType.r0, opSize.Uint), addr);
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
        var funcCtx = FunctionContext.Create(func, localVarNodes, _structTable);

        // Генераторы для тела функции
        var getLabel = GetLabel;
        var exprGen = new ExpressionGenerator(_asm, _globalMem, funcCtx, _functionTable, getLabel, _structTable);
        var stmtGen = new StatementGenerator(_asm, exprGen, funcCtx, getLabel, _globalMem, _structTable);

        bool isMain = func.Name == "main";

        // Пролог
        if (!isMain)
        {
            foreach (var reg in funcCtx.UsedRegisters.OrderBy(r => (int)r))
                _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.PUSH.Uint, (uint)reg));

            foreach (var param in func.Parameters)
            {
                if (!funcCtx.VarMap.TryGetValue(param.Name, out var loc) || !loc.IsRegister)
                    throw new Exception($"Parameter '{param.Name}' not allocated to a register");
                string globalName = $"__param_{func.Name}_{param.Name}";
                var addr = _globalMem.GetInfo(globalName)!.Value.Address;
                OpCodeSize size;
                if (param.IsPointer)
                    size = OpCodeSize.S64;
                else if (_structTable.ContainsKey(param.Type))
                    size = OpCodeSize.S64;   // значение-структура пока не передаётся в регистр, но на всякий случай
                else
                    size = CodeGenUtils.GetSizeForType(param.Type);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLOAD((uint)loc.Register, size.Uint), addr);
            }

            if (funcCtx.TotalLocalSize > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)funcCtx.TotalLocalSize);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.rSP, CodeGenUtils.TMP_REG));
            }
        }
        else
        {
            if (funcCtx.TotalLocalSize > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)funcCtx.TotalLocalSize);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.rSP, CodeGenUtils.TMP_REG));
            }
        }

        // Тело
        stmtGen.GenerateBlock(func.Body);

        // Эпилог
        if (isMain)
        {
            _asm.EmitInstruction(InstructionEncoder.EncodeEND());
        }
        else
        {
            _asm.MarkLabel(funcCtx.EpilogueLabel);
            if (funcCtx.TotalLocalSize > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)funcCtx.TotalLocalSize);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.rSP, CodeGenUtils.TMP_REG));
            }
            for (int i = funcCtx.UsedRegisters.Count - 1; i >= 0; i--)
                _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.POP.Uint, (uint)funcCtx.UsedRegisters[i]));
            _asm.EmitInstruction(InstructionEncoder.EncodeRET());
        }
    }

    private static void CollectLocalVars(BlockNode block, Dictionary<string, VariableNode> map)
    {
        foreach (var stmt in block.Statements)
        {
            switch (stmt)
            {
                case VariableNode varNode:
                    map[varNode.Name] = varNode;
                    break;
                case BlockNode nested:
                    CollectLocalVars(nested, map);
                    break;
                case IfNode ifn:
                    CollectLocalVars(ifn.ThenBlock, map); if (ifn.ElseBlock != null) CollectLocalVars(ifn.ElseBlock, map); break;
                case WhileNode wh:
                    CollectLocalVars(wh.Body, map);
                    break;
                case ForNode fr:
                    {
                        if (fr.Init is VariableNode forVar)
                            map[forVar.Name] = forVar;
                        CollectLocalVars(fr.Body, map);
                        break;
                    }
            }
        }
    }

    private string GetLabel() => $"L{_labelCounter++}";
}