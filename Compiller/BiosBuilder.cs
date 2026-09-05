using Kernel.Common;

namespace Compiller;

public class BiosBuilder
{
    private readonly MemoryStream _ms = new();
    private readonly BinaryWriter _writer;
    private readonly ulong _baseAddress;
    private readonly Dictionary<string, long> _labels = [];
    private readonly List<(long patchPos, string label)> _patches = [];

    public BiosBuilder(RamSize baseAddress)
    {
        _baseAddress = (ulong)baseAddress;
        _writer = new BinaryWriter(_ms);
    }

    public void EmitInstruction(uint instruction)
    {
        _writer.Write(instruction);
    }

    public void EmitInstruction64(uint instruction, ulong data)
    {
        _writer.Write(instruction);
        Align8();
        _writer.Write(data);
    }

    public void EmitJump(uint jmpOpcode, string label)
    {
        _writer.Write(jmpOpcode);
        Align8();
        long addrPos = _ms.Position;
        _patches.Add((addrPos, label));
        _writer.Write(0UL); // placeholder
    }

    public void MarkLabel(string label)
    {
        _labels[label] = _ms.Position;
    }

    public void EmitJumpToAbsolute(uint jmpOpcode, ulong absoluteTarget)
    {
        _writer.Write(jmpOpcode);
        Align8();
        _writer.Write(absoluteTarget); // сразу пишем нужный адрес
    }

    public byte[] Build()
    {
        byte[] bios = _ms.ToArray();
        foreach (var (patchPos, label) in _patches)
        {
            if (!_labels.TryGetValue(label, out long targetPos))
                throw new InvalidOperationException($"Undefined label: {label}");

            ulong absoluteAddr = _baseAddress + (ulong)targetPos;
            BitConverter.GetBytes(absoluteAddr).CopyTo(bios, (int)patchPos);
        }
        return bios;
    }

    private void Align8()
    {
        long pos = _ms.Position;
        long pad = ((pos + 7) & ~7) - pos;
        if (pad > 0) _writer.Write(new byte[pad]);
    }
}