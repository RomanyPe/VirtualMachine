using Kernel.ControllersData;
using Kernel.ProcessorSystem;
using Kernel.RamSystem;
using System.Diagnostics;
using System.Text;

namespace Kernel.BiosSystem;


public class Device : IDisposable
{
    private static readonly StringBuilder _sharedBuilder = new(1024);
    private readonly NameDeviceToken _nameDevice;
    private readonly MemoryBus _ram;
    private readonly Processor _processor;
    private readonly PortBus _portBus;
    private Thread? _simulationThread;
    private readonly Lock _consoleLock = new();
    private long stepCounter = 0;
    public ulong BaseAddress;

    public byte[] RamArray => _ram.Memory;
    public bool IsRunning => _processor.IsRunning;
    public bool HaveBios => _ram.HaveBios;
    public ulong MaxRamSize => _ram.RamSize;
    public long? StepCount => _processor.IsRunning ? null : stepCounter;

    public Device(byte[] biosFirmware, DeviceInitData initData)
    {
        _nameDevice = new(initData.Name);
        _portBus = new PortBus(initData.SizePort, initData.SizeDev, _nameDevice, initData.NamePortBus);
        _ram = new MemoryBus(initData.Size, _nameDevice, initData.NameRam, biosFirmware);
        _processor = ProcessorPoolEmulator.Rent(_ram, _nameDevice, _portBus, _consoleLock, initData.NameProc);
    }

    public Device(byte[] biosFirmware, PortBus portBus, RamSize size, string? name = null!, string? nameProc = null!, string? nameRam = null!)
    {
        _nameDevice = new(name);
        _portBus = portBus;
        _ram = new MemoryBus(size, _nameDevice,  nameRam, biosFirmware);
        _processor = new Processor(_ram, _nameDevice, _portBus, _consoleLock, nameProc);
    }

    public Device CreateDeviceForPort(RamSize size, byte[] biosFirmware, string? name = null!, string? nameProc = null!, string? nameRam = null!)
    {
        var dev = new Device(biosFirmware, _portBus, size, name, nameProc, nameRam);
        return dev;
    }

    public void GetAllData()
    {
        if (_processor.IsRunning) return;
        _processor.GetAllData();
    }

    private void ConsoleLock(string text, NotificationType level = NotificationType.Log)
    {
        DeviceHelpers.LogFromDevice(in _nameDevice, text, level);
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

    public void LaunchDevice(ulong? start = null, bool isDebug = false, int delay = 0, bool snowTimer = false)
    {
        
        ConsoleLock("\n === ЗАПУСК ПОТОКА СИМУЛЯЦИИ ===");

        ulong startIndex = start ?? _ram.RamSize;
        stepCounter = 0;
        // Создаем новый поток и передаем ему метод выполнения
        _simulationThread = new Thread(() => RunSimulationLoop(startIndex, isDebug, delay, snowTimer))
        {
            Name = $"VM_Thread_{_nameDevice}",
            IsBackground = true
        };

        _simulationThread.Start();
    }


    public void BreakPointerLaunchDevice(ulong? start = null)
    {

        ConsoleLock("\n === ЗАПУСК КОМПЬЮТЕРА С ТОЧКАМИ ОСТАНОВА СИМУЛЯЦИИ ===");
        stepCounter = 0;
        ulong startIndex = start ?? _ram.RamSize;

        _processor.LaunchProgramm(startIndex);
    }

    public void StopDevice(int timeoutMilliseconds = 2000)
    {
        // 1. Проверяем, запущен ли поток вообще
        if (_simulationThread == null || !_simulationThread.IsAlive)
        {
            ConsoleLock("Устройство уже остановлено или не запускалось.");
            return;
        }

        ConsoleLock("Инициирована остановка устройства...");

        bool threadTerminatedCleanly = _simulationThread.Join(timeoutMilliseconds);

        if (threadTerminatedCleanly)
        {
            ConsoleLock("Поток симуляции успешно и безопасно завершил работу.");
        }
        else
        {
            ConsoleLock("[Критическая ошибка] Поток ВМ не ответил на запрос остановки вовремя.");
        }

        
        _simulationThread = null;
    }

    public void RunSimulationLoop(ulong start, bool isDebug = false, int delay = 0, bool launchTimer = false)
    {
        Stopwatch? sw = null!;
        if (launchTimer)
        {
            sw = Stopwatch.StartNew();
        }
        
        _processor.LaunchProgramm(start);
        
        while (_processor.IsRunning)
        {
            _processor.Step(isDebug);
            stepCounter++;
            if (isDebug)
            {
                DebugOutput();
            }

            Thread.Sleep(delay);
        }
        if (sw != null)
        {
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
            PrintFullTimeSpanInfo(ts);
            ConsoleLock($"Время симуляции: {elapsedTime}");
        }
        ConsoleLock("\n === ЗАВЕРШЕНИЕ (HALT) ===");
    }

    public void PrintFullTimeSpanInfo(TimeSpan ts)
    {
        lock (_consoleLock)
        {
            _sharedBuilder.Clear(); // Очищаем старые данные, память кучи не выделяется заново!

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

    public RAMResultInt8 Read(ulong offset)
    {
        return _ram.ReadInt8LE(offset);
    }

    public RAMResultInt8 Write(ulong offset, byte value)
    {
        return _ram.WriteInt8LE(offset, value);
    }


    public void NextStepProcessor(bool isDebug = false)
    {
        if (!_processor.IsRunning) return;

        _processor.Step(isDebug);
        stepCounter++;
        if (isDebug) DebugOutput();

    }

    public void NextStepProcessorCount(ulong count = 5,bool isDebug = false)
    {
        if (!_processor.IsRunning) return;
        for (ulong i = 0; i < count; i++) 
        {
            _processor.Step(isDebug);
            stepCounter++;
            if (isDebug) DebugOutput();
        }
    }

    private void DebugOutput()
    {
        ulong currentIp = _processor.GetRegValue(Processor.RegType.rIP);
        ConsoleLock($"\n[Такт {stepCounter}] Выполнен IP: {currentIp} -> Следующий IP: {_processor.GetRegValue(Processor.RegType.rIP)}");
        ConsoleLock($"r0: {_processor.GetRegValue(Processor.RegType.r0)} | r1: {_processor.GetRegValue(Processor.RegType.r1)} | rFL: {_processor.GetRegValue(Processor.RegType.rFL)}");
    }

    public void Dispose()
    {
        _ram?.Dispose();
        ProcessorPoolEmulator.Return(_processor);
        GC.SuppressFinalize(this);
    }
}
