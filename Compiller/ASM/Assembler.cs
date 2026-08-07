using Kernel.BiosSystem;

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
        if (_labels.ContainsKey(name))
            throw new InvalidOperationException($"Метка '{name}' уже определена.");
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

    public bool HasLabel(string name) => _labels.ContainsKey(name);
    public long GetStreamPosition() => _stream.Position;

    public void AddPatch(long position, string label)
    {
        _patches.Add((position, label));
    }

}