namespace VMApplication.CallBacks;

public readonly struct CallBackOnLaunch(Action launch = null!, Action step = null!, Action end = null!)
{
    public readonly Action OnLaunch = launch;
    public readonly Action OnStep = step;
    public readonly Action OnEnd = end;
}
