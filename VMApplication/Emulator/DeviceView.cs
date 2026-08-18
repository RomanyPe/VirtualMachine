namespace VMApplication.Emulator;

public readonly struct DeviceView(int id,
                                  uint ramSize,
                                  uint portSize,
                                  uint sector, ReadOnlyMemory<byte> ram,
                                  DateTime createdAt,
                                  string? name = null)
{
    public int Id { get; init; } = id;
    public uint RamSize { get; init; } = ramSize;
    public uint PortSize { get; init; } = portSize;
    public uint Sector { get; init; } = sector;
    public ReadOnlyMemory<byte> Ram { get; init; } = ram;
    public string? Name { get; init; } = name;
    public DateTime CreatedAt { get; init; } = createdAt;
    public string PortRange { get; init; } = CreatePortRange(sector, portSize);

    public static string CreatePortRange(uint sector, uint portSize)
    {
        uint start = sector * portSize;
        uint end = start + portSize - 1;
        return $"{start}–{end}";
    }
}