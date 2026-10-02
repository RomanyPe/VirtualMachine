using Kernel.Common;
using Kernel.ControllersData;
using Kernel.ProcessorSystem;
using Kernel.RamSystem;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Kernel.BiosSystem;

public sealed class Device : IPortController
{
    
    private sealed class SimulationResult(TimeSpan elapsed, long steps) : ISimulationResult
    {
        public TimeSpan Elapsed => elapsed;
        public long Steps => steps;
    }

    private readonly IDeviceLoggerContext _deviceCtx;
    private readonly byte[] _ioMemory;
    private readonly ManualResetEventSlim _wakeSignal = new(false);
    private readonly MemoryBus _ram;
    private readonly Processor _processor;
    private readonly PortBus _portBus;
    private long stepCounter = 0;
    private int _disposed;
    public Action<IDeviceLoggerContext>? ActionOnWake { get; set; } = null;

    public DateTime CreatedAt { get; } = DateTime.Now;
    public ReadOnlyMemory<byte> AsRamMemory(int start, int length) => _ram.AsMemory(start, length);
    public ReadOnlyMemory<byte> RamArray => _ram.ReadOnlyMemory;

    private bool _running = false;

    public bool IsRunning => _running;
    public bool HaveBios => _ram.HaveBios;
    public ulong MaxRamSize => _ram.RamSize;
    public long? StepCount => _processor.IsRunning ? null : stepCounter;

    public Device(byte[] biosFirmware, PortBus portBus, RamSize size, RamSize sizeBios, IDeviceLoggerContext deviceCtx, IProcessorFaultPolicy? processorFaultPolicy = null)
    {
        _portBus = portBus;
        _ram = new MemoryBus(size, biosFirmware, sizeBios);
        _processor = new Processor(_ram, _portBus, processorFaultPolicy);
        _ioMemory = new byte[_portBus.PortsOnDevice];
        _deviceCtx = deviceCtx;
    }

    public void CopyRegisters(Span<ulong> destination)
    {
        if (!_processor.IsRunning)
        {
            _processor.CopyRegisters(destination);
        }
    }
    public ulong[]? GetRegistersSnapshot()
    {
        return _processor.IsRunning ? null : _processor.GetRegistersSnapshot();
    }

    public void LoadProgram(byte[] program, ulong loadAddress)
    {
        for (ulong i = 0; i < (ulong)program.Length; i++)
        {
            RAMResultInt8 result = _ram.WriteInt8LE(loadAddress + i, program[i]);
            if (!result.IsSuccess)
                throw new InvalidOperationException(
                    $"Failed to write the program to the address 0x{loadAddress + i:X}");
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

    public void RunSimulation(ulong? start,
                             bool isDebug,
                             int delay,
                             bool snowTimer,
                             Action<IDeviceLoggerContext>? onLaunch,
                             Action<IDeviceLoggerContext>? onStart,
                             Action<IDeviceLoggerContext, ISimulationResult?>? onEnd)
    {

        onLaunch?.Invoke(_deviceCtx);

        ulong startIndex = start ?? _ram.RamSize;
        stepCounter = 0;

        RunSimulationLoop(startIndex, isDebug, delay, snowTimer, onStart, onEnd);
    }


    public void BreakPointerLaunchDevice(ulong? start, 
                            Action<IDeviceLoggerContext>? titleAct)
    {
        titleAct?.Invoke(_deviceCtx);
        stepCounter = 0;
        ulong startIndex = start ?? _ram.RamSize;

        _processor.LaunchProgramm(startIndex);
    }

    private void RequestStop()
    {
        _processor.EnqueueBiosStatus(BiosStatus.EndProgramm);
        _processor.EnqueueExternalCommand((uint)OpCode.WAKE);
        _wakeSignal.Set();
    }

    public void Stop() => RequestStop();

    private void InitHeap(ulong loadAddress, ReadOnlySpan<byte> program)
    {
        ulong programEnd = loadAddress + (ulong)program.Length;
        ulong heapStart = programEnd.AlignUp(8UL);
        _processor.InitRegHP(heapStart);
    }

    private void RunSimulationLoop(ulong start,
                               bool isDebug,
                               int delay,
                               bool launchTimer,
                               Action<IDeviceLoggerContext>? startAct,
                               Action<IDeviceLoggerContext, ISimulationResult?>? endAct)
    {
        bool useSleepMode = delay != 0;

        startAct?.Invoke(_deviceCtx);
        Stopwatch? sw = launchTimer ? Stopwatch.StartNew() : null;

        _processor.LaunchProgramm(start);
        Volatile.Write(ref _running, _processor.IsRunning);

        // Выбор пути один раз
        if (isDebug || useSleepMode)
            RunSimulationLoopSlow(isDebug, delay, useSleepMode);
        else
            RunSimulationLoopFast();

        Volatile.Write(ref _running, _processor.IsRunning);

        SimulationResult? timeSpan = null;
        if (sw != null)
        {
            sw.Stop();
            timeSpan = new(sw.Elapsed, stepCounter);
        }

        endAct?.Invoke(_deviceCtx, timeSpan);
    }

    private void RunSimulationLoopFast()
    {
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
        }
    }

    private void RunSimulationLoopSlow(bool isDebug, int delay, bool useSleepMode)
    {
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
        ActionOnWake?.Invoke(_deviceCtx);
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
        _deviceCtx.Log($"[Такт {stepCounter}] Выполнен IP: {currentIp} -> Следующий IP: {_processor.GetRegValue(RegType.rIP)}");
        _deviceCtx.Log($"r0: {_processor.GetRegValue(RegType.r0)} | r1: {_processor.GetRegValue(RegType.r1)} | rFL: {_processor.GetRegValue(RegType.rFL)}");
    }

    public void UpdateBios(byte[] newBios)
    {
        _ram.SetBios(newBios);
    }

    public void ResetMemoryRam() => _ram.ClearMemory();

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        Stop();
        _ram?.Dispose();
        GC.SuppressFinalize(this);
    }

    public void ClearRegisters()
    {
        _processor.Reset();
    }
}