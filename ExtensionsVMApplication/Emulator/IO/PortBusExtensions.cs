using Kernel.ControllersData;

namespace ExtensionsVMApplication.Emulator.IO;

public static class PortBusExtensions
{
    extension(PortBus bus)
    {
        public ulong GetBaseAddress(uint sector)
            => (ulong)sector * bus.PortsOnDevice;
    }
}