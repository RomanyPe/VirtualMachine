using Kernel.BiosSystem;

namespace VMApplication.Emulator;

public class DeviceStepMode
{
    private readonly Device _device;

    internal DeviceStepMode(Device device) => _device = device;
    

    public void Step(bool debug = false) => _device.NextStepProcessor(debug);
    public void MultyStep(bool debug = false, int count = 5) => _device.NextStepProcessorCount(count, debug);

}
