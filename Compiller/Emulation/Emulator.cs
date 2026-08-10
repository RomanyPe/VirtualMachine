using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.ControllersData;
using Kernel.Utilites;
using static Kernel.Utilites.ManagerDevices;

namespace Compiller.Emulation;

/// <summary>
/// Эмулятор устройства: управляет загрузкой программ, запуском, пошаговым выполнением.
/// Теперь поддерживает несколько устройств через ManagerDevices.
/// </summary>
public class Emulator
{
    private PortBus _portBus;
    private readonly ManagerDevices _manager;

    private int _mainDeviceId = -1;

    public Emulator(SizePort totalPorts = SizePort.Size16KB, SizePortOnDevice portsPerDevice = SizePortOnDevice.Size16B)
    {
        _portBus = new PortBus(totalPorts, portsPerDevice, new NameDeviceToken("System"), "PortBus");
        _manager = new ManagerDevices(_portBus);
    }
    public void ReloadEmulator(SizePort totalPorts = SizePort.Size16KB, SizePortOnDevice portsPerDevice = SizePortOnDevice.Size16B)
    {
        _portBus = new PortBus(totalPorts, portsPerDevice, new NameDeviceToken("System"), "PortBus");
    }
    public Device? CurrentDevice => MainDevice;
    public int MainDeviceId => _mainDeviceId;
    public int CountDevice => _manager.Count;
    public DeviceInfo? GetDeviceInfo(int i) => _manager.GetDeviceInfo(i);
    public DeviceInfo FirstDevice => _manager.FirstDeviceData;
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
            SizePort.Size16KB,
            SizePortOnDevice.Size16B,
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
        _manager.ClearAllDevices();
        _mainDeviceId = -1;
    }

    public bool ChangeDeviceSector(int deviceId, uint newSector) => _manager.ChangeSector(deviceId, newSector);
}