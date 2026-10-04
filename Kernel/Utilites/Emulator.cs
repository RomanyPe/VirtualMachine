using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.Contracts;
using Kernel.ControllersData;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using static Kernel.Utilites.ManagerDevices;

namespace Kernel.Utilites;

/// <summary>
/// Эмулятор устройства: управляет загрузкой программ, запуском, пошаговым выполнением.
/// Теперь поддерживает несколько устройств через ManagerDevices.
/// </summary>
public class Emulator : IDisposable
{
    private readonly PortBus _portBus;
    private readonly ManagerDevices _manager;

    private int _mainDeviceId = -1;

    public Emulator(SizePort totalPorts = SizePort.Size16KB, SizePortOnDevice portsPerDevice = SizePortOnDevice.Size16B)
    {
        _portBus = new PortBus(totalPorts, portsPerDevice);
        _manager = new ManagerDevices(_portBus);
    }

    public int MainDeviceId => _mainDeviceId;
    public DeviceInfo? GetDeviceInfo(int i) => _manager.GetDeviceInfo(i);
    public IEnumerable<DeviceInfo> AllDeviceInfo => _manager.GetAllDevices();
    public void SetMainDevice(int id) => _mainDeviceId = id;

    public int AddDevice(
        IPortController device,
        string? name,
        uint sector)
    {
        int id = _manager.AddDevice(device, name, sector);

        if (id != -1 && _mainDeviceId == -1)
            _mainDeviceId = id;

        return id;
    }

    public Device CreateDevice(
        byte[] bios,
        RamSize size,
        RamSize sizeBios,
        IDeviceLoggerContext ctx,
        IProcessorFaultPolicy? processorFaultPolicy = null)
    {
        return new Device(bios, _portBus, size, sizeBios, ctx, processorFaultPolicy);
    }


    public IPortController? GetDevice(int id) => _manager.GetDevice(id);

    public T? GetDeviceAs<T>(int id) where T : class, IPortController
        => _manager.GetDeviceAs<T>(id);

    public bool TryGetDevice<T>(int id, [NotNullWhen(true)] out T? device)
        where T : class, IPortController => _manager.TryGetDevice(id, out device);

    //public void CopyRegisters(Span<ulong> destination) => MainDevice?.CopyRegisters(destination);

    //public void CopyRegisters(Span<ulong> destination, int id) => _manager.GetDevice(id)?.CopyRegisters(destination);

    //public ulong[]? GetRegistersSnapshot() => MainDevice?.GetRegistersSnapshot();

    //public ulong[]? GetRegistersSnapshot(int id) => _manager.GetDevice(id)?.GetRegistersSnapshot();


    public void Reset()
    {
        _manager.Dispose();
        _mainDeviceId = -1;
    }

    public bool RemoveDevice(int id)
    {
        bool removed = _manager.RemoveDevice(id);
        if (removed && _mainDeviceId == id)
            _mainDeviceId = -1;
        return removed;
    }

    public bool ChangeDeviceSector(int id, uint newSector) => _manager.ChangeSector(id, newSector);


    public void Dispose()
    {
        Reset();
        GC.SuppressFinalize(this);
    }
}