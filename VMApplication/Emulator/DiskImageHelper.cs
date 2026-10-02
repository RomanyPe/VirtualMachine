using Kernel.LocalMemorySystem;

namespace VMApplication.Emulator;

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