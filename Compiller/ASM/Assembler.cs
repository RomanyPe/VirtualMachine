using Kernel.BiosSystem;
using static Kernel.ProcessorSystem.Processor;

namespace Compiller.ASM;

/// <summary>
/// Сборщик машинного кода с поддержкой меток и выравнивания по 8 байт для 64-битных данных.
/// </summary>
public class Assembler
{
    private readonly MemoryStream _stream;
    private readonly BinaryWriter _writer;
    private readonly Dictionary<string, long> _labels = [];
    private readonly List<(long pos, string label)> _patches = [];
    private readonly ulong _baseAddress;

    public Assembler(ulong baseAddress = 0)
    {
        _baseAddress = baseAddress;
        _stream = new MemoryStream();
        _writer = new BinaryWriter(_stream);
    }

    /// <summary>Пометить текущую позицию меткой.</summary>
    public void MarkLabel(string name)
    {
        _labels[name] = _stream.Position;
    }

    /// <summary>Записать 32-битную инструкцию (без дополнительных данных).</summary>
    public void EmitInstruction(uint instruction)
    {
        _writer.Write(instruction);
    }

    /// <summary>Записать инструкцию с последующим 64-битным операндом (выравнивание по 8).</summary>
    public void EmitInstruction64(uint instruction, ulong data)
    {
        _writer.Write(instruction);
        Align8();
        _writer.Write(data);
    }

    /// <summary>Записать инструкцию перехода с меткой (адрес будет подставлен при сборке).</summary>
    public void EmitJump(uint jmpOpcode, string label)
    {
        _writer.Write(jmpOpcode);
        Align8();
        long pos = _stream.Position;
        _patches.Add((pos, label));
        _writer.Write(0UL); // placeholder
    }

    /// <summary>Записать инструкцию перехода на абсолютный адрес (без патча).</summary>
    public void EmitJumpToAbsolute(uint jmpOpcode, ulong absoluteTarget)
    {
        _writer.Write(jmpOpcode);
        Align8();
        _writer.Write(absoluteTarget);
    }

    private void Align8()
    {
        long pos = _stream.Position;
        long pad = ((pos + 7) & ~7) - pos;
        if (pad > 0)
            _writer.Write(new byte[pad]);
    }

    /// <summary>Собрать байт-код, подставить адреса меток.</summary>
    public byte[] Build()
    {
        byte[] bytes = _stream.ToArray();
        foreach (var (pos, label) in _patches)
        {
            if (!_labels.TryGetValue(label, out long targetPos))
                throw new InvalidOperationException($"Неопределённая метка: {label}");
            ulong absoluteAddr = _baseAddress + (ulong)targetPos;
            BitConverter.GetBytes(absoluteAddr).CopyTo(bytes, (int)pos);
        }
        return bytes;
    }
}

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
        { "CALL", OpCode.CALL }
    };

    public byte[] Assemble(string code, ulong baseAddress = 0)
    {
        _asm = new Assembler(baseAddress);
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

        return _asm.Build();
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
            string sizeStr = mnemonic[5..];
            size = ParseSize(sizeStr);
        }
        else if (mnemonic.StartsWith("STORE."))
        {
            baseMnemonic = "STORE";
            string sizeStr = mnemonic[6..];
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
                ulong value = ParseNumber(tokens[2]);
                _asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)rLdi), value);
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

// Пример использования
public static class Program1
{
    public static void Main(string? asmCode = null)
    {
        asmCode ??= @"
            LDI r0 5;
            LDI r1 3;
            ADD r0 r1;
            JG halt;
            HALT;
        halt:
            HALT;
        ";

        var parser = new AssemblerParser();
        byte[] machineCode = parser.Assemble(asmCode, baseAddress: 0x0000);

        // Загрузка в компьютер и запуск (предполагается, что Computer существует)
        var computer = new Device([], new()); // BIOS пока пустой, но можно передать machineCode как программу
        computer.LoadProgram(machineCode, 0x0000);
        computer.LaunchDevice(0x0000);
        computer.GetAllData();
    }
}