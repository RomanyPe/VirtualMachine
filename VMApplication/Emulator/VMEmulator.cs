using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.Contracts;
using Kernel.ControllersData;
using VMApplication.Logger;
using static Kernel.Utilites.ManagerDevices;

namespace VMApplication.Emulator;

public sealed class VMEmulator(Kernel.Utilites.Emulator emulator, VMHostLogger outputView, SizePortOnDevice portsPerDevice) : IDisposable
{
    private readonly Kernel.Utilites.Emulator _emulator = emulator;
    private readonly VMHostLogger _outputView = outputView;
    private readonly uint _portsOnDevice = 1U << (byte)portsPerDevice;

    public uint PortsPerDevice => _portsOnDevice;


    public DeviceContext? GetDeviceContext(int? id = null)
    {
        id ??= _emulator.MainDeviceId;
        return _emulator.TryGetDevice<Device>(id.Value, out var device) 
            ? new DeviceContext(device) 
            : null;
    }

    public bool ChangeDeviceSector(int id, uint newSector) => _emulator.ChangeDeviceSector(id, newSector);

    public IEnumerable<DeviceView> GetAllDevices() 
        => _emulator.AllDeviceInfo.Select(VMHostHelper.ConvertDeviceInfo);


    public int AddDevice(IPortController device,
        string? name,
        uint sector)
    {
        int id = _emulator.AddDevice(device, name, sector);
        return id;
    }
    
    public int CreateAndAddDevice(byte[] bios,
                                      RamSize ramSize,
                                      RamSize biosSize,
                                      uint sector,
                                      IDeviceLoggerContext? deviceCtx = null,
                                      string? name = null,
                                      IProcessorFaultPolicy? processorFaultPolicy = null)
    {
        var ctx = CreateDevice(bios, ramSize, biosSize, deviceCtx, name, processorFaultPolicy);
        return AddDevice(ctx, sector);
    }

    public int AddDevice(DeviceContext ctx, uint sector) => AddDevice(ctx.Device, ctx.Name, sector);

    public DeviceContext CreateDevice(byte[]? bios = null!,
                                      RamSize ramSize = RamSize.Size16KB,
                                      RamSize biosSize = RamSize.Size8KB,
                                      IDeviceLoggerContext? deviceCtx = null,
                                      string? name = null,
                                      IProcessorFaultPolicy? processorFaultPolicy = null)
    {
        deviceCtx ??= _outputView.CreateDefaultLoggerContext(new NameDeviceToken(name));
        processorFaultPolicy ??= IProcessorFaultPolicy.Default;
        bios ??= [];
        var device = _emulator.CreateDevice(bios, ramSize, biosSize, deviceCtx, processorFaultPolicy);
        return new DeviceContext(device, name);
    }

    public void SetMainDevice(int deviceId)
    {
        _emulator.SetMainDevice(deviceId);
    }

    public bool RemoveDevice(int id)
    {
        bool removed = _emulator.RemoveDevice(id);
        return removed;
    }

    public bool UpdateDeviceBios(int id, byte[] bios)
    {
        if (_emulator.TryGetDevice<Device>(id, out var device))
        {
            device.UpdateBios(bios);
            return true;
        }
        return false;
    }

    public void Reset() => _emulator.Reset();

    public void Dispose() => _emulator.Dispose();
}