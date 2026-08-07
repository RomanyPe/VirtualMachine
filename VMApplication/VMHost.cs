using Compiller.ASM;
using Compiller.C;
using Compiller.Emulation;
using Kernel.BiosSystem;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static Kernel.Utilites.ManagerDevices;

namespace VMApplication;

public sealed class VMHost
{
    public const uint StartLoadProgramm = 0x200;

    private readonly IProjectService _projectService;
    private string? _projectPath;
    private readonly Emulator _emulator;
    private readonly IOutputView _outputView;
    private readonly IProjectPaths _projectPaths;
    public event Action<IEnumerable<DeviceInfo>>? DeviceListChanged;
    // --------------- Конструктор ---------------
    public VMHost(IProjectService projectService, IOutputView outputView, IProjectPaths paths,
                    SizePort sizePort = SizePort.Size16KB,SizePortOnDev sizePortOn = SizePortOnDev.Size16B)
    {
        _projectService = projectService;
        _emulator = new Emulator(sizePort.ConvertToKernelEnum(), sizePortOn.ConvertToApiEnum());
        _outputView = outputView;
        _projectPath = paths.ProjectPath;
        _projectPaths = paths;

        InitializeLogger(outputView);
    }

    public bool IsProjectOpened => _projectPath != null;
    public string IncludePath => _projectPaths.IncludePath;

    private void InitializeLogger(IOutputView outPutView)
    {
        LoggerProvider.SetLogger(new ActionLogger(SetLogger, outPutView.Clear));
    }

    private void SetLogger(string str, LogLevel log) => _outputView.Append(str, log);


    // --------------- Управление проектом ---------------
    public void OpenProject(string projectPath)
    {
        _projectPath = projectPath;
        _projectService!.OpenProject();
        _outputView.Append($"Проект {projectPath} открыт.", LogLevel.Log);
    }

    public CompilationResult Compile(ulong baseAddress = StartLoadProgramm)
    {
        if (!IsProjectOpened || _projectPath == null)
            return new CompilationResult(null, 0, ["Проект не открыт."]);

        try
        {
            byte[] program = ProjectBuilder.BuildProject(
                _projectService.FileService,
                _projectService.EditorService, _projectPaths,baseAddress);
            return new CompilationResult(program,baseAddress, null);
        }
        catch (Exception ex)
        {
            _outputView.Append(ex.Message, LogLevel.Error);
            return new CompilationResult(null, 0, [ex.Message]);
        }
    }

    public DeviceData? CreateDeviceContext(int? id = null)
    {
        var device = id != null ? _emulator.GetDevice(id.Value) : _emulator.MainDevice;
        if (device == null)
        {
            _outputView.Append("Выбраное устройство/основное не инициализированы", LogLevel.Error);
            return null;
        }
        return new DeviceData(device);
    }

    public DeviceData? GetDeviceData(int id)
    {
        var d = _emulator.GetDevice(id);
        return d != null ? new(d) : null;
    }

    public IEnumerable<DeviceView> GetAllDevices()
    {
        return _emulator.AllDevices.Select(VMHostHelper.ConvertDeviceInfo);
    }

    public string GetDumbRegisters()
    {
        var res = _emulator.DumpRegisters();
        if (string.IsNullOrEmpty(res))
        {
            return "Не получилось получить дамб регистров";
        }
        return res;
    }

    public string GetDumbRegisters(int id)
    {
        var res = _emulator.DumpRegisters(id);
        if (string.IsNullOrEmpty(res))
        {
            return $"Не получилось получить дамб регистров для устройства {id}";
        }
        return res;
    }

    public int CreateDevice(byte[] bios, RamSize ramSize, uint sector, string? name = null, string? procName = null, string? ramName = null, string? portBusName = null)
    {
        int id = _emulator.CreateDevice(bios, ramSize.ConvertToApiEnum(), sector, name, procName, ramName, portBusName);
        if (id != -1)
            DeviceListChanged?.Invoke(_emulator.AllDevices);
        return id;
    }

    public void SetMainDevice(int deviceId)
    {
        _emulator.SetMainDevice(deviceId);
    }

    public DeviceView? MainDevice()
    {
        if (_emulator.MainDeviceId == -1 || _emulator.MainDevice == null) return null;
        var info = _emulator.GetDeviceInfo(_emulator.MainDeviceId);
        if (info == null) return null;
        return info.Value.ConvertDeviceInfo();
    }

    public bool ChangeDeviceSector(int id, uint newSector) => _emulator.ChangeDeviceSector(id, newSector);

    private class ActionLogger(Action<string, LogLevel> logAction, Action clearAction) : ILogger
    {
        private readonly Action<string, LogLevel> _logAction = logAction;
        private readonly Action _clearAction = clearAction;

