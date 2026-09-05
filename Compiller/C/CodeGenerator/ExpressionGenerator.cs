using Compiller.ASM;
using Compiller.ASM.Optimizators;
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// ExpressionGenerator – генерация выражений
// ============================================================
public class ExpressionGenerator(AssemblerBase asm,
                                 GlobalMemoryManager globalMem,
                                 FunctionContext funcCtx,
                                 Dictionary<string, FunctionNode> functionTable,
                                 Func<string> getLabel,
                                 Dictionary<string, StructLayout> structTable)
{
    private readonly AssemblerBase _asm = asm;
    private readonly GlobalMemoryManager _globalMem = globalMem;
    private readonly FunctionContext _funcCtx = funcCtx;
    private readonly Dictionary<string, FunctionNode> _functionTable = functionTable;
    private readonly Dictionary<string, StructLayout> _structTable = structTable;
    private readonly Func<string> _getLabel = getLabel;

    public void GenerateExpression(ASTNode expr)
    {
        switch (expr)
        {
            case NumberNode num:
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), (ulong)num.Value);
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
            case ArrayAccessNode arrAcc:
                LoadArrayElementToR0(arrAcc.ArrayName, arrAcc.Index);
                break;
            case AddressOfNode addrOf:
                GenerateAddressOf(addrOf);
                break;
            case DereferenceNode deref:
                GenerateDereference(deref);
                break;
            case NewArrayNode newArr:
                GenerateNewOp(newArr);
                break;
            case MemberAccessNode member:
                GenerateMemberAccess(member);
                break;
            default:
                ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, $"Unsupported expression: {expr.GetType()}");
                break;
        }
    }

    public void GenerateExpression(ASTNode expr, RegType targetReg)
    {
        if (targetReg == RegType.r0)
        {
            GenerateExpression(expr); // обычный путь
            return;
        }

        // Генерируем в другой регистр
        switch (expr)
        {
            case NumberNode num:
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)targetReg), (ulong)num.Value);
                break;
            case IdentifierNode id:
                LoadVariableToReg(id.Name, targetReg);
                break;
            default:
                GenerateExpression(expr); // в r0
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)targetReg, (uint)RegType.r0));
                break;
        }
    }

    private void LoadVariableToReg(string varName, RegType targetReg)
    {
        if (_funcCtx.VarMap.TryGetValue(varName, out var loca) && loca.StructTypeName != null)
            ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, $"Direct load/store of struct variable '{varName}' is not supported.");
        if (_globalMem.Contains(varName))
        {
            var gInfo = _globalMem.GetInfo(varName)!.Value;
            if (CodeGenUtils.IsStructType(gInfo.Type, _structTable))
                ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, $"Direct load/store of struct variable '{varName}' is not supported.");
        }

        if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
            {
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    OpCode.MOV.Uint, (uint)targetReg, (uint)loc.Register));
            }
            else
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)targetReg, CodeGenUtils.TMP_REG, (uint)loc.TypeSize));
            }
        }
        else if (_globalMem.TryGetAddress(varName, out var addr))
        {
            GlobalInfo? gInfo = _globalMem.GetInfo(varName)!;
            var opSize = CodeGenUtils.GetSizeForType(gInfo.Value.Type);
            _asm.EmitInstruction64(InstructionEncoder.EncodeLOAD((uint)targetReg, (uint)opSize), addr);
        }
        else
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, varName);
    }

    public void GenerateMemberAddress(MemberAccessNode node)
    {
        string? structType = null;
        if (node.Object is IdentifierNode id)
        {
            // Ищем в локальных или глобальных
            if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
            {
                if (loc.StructTypeName != null)
                    structType = loc.StructTypeName;
                else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
                    structType = loc.PointedType; // ptr->field
                else
                    ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonStructAccess, id.Name);
            }
            else if (_globalMem.Contains(id.Name))
            {
                var gInfo = _globalMem.GetInfo(id.Name)!.Value;
                if (_structTable.ContainsKey(gInfo.Type))
                    structType = gInfo.Type;
                else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
                    structType = gInfo.PointedType;
                else
                    ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonStructAccess, id.Name);
            }
            else
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, id.Name);
        }
        else if (node.Object is MemberAccessNode node1)
        {
            // Рекурсивно вычисляем адрес вложенного объекта (результат в r0)
            GenerateMemberAddress(node1);

            // Получаем ТИП поля node1, чтобы понять, в какой структуре искать текущее поле
            var (_, _, innerFieldType) = ResolveMemberAccessType(node1);

            if (!_structTable.TryGetValue(innerFieldType, out var innerLayout))
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UnknownStructType, $"Unknown struct type '{innerFieldType}'", innerFieldType);

            var currentField = innerLayout.GetField(node.FieldName)
                               ?? ThrowHelper.ThrowMiniC<FieldInfo>(ErrorCode.CodeGen_UnknownField, node.FieldName, innerFieldType);

            if (currentField.Offset > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)currentField.Offset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
            }
            return;
        }
        else if (node.Object is DereferenceNode)
        {
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UnknownStructType, "Dereference member access not implemented yet");
        }
        else if (node.Object is ArrayAccessNode arrAcc)
        {
            // arr[i].field — адрес поля
            string? arrStructType = ResolveArrayStructType(arrAcc.ArrayName);
            var layoutstr = _structTable[arrStructType!];
            var fieldstr = layoutstr.GetField(node.FieldName)
                            ?? ThrowHelper.ThrowMiniC<FieldInfo>(ErrorCode.CodeGen_UnknownField, node.FieldName, arrStructType!);

            // Вычисляем адрес элемента arr[i]
            GenerateExpression(arrAcc.Index);
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r2, (uint)RegType.r0));
            LoadVariableToR0(arrAcc.ArrayName);
            int elemSize = layoutstr.Size;
            if (elemSize > 1)
                CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r2, elemSize);
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, (uint)RegType.r2));
            // r0 = адрес arr[i]

            // Прибавляем смещение поля
            if (fieldstr.Offset > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)fieldstr.Offset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
            }
            // Готово: r0 = адрес поля
            return;
        }
        else
        {
            ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, "Unsupported object for member access");
        }

        if (structType == null)
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UnknownStructType, "Cannot determine struct type for member access");

        var layout = _structTable[structType];
        var field = layout.GetField(node.FieldName) ?? ThrowHelper.ThrowMiniC<FieldInfo>(ErrorCode.CodeGen_UnknownField, node.FieldName, structType);

        if (node.IsArrow)
        {
            GenerateExpression(node.Object);  // r0 = ptr
        }
        else
        {
            if (node.Object is IdentifierNode identifer)
                LoadAddressToR0(identifer.Name);
            else ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_InvalidDotAccess, "Dot address only for simple variables");
        }
        if (field.Offset > 0)
        {
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)field.Offset);
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
        }
    }


    private void GenerateMemberAccess(MemberAccessNode node)
    {
        // 1. Определить StructLayout
        // Нужно узнать тип объекта. Он может быть переменной (IdentifierNode) или другим выражением.
        string? structType = null;
        switch (node.Object)
        {
            case IdentifierNode id:
                {
                    // Ищем в локальных или глобальных
                    if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
                    {
                        if (loc.StructTypeName != null)
                            structType = loc.StructTypeName;
                        else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
                            structType = loc.PointedType; // ptr->field
                        else ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonStructAccess, id.Name);
                    }
                    else if (_globalMem.Contains(id.Name))
                    {
                        var gInfo = _globalMem.GetInfo(id.Name)!.Value;
                        if (_structTable.ContainsKey(gInfo.Type))
                            structType = gInfo.Type;
                        else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
                            structType = gInfo.PointedType;
                        else ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonStructAccess, id.Name);
                    }
                    else
                        ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, id.Name);
                    break;
                }

            case MemberAccessNode:
                {
                    // Рекурсивно вычисляем адрес вложенного объекта (без загрузки значения) в r0
                    GenerateMemberAddress(node); // этот метод вычислит адрес поля
                                                 // Теперь r0 содержит адрес поля, загружаем значение
                    var (_, fieldMem, _) = ResolveMemberAccessType(node);
                    OpCodeSize fieldSizeMem = CodeGenUtils.GetSizeForType(fieldMem.Type);
                    _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, fieldSizeMem.Uint));
                    return;
                }

            case DereferenceNode:
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UnknownStructType, "Dereference member access not implemented yet");
                return;
            case ArrayAccessNode arrAcc:
                {
                    string? arrStructType = ResolveArrayStructType(arrAcc.ArrayName);
                    var layoutstr = _structTable[arrStructType!];
                    var fieldstr = layoutstr.GetField(node.FieldName)
                                ?? ThrowHelper.ThrowMiniC<FieldInfo>(ErrorCode.CodeGen_UnknownField, node.FieldName, arrStructType!);

                    GenerateExpression(arrAcc.Index);
                    _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r2, (uint)RegType.r0));
                    LoadVariableToR0(arrAcc.ArrayName);
                    int elemSize = layoutstr.Size;
                    if (elemSize > 1)
                        CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r2, elemSize);
                    _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, (uint)RegType.r2));

                    if (fieldstr.Offset > 0)
                    {
                        _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)fieldstr.Offset);
                        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
                    }

                    // Загружаем значение поля
                    OpCodeSize fieldSizestr = CodeGenUtils.GetSizeForType(fieldstr.Type);
                    _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, fieldSizestr.Uint));
                    return;
                }

            default:
                ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, "Unsupported object for member access");
                break;
        }

        if (structType == null)
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UnknownStructType, "Cannot determine struct type for member access");

        var layout = _structTable[structType];
        var field = layout.GetField(node.FieldName) ?? ThrowHelper.ThrowMiniC<FieldInfo>(ErrorCode.CodeGen_UnknownField, node.FieldName, structType);

        if (node.IsArrow)
        {
            GenerateExpression(node.Object);
        }
        else
        {
            if (node.Object is IdentifierNode identifer)
            {
                LoadAddressToR0(identifer.Name);
            }
            else
            {
                ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, "Dot access only supported for simple variables currently");
            }
        }

        if (field.Offset > 0)
        {
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)field.Offset);
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
        }

        // 4. Загрузить значение поля
        OpCodeSize fieldSize = CodeGenUtils.GetSizeForType(field.Type);
        _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, fieldSize.Uint));
    }

    /// <summary>
    /// Возвращает (StructLayout родительской структуры, FieldInfo поля, тип поля)
    /// </summary>
    private (StructLayout layout, FieldInfo field, string fieldType) ResolveMemberAccessType(MemberAccessNode node)
    {
        string? structType = null;

        if (node.Object is IdentifierNode id)
        {
            // локальная или глобальная переменная
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

            if (structType == null)
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonStructAccess, id.Name);
        }
        else if (node.Object is MemberAccessNode innerMember)
        {
            // рекурсивно получаем тип поля внутреннего доступа
            var (_, _, innerFieldType) = ResolveMemberAccessType(innerMember);
            structType = innerFieldType; // тип поля, к которому обращаемся дальше
        }
        else if (node.Object is ArrayAccessNode arrAcc)
        {
            structType = ResolveArrayStructType(arrAcc.ArrayName);
        }
        else if (node.Object is DereferenceNode)
        {
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UnknownStructType, "Dereference in member access not yet supported");
        }
        else
        {
            ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, "Unsupported object for member access");
        }

        if (structType == null)
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UnknownStructType, "Cannot determine struct type for member access");
        
        if (!_structTable.TryGetValue(structType, out var layout))
            ThrowHelper.ThrowMiniC(ErrorCode.NotImplemented, structType);

        var field = layout.GetField(node.FieldName)
                    ?? ThrowHelper.ThrowMiniC<FieldInfo>(ErrorCode.CodeGen_UnknownField, node.FieldName, structType);

        return (layout, field, field.Type);
    }

    public string? ResolveArrayStructType(string arrayName)
    {
        if (_funcCtx.VarMap.TryGetValue(arrayName, out var loc))
        {
            if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
                return loc.PointedType;
            if (loc.StructTypeName != null)
                return loc.StructTypeName;
        }
        else if (_globalMem.Contains(arrayName))
        {
            var gInfo = _globalMem.GetInfo(arrayName)!.Value;
            if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
                return gInfo.PointedType;
            if (_structTable.ContainsKey(gInfo.Type))
                return gInfo.Type;
        }
        ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonIndexableType, arrayName);
        return null;
    }


    public void LoadAddressToR0(string varName)
    {
        if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_CannotGetRegisterAddress, varName);
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
        }
        else if (_globalMem.TryGetAddress(varName, out var addr))
        {
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), addr);
        }
        else ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, varName);
    }

    private void LoadVariableToR0(string varName)
    {
        if (_funcCtx.VarMap.TryGetValue(varName, out var loca) && loca.StructTypeName != null)
            ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, $"Direct load/store of struct variable '{varName}' is not supported.");
        if (_globalMem.Contains(varName))
        {
            var gInfo = _globalMem.GetInfo(varName)!.Value;
            if (CodeGenUtils.IsStructType(gInfo.Type, _structTable))
                ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, $"Direct load/store of struct variable '{varName}' is not supported.");
        }

        if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
            {
                _asm.EmitInstruction(InstructionEncoder.EncodeR(
                    OpCode.MOV.Uint, (uint)RegType.r0, (uint)loc.Register));
            }
            else
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, CodeGenUtils.TMP_REG, (uint)loc.TypeSize));
            }
        }
        else if (_globalMem.TryGetAddress(varName, out var addr))
        {
            GlobalInfo? gInfo = _globalMem.GetInfo(varName)!;
            var opSize = CodeGenUtils.GetSizeForType(gInfo.Value.Type);
            _asm.EmitInstruction64(InstructionEncoder.EncodeLOAD((uint)RegType.r0, (uint)opSize), addr);
        }
        else
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, varName);
    }

    public void StoreR0ToVariable(string varName)
    {
        if (_funcCtx.VarMap.TryGetValue(varName, out var loca) && loca.StructTypeName != null)
            ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, $"Direct load/store of struct variable '{varName}' is not supported.");
        if (_globalMem.Contains(varName))
        {
            var gInfo = _globalMem.GetInfo(varName)!.Value;
            if (CodeGenUtils.IsStructType(gInfo.Type, _structTable))
                ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, $"Direct load/store of struct variable '{varName}' is not supported.");
        }

        if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
            {
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)loc.Register, (uint)RegType.r0));
            }
            else
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r0, CodeGenUtils.TMP_REG, (uint)loc.TypeSize));
            }
        }
        else if (_globalMem.TryGetAddress(varName, out var addr))
        {
            var gInfo = _globalMem.GetInfo(varName)!;
            var opSize = CodeGenUtils.GetSizeForType(gInfo.Value.Type);
            _asm.EmitInstruction64(InstructionEncoder.EncodeSTORE((uint)RegType.r0, (uint)opSize), addr);
        }
        else
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, varName);
    }

    public void LoadArrayElementToR0(string arrayName, ASTNode indexExpr)
    {
        if (_funcCtx.VarMap.TryGetValue(arrayName, out var loc))
        {
            if (loc.IsArray)
                LoadArrayElementFromLocal(loc, indexExpr);
            else if (loc.IsPointer)
                LoadPointerElementToR0(arrayName, loc, indexExpr);
            else
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonIndexableType, arrayName);
        }
        else if (_globalMem.Contains(arrayName))
        {
            var gInfo = _globalMem.GetInfo(arrayName)!.Value;
            if (gInfo.IsArray)
                LoadArrayElementFromGlobal(gInfo, indexExpr);
            else if (gInfo.IsPointer)
                LoadPointerElementToR0(arrayName, null, indexExpr);
            else
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonIndexableType, arrayName);
        }
        else
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, arrayName);
    }

    public void StoreR0ToArrayElement(string arrayName, ASTNode indexExpr)
    {
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));

        if (_funcCtx.VarMap.TryGetValue(arrayName, out var loc))
        {
            if (loc.IsArray)
                StoreArrayElementToLocal(loc, indexExpr);
            else if (loc.IsPointer)
                StorePointerElement(arrayName, loc, indexExpr);
            else
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonIndexableType, arrayName);
        }
        else if (_globalMem.Contains(arrayName))
        {
            var gInfo = _globalMem.GetInfo(arrayName)!.Value;
            if (gInfo.IsArray)
                StoreArrayElementToGlobal(gInfo, indexExpr);
            else if (gInfo.IsPointer)
                StorePointerElement(arrayName, null, indexExpr);
            else
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_NonIndexableType, arrayName);
        }
        else
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, arrayName);
    }

    private void LoadArrayElementFromLocal(VarLocation loc, ASTNode indexExpr)
    {
        var typeSize = loc.TypeSize;
        GenerateExpression(indexExpr);
        if (typeSize != OpCodeSize.S8)
            CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, CodeGenUtils.GetSizeInBytes(typeSize));
        _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.r0));
        _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, CodeGenUtils.TMP_REG, (uint)typeSize));
    }

    private void LoadArrayElementFromGlobal(GlobalInfo gInfo, ASTNode indexExpr)
    {
        var typeSize = CodeGenUtils.GetSizeForType(gInfo.Type);
        GenerateExpression(indexExpr);
        if (typeSize != OpCodeSize.S8)
            CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, CodeGenUtils.GetSizeInBytes(typeSize));
        _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), gInfo.Address);
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.r0));
        _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, CodeGenUtils.TMP_REG, (uint)typeSize));
    }

    private void LoadPointerElementToR0(string pointerName, VarLocation? loc, ASTNode indexExpr)
    {
        // Получаем размер pointed типа
        OpCodeSize pointedSize;
        if (loc != null)
            pointedSize = CodeGenUtils.GetSizeForType(loc.PointedType!);
        else if (_globalMem.Contains(pointerName))
            pointedSize = CodeGenUtils.GetSizeForType(_globalMem.GetInfo(pointerName)!.Value.PointedType!);
        else pointedSize = CodeGenUtils.GetSizeForType("expection type");
        // 1. Вычисляем индекс → r1
        GenerateExpression(indexExpr);   // r0 = индекс
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));
        // 2. Загружаем адрес указателя → r0
        LoadVariableToR0(pointerName);   // r0 = адрес
                                         // 3. Умножаем индекс на размер элемента
        if (pointedSize != OpCodeSize.S8)
            CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r1, CodeGenUtils.GetSizeInBytes(pointedSize));
        // 4. r0 = адрес + смещение
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, (uint)RegType.r1));
        // 5. Загружаем значение
        _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, (uint)pointedSize));
    }

    private void StoreArrayElementToLocal(VarLocation loc, ASTNode indexExpr)
    {
        var typeSize = loc.TypeSize;
        GenerateExpression(indexExpr);
        if (typeSize != OpCodeSize.S8)
            CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, CodeGenUtils.GetSizeInBytes(typeSize));
        _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.r0));
        _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, CodeGenUtils.TMP_REG, (uint)typeSize));
    }

    private void StoreArrayElementToGlobal(GlobalInfo gInfo, ASTNode indexExpr)
    {
        var typeSize = CodeGenUtils.GetSizeForType(gInfo.Type);
        GenerateExpression(indexExpr);
        if (typeSize != OpCodeSize.S8)
            CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, CodeGenUtils.GetSizeInBytes(typeSize));
        _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), gInfo.Address);
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.r0));
        _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, CodeGenUtils.TMP_REG, (uint)typeSize));
    }

    private void StorePointerElement(string pointerName, VarLocation? loc, ASTNode indexExpr)
    {
        OpCodeSize pointedSize;
        if (loc != null)
            pointedSize = CodeGenUtils.GetSizeForType(loc.PointedType!);
        else if (_globalMem.Contains(pointerName))
            pointedSize = CodeGenUtils.GetSizeForType(_globalMem.GetInfo(pointerName)!.Value.PointedType!);
        else
            pointedSize = CodeGenUtils.GetSizeForType("expection type");

        // 1. Вычисляем индекс → r2
        GenerateExpression(indexExpr);
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r2, (uint)RegType.r0));

        // 2. Умножаем индекс на размер элемента (в r2)
        if (pointedSize != OpCodeSize.S8)
        {
            int elemSize = CodeGenUtils.GetSizeInBytes(pointedSize);
            CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r2, elemSize);
        }

        // 3. Загружаем адрес указателя → r0
        LoadVariableToR0(pointerName);

        // 4. r0 = адрес + смещение
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, (uint)RegType.r2));

        // 5. Сохраняем значение из r1 (которое пришло из StoreR0ToArrayElement)
        _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, (uint)RegType.r0, (uint)pointedSize));
    }

    public void GenerateCondition(ASTNode condition, string? trueLabel, string? falseLabel)
    {
        if (condition is BinaryOpNode logical && (logical.Operator == "&&" || logical.Operator == "||"))
        {
            if (logical.Operator == "&&")
            {
                GenerateCondition(logical.Left, null, falseLabel);
                GenerateCondition(logical.Right, trueLabel, falseLabel);
            }
            else
            {
                GenerateCondition(logical.Left, trueLabel, null);
                GenerateCondition(logical.Right, trueLabel, falseLabel);
            }
            return;
        }

        if (condition is BinaryOpNode binop && CodeGenUtils.IsComparisonOperator(binop.Operator))
        {
            GenerateExpression(binop.Left);
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));
            GenerateExpression(binop.Right);
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.r1, (uint)RegType.r0));
            GenerateComparisonJump(binop.Operator, trueLabel, falseLabel);
        }
        else
        {
            GenerateExpression(condition);
            if (trueLabel != null && falseLabel != null)
            {
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), falseLabel);
            }
            else if (trueLabel != null)
            {
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
            }
            else if (falseLabel != null)
            {
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), falseLabel);
            }
        }
    }

    private void GenerateComparisonJump(string op, string? trueLabel, string? falseLabel)
    {
        void EmitJmp(string label) => _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), label);
        void EmitCond(OpCode jmpOp, string label) => _asm.EmitJump(InstructionEncoder.EncodeJ((uint)jmpOp), label);

        if (trueLabel != null && falseLabel == null)
        {
            string skip = _getLabel();
            GenerateComparisonJump(op, trueLabel, skip);
            _asm.MarkLabel(skip);
            return;
        }
        if (falseLabel != null && trueLabel == null)
        {
            string skip = _getLabel();
            GenerateComparisonJump(op, skip, falseLabel);
            _asm.MarkLabel(skip);
            return;
        }
        if (trueLabel == null && falseLabel == null)
            return;

        switch (op)
        {
            case "==": EmitCond(OpCode.JZ, trueLabel!); EmitJmp(falseLabel!); break;
            case "!=": EmitCond(OpCode.JNZ, trueLabel!); EmitJmp(falseLabel!); break;
            case "<": EmitCond(OpCode.JL, trueLabel!); EmitJmp(falseLabel!); break;
            case ">":
                EmitCond(OpCode.JL, falseLabel!);
                EmitCond(OpCode.JZ, falseLabel!);
                EmitJmp(trueLabel!);
                break;
            case "<=":
                EmitCond(OpCode.JL, trueLabel!);
                EmitCond(OpCode.JZ, trueLabel!);
                EmitJmp(falseLabel!);
                break;
            case ">=":
                EmitCond(OpCode.JL, falseLabel!);
                EmitJmp(trueLabel!);
                break;
        }
    }

    private void GenerateLogicalAndOr(BinaryOpNode binop)
    {
        string endLabel = _getLabel();
        string trueLabel = _getLabel();
        string falseLabel = _getLabel();

        GenerateExpression(binop.Left);

        if (binop.Operator == "&&")
        {
            _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), falseLabel);
        }
        else
        {
            _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
        }

        GenerateExpression(binop.Right);

        if (binop.Operator == "&&")
        {
            _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), falseLabel);
        }
        else
        {
            _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
        }

        if (binop.Operator == "&&")
        {
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
        }
        else
        {
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 0);
        }
        _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);

        _asm.MarkLabel(trueLabel);
        _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
        _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);

        _asm.MarkLabel(falseLabel);
        _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 0);

        _asm.MarkLabel(endLabel);
    }


    private void GenerateBinaryOp(BinaryOpNode binop)
    {
        if (binop.Operator == "&&" || binop.Operator == "||")
        {
            GenerateLogicalAndOr(binop);
            return;
        }

#if true
        // Сворачивание констант
        if (binop.Left is NumberNode l && binop.Right is NumberNode r)
        {
            long result = ComputeConstant(l.Value, r.Value, binop.Operator);
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), (ulong)result);
            return;
        }
