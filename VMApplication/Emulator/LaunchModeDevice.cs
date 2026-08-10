using Kernel.BiosSystem;
using VMApplication.CallBacks;
using VMApplication.Project;

namespace VMApplication.Emulator;

public class LaunchModeDevice
{
    private readonly Device _device;

    internal LaunchModeDevice(Device device) => _device = device;

    public void SetHeapAddress(ulong hp) => _device.InitHeap(hp);

    public void LaunchDevice(ulong startAddress = ProjectBuilder.BaseAdressProgramm,
                             bool debug = false,
                             int delayMs = 0,
                             bool showTimer = false,
                             CallBackOnLaunch callBack = default)
    {
        _device.LaunchDevice(startAddress, debug, delayMs, showTimer, callBack.OnLaunch!, callBack.OnStep!, callBack.OnEnd!);
    }

    public DeviceData StopAndReset()
    {
        _device.StopDevice();
        return new DeviceData(_device);
    }

    public DeviceStepMode StepMode(ulong startAddress = ProjectBuilder.BaseAdressProgramm)
    {
        _device.BreakPointerLaunchDevice(startAddress);
        return new DeviceStepMode(_device);
    }
}