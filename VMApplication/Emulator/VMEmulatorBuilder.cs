using Kernel.Common;
using VMApplication.Logger;

namespace VMApplication.Emulator;

public class VMEmulatorBuilder
{
    private PortSize _portBusSize = PortSize.KB64;
    private DevicePortSize _portsPerDevice = DevicePortSize.B16;

    private VMHostLogger? _logger;

    public VMEmulatorBuilder WithLogger(VMHostLogger logger)
    {
        _logger = logger;
        return this;
    }
    public VMEmulatorBuilder WithPortBusSize(PortSize ports)
    {
        _portBusSize = ports;
        return this;
    }

    public VMEmulatorBuilder WithPortsPerDevice(DevicePortSize portsOnDev)
    {
        _portsPerDevice = portsOnDev;
        return this;
    }

    public VMEmulator Build()
    {
        return _logger == null
            ? throw new InvalidOperationException("Logger обязателен")
            : new VMEmulator(new(_portBusSize, _portsPerDevice), _logger);
    }
}
