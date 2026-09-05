using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.ControllersData;
using Kernel.LocalMemorySystem;
using Kernel.Utilites;
using static Kernel.Utilites.ManagerDevices;

namespace Compiller.Emulation;

/// <summary>
/// Эмулятор устройства: управляет загрузкой программ, запуском, пошаговым выполнением.
/// Теперь поддерживает несколько устройств через ManagerDevices.
/// </summary>
public class Emulator : IDisposable
{
    private readonly PortBus _portBus;
    private readonly ManagerDevices _manager;
    private readonly DiskManager _diskManager;

    private int _mainDeviceId = -1;

    public Emulator(SizePort totalPorts = SizePort.Size16KB, SizePortOnDevice portsPerDevice = SizePortOnDevice.Size16B)
    {
        _portBus = new PortBus(totalPorts, portsPerDevice, new NameDeviceToken("Emulator".AsSpan()), "PortBus");
        _manager = new ManagerDevices(_portBus);
        _diskManager = new DiskManager(_portBus);
    }

    public PortBus PortBus => _portBus;
    public Device? CurrentDevice => MainDevice;
    public int MainDeviceId => _mainDeviceId;
    public int CountDevice => _manager.Count;
    public DeviceInfo? GetDeviceInfo(int i) => _manager.GetDeviceInfo(i);
    public IEnumerable<DeviceInfo> AllDevices => _manager.GetAllDevices();
    public void SetMainDevice(int id) => _mainDeviceId = id;

    public int CreateDevice(
        byte[] biosFirmware,
        RamSize size,
        uint sector,
        string? name = null,
        string? nameProc = null,
        string? nameRam = null,
        string? namePortBus = null)
    {
        int id = _manager.CreateNewDevice(
            size,
            name,
            nameProc,
            nameRam,
            namePortBus,
            sector,
            biosFirmware
        );

        if (id != -1 && _mainDeviceId == -1)
            _mainDeviceId = id;

        return id;
    }

    public Device? GetDevice(int id) => _manager.GetDevice(id);

    public Device? MainDevice => _manager.GetDevice(_mainDeviceId);

    public void Step(bool debugMode) => MainDevice?.NextStepProcessor(debugMode);
    public void Step(int count, bool debugMode) => MainDevice?.NextStepProcessorCount(count, debugMode);

    public string? DumpRegisters() => MainDevice?.GetAllData();

    public string? DumpRegisters(int id) => _manager.GetDevice(id)?.GetAllData();


    public void Reset()
    {
        _manager.Dispose();
        _mainDeviceId = -1;
        _diskManager.Dispose();
    }

    public int CreateDisk(string imagePath)
    {
        if (!Path.Exists(imagePath)) return -2;
        return _diskManager.CreateDisk(imagePath);
    }

    public int CreateDisk(string imagePath, uint sector)
    {
        if (!Path.Exists(imagePath)) return -3;
        return _diskManager.CreateDisk(imagePath, sector);
    }

    public bool RemoveDevice(int id)
    {
        bool removed = _manager.RemoveDevice(id);
        if (removed && _mainDeviceId == id)
            _mainDeviceId = -1;
        return removed;
    }

    public bool ChangeDiskSector(uint oldSector, uint newSector) => _diskManager.ChangeDiskSector(oldSector, newSector);
    public bool RemoveDisk(uint sector) => _diskManager.RemoveDisk(sector);
    public DiskDevice? GetDisk(uint sector) => _diskManager.GetDisk(sector);
    public bool ChangeDeviceSector(int id, uint newSector) => _manager.ChangeSector(id, newSector);
    public IEnumerable<int> GetDiskSectors() => _diskManager.GetAllDisks();

    public IEnumerable<DiskInfo> GetAllDiskData()
    {
       return _diskManager.GetAllDiskInfo();
    }

    public void Dispose()
    {
        Reset();
        GC.SuppressFinalize(this);
    }
}