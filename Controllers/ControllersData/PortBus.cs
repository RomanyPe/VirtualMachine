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
        if (device != null) return device.Read(offset);
        else return new RAMResultInt8(BiosStatus.NullDeviceOutput, address, _nameDevice);
    }

    public RAMResultInt8 WritePort(ulong address, byte value)
    {
        var (device, offset) = ResolveAddress(address);
        if (device != null) return device.Write(offset, value);
        else return new RAMResultInt8(BiosStatus.NullDeviceInput, address, _nameDevice);
    }
}

public enum SizePort : ulong
{
    [Description("64 байта")] Size64B = 1U << 6,
    [Description("128 байт")] Size128B = 1U << 7,
    [Description("256 байт")] Size256B = 1U << 8,
    [Description("512 байт")] Size512B = 1U << 9,
    [Description("1 КБ")] Size1KB = 1U << 10,
    [Description("4 КБ")] Size4KB = 1U << 12,
    [Description("8 КБ")] Size8KB = 1U << 13,
    [Description("16 КБ")] Size16KB = 1U << 14,
    [Description("64 КБ")] Size64KB = 1U << 16,
    [Description("128 КБ")] Size128KB = 1U << 17,
    [Description("256 КБ")] Size256KB = 1U << 18,
    [Description("512 КБ")] Size512KB = 1U << 19,
}

public enum SizePortOnDev : uint
{
    [Description("4 байта")] Size4B = 1U << 2,
    [Description("8 байт")] Size8B = 1U << 3,
    [Description("16 байт")] Size16B = 1U << 4,
    [Description("32 байта")] Size32B = 1U << 5,
}