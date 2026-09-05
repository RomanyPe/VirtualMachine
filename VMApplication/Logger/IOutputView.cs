using Kernel.Common;

namespace VMApplication.Logger;

public interface IOutputView
{
    void AppendLine(string message, LogLevel level = LogLevel.Log);    // LogLevel = Info, Warning, Error
    void Append(char message);
    void Clear();
}