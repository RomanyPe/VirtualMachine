using Kernel.Contracts;
using Kernel.ControllersData;

namespace VMApplication.Project.IO;

public static class PortBusExtensions
{
    extension(PortBus bus)
    {
        public bool RegisterDeviceAtAddress(IPortUse device, ulong address)
        {
            uint portsOnDevice = bus.PortsOnDevice;                // 8
            if (address % portsOnDevice != 0) return false;        // не выровнен — не сектор
            uint sector = (uint)(address / portsOnDevice);
            return bus.RegisterDevice(device, sector);
        }

        public ulong GetBaseAddress(uint sector)
            => (ulong)sector * bus.PortsOnDevice;
    }
}