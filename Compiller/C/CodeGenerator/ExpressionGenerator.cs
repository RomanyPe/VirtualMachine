using Compiller.ASM;
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// ExpressionGenerator – генерация выражений
// ============================================================
public class ExpressionGenerator(Assembler asm,
                                 GlobalMemoryManager globalMem,
                                 FunctionContext funcCtx,
                                 Dictionary<string, FunctionNode> functionTable,
                                 Func<string> getLabel,
                                 Dictionary<string, StructLayout> structTable)
{
    private readonly Assembler _asm = asm;
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
                throw new Exception($"Unsupported expression: {expr.GetType()}");
        }
    }

    public void GenerateMemberAddress(MemberAccessNode node)
    {
        string? structType;
        if (node.Object is IdentifierNode id)
        {
            // Ищем в локальных или глобальных
            if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
            {
                if (loc.StructTypeName != null)
                    structType = loc.StructTypeName;
                else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
                    structType = loc.PointedType; // ptr->field
                else throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
            }
            else if (_globalMem.Contains(id.Name))
            {
                var gInfo = _globalMem.GetInfo(id.Name)!.Value;
                if (_structTable.ContainsKey(gInfo.Type))
                    structType = gInfo.Type;
                else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
                    structType = gInfo.PointedType;
                else throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
            }
            else throw new Exception($"Unknown variable '{id.Name}'");
        }
        else if (node.Object is MemberAccessNode node1)
        {
            // Рекурсивно вычисляем адрес вложенного объекта (результат в r0)
            GenerateMemberAddress(node1);

            // Получаем ТИП поля node1, чтобы понять, в какой структуре искать текущее поле
            var (_, _, innerFieldType) = ResolveMemberAccessType(node1);

            if (!_structTable.TryGetValue(innerFieldType, out var innerLayout))
                throw new Exception($"Unknown struct type '{innerFieldType}'");

            var currentField = innerLayout.GetField(node.FieldName)
                               ?? throw new Exception($"Field '{node.FieldName}' not found in struct '{innerFieldType}'");

            if (currentField.Offset > 0)
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)currentField.Offset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
            }
            return;
        }
        else if (node.Object is DereferenceNode)
        {
            // *ptr  ->  ptr – указатель на структуру
            // Тип указателя должен быть известен. Пока не поддерживается, но можно через контекст.
            throw new Exception("Dereference member access not implemented yet");
        }
        else if (node.Object is ArrayAccessNode arrAcc)
        {
            // arr[i].field — адрес поля
            string? arrStructType = ResolveArrayStructType(arrAcc.ArrayName);
            var layoutstr = _structTable[arrStructType!];
            var fieldstr = layoutstr.GetField(node.FieldName)
                        ?? throw new Exception($"Field '{node.FieldName}' not found in struct '{arrStructType}'");

            // Вычисляем адрес элемента arr[i]
            GenerateExpression(arrAcc.Index);                    // r0 = i
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r2, (uint)RegType.r0));
            LoadVariableToR0(arrAcc.ArrayName);                 // r0 = arr (указатель)
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
            throw new Exception("Unsupported object for member access");
        }

        if (structType == null)
            throw new Exception("Cannot determine struct type for member access");

        var layout = _structTable[structType];
        var field = layout.GetField(node.FieldName) ?? throw new Exception($"Field '{node.FieldName}' not found in struct '{structType}'");

        if (node.IsArrow)
        {
            GenerateExpression(node.Object);  // r0 = ptr
        }
        else
        {
            if (node.Object is IdentifierNode identifer)
                LoadAddressToR0(identifer.Name);
            else throw new Exception("Dot address only for simple variables");
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
        string? structType;
        if (node.Object is IdentifierNode id)
        {
            // Ищем в локальных или глобальных
            if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
            {
                if (loc.StructTypeName != null)
                    structType = loc.StructTypeName;
                else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
                    structType = loc.PointedType; // ptr->field
                else throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
            }
            else if (_globalMem.Contains(id.Name))
            {
                var gInfo = _globalMem.GetInfo(id.Name)!.Value;
                if (_structTable.ContainsKey(gInfo.Type))
                    structType = gInfo.Type;
                else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
                    structType = gInfo.PointedType;
                else throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
            }
            else throw new Exception($"Unknown variable '{id.Name}'");
        }
        else if (node.Object is MemberAccessNode)
        {
            // Рекурсивно вычисляем адрес вложенного объекта (без загрузки значения) в r0
            GenerateMemberAddress(node); // этот метод вычислит адрес поля
                                         // Теперь r0 содержит адрес поля, загружаем значение
            var (_, fieldMem, _) = ResolveMemberAccessType(node);
            OpCodeSize fieldSizeMem = CodeGenUtils.GetSizeForType(fieldMem.Type);
            _asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, fieldSizeMem.Uint));
            return;
        }
        else if (node.Object is DereferenceNode)
        {
            // *ptr  ->  ptr – указатель на структуру
            // Тип указателя должен быть известен. Пока не поддерживается, но можно через контекст.
            throw new Exception("Dereference member access not implemented yet");
        }
        else if (node.Object is ArrayAccessNode arrAcc)
        {
            string? arrStructType = ResolveArrayStructType(arrAcc.ArrayName);
            var layoutstr = _structTable[arrStructType!];
            var fieldstr = layoutstr.GetField(node.FieldName)
                        ?? throw new Exception($"Field '{node.FieldName}' not found in struct '{arrStructType}'");

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
        else
        {
            throw new Exception("Unsupported object for member access");
        }

        if (structType == null)
            throw new Exception("Cannot determine struct type for member access");

        var layout = _structTable[structType];
        var field = layout.GetField(node.FieldName) ?? throw new Exception($"Field '{node.FieldName}' not found in struct '{structType}'");

        // 2. Загрузить адрес начала структуры в r0
        if (node.IsArrow)
        {
            // Для ptr->field: ptr содержит адрес структуры, загружаем его.
            GenerateExpression(node.Object);  // r0 = ptr (значение указателя)
        }
        else
        {
            if (node.Object is IdentifierNode identifer)
            {
                LoadAddressToR0(identifer.Name);
            }
            else
            {
                throw new Exception("Dot access only supported for simple variables currently");
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
                throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
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
            throw new Exception("Dereference in member access not yet supported");
        }
        else
        {
            throw new Exception("Unsupported object for member access");
        }

        if (structType == null)
            throw new Exception("Cannot determine struct type for member access");

        if (!_structTable.TryGetValue(structType, out var layout))
            throw new Exception($"Unknown struct type '{structType}'");

        var field = layout.GetField(node.FieldName)
                    ?? throw new Exception($"Field '{node.FieldName}' not found in struct '{structType}'");

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
        throw new Exception($"'{arrayName}' is not a struct array or pointer to struct");
    }


    public void LoadAddressToR0(string varName)
    {
        if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
        {
            if (loc.IsRegister)
                throw new Exception($"Cannot take address of register variable '{varName}'");
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
            _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
        }
        else if (_globalMem.TryGetAddress(varName, out var addr))
        {
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), addr);
        }
        else throw new Exception($"Undefined variable '{varName}'");
    }

    private void LoadVariableToR0(string varName)
    {
        if (_funcCtx.VarMap.TryGetValue(varName, out var loca) && loca.StructTypeName != null)
            throw new Exception($"Direct load/store of struct variable '{varName}' is not supported.");
        if (_globalMem.Contains(varName))
        {
            var gInfo = _globalMem.GetInfo(varName)!.Value;
            if (CodeGenUtils.IsStructType(gInfo.Type, _structTable))
                throw new Exception($"Direct load/store of struct variable '{varName}' is not supported.");
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
            throw new Exception($"Undefined variable: {varName}");
    }

    public void StoreR0ToVariable(string varName)
    {
        if (_funcCtx.VarMap.TryGetValue(varName, out var loca) && loca.StructTypeName != null)
            throw new Exception($"Direct load/store of struct variable '{varName}' is not supported.");
        if (_globalMem.Contains(varName))
        {
            var gInfo = _globalMem.GetInfo(varName)!.Value;
            if (CodeGenUtils.IsStructType(gInfo.Type, _structTable))
                throw new Exception($"Direct load/store of struct variable '{varName}' is not supported.");
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
            throw new Exception($"Undefined variable: {varName}");
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
                throw new Exception($"{arrayName} is not an array or pointer");
        }
        else if (_globalMem.Contains(arrayName))
        {
            var gInfo = _globalMem.GetInfo(arrayName)!.Value;
            if (gInfo.IsArray)
                LoadArrayElementFromGlobal(gInfo, indexExpr);
            else if (gInfo.IsPointer)
                LoadPointerElementToR0(arrayName, null, indexExpr);
            else
                throw new Exception($"{arrayName} is not an array or pointer");
        }
        else
            throw new Exception($"Undefined variable: {arrayName}");
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
                throw new Exception($"{arrayName} is not an array or pointer");
        }
        else if (_globalMem.Contains(arrayName))
        {
            var gInfo = _globalMem.GetInfo(arrayName)!.Value;
            if (gInfo.IsArray)
                StoreArrayElementToGlobal(gInfo, indexExpr);
            else if (gInfo.IsPointer)
                StorePointerElement(arrayName, null, indexExpr);
            else
                throw new Exception($"{arrayName} is not an array or pointer");
        }
        else
            throw new Exception($"Undefined variable: {arrayName}");
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
        else
            throw new Exception("Cannot determine pointed type");

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
            throw new Exception("Cannot determine pointed type");

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

    private void GenerateBinaryOp(BinaryOpNode binop)
    {
        GenerateExpression(binop.Left);
        _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));
        GenerateExpression(binop.Right);

        switch (binop.Operator)
        {
            case "+":
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r1, (uint)RegType.r0));
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));
                break;
            case "-":
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.r1, (uint)RegType.r0));
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));
                break;
            case "*":
                _asm.EmitInstruction(InstructionEncoder.EncodeMULT_INT((uint)RegType.r1, (uint)RegType.r0));
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));
                break;
            case "/":
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.DIV.Uint, (uint)RegType.r1, (uint)RegType.r0));
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));
                break;
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
                // заглушка
                break;
            default:
                throw new Exception($"Unsupported operator: {binop.Operator}");
        }
    }

    private void GenerateComparison(string op)
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
            // ... остальные case'ы "<", ">", "<=", ">=", "==", "!=" – полностью сохранены, как в оригинале
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
                throw new NotImplementedException($"Comparison '{op}' not implemented");
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
                throw new Exception($"Unsupported unary operator: {unop.Operator}");
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
                    throw new Exception($"Cannot take address of register variable '{varName}'");
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
                _asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
            }
            else if (_globalMem.TryGetAddress(varName, out var addr))
            {
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), addr);
            }
            else throw new Exception($"Undefined variable '{varName}'");
        }
        else
            throw new Exception("Address-of only supports simple variables currently");
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
        throw new Exception($"Cannot determine pointed type for dereference of '{expr}'");
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
            throw new Exception($"Function '{call.Name}' not found");

        if (call.Arguments.Count != targetFunc.Parameters.Count)
            throw new Exception($"Argument count mismatch for function '{call.Name}'");

        for (int i = 0; i < call.Arguments.Count; i++)
        {
            GenerateExpression(call.Arguments[i]);
            var param = targetFunc.Parameters[i];
            string globalName = $"__param_{targetFunc.Name}_{param.Name}";
            if (!_globalMem.TryGetAddress(globalName, out var addr))
                throw new Exception($"Parameter global not found: {globalName}");
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
            throw new Exception($"Undefined variable: {varName}");
    }
}
