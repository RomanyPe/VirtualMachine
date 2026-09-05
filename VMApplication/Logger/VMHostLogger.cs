using Kernel.Common;

namespace VMApplication.Logger;

public sealed class VMHostLogger
{
    private readonly IOutputView _outputView;
    private readonly string _defaultName;

    public string LoggerName { get; private set; }

    public VMHostLogger(IOutputView outputView, string defaultName)
    {
        _outputView = outputView ?? throw new ArgumentNullException(nameof(outputView));
        _defaultName = string.IsNullOrWhiteSpace(defaultName) ? "default" : defaultName;
        LoggerName = _defaultName;
    }

    public void TestCurrentLoggerSystem()
    {
        _outputView?.AppendLine("Log [Debug log]", LogLevel.Log);
        _outputView?.AppendLine("Warning [Debug log]", LogLevel.Warning);
        _outputView?.AppendLine("Error [Debug log]", LogLevel.Error);
    }

    public void RegistryLogger(string name, IOutputView outPutView)
    {
        LoggerProvider.RegistryLogger(name, new ActionLogger(_outputView.AppendLine, outPutView.Clear, _outputView.Append));
    }

    public void ChoiceLogger(string? name = null)
    {
        LoggerName = name!;
        LoggerProvider.ChoiceLogger(LoggerName);
    }

    public void DeleteLogger(string name)
    {
        if (name == _defaultName)
            _outputView.AppendLine("Удален первоначальный логгер", LogLevel.Warning);

        LoggerProvider.DeleteLogger(name);
    }

    public void AppendLine(string str, LogLevel log) => _outputView.AppendLine(str, log);
    public void Clear() => _outputView.Clear();

    private class ActionLogger(Action<string, LogLevel> logAction, Action clearAction, Action<char> writeAction) : ILogger
    {
        private readonly Action<string, LogLevel> _logAction = logAction;
        private readonly Action _clearAction = clearAction;
        private readonly Action<char> _writeAction = writeAction;

        public void CharOutPut(char c) => _writeAction(c);
        public void Info(string message) => _logAction(message, LogLevel.Log);
        public void Warning(string message) => _logAction(message, LogLevel.Warning);
        public void Error(string message) => _logAction(message, LogLevel.Error);
        public void Clear() => _clearAction();
    }
}
