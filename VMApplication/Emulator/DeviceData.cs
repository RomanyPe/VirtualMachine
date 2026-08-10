using Kernel.BiosSystem;
using VMApplication.Project;

namespace VMApplication.Emulator;

public class DeviceData : IDisposable
{
    private readonly Device _device;

    internal DeviceData(Device device) => _device = device;

    public bool IsRunning => _device.IsRunning;
    public long? StepCount => _device.StepCount;
    public ulong MaxRamSize => _device.MaxRamSize;
    public bool HaveBios => _device.HaveBios;
    public DateTime CreatedAt => _device.CreatedAt;

    public void Stop() => _device.StopDevice();

    public LaunchModeDevice LoadProgram(byte[] program, ulong loadAddress = ProjectBuilder.BaseAdressProgramm)
    {
        _device.LoadProgram(program, loadAddress);
        return new LaunchModeDevice(_device);
    }

    public LaunchModeDevice? TryFastLoadProgram(ReadOnlySpan<byte> program, ulong loadAddress, out string? error)
    {
        if (_device.TryLoadProgramFast(program, loadAddress))
        {
            error = null;
            return new LaunchModeDevice(_device);
        }
        error = $"Не удалось загрузить программу: выход за границы памяти (loadAddress + {program.Length} > RAM) или другая не инициализированная причина";
        return null;
    }

    public ReadOnlyMemory<byte> ReadMemory(ulong address, int length)
    {
        if (_device.IsRunning)
            throw new InvalidOperationException("Нельзя читать память во время симуляции.");

        return new ReadOnlyMemory<byte>(_device.RamArray, (int)address, length);
    }

    public ReadOnlyMemory<byte> ReadMemory()
    {
        if (_device.IsRunning)
            throw new InvalidOperationException("Нельзя читать память во время симуляции.");
        var ram = _device.RamArray;
        return new ReadOnlyMemory<byte>(ram, 0, ram.Length);
    }

    public void Dispose()
    {
        _device.Dispose();
        GC.SuppressFinalize(this);
    }

    public byte ReadPort(ulong offset) => _device.ReadPort(offset);

}
