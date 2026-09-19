using Kernel.Common;
using Kernel.LocalMemorySystem;
using VMApplication.Logger;
using static Kernel.Utilites.ManagerDevices;

namespace VMApplication.Emulator;

public sealed class VMEmulator(Compiller.Emulation.Emulator emulator, VMHostLogger outputView, SizePortOnDevice portsPerDevice) : IDisposable
{
    private readonly Compiller.Emulation.Emulator _emulator = emulator;
    private readonly VMHostLogger _outputView = outputView;
    private readonly uint _portsOnDevice = 1U << (byte)portsPerDevice;

    public uint PortsPerDevice => _portsOnDevice;

    public event Action<IEnumerable<DeviceInfo>>? DeviceListChanged;

    public DeviceContext? CreateDeviceContext(int? id = null)
    {
        var device = id != null ? _emulator.GetDevice(id.Value) : _emulator.MainDevice;
        if (device == null)
        {
            _outputView.AppendLine("Выбраное устройство/основное не инициализированы", LogLevel.Error);
            return null;
        }
        return new DeviceContext(device);
    }

    public bool ChangeDeviceSector(int id, uint newSector) => _emulator.ChangeDeviceSector(id, newSector);

    public DeviceContext? GetDeviceData(int id)
    {
        var d = _emulator.GetDevice(id);
        return d != null ? new(d) : null;
    }
    public bool ChangeDiskSector(uint oldSector, uint newSector)
        => _emulator.ChangeDiskSector(oldSector, newSector);

    public IEnumerable<DeviceView> GetAllDevices() 
        => _emulator.AllDevices.Select(VMHostHelper.ConvertDeviceInfo);

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

    public int CreateDevice(byte[] bios, RamSize ramSize, uint sector, string? name = null, string? procName = null, string? portBusName = null)
    {
        int id = _emulator.CreateDevice(bios, ramSize, sector, name, procName, portBusName);
        if (id >= 0)
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
            _outputView.AppendLine($"Disk created in sector {sector} (base port address: {sector * (int)_portsOnDevice})", LogLevel.Log);
        else
            _outputView.AppendLine("Failed to create disk (no free port sectors available)", LogLevel.Error);
        return sector;
    }

    public int CreateDisk(string imagePath, uint sector)
    {
        int flag = _emulator.CreateDisk(imagePath, sector);
        if (flag < 0)
            _outputView.AppendLine($"Failed to create disk (sector is occupied), Code Error {flag}", LogLevel.Error);
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
    public void WriteBootableProgramAndCreateImage(string imagePath, byte[] program)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentException("Путь к файлу образа не может быть пустым.", nameof(imagePath));

        if (program == null || program.Length == 0)
            throw new ArgumentException("Массив программы не может быть пустым.", nameof(program));

        int sectorCount = Math.Max(1, (program.Length + 8 + DiskDevice.SectorSize - 1) / DiskDevice.SectorSize);

        using (FileStream fs = File.Create(imagePath))
        {
            fs.SetLength(DiskDevice.SectorSize * sectorCount);
        }

        using var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Write);
        Span<byte> lengthBytes = stackalloc byte[8];
        BitConverter.TryWriteBytes(lengthBytes, (ulong)program.Length);
        stream.Write(lengthBytes);
        stream.Write(program);
    }

    public void WriteBootableProgram(string imagePath, byte[] program)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentException("Путь к файлу образа не может быть пустым.", nameof(imagePath));

        if (program == null || program.Length == 0)
            throw new ArgumentException("Массив программы не может быть пустым.", nameof(program));

        if (!File.Exists(imagePath))
            throw new FileNotFoundException($"Файл образа не найден по пути: {imagePath}", imagePath);

        long requiredSize = 8 + program.Length;
        long fileSize = new FileInfo(imagePath).Length;
        if (fileSize < requiredSize)
        {
            throw new ArgumentException(
                $"Размер программы с заголовком ({requiredSize} байт) превышает размер файла образа ({fileSize} байт).",
                nameof(program));
        }
        using var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Write);

        Span<byte> lengthBytes = stackalloc byte[8];
        BitConverter.TryWriteBytes(lengthBytes, (ulong)program.Length);

        stream.Write(lengthBytes);
        stream.Write(program);
    }


    public int CreateDisk(string imagePath, int sectorCount, uint sector)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sectorCount);

        long diskSize = DiskDevice.SectorSize * sectorCount;

        using var fs = new FileStream(imagePath, FileMode.Create, FileAccess.Write);
        fs.SetLength(diskSize);
        return _emulator.CreateDisk(imagePath, sector);

    }

    public int CreateDisk(string imagePath, int sectorCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sectorCount);

        long size = DiskDevice.SectorSize * sectorCount;
        using var fs = File.Create(imagePath);
        fs.SetLength(size);

        return _emulator.CreateDisk(imagePath);
    }

    // Получение обёртки диска
    public DiskContext? GetDiskContext(uint sector)
    {
        var disk = _emulator.GetDisk(sector);
        if (disk == null)
            return null;

        string imagePath = disk.ImagePath; // предположим, мы добавили такое свойство
        return new DiskContext(disk, sector, imagePath, this);
    }

    // Удаление диска
    public bool RemoveDisk(uint sector)
    {
        bool success = _emulator.RemoveDisk(sector);
        if (success)
            _outputView.AppendLine($"Диск в секторе {sector} удалён", LogLevel.Log);
        return success;
    }

    // Список всех дисков (секторов)
    public IEnumerable<int> ListDisksSectors() => _emulator.GetDiskSectors();

    public IEnumerable<DiskInfo> GetAllDiskInfo() => _emulator.GetAllDiskData();

    public void Reset() => _emulator.Reset();

    public void Dispose() => _emulator.Dispose();
}