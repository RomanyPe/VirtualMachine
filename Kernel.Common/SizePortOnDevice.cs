using System.ComponentModel;

namespace Kernel.Common;

public enum SizePortOnDevice : uint
{
    [Description("4 байта")] Size4B = 2,
    [Description("8 байт")] Size8B = 3,
    [Description("16 байт")] Size16B = 4,
    [Description("32 байта")] Size32B = 5,
}

public readonly struct DiskInfo(long sizeDisk, int countSectors, uint port, DateTime createdAt, string pathToFile)
{
    public readonly long SizeDisk = sizeDisk;
    public readonly int CountSectors = countSectors;
    public readonly uint Port = port;
    public readonly DateTime CreatedAt = createdAt;
    public readonly string PathToFile = pathToFile;
}
