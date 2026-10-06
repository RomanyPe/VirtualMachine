using Kernel.Common;
using VMApplication.Logger;

namespace VMApplication.Default;

public sealed class ConsoleOutputView : IOutputView
{
    public void AppendLine(string message, LogLevel level)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = level switch
        {
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => originalColor
        };
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public void Clear() => Console.Clear();
}
