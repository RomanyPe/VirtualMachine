using Kernel.BiosSystem;
using Kernel.Common;
using ThreadingSystem;
using ThreadingSystem.ThreadControl;
using VMApplication.CallBacks;
using VMApplication.Project;

namespace VMApplication.Emulator;

public class LaunchModeDevice
{
    private readonly Device _device;

    internal LaunchModeDevice(Device device) => _device = device;

    public void SetHeapAddress(ulong hp) => _device.InitHeap(hp);

    [Obsolete(
    """
    Используйте метод 'LaunchDeviceAsThreadTask', работающий через ThreadScheduler.
    Внимание: Прямой запуск потока может конфликтовать с планировщиком в режиме MaxThreadCPU, 
    вызывая просадки производительности. Сохраняйте возвращаемый 'ThreadHandle' для последующей остановки.
    """, error: false)]
    public void LaunchDeviceOnDedicatedThread(ulong startAddress = ProjectBuilder.BaseAdressProgram,
                             bool debug = false,
                             int delayMs = 0,
                             bool showTimer = false,
                             CallBackOnLaunch callBack = default)
    {
        _device.LaunchDeviceOnDedicatedThread(startAddress,
                             debug,
                             delayMs,
                             showTimer,
                             callBack.OnTitleLaunch,
                             callBack.OnStart,
                             callBack.OnEnd);
    }

    public ThreadHandle? LaunchManagedDeviceThread(ThreadScheduler scheduler,
        ulong startAddress = ProjectBuilder.BaseAdressProgram,
        bool debug = false,
        int delayMs = 0,
        bool showTimer = false,
        CallBackOnLaunch callBack = default)
    {
        return _device.LaunchManagedDeviceThread(
            scheduler,
            startAddress,
            debug,
            delayMs,
            showTimer,
            callBack.OnTitleLaunch,
            callBack.OnStart,
            callBack.OnEnd);
    }

    public void LaunchDeviceOnMainThread(ulong startAddress = ProjectBuilder.BaseAdressProgram,
                             bool debug = false,
                             int delayMs = 0,
                             bool showTimer = false,
                             CallBackOnLaunch callBack = default)
    {
        _device.LaunchDeviceOnMainThread(startAddress,
                             debug,
                             delayMs,
                             showTimer,
                             callBack.OnTitleLaunch,
                             callBack.OnStart,
                             callBack.OnEnd);
    }

    [Obsolete("""
    Используйте новую перегрузку 'StopAndReset(ThreadHandle, ...)' и затем вручную сбросьте состояние.
    
    Внимание: Этот метод жестко завязан на старый механизм 'StopDevice'. 
    Попытка вызвать его для задачи из ThreadScheduler приведет к утечке ресурсов или зависанию, 
    так как у него нет доступа к дескриптору потока (ThreadHandle).
    """, error: false)]
    public DeviceContext StopAndReset(int stopTime = 3000, CallBackOnStopDecidedThread callBack = default)
    {
        _device.StopDevice(stopTime,
                           callBack.OnThreadIsLiveTrue,
                           callBack.OnThreadIsDead,
                           callBack.OnThreadStopedTrue,
                           callBack.OnThreadIsDead);
        return new DeviceContext(_device);
    }
    public DeviceContext StopAndReset(ThreadHandle handle, TimeSpan stopTime, CallBackOnStopDecidedThread callBack = default)
    {
        _device.Stop(handle,
                     stopTime,
                     callBack.OnThreadStopedTrue,
                     callBack.OnThreadIsDead);
        return new DeviceContext(_device);
    }

    public DeviceStepMode StepMode(ulong startAddress = ProjectBuilder.BaseAdressProgram, Action<IDeviceLoggerContext>? titleAct = null)
    {
        _device.BreakPointerLaunchDevice(startAddress, titleAct);
        return new DeviceStepMode(_device);
    }
}