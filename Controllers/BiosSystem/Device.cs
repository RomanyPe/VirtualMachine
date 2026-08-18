using Kernel.Common;
using Kernel.ControllersData;
using Kernel.ProcessorSystem;
using Kernel.RamSystem;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kernel.BiosSystem;

public sealed class Device : IDisposable, IPortUse
{
    private readonly byte[] _ioMemory;
    private static readonly StringBuilder _sharedBuilder = new(1024);
    private readonly ManualResetEventSlim _wakeSignal = new(false);
    private readonly NameDeviceToken _nameDevice;
    private readonly MemoryBus _ram;
    private readonly Processor _processor;
    private readonly PortBus _portBus;
    private Thread? _simulationThread;
    private readonly Lock _consoleLock = new();
    private long stepCounter = 0;

    public DateTime CreatedAt { get; } = DateTime.Now;

    public Span<byte> AsRamSpan(int start, int length) => _ram.AsSpan(start, length);
    public Span<byte> RamSpan => _ram.Span;
    
    public Memory<byte> AsRamMemory() => _ram.Memory;
    public Memory<byte> AsRamMemory(int start, int length) => _ram.AsMemory(start, length);

    public ReadOnlyMemory<byte> RamArray => _ram.ReadOnlyMemory;
    public bool IsRunning => _processor.IsRunning;
    public bool HaveBios => _ram.HaveBios;
    public ulong MaxRamSize => _ram.RamSize;
    public long? StepCount => _processor.IsRunning ? null : stepCounter;

    public Device(byte[] biosFirmware, PortBus portBus, RamSize size, string? name = null!, string? nameProc = null!, string? nameRam = null!)
    {
        _nameDevice = new(name);
        _portBus = portBus;
        _ram = new MemoryBus(size, _nameDevice, nameRam, biosFirmware);
        _processor = new Processor(_ram, _nameDevice, _portBus, _consoleLock, nameProc);
        _ioMemory = new byte[_portBus.PortsOnDevice];
    }

    public string GetAllData()
    {
        return _processor.IsRunning ? string.Empty : _processor.DumbRegs();
    }

    private void ConsoleLock(string text, LogLevel level = LogLevel.Log)
    {
        LoggerKernel.LogFromDevice(in _nameDevice, text, level);
    }

    public void LoadProgram(byte[] program, ulong loadAddress)
    {
        for (ulong i = 0; i < (ulong)program.Length; i++)
        {
            RAMResultInt8 result = _ram.WriteInt8LE(loadAddress + i, program[i]);
            if (!result.IsSuccess)
                throw new InvalidOperationException(
                    $"Не удалось записать программу по адресу 0x{loadAddress + i:X}");
        }
    }

    public bool TryLoadProgramFast(ReadOnlySpan<byte> program, ulong loadAddress)
    {
        Memory<byte> ram = _ram.Memory;
        ulong ramLength = (ulong)ram.Length;
        ulong programLength = (ulong)program.Length;

        if (loadAddress + programLength < loadAddress || loadAddress + programLength > ramLength)
        {
            return false;
        }

        Span<byte> target = ram.Span.Slice((int)loadAddress, program.Length);
        program.CopyTo(target);

        return true;
    }

    public void LaunchDevice(ulong? start,
                             bool isDebug,
                             int delay,
                             bool snowTimer,
                             Action<Action<string, LogLevel>>? titleAct,
                             Action<Action<string, LogLevel>>? startAct,
                             Action<Action<string, LogLevel>>? everyStep,
                             Action<Action<string, LogLevel>>? endAct)
    {

        titleAct?.Invoke(ConsoleLock);

        ulong startIndex = start ?? _ram.RamSize;
        stepCounter = 0;
        // Создаем новый поток и передаем ему метод выполнения
        _simulationThread = new Thread(() =>
        RunSimulationLoop(startIndex, isDebug, delay, snowTimer, startAct, everyStep, endAct))
        {
            Name = $"VM_Thread_{_nameDevice}",
            IsBackground = true
        };

        _simulationThread.Start();
    }

    public void BreakPointerLaunchDevice(ulong? start, 
                            Action<Action<string, LogLevel>>? titleAct)
    {
        titleAct?.Invoke(ConsoleLock);
        stepCounter = 0;
        ulong startIndex = start ?? _ram.RamSize;

        _processor.LaunchProgramm(startIndex);
    }

    public void StopDevice(int timeoutMilliseconds,
                           Action<Action<string, LogLevel>>? threadLiveTrue,
                           Action<Action<string, LogLevel>>? threadLiveFalse,
                           Action<Action<string, LogLevel>>? threadStopTrue,
                           Action<Action<string, LogLevel>>? threadStopFalse)
    {
        // 1. Проверяем, запущен ли поток вообще
        if (_simulationThread == null || !_simulationThread.IsAlive)
        {
            threadLiveFalse?.Invoke(ConsoleLock);
            return;
        }

        threadLiveTrue?.Invoke(ConsoleLock);

        _processor.EnqueueBiosStatus(BiosStatus.EndProgramm);

        if (_simulationThread.Join(timeoutMilliseconds))
        {
            threadStopTrue?.Invoke(ConsoleLock);
        }
        else
        {
            threadStopFalse?.Invoke(ConsoleLock);
        }

        _simulationThread = null;
    }

