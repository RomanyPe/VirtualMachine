using Kernel.BiosSystem;
using Kernel.Common;
using ThreadingSystem;
using ThreadingSystem.ThreadControl;
using VMApplication.Project;

namespace VMApplication.Emulator;

public class LaunchModeDevice
{
    private readonly Device _device;

    internal LaunchModeDevice(Device device) => _device = device;

    public void LaunchDeviceOnDedicatedThread(ulong startAddress = ProjectBuilder.BaseAdressProgram,
                             bool debug = false,
                             int delayMs = 0,
                             bool showTimer = false,
                             Action<IDeviceLoggerContext>? onLaunch = null,
                             Action<IDeviceLoggerContext>? onStart = null,
                             Action<IDeviceLoggerContext>? onEnd = null)
    {
    _device.LaunchDeviceOnDedicatedThread(startAddress,
                             debug,
                             delayMs,
                             showTimer,
                             onLaunch,
                             onStart,
                             onEnd);
    }

    public ThreadHandle? LaunchManagedDeviceThread(ThreadScheduler scheduler,
        ulong startAddress = ProjectBuilder.BaseAdressProgram,
        bool debug = false,
        int delayMs = 0,
        bool showTimer = false,
        Action<IDeviceLoggerContext>? onLaunch = null,
        Action<IDeviceLoggerContext>? onStart = null,
        Action<IDeviceLoggerContext>? onEnd = null)
    {
        return _device.LaunchDeviceAsThreadTask(
            scheduler,
            startAddress,
            debug,
            delayMs,
            showTimer,
            onLaunch,
            onStart,
            onEnd);
    }

    public void LaunchDeviceOnMainThread(ulong startAddress = ProjectBuilder.BaseAdressProgram,
                             bool debug = false,
                             int delayMs = 0,
                             bool showTimer = false,
                             Action<IDeviceLoggerContext>? onLaunch = null,
                             Action<IDeviceLoggerContext>? onStart = null,
                             Action<IDeviceLoggerContext>? onEnd = null)
    {
        _device.LaunchDeviceOnMainThread(startAddress,
                             debug,
                             delayMs,
                             showTimer,
                             onLaunch,
                             onStart,
                             onEnd);
    }

    public DeviceContext StopAndReset(int stopTime = 3000,
            Action<IDeviceLoggerContext>? onThreadIsDead = null,
            Action<IDeviceLoggerContext>? onThreadIsLiveTrue = null,
            Action<IDeviceLoggerContext>? onThreadStopedTrue = null,
            Action<IDeviceLoggerContext>? OnThreadStopedFalse = null)
    {
            _device.StopDevice(stopTime,
                               onThreadIsLiveTrue,
                               onThreadIsDead,
                               onThreadStopedTrue,
                               OnThreadStopedFalse);
            return new DeviceContext(_device);
    }
    public DeviceContext StopAndReset(ThreadHandle handle, TimeSpan stopTime,
                Action<IDeviceLoggerContext>? onSuccess = null,
                Action<IDeviceLoggerContext>? onTimeout = null)
    {
        _device.Stop(handle,
                     stopTime,
                     onSuccess,
                     onTimeout);
        return new DeviceContext(_device);
    }

    public DeviceStepMode StepMode(ulong startAddress = ProjectBuilder.BaseAdressProgram, Action<IDeviceLoggerContext>? titleAct = null)
    {
        _device.BreakPointerLaunchDevice(startAddress, titleAct);
        return new DeviceStepMode(_device);
    }
}