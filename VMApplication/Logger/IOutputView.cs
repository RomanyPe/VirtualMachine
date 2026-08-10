namespace VMApplication.Logger;

public interface IOutputView
{
    void Append(string message, LogLevel level = LogLevel.Log);    // LogLevel = Info, Warning, Error
    void Clear();
}