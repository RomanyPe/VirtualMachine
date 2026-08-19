using Kernel.Common;

namespace VMApplication.CallBacks;

public readonly struct CallBackOnLaunch(Action<Action<string, LogLevel>>? onLaunch = null,
                                        Action<Action<string, LogLevel>>? onStart = null,
                                        Action<Action<string, LogLevel>>? onEnd = null)
{
    public readonly Action<Action<string, LogLevel>>? OnTitleLaunch = onLaunch;
    public readonly Action<Action<string, LogLevel>>? OnStart = onStart;
    public readonly Action<Action<string, LogLevel>>? OnEnd = onEnd;
}

public readonly struct CallBackOnStop(Action<Action<string, LogLevel>>? act1 = null,
                                        Action<Action<string, LogLevel>>? act2 = null,
                                        Action<Action<string, LogLevel>>? act3 = null,
                                        Action<Action<string, LogLevel>>? act4 = null)
{
    public readonly Action<Action<string, LogLevel>>? OnThreadIsLiveFalse = act1;
    public readonly Action<Action<string, LogLevel>>? OnThreadIsLiveTrue = act2;
    public readonly Action<Action<string, LogLevel>>? OnThreadStopedTrue = act3;
    public readonly Action<Action<string, LogLevel>>? OnThreadStopedFalse = act4;
}