namespace Kernel.Common;

public interface IDeviceLoggerContext
{
    void Log(string message, LogLevel logLevel);
}

public interface ISimulationResult
{
    TimeSpan Elapsed { get; }
    long Steps { get; }
}