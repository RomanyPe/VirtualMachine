using Kernel.BiosSystem;
using Kernel.RamSystem;
using System.ComponentModel;

namespace Kernel.ControllersData;

public class PortBus(SizePort ports, SizePortOnDev portsOnDev, NameDeviceToken nameDevice, ReadOnlySpan<char> name)
{
    private readonly ulong _totalPorts = (ulong)ports;
    private readonly uint _portsOnDevice = (uint)portsOnDev;
    private readonly uint _sectorCount = (uint)ports / (uint)portsOnDev;
    private readonly Device[] _devices = new Device[(uint)ports / (uint)portsOnDev];
    private readonly NameDeviceToken _nameDevice = nameDevice.CreateChild(name);

    public NameDeviceToken NameDevice => _nameDevice;
    public uint SectorCount => _sectorCount;
    public int AllocateFreeSector()
    {
        for (int i = 0; i < _sectorCount; i++)
            if (_devices[i] == null) return i;
        return -1;
    }

    public bool RegisterDevice(Device device, uint sector)
    {
        if (sector >= _sectorCount || _devices[sector] != null) return false;

        _devices[sector] = device;
        return true;
    }

    public void UnregisterDevice(uint sector)
    {
        if (sector < _sectorCount)
            _devices[sector] = null!;
    }

    private (Device? device, ulong offset) ResolveAddress(ulong address)
    {
        uint sector = (uint)(address / _portsOnDevice);
        if (sector >= _sectorCount) return (null, 0);

        var dev = _devices[sector];
        if (dev != null)
        {
            ulong offset = address % _portsOnDevice;
            return (dev, offset);
        }
        else
        {
            return (null, address);
        }
    }

    public RAMResultInt8 ReadPort(ulong address)
    {
        var (device, offset) = ResolveAddress(address);
        if (device != null) return new(device.ReadPort(offset));
        else return new RAMResultInt8(BiosStatus.NullDeviceOutput, address, _nameDevice);
    }

    public RAMResultInt8 WritePort(ulong address, byte value)
    {
        var (device, offset) = ResolveAddress(address);
        if (device != null)
        {
            device.WritePort(offset, value);
            return new(value);
        }
        else return new RAMResultInt8(BiosStatus.NullDeviceInput, address, _nameDevice);
    }
}

public enum SizePort : ulong
{
    Size64B = 1U << 6,
    Size128B = 1U << 7,
    Size256B = 1U << 8,
    Size512B = 1U << 9,
    Size1KB = 1U << 10,
    Size4KB = 1U << 12,
    Size8KB = 1U << 13,
    Size16KB = 1U << 14,
    Size64KB = 1U << 16,
    Size128KB = 1U << 17,
    Size256KB = 1U << 18,
    Size512KB = 1U << 19,
}

public enum SizePortOnDev : uint
{
    Size4B = 1U << 2,
    Size8B = 1U << 3,
    Size16B = 1U << 4,
    Size32B = 1U << 5,
}