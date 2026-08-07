using Kernel.ProcessorSystem;

namespace VMApplication;

public interface IOutputView
{
    bool UseConsole { get; set; }
    void Append(string message, LogLevel level = LogLevel.Log);    // LogLevel = Info, Warning, Error
    void Clear();
}

