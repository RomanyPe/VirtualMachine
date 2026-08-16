using Kernel.Common;
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
            _outputView.Append($"Диск создан в секторе {sector} (базовый адрес портов: {sector * (int)_portsPerDevice})", LogLevel.Log);
        else
            _outputView.Append("Не удалось создать диск (нет свободных секторов портов)", LogLevel.Error);
        return sector;
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
