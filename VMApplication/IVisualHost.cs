using Kernel.ProcessorSystem;
using static Kernel.Utilites.ManagerDevices;

namespace VMApplication;

public interface IVisualHost
{
    void ShowOutput(string message, LogLevel level);
    void RefreshDeviceList(IEnumerable<DeviceInfo> devices);
}

