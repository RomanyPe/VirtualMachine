using Kernel.Common;

namespace VMApplication.CallBacks;

public readonly struct CallBackOnLaunch(Action<IDeviceLoggerContext>? onLaunch = null,
                                        Action<IDeviceLoggerContext>? onStart = null,
                                        Action<IDeviceLoggerContext>? onEnd = null)
{
    public readonly Action<IDeviceLoggerContext>? OnTitleLaunch = onLaunch;
    public readonly Action<IDeviceLoggerContext>? OnStart = onStart;
    public readonly Action<IDeviceLoggerContext>? OnEnd = onEnd;
}

public readonly struct CallBackOnStopDecidedThread(Action<IDeviceLoggerContext>? act1 = null,
                                        Action<IDeviceLoggerContext>? act2 = null,
                                        Action<IDeviceLoggerContext>? act3 = null,
                                        Action<IDeviceLoggerContext>? act4 = null)
{
    public readonly Action<IDeviceLoggerContext>? OnThreadIsDead = act1;
    public readonly Action<IDeviceLoggerContext>? OnThreadIsLiveTrue = act2;
    public readonly Action<IDeviceLoggerContext>? OnThreadStopedTrue = act3;
    public readonly Action<IDeviceLoggerContext>? OnThreadStopedFalse = act4;
}

public readonly struct CallBackOnStop(Action<IDeviceLoggerContext>? act3 = null,
                                        Action<IDeviceLoggerContext>? act4 = null)
{
    public readonly Action<IDeviceLoggerContext>? OnThreadStopedTrue = act3;
    public readonly Action<IDeviceLoggerContext>? OnThreadStopedFalse = act4;
}