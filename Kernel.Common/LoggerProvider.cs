namespace Kernel.Common;

/// <summary>
/// Статический провайдер, через который библиотеки отправляют сообщения.
/// </summary>
public static class LoggerProvider
{
    private static readonly Dictionary<string, ILogger> _loggers = [];
    private static readonly AsyncLocal<string?> _currentLoggerName = new();
    public static ILogger? Current => CurrentLogger;

    public static void RegistryLogger(string name, ILogger logger)
    {
        lock (_loggers)
            _loggers[name] = logger;
    }

    public static void DeleteLogger(string name)
    {
        lock (_loggers)
            _loggers.Remove(name);
    }

    /// <summary>Задать активный логгер для текущего потока/контекста.</summary>
    public static void ChoiceLogger(string name)
    {
        _currentLoggerName.Value = name;
    }

    private static ILogger? CurrentLogger
    {
        get
        {
            var name = _currentLoggerName.Value;
            if (name == null) return null;
            lock (_loggers)
                return _loggers.TryGetValue(name, out var logger) ? logger : null;
        }
    }

    public static bool Has(string name)
    {
        lock (_loggers)
            return _loggers.ContainsKey(name);
    }
    public static IReadOnlyCollection<string> GetAllRegisteredLoggers()
    {
        lock (_loggers)
            return [.. _loggers.Keys];
    }

    public static void Info(char c) => CurrentLogger?.Info(c.AsText);
    public static void Info(string message) => CurrentLogger?.Info(message);
    public static void Warning(string message) => CurrentLogger?.Warning(message);
    public static void Error(string message) => CurrentLogger?.Error(message);
    public static void Clear() => CurrentLogger?.Clear();
}
