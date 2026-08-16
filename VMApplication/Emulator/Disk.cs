using Kernel.LocalMemorySystem;

namespace VMApplication.Emulator;

public class DiskData : IDisposable
{
    private readonly DiskDevice _disk;
    private readonly VMEmulator _emulator; // для удаления
    private readonly long _totalSectors;
    public int Sector { get; }
    public string ImagePath { get; }
    public bool IsMounted { get; private set; } = true;

    internal DiskData(DiskDevice disk, int sector, string imagePath, VMEmulator emulator)
    {
        _disk = disk;
        Sector = sector;
        ImagePath = imagePath;
        _emulator = emulator;
        _totalSectors = (new FileInfo(imagePath).Length / DiskDevice.SectorSize);
    }

    public long TotalSectors => _totalSectors;
    public static int SectorSize => DiskDevice.SectorSize;
    public int BasePortAddress => Sector * (int)_emulator.PortsPerDevice;
    public ReadOnlyMemory<byte> ReadSectorBuffer => new(_disk.SectorBuffer);

    public byte[]? ReadSectorDirect(uint lba) => _disk.ReadSectorDirect(lba);

    /// <summary>
    /// Отмонтировать диск и удалить его из системы.
    /// </summary>
    public bool Remove()
    {
        if (!IsMounted) return false;
        bool success = _emulator.RemoveDisk((uint)Sector);
        if (success)
            IsMounted = false;
        return success;
    }

    public void Dispose()
    {
        if (IsMounted)
            Remove();
        GC.SuppressFinalize(this);
    }
}
