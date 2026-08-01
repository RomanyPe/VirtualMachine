using Compiller.C;
using Kernel.BiosSystem;
using Kernel.ControllersData;
using Kernel.RamSystem;
using Kernel.Utilites;

namespace Compiller.Emulation;

/// <summary>
/// Эмулятор устройства: управляет загрузкой программ, запуском, пошаговым выполнением.
/// Теперь поддерживает несколько устройств через ManagerDevices.
/// </summary>
public class Emulator
{
    private readonly PortBus _portBus;
    private readonly ManagerDevices _manager;

    private int _mainDeviceId = -1;

    private readonly List<ProgramData> _programs = [];

    // --- Конструкторы ---

    /// <summary>
    /// Создаёт эмулятор с общей шиной портов заданного размера.
    /// </summary>
    /// <param name="totalPorts">Общий размер пространства портов.</param>
    /// <param name="portsPerDevice">Количество портов, выделяемое на одно устройство.</param>
    public Emulator(SizePort totalPorts = SizePort.Size16KB, SizePortOnDev portsPerDevice = SizePortOnDev.Size16B)
    {
        _portBus = new PortBus(totalPorts, portsPerDevice, new NameDeviceToken("System"), "PortBus");
        _manager = new ManagerDevices(_portBus);
    }

    // --- Создание устройств ---

    /// <summary>
    /// Создаёт новое устройство и регистрирует его на указанном секторе шины портов.
    /// </summary>
    /// <param name="biosFirmware">BIOS-прошивка (может быть пустой).</param>
    /// <param name="size">Объём RAM устройства.</param>
    /// <param name="sector">Номер сектора на шине портов (0 .. (totalPorts/portsPerDevice - 1)).</param>
    /// <param name="name">Имя устройства (для логов).</param>
    /// <param name="nameProc">Имя процессора (для логов).</param>
    /// <param name="nameRam">Имя RAM (для логов).</param>
    /// <param name="namePortBus">Имя шины портов (для логов).</param>
    /// <returns>ID созданного устройства или -1 в случае ошибки.</returns>
    public int CreateDevice(
        byte[] biosFirmware,
        RamSize size,
        uint sector,
        string? name = null,
        string? nameProc = null,
        string? nameRam = null,
        string? namePortBus = null)
    {
        int id = _manager.CreateNewDevice(
            size,
            SizePort.Size16KB,      // Можно сделать параметрами, если нужно
            SizePortOnDev.Size16B,
            name,
            nameProc,
            nameRam,
            namePortBus,
            sector,
            biosFirmware
        );

        if (id != -1 && _mainDeviceId == -1)
            _mainDeviceId = id; // первое созданное устройство становится основным

        return id;
    }

    /// <summary>
    /// Возвращает объект устройства по его ID.
    /// </summary>
    public Device? GetDevice(int id) => _manager.GetDevice(id);

    /// <summary>
    /// Возвращает основное устройство (первое созданное).
    /// </summary>
    public Device? MainDevice => _manager.GetDevice(_mainDeviceId);

    // --- Загрузка программ (теперь на основное устройство) ---

    private void AddProgram(Device device, byte[] program, ulong startAddress)
    {
        _programs.Add(new ProgramData(startAddress, program.Length));
        device.LoadProgram(program, startAddress);
    }

    /// <summary>Загружает скомпилированную программу в основное устройство.</summary>
    public ResultComputer LoadProgram(byte[] program, ulong startAddress = 0x0000)
    {
        var device = MainDevice;
        if (device == null)
            return new ResultComputer(ResultOperation.InvalidData);
        if ((ulong)program.Length > device.MaxRamSize)
            return new ResultComputer(ResultOperation.OutOfMemory);

        AddProgram(device, program, startAddress);
        return new ResultComputer(device);
    }

    // --- Компиляция, загрузка и запуск (основное устройство) ---

    /// <summary>Компилирует, загружает и запускает программу на основном устройстве.</summary>
    public ResultComputer CompileLoadAndRun(
        string source,
        ulong startAddress = 0x0000,
        bool launchWithBios = false,
        bool disassemble = false,
        bool isDebug = false,
        int delayThread = 0,
        bool snowTimer = false)
    {
        // Создаём основное устройство, если его ещё нет
        if (_mainDeviceId == -1)
        {
            _mainDeviceId = CreateDevice([], RamSize.Size1MB, 0);
        }

        var device = MainDevice;
        if (device == null)
            return new ResultComputer(ResultOperation.InvalidData);

        byte[] program = MiniCCompiler.Compile(source);
        if (disassemble)
            MiniCCompiler.DisassembleCode(program);

        var loadResult = LoadProgram(program, startAddress);
        if (loadResult.Result != ResultOperation.Success) return loadResult;

        return Launch(launchWithBios, startAddress, isDebug, delayThread, snowTimer);
    }

    /// <summary>Компиляция и загрузка программы в режиме Debug.</summary>
    public ResultComputer CompileLoadAndDebug(string source, ulong startAddress = 0x0000)
    {
        if (_mainDeviceId == -1)
            _mainDeviceId = CreateDevice([], RamSize.Size1MB, 0);

        var device = MainDevice;
        if (device == null)
            return new ResultComputer(ResultOperation.InvalidData);

        byte[] program = MiniCCompiler.Compile(source);
        MiniCCompiler.DisassembleCode(program);

        var loadResult = LoadProgram(program, startAddress);
        if (loadResult.Result != ResultOperation.Success) return loadResult;

        device.BreakPointerLaunchDevice(startAddress);
        device.GetAllData();
        return new ResultComputer(device);
    }

    // --- Запуск ---

    private bool ContainsProgramAt(ulong address) =>
        _programs.Any(p => p.Adress == address);

    /// <summary>Запускает основное устройство.</summary>
    public ResultComputer Launch(
        bool launchWithBios,
        ulong startAddress = 0x0000,
        bool isDebug = false,
        int delay = 0,
        bool snowTimer = false)
    {
        var device = MainDevice;
        if (device == null)
            return new ResultComputer(ResultOperation.InvalidData);

        if (launchWithBios)
        {
            if (!device.HaveBios)
                return new ResultComputer(ResultOperation.BiosNotFound);
            device.LaunchDevice(isDebug: isDebug, delay: delay, snowTimer: snowTimer);
            return new ResultComputer(device);
        }

        if (ContainsProgramAt(startAddress))
        {
            device.LaunchDevice(startAddress, isDebug, delay, snowTimer);
            return new ResultComputer(device);
        }

        return new ResultComputer(ResultOperation.ProgramNotFound);
    }

    // --- Пошаговое выполнение (основное устройство) ---

    public void Step(bool debugMode) => MainDevice?.NextStepProcessor(debugMode);
    public void Step(ulong count, bool debugMode) => MainDevice?.NextStepProcessorCount(count, debugMode);

    // --- Отладка ---

    public void Dump() => MainDevice?.GetAllData();

    // --- Сброс ---

    public void Reset()
    {
        _manager.ClearAllDevices(); // нужно добавить этот метод в ManagerDevices
        _mainDeviceId = -1;
        _programs.Clear();
    }

    // --- Для обратной совместимости: свойство CurrentDevice ---

    public Device? CurrentDevice => MainDevice;
}