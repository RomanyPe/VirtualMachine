using Kernel.Common;

namespace Kernel.Contracts;

public interface IDeviceLoggerContext
{
    void Log(string message, LogLevel logLevel = LogLevel.Log);

}
