namespace Kernel.Common;

public interface IDeviceLoggerContext
{
    void Log(string message, LogLevel logLevel = LogLevel.Log);

}

public interface ISimulationResult
{
    TimeSpan Elapsed { get; }
    long Steps { get; }
}