#endif

        // Попытка оптимизации с использованием регистров переменных
        if (TryOptimizeBinaryOpWithRegisters(binop))
            return;

        // Стандартный путь: left -> r0, right -> r1
        GenerateExpression(binop.Left, RegType.r0);
        GenerateExpression(binop.Right, RegType.r1);
        EmitBinaryOperation(binop.Operator, RegType.r0, RegType.r1);
    }

    // Вспомогательный метод для выбора оптимальных регистров
    private bool TryOptimizeBinaryOpWithRegisters(BinaryOpNode binop)
    {
        // 1. Левый операнд - константа, правый - переменная в регистре
        if (binop.Left is NumberNode leftNum && binop.Right is IdentifierNode rightId &&
            _funcCtx.VarMap.TryGetValue(rightId.Name, out var rightLoc) && rightLoc.IsRegister)
        {
            // Генерируем левую константу в r0
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), (ulong)leftNum.Value);
            // Выполняем операцию: r0 = r0 op rightReg
            EmitBinaryOperation(binop.Operator, RegType.r0, rightLoc.Register);
            return true;
        }

        // 2. Левый операнд - переменная в регистре, правый - константа
        if (binop.Left is IdentifierNode leftId && binop.Right is NumberNode rightNum &&
            _funcCtx.VarMap.TryGetValue(leftId.Name, out var leftLoc) && leftLoc.IsRegister)
        {
            // Генерируем правую константу в r1
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r1), (ulong)rightNum.Value);
            // Выполняем операцию: leftReg = leftReg op r1
            EmitBinaryOperation(binop.Operator, leftLoc.Register, RegType.r1);
            // Результат перемещаем в r0
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)leftLoc.Register));
            return true;
        }

        // 3. Оба операнда - переменные в регистрах
        // Для случая оба операнда в регистрах:
        if (binop.Left is IdentifierNode leftId2 && binop.Right is IdentifierNode rightId2 &&
            _funcCtx.VarMap.TryGetValue(leftId2.Name, out var leftLoc2) && leftLoc2.IsRegister &&
            _funcCtx.VarMap.TryGetValue(rightId2.Name, out var rightLoc2) && rightLoc2.IsRegister)
        {
            // Загрузить левый операнд в r0
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)leftLoc2.Register));
            // Выполнить операцию r0 = r0 op r1
            EmitBinaryOperation(binop.Operator, RegType.r0, rightLoc2.Register);
            return true;
        }

        // 4. Левый операнд - переменная в регистре, правый - сложное выражение (не переменная)
        // В этом случае стандартный путь сгенерирует left в r0, right в r1, но это может быть неоптимально.
        // Оставляем стандартный путь.

        return false;
    }

    // Метод для генерации конкретной арифметической операции между двумя регистрами
    private void EmitBinaryOperation(string op, RegType dest, RegType src)
    {
        switch (op)
        {
            case "+": _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)dest, (uint)src)); break;
            case "-": _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)dest, (uint)src)); break;
            case "*": _asm.EmitInstruction(InstructionEncoder.EncodeMULT_INT((uint)dest, (uint)src)); break;
            case "/": _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.DIV.Uint, (uint)dest, (uint)src)); break;
            default: ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, $"Unsupported operator in EmitBinaryOperation: {op}"); break;
        }
    }
    public static long ComputeConstant(long value1, long value2, string op)
    {
        long result = op switch
        {
            "+" => value1 + value2,
            "-" => value1 - value2,
            "*" => value1 * value2,
            "/" => ComputeDiv(value1, value2),
            _ => ThrowHelper.ThrowMiniC<long>(ErrorCode.NotSupported, $"Unsupported operator: {op}"),
        };
        LoggerKernel.LogFromSystem("Compiler", $"ComputeConstant: {value1} {op} {value2} = {result}");
        return result;
    }

    private static long ComputeDiv(long value1, long value2) => value2 != 0 ? value1 / value2
            : ThrowHelper.ThrowMiniC<long>(ErrorCode.NotSupported, $"Div on zero");

    public void GenerateComparison(string op)
    {
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.r1, (uint)RegType.r0));
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));

        string trueLabel = _getLabel();
        string endLabel = _getLabel();
        _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 0);

        switch (op)
        {
            case "<":
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 2);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;
            case ">":
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 3);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;
            case "<=":
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 3);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;
            case ">=":
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 2);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;
            case "==":
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 1);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;
            case "!=":
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 1);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), trueLabel);
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;
            default:
                ThrowHelper.ThrowMiniC(ErrorCode.NotImplemented, op);
                return;
        }
    }

    private void GenerateUnaryOp(UnaryOpNode unop)
    {
        switch (unop.Operator)
        {
            case "-":
                GenerateExpression(unop.Operand);
                _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.NOT.Uint, (uint)RegType.r0));
                _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.INC.Uint, (uint)RegType.r0));
                break;
            case "!":
                GenerateExpression(unop.Operand);
                string trueLabel = _getLabel();
                string endLabel = _getLabel();
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 0);
                _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
                _asm.MarkLabel(trueLabel);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
                _asm.MarkLabel(endLabel);
                break;
            case "~":
                GenerateExpression(unop.Operand);
                _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.NOT.Uint, (uint)RegType.r0));
                break;
            default:
                ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, $"Unsupported unary operator: {unop.Operator}");
                return;
        }
    }

    private void GenerateAddressOf(AddressOfNode addrOf)
    {
        if (addrOf.Operand is IdentifierNode id)
        {
            string varName = id.Name;
            if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
            {
                if (loc.IsRegister)
                    ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_CannotGetRegisterAddress, varName);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
            }
            else if (_globalMem.TryGetAddress(varName, out var addr))
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), addr);
            }
            else
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, varName);
        }
        else
            ThrowHelper.ThrowMiniC(ErrorCode.NotSupported, "Address-of only supports simple variables currently");
    }

    private void GenerateDereference(DereferenceNode deref)
    {
        GenerateExpression(deref.Operand);
        OpCodeSize size = GetPointedSize(deref.Operand);
        _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, size.Uint));
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
        return CodeGenUtils.GetSizeForType($"Cannot determine pointed type for dereference of '{expr}'");
        
    }

    private void GenerateNewOp(NewArrayNode newArr)
    {
        int elementSize;
        if (_structTable.TryGetValue(newArr.Type, out var layout))
            elementSize = layout.Size;
        else
            elementSize = CodeGenUtils.GetSizeInBytes(CodeGenUtils.GetSizeForType(newArr.Type));

        GenerateExpression(newArr.Size);   // r0 = количество элементов
        if (elementSize > 1)
            CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, elementSize);
        _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.ALLOC.Uint, 0));
    }

    private void GenerateFunctionCall(FunctionCallNode call)
    {
        if (!_functionTable.TryGetValue(call.Name, out var targetFunc))
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UnknownFunction, call.Name);

        if (call.Arguments.Count != targetFunc.Parameters.Count)
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_ArgumentCountMismatch, call.Name);

        for (int i = 0; i < call.Arguments.Count; i++)
        {
            GenerateExpression(call.Arguments[i]);
            var param = targetFunc.Parameters[i];
            string globalName = $"__param_{targetFunc.Name}_{param.Name}";
            if (!_globalMem.TryGetAddress(globalName, out var addr))
                ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, globalName);
            OpCodeSize opSize;
            if (param.IsPointer)
                opSize = OpCodeSize.S64;
            else if (_structTable.ContainsKey(param.Type))
                opSize = OpCodeSize.S64;   // значение-структура – пока не реализовано
            else
                opSize = CodeGenUtils.GetSizeForType(param.Type);
            _asm.EmitInstruction64(InstructionEncoder.EncodeSTORE((uint)RegType.r0, opSize.Uint), addr);
        }

        _asm.EmitJump(InstructionEncoder.EncodeCALL(), $"func_{call.Name}");
    }

    public void StoreToVariable(string varName, RegType srcReg)
    {
        if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
            {
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)loc.Register, (uint)srcReg));
            }
            else
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)srcReg, CodeGenUtils.TMP_REG, (uint)loc.TypeSize));
            }
        }
        else if (_globalMem.TryGetAddress(varName, out var addr))
        {
            var gInfo = _globalMem.GetInfo(varName)!;
            var opSize = CodeGenUtils.GetSizeForType(gInfo.Value.Type);
            _asm.EmitInstruction64(InstructionEncoder.EncodeSTORE((uint)srcReg, (uint)opSize), addr);
        }
        else
            ThrowHelper.ThrowMiniC(ErrorCode.CodeGen_UndefinedVariable, varName);
    }
}
