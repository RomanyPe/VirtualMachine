using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.RamSystem;

namespace Kernel.ControllersData;

public interface IPortUse
{
    public byte ReadPort(ulong offset);
    public void WritePort(ulong offset, byte value);
    public void WakeProcessor();
}

public sealed class PortBus(SizePort ports, SizePortOnDevice portsOnDev, NameDeviceToken nameDevice, ReadOnlySpan<char> name)
{
    private readonly ulong _totalPorts = 1UL << (byte)ports;
    private readonly uint _portsOnDevice = 1U << (byte)portsOnDev;

    private readonly uint _sectorCount = 1U << ((byte)ports - (byte)portsOnDev);
    private readonly IPortUse[] _devices = new IPortUse[1U << ((byte)ports - (byte)portsOnDev)];

    private readonly NameDeviceToken _nameDevice = nameDevice.CreateChild(name);

    private readonly byte _deviceShift = (byte)portsOnDev;
    private readonly ulong _offsetMask = (1U << (byte)portsOnDev) - 1U;

    public NameDeviceToken NameDevice => _nameDevice;
    public uint SectorCount => _sectorCount;
    public uint PortsOnDevice => _portsOnDevice;
    public ulong TotalPorts => _totalPorts;

    public int AllocateFreeSector()
    {
        for (int i = 0; i < _sectorCount; i++)
            if (_devices[i] == null) return i;
        return -1;
    }

    public bool IsFreeSector(uint sector) => sector < _sectorCount && _devices[sector] == null;

    public bool RegisterDevice(IPortUse device, uint sector)
    {
        if (IsFreeSector(sector)) return false;

        _devices[sector] = device;
        return true;
    }

    public void UnregisterDevice(uint sector)
    {
        if (sector < _sectorCount)
            _devices[sector] = null!;
    }

    private (IPortUse? device, ulong offset) ResolveAddress(ulong address)
    {
        uint sector = (uint)(address >> _deviceShift);
        if (sector >= _sectorCount) return (null, 0);

        var dev = _devices[sector];
        if (dev != null)
        {
            ulong offset = address & _offsetMask;
            return (dev, offset);
        }
        return (null, address);
    }


    public RAMResultInt8 ReadPort(ulong address)
    {
        var (device, offset) = ResolveAddress(address);
        return device != null
            ? new(device.ReadPort(offset))
            : new(BiosStatus.NullDeviceOutput, address, _nameDevice);
    }

    public RAMResultInt8 WritePort(ulong address, byte value)
    {
        var (device, offset) = ResolveAddress(address);
        if (device != null)
        {
            device.WritePort(offset, value);
            return new(value);
        }
        return new(BiosStatus.NullDeviceInput, address, _nameDevice);
    }

    public bool WakeProcessor(ulong address)
    {
        var (device, _) = ResolveAddress(address);
        if (device != null)
        {
            device.WakeProcessor();
            return true;
        }
        return false;
    }

    public IPortUse? GetDevice(uint sector) => _devices[sector];
}