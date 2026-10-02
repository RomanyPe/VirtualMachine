using Kernel.Common;
using System.Runtime.CompilerServices;

namespace VMApplication.Logger;

public sealed class VMHostLogger(IOutputView outputView) : IOutputView
{
    private readonly IOutputView _outputView = outputView;

    public IDeviceLoggerContext CreateDefaultLoggerContext(NameDeviceToken nameDeviceToken)
        => new DeviceLoggerContext(nameDeviceToken, _outputView);

    public void AppendLine(string str, LogLevel log) => _outputView.AppendLine(str, log);

    public void Clear() => _outputView.Clear();

    private class DeviceLoggerContext(NameDeviceToken nameDeviceToken, IOutputView outputView) : IDeviceLoggerContext
    {

        public void Log(string message, LogLevel logLevel = LogLevel.Log)
        {
            LogFromDevice(nameDeviceToken, message, logLevel);
        }


        private static string FormatLogString(ReadOnlySpan<char> prefix, string text)
        {
            int exactLength = 1 + prefix.Length + 2 + text.Length;

            return string.Create(exactLength, (prefix: prefix.ToString(), text), static (span, state) =>
            {
                span[0] = '[';
                ReadOnlySpan<char> pSpan = state.prefix.AsSpan();
                pSpan.CopyTo(span[1..]);

                int index = 1 + pSpan.Length;
                span[index] = ']';
                span[index + 1] = ' ';

                state.text.AsSpan().CopyTo(span[(index + 2)..]);
            });
        }

        private void LogFromDevice(in NameDeviceToken nameDevice, string text, LogLevel level)
        {
            ReadOnlySpan<char> nameSpan = nameDevice.Name.AsSpan();
            ExecuteLog(nameSpan, text, level, outputView);
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExecuteLog(ReadOnlySpan<char> prefix, string text, LogLevel level, IOutputView logger)
        {
            string fullMessage = FormatLogString(prefix, text);
            logger.AppendLine(fullMessage, level);
        }
    }
}
