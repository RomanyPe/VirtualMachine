using Kernel.Common;
using VMApplication.Logger;

namespace VMApplication.Emulator;

public class VMEmulatorBuilder
{
    private SizePort _portBusSize = SizePort.Size16KB;
    private SizePortOnDevice _portsPerDevice = SizePortOnDevice.Size16B;

    private VMHostLogger? _logger;

    public VMEmulatorBuilder WithLogger(VMHostLogger logger)
    {
        _logger = logger;
        return this;
    }
    public VMEmulatorBuilder WithPortBusSize(SizePort size)
    {
        _portBusSize = size;
        return this;
    }

    public VMEmulatorBuilder WithPortsPerDevice(SizePortOnDevice ports)
    {
        _portsPerDevice = ports;
        return this;
    }

    public VMEmulator Build()
    {
        return _logger == null
            ? throw new InvalidOperationException("Logger обязателен")
            : new VMEmulator(new(_portBusSize, _portsPerDevice), _logger, _portsPerDevice);
    }
}
