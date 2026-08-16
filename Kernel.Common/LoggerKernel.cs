using System.Runtime.CompilerServices;

namespace Kernel.Common;

public static class LoggerKernel
{

    private static string FormatLogString(ReadOnlySpan<char> prefix, string text)
    {
        int exactLength = 1 + prefix.Length + 2 + text.Length;

        return string.Create(exactLength, (prefix: prefix.ToString(), text), (span, state) =>
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

    public static void LogFromDevice(in NameDeviceToken nameDevice, string text, LogLevel level = LogLevel.Log)
    {
        ReadOnlySpan<char> nameSpan = nameDevice.Name.AsSpan();
        ExecuteLog(nameSpan, text, level);
    }

    public static void LogFromSystem(string nameSystem, string text, LogLevel level = LogLevel.Log)
    {
        ReadOnlySpan<char> nameSpan = nameSystem.AsSpan();
        ExecuteLog(nameSpan, text, level);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ExecuteLog(ReadOnlySpan<char> prefix, string text, LogLevel level)
    {
        string fullMessage = FormatLogString(prefix, text);

        switch (level)
        {
            case LogLevel.Warning: LoggerProvider.Warning(fullMessage); break;
            case LogLevel.Error: LoggerProvider.Error(fullMessage); break;
            default: LoggerProvider.Info(fullMessage); break;
        }
    }

    public static void ClearLog() => LoggerProvider.Clear();
}
