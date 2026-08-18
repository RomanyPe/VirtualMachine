using Kernel.Common;
using Kernel.LocalMemorySystem;
using VMApplication.Logger;
using static Kernel.Utilites.ManagerDevices;

namespace VMApplication.Emulator;

public sealed class VMEmulator(Compiller.Emulation.Emulator emulator, VMHostLogger outputView, SizePortOnDevice portsPerDevice)
{
    private readonly Compiller.Emulation.Emulator _emulator = emulator;
    private readonly VMHostLogger _outputView = outputView;
    private readonly SizePortOnDevice _portsPerDevice = portsPerDevice;

    public SizePortOnDevice PortsPerDevice => _portsPerDevice;

    public event Action<IEnumerable<DeviceInfo>>? DeviceListChanged;

    public DeviceData? CreateDeviceContext(int? id = null)
    {
        var device = id != null ? _emulator.GetDevice(id.Value) : _emulator.MainDevice;
        if (device == null)
        {
            _outputView.Append("Выбраное устройство/основное не инициализированы", LogLevel.Error);
            return null;
        }
        return new DeviceData(device);
    }

    public bool ChangeDeviceSector(int id, uint newSector) => _emulator.ChangeDeviceSector(id, newSector);

    public DeviceData? GetDeviceData(int id)
    {
        var d = _emulator.GetDevice(id);
        return d != null ? new(d) : null;
    }

    public IEnumerable<DeviceView> GetAllDevices()
    {
        return _emulator.AllDevices.Select(VMHostHelper.ConvertDeviceInfo);
    }

    public string GetDumpRegisters()
    {
        var res = _emulator.DumpRegisters();
        if (string.IsNullOrEmpty(res))
        {
            return "Не получилось получить дамб регистров";
        }
        return res;
    }

    public string GetDumpRegisters(int id)
    {
        var res = _emulator.DumpRegisters(id);
        if (string.IsNullOrEmpty(res))
        {
            return $"Не получилось получить дамб регистров для устройства {id}";
        }
        return res;
    }

    public int CreateDevice(byte[] bios, RamSize ramSize, uint sector, string? name = null, string? procName = null, string? ramName = null, string? portBusName = null)
    {
        int id = _emulator.CreateDevice(bios, ramSize, sector, name, procName, ramName, portBusName);
        if (id != -1)
            DeviceListChanged?.Invoke(_emulator.AllDevices);
        return id;
    }

    public void SetMainDevice(int deviceId)
    {
        _emulator.SetMainDevice(deviceId);
    }

    public DeviceView? MainDevice()
    {
        if (_emulator.MainDeviceId == -1 || _emulator.MainDevice == null) return null;
        var info = _emulator.GetDeviceInfo(_emulator.MainDeviceId);
        if (info == null) return null;
        return info.Value.ConvertDeviceInfo();
    }

    public int CreateDisk(string imagePath)
    {
        int sector = _emulator.CreateDisk(imagePath);
        if (sector != -1)
            _outputView.Append($"Disk created in sector {sector} (base port address: {sector * (int)_portsPerDevice})", LogLevel.Log);
        else
            _outputView.Append("Failed to create disk (no free port sectors available)", LogLevel.Error);
        return sector;
    }

    public bool CreateDisk(string imagePath, uint sector)
    {
        bool flag = _emulator.CreateDisk(imagePath, sector);
        if (flag)
            _outputView.Append($"Disk created in sector {sector} (base port address: {sector * (int)_portsPerDevice})", LogLevel.Log);
        else
            _outputView.Append("Failed to create disk (sector is occupied)", LogLevel.Error);
        return flag;
    }


    public bool RemoveDevice(int id)
    {
        bool removed = _emulator.RemoveDevice(id);
        if (removed)
            DeviceListChanged?.Invoke(_emulator.AllDevices);
        return removed;
    }

    public bool UpdateDeviceBios(int id, byte[] bios)
    {
        var device = _emulator.GetDevice(id);
        if (device == null) return false;

        device.UpdateBios(bios);
        DeviceListChanged?.Invoke(_emulator.AllDevices);
        return true;
    }

    /// <summary>
    /// Создаёт файл образа диска заданного размера и регистрирует его в эмуляторе.
    /// </summary>
    /// <param name="imagePath">Путь к файлу образа.</param>
    /// <param name="sectorCount">Количество секторов (размер сектора 512 байт).</param>
    /// <returns>Номер сектора, выделенного под диск, или -1 при ошибке.</returns>
    public int CreateDisk(string imagePath, int sectorCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sectorCount);

        long diskSize = DiskDevice.SectorSize * sectorCount;

        // Создаём файл нужного размера (можно перезаписать существующий)
        using (var fs = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
        {
            fs.SetLength(diskSize);
        }

        return _emulator.CreateDisk(imagePath);
    }

    // Получение обёртки диска
    public DiskData? GetDiskData(int sector)
    {
        var disk = _emulator.GetDisk((uint)sector);
        if (disk == null)
            return null;
        // Нужно знать путь к файлу образа – либо хранить его в DiskDevice, либо передавать при создании
        // Можно добавить в DiskDevice свойство ImagePath
        string imagePath = disk.ImagePath; // предположим, мы добавили такое свойство
        return new DiskData(disk, sector, imagePath, this);
    }

    // Удаление диска
    public bool RemoveDisk(uint sector)
    {
        bool success = _emulator.RemoveDisk(sector);
        if (success)
            _outputView.Append($"Диск в секторе {sector} удалён", LogLevel.Log);
        return success;
    }

    // Список всех дисков (секторов)
    public IEnumerable<int> ListDisks()
    {
        // Нужен метод в Compiller.Emulation.Emulator или DiskManager для перечисления
        // Можно добавить в DiskManager метод GetSectors()
        return _emulator.GetDiskSectors(); // пример
    }
}

public static class DiskImageWriter
{
    /// <summary>
    /// Создаёт загрузочный образ диска: первые 8 байт — размер программы (ulong), затем сама программа.
    /// Файл образа создаётся/перезаписывается.
    /// </summary>
    /// <param name="imagePath">Путь к образу.</param>
    /// <param name="program">Скомпилированные байты программы.</param>
    /// <param name="sectorCount">Количество секторов образа (по умолчанию 1).</param>
    public static void WriteBootableImage(string imagePath, byte[] program, int sectorCount = 1)
    {
        long diskSize = DiskDevice.SectorSize * sectorCount;
        using var fs = new FileStream(imagePath, FileMode.Create, FileAccess.Write);
        fs.SetLength(diskSize);

        // Записываем размер программы (8 байт, little-endian)
        ulong length = (ulong)program.Length;
        Span<byte> lengthBytes = stackalloc byte[8];
        BitConverter.TryWriteBytes(lengthBytes, length);
        fs.Write(lengthBytes);

        // Записываем программу
        fs.Write(program, 0, program.Length);
        // Остальная часть образа заполнена нулями
    }
}