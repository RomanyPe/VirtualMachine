namespace ASM_gen.Information_Window;

public readonly ref struct MemoryRow
{
    private readonly ulong _address;
    private readonly ReadOnlySpan<byte> _memory;

    public MemoryRow(ulong address, ReadOnlyMemory<byte> memory)
    {
        _address = address;

        int length = Math.Min(16, memory.Length - (int)address);
        _memory = memory.Span.Slice((int)address, length);
    }

    public string AddressHex => _address.ToString("X8");

    public readonly string GetBytesHex()
    {
        if (_memory.IsEmpty) return string.Empty;

        return string.Create(_memory.Length * 3 - 1, _memory, (dest, src) =>
        {
            for (int i = 0; i < src.Length; i++)
            {
                src[i].TryFormat(dest.Slice(i * 3, 2), out _, "X2");

                if (i < src.Length - 1)
                {
                    dest[i * 3 + 2] = ' ';
                }
            }
        });
    }

    public readonly string GetAsciiText()
    {
        if (_memory.IsEmpty) return string.Empty;

        return string.Create(_memory.Length, _memory, (dest, src) =>
        {
            for (int i = 0; i < src.Length; i++)
            {
                byte b = src[i];
                dest[i] = (b is >= 32 and <= 126) ? (char)b : '.';
            }
        });
    }
}