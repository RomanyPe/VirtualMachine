using Kernel.Common;
using Kernel.ControllersData;
using Kernel.ProcessorSystem;
using Kernel.RamSystem;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using ThreadingSystem;
using ThreadingSystem.ThreadControl;

namespace Kernel.BiosSystem;

public sealed class Device : IDisposable, IPortUse
{
    private sealed class DeviceExecutionContext(NameDeviceToken name) : IDeviceLoggerContext
    {
        private readonly NameDeviceToken _nameDevice = name;
        public void Log(string msg, LogLevel lvl = LogLevel.Log)
        {
            LoggerKernel.LogFromDevice(in _nameDevice, msg, lvl);
        }
    }

    private readonly DeviceExecutionContext _ctx;
    private readonly byte[] _ioMemory;
    private static readonly StringBuilder _sharedBuilder = new(1024);
    private readonly ManualResetEventSlim _wakeSignal = new(false);
    private readonly NameDeviceToken _nameDevice;
    private readonly MemoryBus _ram;
    private readonly Processor _processor;
    private readonly PortBus _portBus;
    private Thread? _simulationThread;
    private readonly Lock __ctx = new();
    private long stepCounter = 0;

    public Action<IDeviceLoggerContext>? ActionOnWake { get; set; } = null;

    public DateTime CreatedAt { get; } = DateTime.Now;
    public ReadOnlyMemory<byte> AsRamMemory(int start, int length) => _ram.AsMemory(start, length);
    public ReadOnlyMemory<byte> RamArray => _ram.ReadOnlyMemory;
    public bool IsRunning => _processor.IsRunning;
    public bool HaveBios => _ram.HaveBios;
    public ulong MaxRamSize => _ram.RamSize;
    public long? StepCount => _processor.IsRunning ? null : stepCounter;
    public ulong CurrentIP => _processor.GetRegValue(RegType.rIP);

    public Device(byte[] biosFirmware, PortBus portBus, RamSize size, string? name = null!, string? nameProc = null!, string? nameRam = null!)
    {
        _nameDevice = new(name);
        _portBus = portBus;
        _ram = new MemoryBus(size, _nameDevice, nameRam, biosFirmware);
        _processor = new Processor(_ram, _nameDevice, _portBus, __ctx, nameProc);
        _ioMemory = new byte[_portBus.PortsOnDevice];
        _ctx = new DeviceExecutionContext(_nameDevice);
    }

    public string GetAllData()
    {
        return _processor.IsRunning ? string.Empty : _processor.DumbRegs();
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
        InitHeap(loadAddress, program.AsSpan());
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
        InitHeap(loadAddress, program);
        return true;
    }
    public void LaunchDeviceOnDedicatedThread(ulong? start,
                             bool isDebug,
                             int delay,
                             bool snowTimer,
                             Action<IDeviceLoggerContext>? titleAct,
                             Action<IDeviceLoggerContext>? startAct,
                             Action<IDeviceLoggerContext>? endAct)
    {

        titleAct?.Invoke(_ctx);

        ulong startIndex = start ?? _ram.RamSize;
        stepCounter = 0;

        _simulationThread = new Thread(() =>
        RunSimulationLoop(startIndex, isDebug, delay, snowTimer, startAct, endAct))
        {
            Name = $"VM_Thread_{_nameDevice}",
            IsBackground = true
        };

        _simulationThread.Start();
    }

    public void LaunchDeviceOnMainThread(ulong? start,
                             bool isDebug,
                             int delay,
                             bool snowTimer,
                             Action<IDeviceLoggerContext>? titleAct,
                             Action<IDeviceLoggerContext>? startAct,
                             Action<IDeviceLoggerContext>? endAct)
    {

        titleAct?.Invoke(_ctx);

        ulong startIndex = start ?? _ram.RamSize;
        stepCounter = 0;

        RunSimulationLoop(startIndex, isDebug, delay, snowTimer, startAct, endAct);
    }

    public ThreadHandle? LaunchDeviceAsThreadTask(ThreadScheduler scheduler, ulong? start,
                             bool isDebug,
                             int delay,
                             bool snowTimer,
                             Action<IDeviceLoggerContext>? titleAct,
                             Action<IDeviceLoggerContext>? startAct,
                             Action<IDeviceLoggerContext>? endAct)
    {
        titleAct?.Invoke(_ctx);

        ulong startIndex = start ?? _ram.RamSize;
        stepCounter = 0;

        var task = ThreadScheduler.CreateAsTask(() => RunSimulationLoop(startIndex, isDebug, delay, snowTimer, startAct, endAct));
        int? id = scheduler.FoundFreeWorkerId();

        if (id == null) return null;

        return scheduler.TryAddNewTaskToWorker(id.Value, task, out ThreadHandle handle) ? handle : null;
    }


