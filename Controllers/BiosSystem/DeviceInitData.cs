using Kernel.ControllersData;
using Kernel.RamSystem;

namespace Kernel.BiosSystem;

public readonly ref struct DeviceInitData
{
    public readonly RamSize Size;
    public readonly SizePort SizePort;
    public readonly SizePortOnDev SizeDev;
    public readonly ReadOnlySpan<char> Name;
    public readonly ReadOnlySpan<char> NameProc;
    public readonly ReadOnlySpan<char> NameRam;
    public readonly ReadOnlySpan<char> NamePortBus;

    // Первичные конструкторы не поддерживают ReadOnlySpan в качестве параметров по умолчанию по старой схеме,
    // поэтому классический конструктор здесь будет надежнее и чище:
    public DeviceInitData(
        RamSize size = RamSize.Size4MB,
        SizePort sizePort = SizePort.Size1KB,
        SizePortOnDev sizeDev = SizePortOnDev.Size16B,
        ReadOnlySpan<char> name = default,
        ReadOnlySpan<char> nameProc = default,
        ReadOnlySpan<char> nameRam = default,
        ReadOnlySpan<char> namePortBus = default)
    {
        Size = size;
        SizePort = sizePort;
        SizeDev = sizeDev;
        Name = name.IsEmpty ? NameDeviceToken.UnknownName : name;
        NameProc = nameProc.IsEmpty ? NameDeviceToken.UnknownName : nameProc;
        NameRam = nameRam.IsEmpty ? NameDeviceToken.UnknownName : nameRam;
        NamePortBus = namePortBus.IsEmpty ? NameDeviceToken.UnknownName : namePortBus;
    }
}
