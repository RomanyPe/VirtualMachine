using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.Contracts;
using VMApplication.Logger;

namespace VMApplication.Emulator;

public sealed class VMEmulator(Kernel.Utilites.Emulator emulator, VMHostLogger outputView) : IDisposable
{
    private readonly Kernel.Utilites.Emulator _emulator = emulator;
    private readonly VMHostLogger _outputView = outputView;

    public DeviceContext? GetDeviceContext(int id)
    {
        return _emulator.TryGetDevice<Device>(id, out var device) 
            ? new DeviceContext(device) 
            : null;
    }

    public bool ChangeDeviceSector(int id, uint newSector) => _emulator.ChangeDeviceSector(id, newSector);

    public IReadOnlyCollection<DeviceInfo> GetAllDevices()
        => [.. _emulator.AllDeviceInfo];


    public int AddDevice(IPortController device,
        string? name,
        uint sector)
    {
        return _emulator.AddDevice(device, name, sector);
    }
    
    public int CreateAndAddDevice(RamSize ramSize,
                                      uint sector,
                                      IDeviceLoggerContext? deviceCtx = null,
                                      string? name = null,
                                      IProcessorFaultPolicy? processorFaultPolicy = null)
    {
        var ctx = CreateDevice(ramSize, deviceCtx, name, processorFaultPolicy);
        return AddDevice(ctx, sector);
    }

    public int AddDevice(DeviceContext ctx, uint sector) => AddDevice(ctx.UnsafeGetDevice, ctx.Name, sector);

    public DeviceContext CreateDevice(RamSize? ramSize = null,
                                      IDeviceLoggerContext? deviceCtx = null,
                                      string? name = null,
                                      IProcessorFaultPolicy? processorFaultPolicy = null)
    {
        ramSize ??= RamSize.MB1;
        deviceCtx ??= _outputView.CreateDefaultLoggerContext(new NameDeviceToken(name));
        processorFaultPolicy ??= IProcessorFaultPolicy.Default;
        var device = _emulator.CreateDevice(ramSize.Value, deviceCtx, processorFaultPolicy);
        return new DeviceContext(device, name);
    }

    public bool RemoveDevice(int id)
    {
        bool removed = _emulator.RemoveDevice(id);
        return removed;
    }

    public void Dispose() => _emulator.Dispose();
}