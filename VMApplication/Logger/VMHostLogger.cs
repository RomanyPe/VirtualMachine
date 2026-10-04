using Kernel.Common;
using Kernel.Contracts;

namespace VMApplication.Logger;

public sealed class VMHostLogger(IOutputView outputView)
{
    private readonly IOutputView _outputView = outputView;

    public IDeviceLoggerContext CreateDefaultLoggerContext(NameDeviceToken nameDeviceToken)
        => new DeviceLoggerContext(nameDeviceToken, _outputView);

    public void AppendLine(string str, LogLevel log) => _outputView.AppendLine(str, log);

    private class DeviceLoggerContext(NameDeviceToken nameDeviceToken, IOutputView outputView) : IDeviceLoggerContext
    {

        public void Log(string message, LogLevel logLevel = LogLevel.Log)
        {
            string full = string.Concat("[", nameDeviceToken.Name, "] ", message);
            outputView.AppendLine(full, logLevel);
        }
    }
}
