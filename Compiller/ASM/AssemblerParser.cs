using Compiller.ASM.Optimizators;
using Kernel.Common;
using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace Compiller.ASM;

/// <summary>
/// Парсер ассемблерного текста, преобразующий его в байт-код.
/// Поддерживаемые инструкции:
///   NOP, END, RET,
///   MOV, ADD, SUB, AND, OR, XOR (два регистра),
///   INC, DEC, NOT, PUSH, POP (один регистр),
///   LDI (регистр, константа),
///   LOAD.S8|.S16|.S32|.S64 (регистр, адрес),
///   STORE.S8|.S16|.S32|.S64 (регистр, адрес),
///   JMP, JZ, JNZ, JG, JL, CALL (метка или абсолютный адрес).
/// </summary>
public class AssemblerParser
{
    private static readonly Dictionary<string, RegType> _regMapLex = new(StringComparer.OrdinalIgnoreCase)
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
        { "r23", RegType.r23 },
        { "rTB", RegType.rTB },
        { "rCD", RegType.rCD },
        { "rFL", RegType.rFL },
        { "rCL", RegType.rCL },
        { "rSP", RegType.rSP },
        { "rHP", RegType.rHP },
        { "rIP", RegType.rIP }
    };


    private static readonly Dictionary<string, OpCode> _opMapLex = new(StringComparer.OrdinalIgnoreCase)
    {
        { "NOP", OpCode.NOP },
        { "END", OpCode.END },
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
        { "DIV", OpCode.DIV },
        { "HALT", OpCode.HALT },
        { "WAKE", OpCode.WAKE},
        { "WAKE_INT", OpCode.WAKE_INT },
        { "LOAD_IND_UNSAFE", OpCode.LOAD_IND_UNSAFE},
        { "LOAD_UNSAFE", OpCode.LOAD_UNSAFE},
        { "STORE_IND_UNSAFE", OpCode.STORE_IND_UNSAFE},
        { "STORE_UNSAFE", OpCode.STORE_UNSAFE},
        { "PUSH_UNSAFE", OpCode.PUSH_UNSAFE},
        { "POP_UNSAFE", OpCode.POP_UNSAFE}
    };

    private AssemblerBase _asm = null!;
    private readonly FrozenDictionary<string, RegType> _regMap = _regMapLex.ToFrozenDictionary();
    private readonly FrozenDictionary<string, OpCode> _opMap = _opMapLex.ToFrozenDictionary();

    public byte[] Assemble(string code, ulong baseAddress = 0)
    {
        var asm = new Assembler(baseAddress);
        Assemble(code, asm);
        return asm.Build();
    }

    public byte[] Build(string code, AssemblerBase asm)
    {
        Assemble(code, asm);
        return asm.Build();
    }

    public void Assemble(string code, AssemblerBase asm)
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
            baseMnemonic = mnemonic[..4]; // Выделяем "LOAD" как срез (0 аллокаций!)
            size = ParseSize(mnemonic[5..]); // Передаем в парсер всё, что после точки
        }
        else if (mnemonic.StartsWith("STORE."))
        {
            baseMnemonic = mnemonic[..5]; // "STORE"
            size = ParseSize(mnemonic[6..]);
        }
        else if (mnemonic.StartsWith("LOAD_IND."))
        {
            baseMnemonic = mnemonic[..8]; // "LOAD_IND"
            size = ParseSize(mnemonic[9..]);
        }
        else if (mnemonic.StartsWith("STORE_IND."))
        {
            baseMnemonic = mnemonic[..9]; // "STORE_IND"
            size = ParseSize(mnemonic[10..]); // Точка на 9-й позиции, размер начинается с 10
        }

        if (!_opMap.TryGetValue(baseMnemonic, out OpCode opCode))
            ThrowHelper.ThrowMiniC(ErrorCode.Asm_InvalidInstruction, baseMnemonic);

        switch (baseMnemonic)
        {
            // Без операндов
            case "NOP": _asm.EmitInstruction(InstructionEncoder.EncodeNOP()); break;
            case "END": _asm.EmitInstruction(InstructionEncoder.EncodeEND()); break;
            case "RET": _asm.EmitInstruction(InstructionEncoder.EncodeRET()); break;
            case "HALT": _asm.EmitInstruction(InstructionEncoder.EncodeHALT()); break;
            case "WAKE": _asm.EmitInstruction(InstructionEncoder.EncodeWAKE()); break;

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
                FormatR(tokens, baseMnemonic, opCode);
                break;

            // Формат U (один регистр)
            case "WAKE_INT":
            case "PRINT":
            case "INC":
            case "DEC":
            case "NOT":
            case "PUSH":
            case "POP":
            case "PUSH_UNSAFE":
            case "POP_UNSAFE":
            case "PRINT_INT":
                FormatU(tokens, baseMnemonic, opCode);
                break;

            // LDI: регистр, константа
            case "LDI":
                FormatLDI(tokens, baseMnemonic);
                break;

            // LOAD/STORE: регистр, адрес (константа)
            case "LOAD":
            case "STORE":
            case "LOAD_UNSAFE":
            case "STORE_UNSAFE":
                FormatLOAD__STORE(tokens, size, baseMnemonic);
                break;

            case "LOAD_IND":
            case "STORE_IND":
            case "LOAD_IND_UNSAFE":
            case "STORE_IND_UNSAFE":
                FormateLOAD_IND__STORE_IND(tokens, size, baseMnemonic, opCode);
                break;

            // Переходы и CALL: метка или абсолютный адрес
            case "JMP":
            case "JZ":
            case "JNZ":
            case "JG":
            case "JL":
            case "CALL":
                FormatJumpType(tokens, baseMnemonic, opCode);
                break;

            case "ALLOC":
                _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.ALLOC.Uint, 0));
                break;

            case "INT":
                if (tokens.Length < 2)
                    ThrowHelper.ThrowMiniC(ErrorCode.Asm_InvalidInstruction,baseMnemonic);
                RegType rInt = ParseReg(tokens[1]);
                _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.INT.Uint, (uint)rInt));
                break;

            case "IRET":
                _asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.IRET.Uint, 0));
                break;

            default:
                ThrowHelper.ThrowMiniC(ErrorCode.Asm_UnknownMnemonic,baseMnemonic);
                break;
        }
    }

    private void FormatJumpType(string[] tokens, string baseMnemonic, OpCode opCode)
    {
        if (tokens.Length < 2)
            ThrowHelper.ThrowMiniC(ErrorCode.Asm_InvalidInstruction, baseMnemonic);
        string target = tokens[1];
        uint jmpOpcode = InstructionEncoder.EncodeJ(opCode.Uint);
        if (IsNumber(target))
        {
            ulong absTarget = ParseNumber(target);
            _asm.EmitJumpToAbsolute(jmpOpcode, absTarget);
        }
        else
        {
            _asm.EmitJump(jmpOpcode, target);
        }
    }

    private void FormateLOAD_IND__STORE_IND(string[] tokens, OpCodeSize size, string baseMnemonic, OpCode opCode)
    {
        if (tokens.Length < 3)
            ThrowHelper.ThrowMiniC(ErrorCode.Asm_InvalidInstruction, baseMnemonic);
        RegType rInd1 = ParseReg(tokens[1]);
        RegType rInd2 = ParseReg(tokens[2]);
        _asm.EmitInstruction(InstructionEncoder.EncodeRS(opCode.Uint, (uint)rInd1, (uint)rInd2, size.Uint));
    }

    private void FormatLOAD__STORE(string[] tokens, OpCodeSize size, string baseMnemonic)
    {
        if (tokens.Length < 3)
            ThrowHelper.ThrowMiniC(ErrorCode.Asm_InvalidInstruction, baseMnemonic);
        RegType rMem = ParseReg(tokens[1]);
        ulong addr = ParseNumber(tokens[2]);
        uint encoded = baseMnemonic == "LOAD"
            ? InstructionEncoder.EncodeLOAD((uint)rMem, size.Uint)
            : InstructionEncoder.EncodeSTORE((uint)rMem, size.Uint);
        _asm.EmitInstruction64(encoded, addr);
    }

    private void FormatLDI(string[] tokens, string baseMnemonic)
    {
        if (tokens.Length < 3)
            ThrowHelper.ThrowMiniC(ErrorCode.Asm_InvalidInstruction, baseMnemonic);
        RegType rLdi = ParseReg(tokens[1]);
        string operand = tokens[2];
        if (IsNumber(operand))
        {
            ulong value = ParseNumber(operand);
            _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)rLdi), value);
        }
        else
        {
            uint encodedldi = InstructionEncoder.EncodeLDI((uint)rLdi);
            _asm.EmitInstruction64WithLabel(encodedldi, operand);
        }
    }

    private void FormatU(string[] tokens, string baseMnemonic, OpCode opCode)
    {
        if (tokens.Length < 2)
            ThrowHelper.ThrowMiniC(ErrorCode.Asm_InvalidInstruction, baseMnemonic);
        RegType rU = ParseReg(tokens[1]);
        _asm.EmitInstruction(InstructionEncoder.EncodeU(opCode.Uint, (uint)rU));
    }

    private void FormatR(string[] tokens, string baseMnemonic, OpCode opCode)
    {
        if (tokens.Length < 3)
            ThrowHelper.ThrowMiniC(ErrorCode.Asm_InvalidInstruction, baseMnemonic);
        RegType r1 = ParseReg(tokens[1]);
        RegType r2 = ParseReg(tokens[2]);
        _asm.EmitInstruction(InstructionEncoder.EncodeR(opCode.Uint, (uint)r1, (uint)r2));
    }

    private RegType ParseReg(string s)
    {
        if (_regMap.TryGetValue(s, out RegType reg))
            return reg;
        return ThrowHelper.ThrowMiniC<RegType>(ErrorCode.Asm_UnknownRegister, s);
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
        _ => ThrowHelper.ThrowMiniC<OpCodeSize>(ErrorCode.CodeGen_UnknownOpCodeSize,s)
    };
}