    public void InitHeap(ulong hp) => _processor.InitReg(hp);

    public void RunSimulationLoop(ulong start,
                                  bool isDebug,
                                  int delay,
                                  bool launchTimer,
                                  Action<Action<string, LogLevel>>? startAct,
                                  Action<Action<string, LogLevel>>? everyStep,
                                  Action<Action<string, LogLevel>>? endAct)
    {
        startAct?.Invoke(ConsoleLock);
        bool useSleepMode = delay != 0;
        Stopwatch? sw = null;
        if (launchTimer)
        {
            sw = Stopwatch.StartNew();
        }

        _processor.LaunchProgramm(start);

        while (_processor.IsRunning)
        {
            if (_processor.IsSleeping)
            {
                _wakeSignal.Wait(10);
                _wakeSignal.Reset();
                continue;
            }
            stepCounter++;
            _processor.Step();

            if (isDebug) DebugOutput();
            if (useSleepMode) Thread.Sleep(delay);

            everyStep?.Invoke(ConsoleLock);

        }

        if (sw != null)
        {
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
            PrintFullTimeSpanInfo(ts);
            ConsoleLock($"Время симуляции: {elapsedTime}");
        }

        endAct?.Invoke(ConsoleLock);
    }

    public void PrintFullTimeSpanInfo(TimeSpan ts)
    {
        lock (_consoleLock)
        {
            _sharedBuilder.Clear();

            // Используем ISpanFormattable под капотом .NET, который пишет числа прямо в буфер
            _sharedBuilder.Append($"=== ПОДРОБНАЯ СТАТИСТИКА ВРЕМЕНИ ===\n" +
                                  $"Дни:          {ts.Days}\n" +
                                  $"Часы:         {ts.Hours}\n" +
                                  $"Минуты:       {ts.Minutes}\n" +
                                  $"Секунды:      {ts.Seconds}\n" +
                                  $"Миллисекунды: {ts.Milliseconds}\n" +
                                  $"Микросекунды: {ts.Microseconds}\n" +
                                  $"Наносекунды:  {ts.Nanoseconds}\n" +
                                  "------------------------------------\n" +
                                  $"Всего дней:          {ts.TotalDays:F6}\n" +
                                  $"Всего часов:         {ts.TotalHours:F4}\n" +
                                  $"Всего минут:         {ts.TotalMinutes:F2}\n" +
                                  $"Всего секунд:        {ts.TotalSeconds:F3}\n" +
                                  $"Всего миллисекунд:   {ts.TotalMilliseconds:F0}\n" +
                                  $"Всего микросекунд:   {ts.TotalMicroseconds:F0}\n" +
                                  $"Всего наносекунд:    {ts.TotalNanoseconds:F0}\n" +
                                  $"Всего тиков (.NET):  {ts.Ticks}\n" +
                                  "------------------------------------\n" +
                                  $"Общее количество тактов: {StepCount}\n" +
                                  "====================================\n");

            ConsoleLock(_sharedBuilder.ToString());
        }
    }

    public byte ReadPort(ulong offset)
    {
        if (offset >= (ulong)_ioMemory.Length)
            return 0; // или ошибка
        return _ioMemory[offset];
    }

    public void WritePort(ulong offset, byte value)
    {
        if (offset < (ulong)_ioMemory.Length)
            _ioMemory[offset] = value;
    }

    public void WakeProcessor()
    {
        _wakeSignal.Set();
    }

    public bool TryDequeueExternalCommand(out uint cmd) => _processor.TryDequeueExternalCommand(out cmd);

    public void EnqueueExternalCommand(uint instruction)
    {
        _processor.EnqueueExternalCommand(instruction);
        _wakeSignal.Set();
    }
    public void NextStepProcessor(bool isDebug = false)
    {
        if (!_processor.IsRunning) return;

        _processor.Step();
        stepCounter++;
        if (isDebug) DebugOutput();

    }

    public void NextStepProcessorCount(int count = 5, bool isDebug = false)
    {
        if (!_processor.IsRunning) return;
        for (int i = 0; i < count; i++)
        {
            _processor.Step();
            stepCounter++;
            if (isDebug) DebugOutput();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void DebugOutput()
    {
        ulong currentIp = _processor.GetRegValue(RegType.rIP);
        ConsoleLock($"[Такт {stepCounter}] Выполнен IP: {currentIp} -> Следующий IP: {_processor.GetRegValue(RegType.rIP)}");
        ConsoleLock($"r0: {_processor.GetRegValue(RegType.r0)} | r1: {_processor.GetRegValue(RegType.r1)} | rFL: {_processor.GetRegValue(RegType.rFL)}");
    }

    public void UpdateBios(byte[] newBios)
    {
        _ram.SetBios(newBios);
    }

    public void ResetMemoryRam()
    {
        _ram.ClearMemory();
    }

    public void Dispose()
    {
        _ram?.Dispose();
        ProcessorPoolEmulator.Return(_processor);
        GC.SuppressFinalize(this);
    }
}

public static class DeviceHelper
{

}