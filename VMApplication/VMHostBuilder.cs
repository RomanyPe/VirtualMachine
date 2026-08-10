using VMApplication.Emulator;
using VMApplication.Logger;
using VMApplication.Project;

namespace VMApplication;

public sealed class VMHostBuilder
{
    private VMHostLogger? _logger;
    private VMHostProject? _project; 
    private VMEmulator? _emulator;

    public VMHostBuilder WithLogger(VMHostLogger logger)
    {
        _logger = logger;
        return this;
    }

    public VMHostBuilder WithProject(VMHostProject obj)
    {
        _project = obj;
        return this;
    }

    public VMHostBuilder WithEmulator(VMEmulator obj)
    {
        _emulator = obj;
        return this;
    }

    public VMHost Build()
    {
        return _logger == null || _project == null || _emulator == null
            ? throw new InvalidOperationException("ProjectService, OutputView и Paths обязательны.")
            : new VMHost(_logger, _project, _emulator);
    }
}