        public bool UseConsole => false;
        public void Info(string message) => _logAction(message, LogLevel.Log);
        public void Warning(string message) => _logAction(message, LogLevel.Warning);
        public void Error(string message) => _logAction(message, LogLevel.Error);
        public void Clear() => _clearAction();
    }
}

/*
 public sealed class VMHost(SizePort sizePort = SizePort.Size16KB, SizePortOnDev sizePortOn = SizePortOnDev.Size16B)
{
    public const uint StartLoadProgramm = 0x200;

    private IProjectService? _projectService;
    private readonly Emulator _emulator = new(sizePort.ConvertToKernelEnum(), sizePortOn.ConvertToKernelEnum());
    private IOutputView? _outputView;
    private IProjectPaths? _projectPaths;

    public event Action<IEnumerable<DeviceInfo>>? DeviceListChanged;

    public void ResetHost()
    {
        if (_emulator != null)
        {
            foreach (var dev in _emulator.AllDevices) 
                dev.Device.StopDevice();
            _emulator.Reset();
        }
        _projectService?.SaveAllFiles();

        _projectService = null;
        _outputView = null;
        _projectPaths = null;
    }

    public void ResetViewModels(IProjectService projectService, IOutputView outputView, IProjectPaths paths,
                    SizePort sizePort = SizePort.Size16KB, SizePortOnDev sizePortOn = SizePortOnDev.Size16B)
    {
        _emulator.ReloadEmulator(sizePort.ConvertToKernelEnum(), sizePortOn.ConvertToKernelEnum());
        
        _projectService = projectService;
        _outputView = outputView;
        
        _projectPaths = paths;
        InitializeLogger(outputView);
    }


    public bool IsProjectOpened => _projectPaths != null;
    public string IncludePath => _projectPaths != null ? _projectPaths.IncludePath : string.Empty;

    private void InitializeLogger(IOutputView outPutView)
    {
        LoggerProvider.SetLogger(new ActionLogger(SetLogger, outPutView.Clear));
    }

    private void SetLogger(string str, LogLevel log) => _outputView?.Append(str, log);


    // --------------- Управление проектом ---------------
    public void OpenProject(IProjectPaths projectPath)
    {
        if (_projectService == null || _emulator == null)
        {
            _outputView?.Append("Компоненты системы не инициализированы", LogLevel.Error);
            return;
        }
        _projectPaths = projectPath;
        _projectService.OpenProject();
        _outputView?.Append($"Проект {projectPath} открыт.", LogLevel.Log);
    }

    public CompilationResult Compile(ulong baseAddress = StartLoadProgramm)
    {
        if (!IsProjectOpened || _projectPaths == null)
            return new CompilationResult(null, ["Проект не открыт."]);

        if (_projectService == null || _emulator == null || _outputView == null) 
            return new CompilationResult(null, ["Компоненты проекта не инициализированы"]);

        try
        {
            byte[] program = ProjectBuilder.BuildProject(
                _projectService.FileService,
                _projectService.EditorService, 
                _projectPaths,baseAddress);
            return new CompilationResult(program, null);
        }
        catch (Exception ex)
        {
            _outputView.Append(ex.Message, LogLevel.Error);
            return new CompilationResult(null, [ex.Message]);
        }
    }

    public DeviceData? CreateDeviceContext(int? id = null)
    {

        if (_projectService == null || _emulator == null)
        {
            _outputView?.Append("Компоненты системы не инициализированы", LogLevel.Error);
            return null;
        }
        var device = id != null ? _emulator.GetDevice(id.Value) : _emulator.MainDevice;
        
        if (device == null)
        {
            _outputView?.Append("Выбраное устройство/основное не инициализированы", LogLevel.Error);
            return null;
        }
        return new DeviceData(device);
    }

    public DeviceData? GetDeviceData(int id)
    {
        if (_emulator == null)
        {
            _outputView?.Append("Компоненты системы не инициализированы", LogLevel.Error);
            return null;
        }

        var d = _emulator.GetDevice(id);
        return d != null ? new(d) : null;
    }

    public IEnumerable<DeviceView> GetAllDevices()
    {
        if (_emulator == null)
        {
            _outputView?.Append("Компоненты системы не инициализированы", LogLevel.Error);
            return [];
        }
        return _emulator.AllDevices.Select(VMHostHelper.ConvertDeviceInfo);
    }


    public int CreateDevice(byte[] bios, RamSize ramSize, uint sector, string? name = null, string? procName = null, string? ramName = null, string? portBusName = null)
    {
        if (_emulator == null)
        {
            _outputView?.Append("Компоненты системы не инициализированы", LogLevel.Error);
            return -1;
        }

        int id = _emulator.CreateDevice(bios, ramSize.ConvertToApiEnum(), sector, name, procName, ramName, portBusName);
        if (id != -1)
            DeviceListChanged?.Invoke(_emulator.AllDevices);
        return id;
    }

    public void SetMainDevice(int deviceId)
    {
        if (_emulator == null)
        {
            _outputView?.Append("Компоненты системы не инициализированы", LogLevel.Error);
            return;
        }

        _emulator.SetMainDevice(deviceId);
    }

    public DeviceView? MainDevice()
    {
        if (_emulator == null)
        {
            _outputView?.Append("Компоненты системы не инициализированы", LogLevel.Error);
            return null;
        }
        if (_emulator.MainDeviceId == -1 || _emulator.MainDevice == null) return null;
        var info = _emulator.GetDeviceInfo(_emulator.MainDeviceId);
        if (info == null) return null;
        return info.Value.ConvertDeviceInfo();
    }

    public bool ChangeDeviceSector(int id, uint newSector)
    {
        if (_emulator == null)
        {
            _outputView?.Append("Компоненты системы не инициализированы", LogLevel.Error);
            return false;
        }
        return _emulator.ChangeDeviceSector(id, newSector);
    }

    private class ActionLogger(Action<string, LogLevel> logAction, Action clearAction) : ILogger
    {
        private readonly Action<string, LogLevel> _logAction = logAction;
        private readonly Action _clearAction = clearAction;

        public bool UseConsole => false;
        public void Info(string message) => _logAction(message, LogLevel.Log);
        public void Warning(string message) => _logAction(message, LogLevel.Warning);
        public void Error(string message) => _logAction(message, LogLevel.Error);
        public void Clear() => _clearAction();
    }
}

 */