    public void BreakPointerLaunchDevice(ulong? start, 
                            Action<IDeviceLoggerContext>? titleAct)
    {
        titleAct?.Invoke(_ctx);
        stepCounter = 0;
        ulong startIndex = start ?? _ram.RamSize;

        _processor.LaunchProgramm(startIndex);
    }

    public void StopDevice(int timeoutMilliseconds,
                           Action<IDeviceLoggerContext>? threadLiveTrue,
                           Action<IDeviceLoggerContext>? threadLiveFalse,
                           Action<IDeviceLoggerContext>? threadStopTrue,
                           Action<IDeviceLoggerContext>? threadStopFalse)
    {
        // 1. Проверяем, запущен ли поток вообще
        if (_simulationThread == null || !_simulationThread.IsAlive)
        {
            threadLiveFalse?.Invoke(_ctx);
            return;
        }

        threadLiveTrue?.Invoke(_ctx);

        RequestStop();

        if (_simulationThread.Join(timeoutMilliseconds))
        {
            threadStopTrue?.Invoke(_ctx);
        }
        else
        {
            threadStopFalse?.Invoke(_ctx);
        }

        _simulationThread = null;
    }

    private void RequestStop()
    {
        _processor.EnqueueBiosStatus(BiosStatus.EndProgramm);
        _processor.EnqueueExternalCommand((uint)OpCode.WAKE);
        _wakeSignal.Set();
    }

    public void Stop(ThreadHandle handle,
                     TimeSpan timeout,
                     Action<IDeviceLoggerContext>? onSuccess = null,
                     Action<IDeviceLoggerContext>? onTimeout = null)
    {
        RequestStop();

        if (handle.Wait(timeout))
        {
            onSuccess?.Invoke(_ctx);
            handle.Dispose();
        }
        else
        {
            onTimeout?.Invoke(_ctx);
        }
    }

    private void InitHeap(ulong loadAddress, ReadOnlySpan<byte> program)
    {
        ulong programEnd = loadAddress + (ulong)program.Length;
        ulong heapStart = programEnd.AlignUp(8UL);
        _processor.InitRegHP(heapStart);
    }

    public void RunSimulationLoop(ulong start,
                                  bool isDebug,
                                  int delay,
                                  bool launchTimer,
                                  Action<IDeviceLoggerContext>? startAct,
                                  Action<IDeviceLoggerContext>? endAct)
    {
        startAct?.Invoke(_ctx);
        bool useSleepMode = delay != 0;
        Stopwatch? sw = null;
        if (launchTimer)
        {
            sw = Stopwatch.StartNew();
        }

        _processor.LaunchProgramm(start);

        while (_processor.IsRunning)
        {
            _processor.Step();

            if (_processor.IsSleeping)
            {
                _wakeSignal.Wait(15);
                _wakeSignal.Reset();
            }
            else
            {
                stepCounter++;
            }

            if (isDebug) DebugOutput();
            if (useSleepMode) Thread.Sleep(delay);
        }

        if (sw != null)
        {
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
            PrintFullTimeSpanInfo(ts);
            _ctx.Log($"Время симуляции: {elapsedTime}");
        }

        endAct?.Invoke(_ctx);
    }

    public void PrintFullTimeSpanInfo(TimeSpan ts)
    {
        lock (__ctx)
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

            _ctx.Log(_sharedBuilder.ToString());
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
        _processor.EnqueueExternalCommand((uint)OpCode.WAKE);
        ActionOnWake?.Invoke(_ctx);
        _wakeSignal.Set();
    }

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
        _ctx.Log($"[Такт {stepCounter}] Выполнен IP: {currentIp} -> Следующий IP: {_processor.GetRegValue(RegType.rIP)}");
        _ctx.Log($"r0: {_processor.GetRegValue(RegType.r0)} | r1: {_processor.GetRegValue(RegType.r1)} | rFL: {_processor.GetRegValue(RegType.rFL)}");
    }

    public void UpdateBios(byte[] newBios)
    {
        _ram.SetBios(newBios);
    }

    public void ResetMemoryRam()
    {
        _ram.ClearMemory();
    }

    private int _disposed;

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        // Попросим поток остановиться, если он ещё жив
        var simulationThread = _simulationThread;
        if (simulationThread != null && simulationThread.IsAlive)
        {
            _processor.EnqueueBiosStatus(BiosStatus.EndProgramm);
            _processor.EnqueueExternalCommand((uint)OpCode.WAKE);
            _wakeSignal.Set();
            simulationThread.Join(TimeSpan.FromSeconds(10));
        }

        _ram?.Dispose();
        ProcessorPoolEmulator.Return(_processor);
        GC.SuppressFinalize(this);
    }

    public void ClearRegisters()
    {
        _processor.Reset();
    }
}