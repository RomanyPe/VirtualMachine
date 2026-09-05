using Kernel.Common;
using Kernel.ControllersData;
using Kernel.LocalMemorySystem;

namespace Kernel.Utilites;

public class DiskManager(PortBus portBus) : IDisposable
{
    private readonly PortBus _portBus = portBus;
    private readonly Dictionary<uint, DiskDevice> _disks = []; // ключ – номер сектора

    /// <summary>
    /// Создаёт диск и регистрирует его. Возвращает номер сектора или -1.
    /// </summary>
    public int CreateDisk(string imagePath)
    {
        int sector = _portBus.AllocateFreeSector();
        if (sector < 0) return sector;
        if (!_portBus.IsFreeSector((uint)sector)) return -5;

        var disk = new DiskDevice(imagePath);
        if (!_portBus.RegisterDevice(disk, (uint)sector))
        {
            disk.Dispose();
            return -1;
        }

        _disks[(uint)sector] = disk;
        return sector;
    }

    public int CreateDisk(string imagePath, uint sector)
    {
        if (!_portBus.IsFreeSector(sector)) return -1;

        var disk = new DiskDevice(imagePath);
        if (!_portBus.RegisterDevice(disk, sector))
        {
            disk.Dispose();
            return -2;
        }

        _disks[sector] = disk;
        return 0;
    }

    public bool ChangeDiskSector(uint oldSector, uint newSector)
    {
        if (!_disks.TryGetValue(oldSector, out var disk))
            return false;

        if (!_portBus.IsFreeSector(newSector))
            return false;

        if (!_portBus.RegisterDevice(disk, newSector))
            return false;

        _portBus.UnregisterDevice(oldSector);
        _disks.Remove(oldSector);
        _disks[newSector] = disk;
        return true;
    }

    public bool RemoveDisk(uint sector)
    {
        if (!_disks.TryGetValue(sector, out var disk))
            return false;
        _portBus.UnregisterDevice(sector);
        disk.Dispose();
        _disks.Remove(sector);
        return true;
    }

    public IEnumerable<int> GetAllDisks() => from KeyValuePair<uint, DiskDevice> disk in _disks
                                             select (int)disk.Key;

    public IEnumerable<DiskInfo> GetAllDiskInfo()
    {
        foreach (KeyValuePair<uint, DiskDevice> item in _disks)
        {
            yield return new DiskInfo(item.Value.SizeDisk,
                                      item.Value.CountSectors,
                                      item.Key,
                                      item.Value.CreatedAt,
                                      item.Value.ImagePath);
        }
    }

    public DiskDevice? GetDisk(uint sector) => _disks.GetValueOrDefault(sector);

    public void Clear()
    {
        foreach (var sector in _disks.Keys.ToArray())
            RemoveDisk(sector);
    }

    public void Dispose()
    {
        Clear();
        GC.SuppressFinalize(this);
    }
}