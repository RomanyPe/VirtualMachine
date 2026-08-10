namespace Kernel.Common;

/// <summary>
/// Абстракция для вывода сообщений (без привязки к конкретному UI).
/// </summary>
public interface ILogger
{
    void Info(string message);
    void Warning(string message);
    void Error(string message);
    void Clear();
}
