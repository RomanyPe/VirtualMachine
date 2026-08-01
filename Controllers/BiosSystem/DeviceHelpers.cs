using Kernel.ProcessorSystem;
using System.Runtime.CompilerServices;

namespace Kernel.BiosSystem;

public static class DeviceHelpers
{
    // Оптимизированный общий метод для вывода в консоль БЕЗ создания строки интерполяции
    private static void WriteToConsoleWithColor(ReadOnlySpan<char> prefix, string text, ConsoleColor? color = null)
    {
        if (color.HasValue) Console.ForegroundColor = color.Value;

        // Пишем в консоль по частям, используя Span — ноль аллокаций!
        Console.Write('[');
        Console.Write(prefix);
        Console.Write("] ");
        Console.WriteLine(text);

        if (color.HasValue) Console.ResetColor();
    }

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

    public static void LogFromDevice(in NameDeviceToken nameDevice, string text, NotificationType level = NotificationType.Log)
    {
        ReadOnlySpan<char> nameSpan = nameDevice.Name.AsSpan();
        ExecuteLog(nameSpan, text, level);
    }

    public static void LogFromSystem(string nameSystem, string text, NotificationType level = NotificationType.Log)
    {
        ReadOnlySpan<char> nameSpan = nameSystem.AsSpan();
        ExecuteLog(nameSpan, text, level);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ExecuteLog(ReadOnlySpan<char> prefix, string text, NotificationType level)
    {
        if (!LoggerProvider.UseConsole)
        {
            string fullMessage = FormatLogString(prefix, text);

            switch (level)
            {
                case NotificationType.Warning: LoggerProvider.Warning(fullMessage); break;
                case NotificationType.Error: LoggerProvider.Error(fullMessage); break;
                default: LoggerProvider.Info(fullMessage); break;
            }
        }
        else
        {
            switch (level)
            {
                case NotificationType.Warning:
                    WriteToConsoleWithColor(prefix, text, ConsoleColor.Yellow);
                    break;
                case NotificationType.Error:
                    WriteToConsoleWithColor(prefix, text, ConsoleColor.Red);
                    break;
                default:
                    WriteToConsoleWithColor(prefix, text);
                    break;
            }
        }
    }

    public static void ClearLog()
    {
        if (LoggerProvider.UseConsole) Console.Clear();
        else LoggerProvider.Clear();
    }
}

/// <summary>
/// Абстракция для вывода сообщений (без привязки к конкретному UI).
/// </summary>
public interface ILogger
{
    bool UseConsole { get; }
    void Info(string message);
    void Warning(string message);
    void Error(string message);
    void Clear();
}

/// <summary>
/// Статический провайдер, через который библиотеки отправляют сообщения.
/// </summary>
public static class LoggerProvider
{
    private static ILogger? _current;

    public static ILogger? Current => _current;

    /// <summary>Установить реализацию логгера (вызывается один раз при старте UI).</summary>
    public static void SetLogger(ILogger logger)
    {
        _current = logger;
    }

    public static bool UseConsole => _current != null && _current.UseConsole;
    public static void Info(string message) => _current?.Info(message);
    public static void Warning(string message) => _current?.Warning(message);
    public static void Error(string message) => _current?.Error(message);
    public static void Clear() => _current?.Clear();
}