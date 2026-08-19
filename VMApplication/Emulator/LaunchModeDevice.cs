using Kernel.BiosSystem;
using Kernel.Common;
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
        _device.LaunchDevice(startAddress,
                             debug,
                             delayMs,
                             showTimer,
                             callBack.OnTitleLaunch,
                             callBack.OnStart,
                             callBack.OnEnd);
    }

    public DeviceData StopAndReset(int stopTime = 3000, CallBackOnStop callBack = default)
    {
        _device.StopDevice(stopTime,
                           callBack.OnThreadIsLiveTrue,
                           callBack.OnThreadIsLiveFalse,
                           callBack.OnThreadStopedTrue,
                           callBack.OnThreadIsLiveFalse);
        return new DeviceData(_device);
    }

    public DeviceStepMode StepMode(ulong startAddress = ProjectBuilder.BaseAdressProgramm, Action<Action<string, LogLevel>>? titleAct = null)
    {
        _device.BreakPointerLaunchDevice(startAddress, titleAct);
        return new DeviceStepMode(_device);
    }
}