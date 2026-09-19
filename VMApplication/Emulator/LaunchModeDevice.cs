using Kernel.BiosSystem;
using Kernel.Common;
using VMApplication.Project;

namespace VMApplication.Emulator;

public class LaunchModeDevice
{
    private readonly Device _device;

    internal LaunchModeDevice(Device device) => _device = device;

    public void Launch(ulong startAddress = ProjectBuilder.BaseAdressProgram,
                             bool debug = false,
                             int delayMs = 0,
                             bool showTimer = false,
                             Action<IDeviceLoggerContext>? onLaunch = null,
                             Action<IDeviceLoggerContext>? onStart = null,
                             Action<IDeviceLoggerContext, ISimulationResult?>? onEnd = null)
    {
        _device.RunSimulation(startAddress,
                             debug,
                             delayMs,
                             showTimer,
                             onLaunch,
                             onStart,
                             onEnd);
    }

    public DeviceContext StopAndReset()
    {
        _device.Stop();
        return new DeviceContext(_device);    
    }
    public void Stop() => _device.Stop();

    public DeviceStepMode StepMode(ulong startAddress = ProjectBuilder.BaseAdressProgram, Action<IDeviceLoggerContext>? titleAct = null)
    {
        _device.BreakPointerLaunchDevice(startAddress, titleAct);
        return new DeviceStepMode(_device);
    }
}