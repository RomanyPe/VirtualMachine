namespace VMApplication.Logger;

public class LoggerBuilder
{
    private IOutputView? _outputView;
    private string _nameLogger = "default";
    public LoggerBuilder WithOutPut(IOutputView outputView)
    {
        _outputView = outputView;
        return this;
    }

    public LoggerBuilder WithNameLogger(string name)
    {
        if (!string.IsNullOrWhiteSpace(name)) _nameLogger = name;
        return this;
    }

    public VMHostLogger Build()
    {
        if (_outputView == null)
        {
            throw new InvalidOperationException("OutputView обязателен");
        }

        var logger = new VMHostLogger(_outputView, _nameLogger!);
        VMHostLogger.RegistryLogger(_nameLogger, _outputView);
        logger.ChoiceLogger(_nameLogger);
        return logger;
    }
}