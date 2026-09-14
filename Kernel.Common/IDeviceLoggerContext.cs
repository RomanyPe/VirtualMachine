namespace Kernel.Common;

public interface IDeviceLoggerContext
{
    void Log(string message, LogLevel logLevel);
}