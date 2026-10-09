using Kernel.BiosSystem;
using Kernel.Contracts;
using System.Diagnostics.CodeAnalysis;
using VMApplication.Project;

namespace VMApplication.Emulator;

public class DeviceContext : IDisposable
{
    private readonly Device _device;
    private int _disposed;


    internal Device UnsafeGetDevice => _device;
    internal DeviceContext(Device device, string? name = null)
    {
        _device = device;
        Name = name;
    }

    public string? Name { get; } 
    public bool IsRunning => _device.IsRunning;
    public long StepCount => _device.StepCount;
    public ulong MaxRamSize => _device.MaxRamSize;

    public INativeReadOnlyBuffer NativeReadOnlyBuffer => _device.NativeReadOnlyBuffer;

    public void Stop() => _device.Stop();

    public void LoadProgram(byte[] program, ulong loadAddress = ProjectBuilder.ZeroAdressProgram)
    {
        ThrowIfRunning(IsRunning, "Cannot load program while device is running. Call Stop() first.");
        _device.LoadProgram(program, loadAddress);
    }

    public bool TryFastLoadProgram(ReadOnlySpan<byte> program, ulong loadAddress,[NotNullWhen(false)] out string? error)
    {
        if (IsRunning)
        {
            error = "запрещено перезаписывать программу пока устройство работает";
            return false;
        }

        if (_device.TryLoadProgramFast(program, loadAddress))
        {
            error = null;
            return true;
        }
        error = $"Не удалось загрузить программу: выход за границы памяти (loadAddress + {program.Length} > RAM) или другая не инициализированная причина";
        return false;
    }
    public void Run(LaunchOptions? opt = null)
    {
        ThrowIfRunning(IsRunning);

        opt ??= LaunchOptions.Default;
        _device.RunSimulation(opt.StartAddress,
            opt.DelayMs,
            opt.ShowTimer,
            opt.OnLaunch,
            opt.OnStart,
            opt.OnEnd);
    }

    public void SetStepMode(
        ulong startAddress = ProjectBuilder.ZeroAdressProgram,
        Action<IDeviceLoggerContext>? titleAct = null)
    {
        ThrowIfRunning(IsRunning);
        _device.BreakPointerLaunchDevice(startAddress, titleAct);
    }

    private static void ThrowIfRunning(
        bool isRunning, 
        string msg = "Cannot launch program while device is running. Call Stop() first.")
    {
        if (isRunning)
            throw new InvalidOperationException(msg);

    }
    public void SingleStep() => _device.NextStepProcessor();
    public void MultiStep(int count = 5) => _device.NextStepProcessorCount(count);

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _device.Dispose();
        GC.SuppressFinalize(this);

    }
    public void ResetMemoryRam() => _device.ResetMemoryRam();
    public byte ReadPort(ulong offset) => _device.ReadPort(offset);
    public void ResetRegisters() => _device.ClearRegisters();
    public void CopyRegisters(Span<ulong> destination) => _device.CopyRegisters(destination);
    public ulong[]? GetRegistersSnapshot() => _device.GetRegistersSnapshot();
}
