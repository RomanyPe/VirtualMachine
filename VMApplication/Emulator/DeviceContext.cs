using Kernel.BiosSystem;
using Kernel.Common;
using ThreadingSystem.ThreadControl;
using VMApplication.Project;

namespace VMApplication.Emulator;

public class DeviceContext : IDisposable
{
    private readonly Device _device;
    private bool? _isBiosMode = null;
    
    internal DeviceContext(Device device) => _device = device;

    public bool IsRunning => _device.IsRunning;
    public long? StepCount => _device.StepCount;
    public ulong MaxRamSize => _device.MaxRamSize;
    public bool HaveBios => _device.HaveBios;
    public DateTime CreatedAt => _device.CreatedAt;

    [Obsolete(
    """
    Используйте перегрузку 'Stop(ThreadHandle, ...)' для работы через ThreadScheduler.
    
    Внимание: Этот метод предназначен только для потоков, запущенных через 'StartOnDedicatedThread'.
    Смешивание вызовов (например, запуск через Scheduler, а остановка этим методом) 
    приведет к зависанию задачи или утечке ресурсов в пуле воркеров.
    """, error: false, DiagnosticId = "VM0001")]
    public void Stop(int stopTime = 3000, 
        Action<IDeviceLoggerContext>? onThreadIsDead = null,
        Action<IDeviceLoggerContext>? onThreadIsLiveTrue = null,
        Action<IDeviceLoggerContext>? onThreadStopedTrue = null,
        Action<IDeviceLoggerContext>? OnThreadStopedFalse = null)
    {
        _device.StopDevice(stopTime,
                           onThreadIsLiveTrue,
                           onThreadIsDead,
                           onThreadStopedTrue,
                           onThreadIsDead);
    }

    public void Stop(ThreadHandle handle, TimeSpan stopTime, 
        Action<IDeviceLoggerContext>? onThreadIsDead = null,
        Action<IDeviceLoggerContext>? onThreadStopedTrue = null)
    {

        _device.Stop(handle,
                     stopTime,
                     onThreadStopedTrue,
                     onThreadIsDead);
    }
    private void SetLoadMode(bool isBios)
    {
        if (_isBiosMode.HasValue)
            throw new InvalidOperationException($"Режим загрузки уже установлен как {(_isBiosMode.Value ? "BIOS" : "прямая загрузка")}. Изменить его нельзя.");
        _isBiosMode = isBios;
    }

    public LaunchModeDevice LoadProgram(byte[] program, ulong loadAddress = ProjectBuilder.BaseAdressProgram)
    {
        SetLoadMode(false);   // фиксируем прямую загрузку
        _device.LoadProgram(program, loadAddress);
        return new LaunchModeDevice(_device);
    }

    public LaunchModeDevice? TryFastLoadProgram(ReadOnlySpan<byte> program, ulong loadAddress, out string? error)
    {

        if (_isBiosMode.HasValue)
        {
            error = $"Режим загрузки уже установлен как {(_isBiosMode.Value ? "BIOS" : "прямая загрузка")}.";
            return null;
        }
        _isBiosMode = false;   // фиксируем прямую загрузку перед попыткой

        if (_device.TryLoadProgramFast(program, loadAddress))
        {
            error = null;
            return new LaunchModeDevice(_device);
        }

        error = $"Не удалось загрузить программу: выход за границы памяти (loadAddress + {program.Length} > RAM) или другая не инициализированная причина";
        return null;
    }

    public LaunchModeDevice LoadBios(byte[] program)
    {
        SetLoadMode(true);   // фиксируем режим BIOS
        _device.UpdateBios(program);
        return new LaunchModeDevice(_device);
    }

    public LaunchModeDevice GetLaunchMode() => new(_device);

    public ReadOnlyMemory<byte> ReadMemory(ulong address, int length)
    {
        if (_device.IsRunning)
            throw new InvalidOperationException("Нельзя читать память во время симуляции.");
        return _device.AsRamMemory((int)address, length);
    }

    public ReadOnlyMemory<byte> ReadMemory()
    {
        if (_device.IsRunning)
            throw new InvalidOperationException("Нельзя читать память во время симуляции.");
        return _device.RamArray;
    }

    public void Dispose()
    {
        _isBiosMode = null;
        _device.Dispose();
        GC.SuppressFinalize(this);
    }

    public void ResetMemoryRam() => _device.ResetMemoryRam();
    public byte ReadPort(ulong offset) => _device.ReadPort(offset);

    public void ResetRegistors() => _device.ClearRegisters();
}
