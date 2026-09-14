using Kernel.LocalMemorySystem;

namespace VMApplication.Emulator;

public class DiskContext : IDisposable
{
    private readonly DiskDevice _disk;
    private readonly VMEmulator _emulator; // для удаления
    private readonly long _totalSectors;
    public uint Sector { get; }
    public string ImagePath { get; }
    public bool IsMounted { get; private set; } = true;

    internal DiskContext(DiskDevice disk, uint sector, string imagePath, VMEmulator emulator)
    {
        _disk = disk;
        Sector = sector;
        ImagePath = imagePath;
        _emulator = emulator;
        _totalSectors = (new FileInfo(imagePath).Length / DiskDevice.SectorSize);
    }

    public long TotalSectors => _totalSectors;
    public static int SectorSize => DiskDevice.SectorSize;
    public uint BasePortAddress => Sector * _emulator.PortsPerDevice;
    public ReadOnlyMemory<byte> ReadSectorBuffer => new(_disk.SectorBuffer);

    public void LoadDataOnDisk(uint address, ReadOnlySpan<byte> data) => _disk.WriteSectorDirect(address, data);
    public void LoadDataOnDisk(uint address, byte[] data) => _disk.WriteSectorDirect(address, data);
    
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

public static class DiskImageHelper
{
    public static byte[]? ReadSector(string imagePath, uint lba)
    {
        if (!File.Exists(imagePath))
            return null;

        using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
        long offset = lba * DiskDevice.SectorSize;
        if (offset + DiskDevice.SectorSize > fs.Length)
            return null;

        var buffer = new byte[DiskDevice.SectorSize];
        fs.Seek(offset, SeekOrigin.Begin);
        fs.ReadExactly(buffer, 0, DiskDevice.SectorSize);
        return buffer;
    }

    public static void WriteSector(string imagePath, uint lba, ReadOnlySpan<byte> data)
    {
        if (data.Length > DiskDevice.SectorSize)
            throw new ArgumentException("Данные превышают размер сектора");

        using var fs = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.Write);
        long offset = lba * DiskDevice.SectorSize;
        if (offset + DiskDevice.SectorSize > fs.Length)
            fs.SetLength(offset + DiskDevice.SectorSize);

        fs.Seek(offset, SeekOrigin.Begin);
        fs.Write(data);
        // Заполнить остаток нулями при необходимости
    }
}