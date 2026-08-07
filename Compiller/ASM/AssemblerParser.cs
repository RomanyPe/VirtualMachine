using static Kernel.ProcessorSystem.Processor;

namespace Compiller.ASM;

/// <summary>
/// Парсер ассемблерного текста, преобразующий его в байт-код.
/// Поддерживаемые инструкции:
///   NOP, HALT, RET,
///   MOV, ADD, SUB, AND, OR, XOR (два регистра),
///   INC, DEC, NOT, PUSH, POP (один регистр),
///   LDI (регистр, константа),
///   LOAD.S8|.S16|.S32|.S64 (регистр, адрес),
///   STORE.S8|.S16|.S32|.S64 (регистр, адрес),
///   JMP, JZ, JNZ, JG, JL, CALL (метка или абсолютный адрес).
/// </summary>
public class AssemblerParser
{
    private Assembler _asm = null!;
    private readonly Dictionary<string, RegType> _regMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "rZ", RegType.rZ },
        { "r0", RegType.r0 },
        { "r1", RegType.r1 },
        { "r2", RegType.r2 },
        { "r3", RegType.r3 },
        { "r4", RegType.r4 },
        { "r5", RegType.r5 },
        { "r6", RegType.r6 },
        { "r7", RegType.r7 },
        { "r8", RegType.r8 },
        { "r9", RegType.r9 },
        { "r10", RegType.r10 },
        { "r11", RegType.r11 },
        { "r12", RegType.r12 },
        { "r13", RegType.r13 },
        { "r14", RegType.r14 },
        { "r15", RegType.r15 },
        { "r16", RegType.r16 },
        { "r17", RegType.r17 },
        { "r18", RegType.r18 },
        { "r19", RegType.r19 },
        { "r20", RegType.r20 },
        { "r21", RegType.r21 },
        { "r22", RegType.r22 },
        { "rCD", RegType.rCD },
        { "rFL", RegType.rFL },
        { "rLP", RegType.rLP },
        { "rCL", RegType.rCL },
        { "rRT", RegType.rRT },
        { "rSP", RegType.rSP },
        { "rHP", RegType.rHP },
        { "rIP", RegType.rIP }
    };

    private readonly Dictionary<string, OpCode> _opMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "NOP", OpCode.NOP },
        { "HALT", OpCode.HALT },
        { "RET", OpCode.RET },
        { "PRINT", OpCode.PRINT },
        { "MOV", OpCode.MOV },
        { "ADD", OpCode.ADD },
        { "SUB", OpCode.SUB },
        { "AND", OpCode.AND },
        { "OR", OpCode.OR },
        { "XOR", OpCode.XOR },
        { "INC", OpCode.INC },
        { "DEC", OpCode.DEC },
        { "NOT", OpCode.NOT },
        { "PUSH", OpCode.PUSH },
        { "POP", OpCode.POP },
        { "LDI", OpCode.LDI },
        { "LOAD", OpCode.LOAD },
        { "STORE", OpCode.STORE },
        { "JMP", OpCode.JMP },
        { "JZ", OpCode.JZ },
        { "JNZ", OpCode.JNZ },
        { "JG", OpCode.JG },
        { "JL", OpCode.JL },
        { "CALL", OpCode.CALL },
        { "LOAD_IND", OpCode.LOAD_IND },
        { "STORE_IND", OpCode.STORE_IND },
        { "PRINT_INT", OpCode.PRINT_INT },
        { "ALLOC", OpCode.ALLOC },
        { "IN", OpCode.IN },
        { "OUT", OpCode.OUT },
        { "INT", OpCode.INT }, 
        { "IRET", OpCode.IRET },
        { "SHR", OpCode.SHR },
        { "MULT_INT", OpCode.MULT_INT },
        { "DIV", OpCode.DIV }
    };

    public byte[] Assemble(string code, ulong baseAddress = 0)
    {
        var asm = new Assembler(baseAddress);
        Assemble(code, asm);
        return asm.Build();
    }

    public void Assemble(string code, Assembler asm)
    {
        _asm = asm;
        ProcessLines(code);
    }

    private void ProcessLines(string code)
    {
        var lines = code.Split(['\n', ';'], StringSplitOptions.RemoveEmptyEntries);
        foreach (var rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // Удаление комментариев
            int commentIdx = line.IndexOf(';');
            if (commentIdx < 0) commentIdx = line.IndexOf("//");
            if (commentIdx >= 0)
                line = line[..commentIdx].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // Обработка меток
            if (line.EndsWith(':'))
            {
                string label = line[..^1].Trim();
                _asm.MarkLabel(label);
                continue;
            }
            if (line.Contains(':'))
            {
                string[] parts = line.Split([':'], 2);
                string label = parts[0].Trim();
                string rest = parts[1].Trim();
                _asm.MarkLabel(label);
                if (!string.IsNullOrEmpty(rest))
                    ParseInstruction(rest);
                continue;
            }

            ParseInstruction(line);
        }
    }
    private void ParseInstruction(string instruction)
    {
        var tokens = instruction.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0) return;

        string mnemonic = tokens[0].ToUpperInvariant();
        OpCodeSize size = OpCodeSize.S64;
        string baseMnemonic = mnemonic;

        // Обработка суффиксов размера для LOAD/STORE
        if (mnemonic.StartsWith("LOAD."))
        {
            baseMnemonic = "LOAD";
            string sizeStr = mnemonic[4..];
            size = ParseSize(sizeStr);
        }
        else if (mnemonic.StartsWith("STORE."))
        {
            baseMnemonic = "STORE";
            string sizeStr = mnemonic[5..];
            size = ParseSize(sizeStr);
        }
        else if (mnemonic.StartsWith("LOAD_IND."))
        {
            baseMnemonic = "LOAD_IND";
            string sizeStr = mnemonic[9..]; // длина "LOAD_IND." = 10
            size = ParseSize(sizeStr);
        }
        else if (mnemonic.StartsWith("STORE_IND."))
        {
            baseMnemonic = "STORE_IND";
            string sizeStr = mnemonic[10..]; // длина "STORE_IND." = 11
            size = ParseSize(sizeStr);
        }

        if (!_opMap.TryGetValue(baseMnemonic, out OpCode opCode))
            throw new Exception($"Неизвестная инструкция: {mnemonic}");

        switch (baseMnemonic)
        {
            // Без операндов
            case "NOP":
                _asm.EmitInstruction(InstructionEncoder.EncodeNOP());
                break;
            case "HALT":
                _asm.EmitInstruction(InstructionEncoder.EncodeHALT());
                break;
            case "RET":
                _asm.EmitInstruction(InstructionEncoder.EncodeRET());
                break;

            // Формат R (два регистра)
            case "MOV":
            case "ADD":
            case "SUB":
            case "AND":
            case "OR":
            case "XOR":
            case "IN":
            case "OUT":
            case "SHR":
            case "DIV":
            case "MULT_INT":
                if (tokens.Length < 3)
                    throw new Exception($"Инструкция {baseMnemonic} требует два регистра");
                RegType r1 = ParseReg(tokens[1]);
                RegType r2 = ParseReg(tokens[2]);
                _asm.EmitInstruction(InstructionEncoder.EncodeR((uint)opCode, (uint)r1, (uint)r2));
                break;

            // Формат U (один регистр)
            case "PRINT":
            case "INC":
            case "DEC":
            case "NOT":
            case "PUSH":
            case "POP":
            case "PRINT_INT":
                if (tokens.Length < 2)
                    throw new Exception($"Инструкция {baseMnemonic} требует один регистр");
                RegType rU = ParseReg(tokens[1]);
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)opCode, (uint)rU));
                break;

            // LDI: регистр, константа
            case "LDI":
                if (tokens.Length < 3)
                    throw new Exception("LDI требует регистр и значение");
                RegType rLdi = ParseReg(tokens[1]);
                string operand = tokens[2];
                if (IsNumber(operand))
                {
                    ulong value = ParseNumber(operand);
                    _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)rLdi), value);
                }
                else
                {
                    // Операнд — метка, откладываем разрешение адреса
                    uint encodedldi = InstructionEncoder.EncodeLDI((uint)rLdi);
                    _asm.EmitInstruction64(encodedldi, 0UL);  // временный 0
                                                           // Добавляем патч: (позиция в потоке, где записан 0, метка)
                    long patchPos = _asm.GetStreamPosition() - 8; // нужно получить позицию в MemoryStream
                    _asm.AddPatch(patchPos, operand);
                }
                break;

            // LOAD/STORE: регистр, адрес (константа)
            case "LOAD":
            case "STORE":
                if (tokens.Length < 3)
                    throw new Exception($"{baseMnemonic} требует регистр и адрес");
                RegType rMem = ParseReg(tokens[1]);
                ulong addr = ParseNumber(tokens[2]);
                uint encoded;
                if (baseMnemonic == "LOAD")
                    encoded = InstructionEncoder.EncodeLOAD((uint)rMem, (uint)size);
                else
                    encoded = InstructionEncoder.EncodeSTORE((uint)rMem, (uint)size);
                _asm.EmitInstruction64(encoded, addr);
                break;

            case "LOAD_IND":
            case "STORE_IND":
                if (tokens.Length < 3)
                    throw new Exception($"{baseMnemonic} требует два регистра");
                RegType rInd1 = ParseReg(tokens[1]);
                RegType rInd2 = ParseReg(tokens[2]);
                // EncodeR теперь принимает размер (добавьте соответствующий метод в InstructionEncoder)
                _asm.EmitInstruction(InstructionEncoder.EncodeRS((uint)opCode, (uint)rInd1, (uint)rInd2, (uint)size));
                break;
            // Переходы и CALL: метка или абсолютный адрес
            case "JMP":
            case "JZ":
            case "JNZ":
            case "JG":
            case "JL":
            case "CALL":
                if (tokens.Length < 2)
                    throw new Exception($"{baseMnemonic} требует целевой адрес");
                string target = tokens[1];
                uint jmpOpcode = InstructionEncoder.EncodeJ((uint)opCode);
                if (IsNumber(target))
                {
                    ulong absTarget = ParseNumber(target);
                    _asm.EmitJumpToAbsolute(jmpOpcode, absTarget);
                }
                else
                {
                    _asm.EmitJump(jmpOpcode, target);
                }
                break;
            case "ALLOC":
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.ALLOC, 0));
                break;

            case "INT":
                if (tokens.Length < 2) throw new Exception("INT требует регистр с номером прерывания");
                RegType rInt = ParseReg(tokens[1]);
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.INT, (uint)rInt));
                break;
            case "IRET":
                _asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.IRET, 0));
                break;

            default:
                throw new Exception($"Инструкция {baseMnemonic} не реализована в парсере");
        }
    }

    private RegType ParseReg(string s)
    {
        if (_regMap.TryGetValue(s, out RegType reg))
            return reg;
        throw new Exception($"Неизвестный регистр: {s}");
    }

    private static ulong ParseNumber(string s)
    {
        s = s.Trim();
        if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return Convert.ToUInt64(s[2..], 16);
        return ulong.Parse(s);
    }

    private static bool IsNumber(string s)
    {
        s = s.Trim();
        if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return ulong.TryParse(s.AsSpan(2), System.Globalization.NumberStyles.HexNumber, null, out _);
        return ulong.TryParse(s, out _);
    }

    private static OpCodeSize ParseSize(string s) => s.ToUpperInvariant() switch
    {
        "S8" => OpCodeSize.S8,
        "S16" => OpCodeSize.S16,
        "S32" => OpCodeSize.S32,
        "S64" => OpCodeSize.S64,
        _ => throw new Exception($"Некорректный размер данных: {s}")
    };
}