public readonly struct DeviceView(int id,
                                  uint ramSize,
                                  uint portSize,
                                  uint sector, byte[] ram,
                                  DateTime createdAt,
                                  string? name = null)
{
    public int Id { get; init; } = id;
    public uint RamSize { get; init; } = ramSize;
    public uint PortSize { get; init; } = portSize;
    public uint Sector { get; init; } = sector;
    public ReadOnlyMemory<byte> Ram { get; init; } = new(ram);
    public string? Name { get; init; } = name;
    public DateTime CreatedAt { get; init; } = createdAt;
    public string PortRange { get; init; } = CreatePortRange(sector, portSize);

    public static string CreatePortRange(uint sector, uint portSize)
    {
        uint start = sector * portSize;
        uint end = start + portSize - 1;
        return $"{start}–{end}";
    }
}


public readonly struct CallBackOnLaunch(Action launch = null!, Action step = null!, Action end = null!)
{
    public readonly Action OnLaunch = launch;
    public readonly Action OnStep = step;
    public readonly Action OnEnd = end;
}
public interface ILogger : Kernel.BiosSystem.ILogger;

public enum LogLevel
{
    Log,
    Warning,
    Error
}

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

    public LaunchModeDevice LoadProgram(byte[] program, ulong loadAddress = VMHost.StartLoadProgramm)
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



public class LaunchModeDevice
{
    private readonly Device _device;

    internal LaunchModeDevice(Device device) => _device = device;

    public void SetHeapAddress(ulong hp) => _device.InitHeap(hp);

    public void LaunchDevice(ulong startAddress = VMHost.StartLoadProgramm,
                             bool debug = false,
                             int delayMs = 0,
                             bool showTimer = false,
                             CallBackOnLaunch callBack = default)
    {
        _device.LaunchDevice(startAddress, debug, delayMs, showTimer, callBack.OnLaunch!, callBack.OnStep!, callBack.OnEnd!);
    }

    public DeviceData StopAndReset()
    {
        _device.StopDevice();
        return new DeviceData(_device);
    }

    public DeviceStepMode StepMode(ulong startAddress = VMHost.StartLoadProgramm)
    {
        _device.BreakPointerLaunchDevice(startAddress);
        return new DeviceStepMode(_device);
    }
}

public class DeviceStepMode
{
    private readonly Device _device;

    internal DeviceStepMode(Device device) => _device = device;
    