public static class AsmLanguageDefinition
{
    private static readonly Regex _mnemonicsRegex;
    private static readonly Regex regex;

    public static readonly FrozenSet<string> Mnemonics;
    public static readonly FrozenSet<string> Registers;

    static AsmLanguageDefinition()
    {
        Mnemonics =
        [
            "NOP", "END", "RET", "PRINT", "MOV", "ADD", "SUB", "AND", "OR", "XOR",
            "INC", "DEC", "NOT", "PUSH", "POP",
            "PUSH_UNSAFE", "POP_UNSAFE", "LDI", "LOAD", "STORE", 
            "LOAD_UNSAFE", "STORE_UNSAFE", "JMP", "JZ",
            "JNZ", "JG", "JL", "CALL", "LOAD_IND", "STORE_IND","LOAD_IND_UNSAFE", "STORE_IND_UNSAFE", "PRINT_INT", "ALLOC",
            "IN", "OUT", "INT", "IRET", "SHR", "MULT_INT", "DIV", "HALT", "WAKE", "WAKE_INT"

        ];
        Registers =
        [
            "rZ", "r0", "r1", "r2", "r3", "r4", "r5", "r6", "r7", "r8", "r9",
            "r10", "r11", "r12", "r13", "r14", "r15", "r16", "r17", "r18", "r19",
            "r20", "r21", "rTB", "rCD", "rFL", "r22", "rCL", "r23", "rSP", "rHP", "rIP"
        ];

        _mnemonicsRegex = new(GetMnemonicsPattern(), RegexOptions.IgnoreCase);
        regex = new(GetRegistersPattern(), RegexOptions.IgnoreCase);
    }

    private static string GetRegistersPattern() => $@"\b({string.Join("|", Registers)})\b";
    private static string GetMnemonicsPattern() => $@"\b({string.Join("|", Mnemonics)})\b";

    public static Regex GetMnemonicRegex() => _mnemonicsRegex;
    public static Regex GetRegisterRegex() => regex;
}