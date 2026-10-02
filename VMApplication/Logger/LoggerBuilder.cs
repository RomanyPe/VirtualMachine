namespace VMApplication.Logger;

public class LoggerBuilder
{
    private IOutputView? _outputView;
    public LoggerBuilder WithOutPut(IOutputView outputView)
    {
        _outputView = outputView;
        return this;
    }

   

    public VMHostLogger Build()
    {
        if (_outputView == null)
        {
            throw new InvalidOperationException("OutputView обязателен");
        }

        var logger = new VMHostLogger(_outputView);
        return logger;
    }
}