    public void Step(bool debug = false) => _device.NextStepProcessor(debug);
    public void MultyStep(bool debug = false, int count = 5) => _device.NextStepProcessorCount(count, debug);

}
public static class ConvertorEnum 
{
    extension(LogLevel log)
    {
        public Kernel.ProcessorSystem.LogLevel ConvertToApiEnum() => log switch
        {
            LogLevel.Log => Kernel.ProcessorSystem.LogLevel.Log,
            LogLevel.Warning => Kernel.ProcessorSystem.LogLevel.Warning,
            LogLevel.Error => Kernel.ProcessorSystem.LogLevel.Error,
            _ => throw new NotImplementedException(),
        };
    }
    extension(RamSize ram)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Kernel.RamSystem.RamSize ConvertToApiEnum()
        {
            return Unsafe.As<RamSize, Kernel.RamSystem.RamSize>(ref ram);
        }
    }
    extension(SizePort port)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Kernel.ControllersData.SizePort ConvertToKernelEnum()
        {
            return Unsafe.As<SizePort, Kernel.ControllersData.SizePort>(ref port);
        }
    }
    extension(SizePortOnDev sizePortOn)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Kernel.ControllersData.SizePortOnDev ConvertToApiEnum()
        {
            return Unsafe.As<SizePortOnDev, Kernel.ControllersData.SizePortOnDev>(ref sizePortOn);
        }
    }
}

public enum SizePort : ulong
{
    [Description("64 байта")] Size64B = 1U << 6,
    [Description("128 байт")] Size128B = 1U << 7,
    [Description("256 байт")] Size256B = 1U << 8,
    [Description("512 байт")] Size512B = 1U << 9,
    [Description("1 КБ")] Size1KB = 1U << 10,
    [Description("4 КБ")] Size4KB = 1U << 12,
    [Description("8 КБ")] Size8KB = 1U << 13,
    [Description("16 КБ")] Size16KB = 1U << 14,
    [Description("64 КБ")] Size64KB = 1U << 16,
    [Description("128 КБ")] Size128KB = 1U << 17,
    [Description("256 КБ")] Size256KB = 1U << 18,
    [Description("512 КБ")] Size512KB = 1U << 19,
}

public enum SizePortOnDev : uint
{
    [Description("4 байта")] Size4B = 1U << 2,
    [Description("8 байт")] Size8B = 1U << 3,
    [Description("16 байт")] Size16B = 1U << 4,
    [Description("32 байта")] Size32B = 1U << 5,
}
public enum SourceLanguage
{
    Asm,
    C,
    None,
}


public enum RamSize : ulong
{
    [Description("128 байт")] Size128B = 1U << 7,
    [Description("256 байт")] Size256B = 1U << 8,
    [Description("512 байт")] Size512B = 1U << 9,

    [Description("1 КБ")] Size1KB = 1U << 10,
    [Description("4 КБ")] Size4KB = 1U << 12,
    [Description("8 КБ")] Size8KB = 1U << 13,
    [Description("16 КБ")] Size16KB = 1U << 14,
    [Description("64 КБ")] Size64KB = 1U << 16,
    [Description("128 КБ")] Size128KB = 1U << 17,
    [Description("256 КБ")] Size256KB = 1U << 18,
    [Description("512 КБ")] Size512KB = 1U << 19,

    [Description("1 МБ")] Size1MB = 1U << 20,
    [Description("4 МБ")] Size4MB = 1U << 22,
    [Description("8 МБ")] Size8MB = 1U << 23,
    [Description("16 МБ")] Size16MB = 1U << 24,
    [Description("32 МБ")] Size32MB = 1U << 25,
    [Description("64 МБ")] Size64MB = 1U << 26,
    [Description("128 МБ")] Size128MB = 1U << 27,
}

public static class VMHostHelper
{
    /// <summary>
    /// RU: Запускает парсер и лексер кода, вызов его не в блоке try - catch приведет к постоянным выбросам исключений
    /// ENG: Executes the code parser and lexer. This method throws exceptions on parsing failures and must be wrapped in a try-catch block.
    /// </summary>
    /// <param name="text"> исходный текст </param>
    public static void LaunchUnsafeParse(string text)
    {
        var lexer = new Lexer(text);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens);
        parser.Parse();
    }

    public static DeviceView ConvertDeviceInfo(this DeviceInfo d)
    {
        return new(d.Id, (uint)d.RamSize, (uint)d.PortSize, d.Sector, d.Device.RamArray, d.CreatedAt, d.Name);
    }

    public static ResultDeCompilation DisassemblCode(ReadOnlyMemory<byte> prog, ulong baseAddress = 0UL)
    {
        var text = Disassembler.Disassemble(prog, out int lenght, out int size, baseAddress);
        return new(text, lenght, size);
    }

    public static ResultDeCompilation DisassemblCode(ReadOnlySpan<byte> prog, ulong baseAddress = 0UL)
    {
        var text = Disassembler.Disassemble(prog, out int lenght, out int size, baseAddress);
        return new(text, lenght, size);
    }

    public static ResultDeCompilation DisassemblCode(byte[] prog, ulong baseAddress = 0UL)
    {
        var text = Disassembler.Disassemble(prog, out int lenght, out int size, baseAddress);
        return new(text, lenght, size);
    }


}

public readonly record struct ResultDeCompilation(string TextAsm, int Lenght, int Size);
