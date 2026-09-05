using Compiller.ASM;
using Compiller.C;
using Kernel.Common;
using System.Runtime.InteropServices.JavaScript;
using VMApplication.Emulator;
using VMApplication.Logger;
using VMApplication.Project;
using static Kernel.Utilites.ManagerDevices;

namespace VMApplication;

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

    extension(DeviceInfo d)
    {
        public DeviceView ConvertDeviceInfo() => new(d.Id,
                                                     (uint)d.RamSize,
                                                     (uint)d.PortSize,
                                                     d.Sector,
                                                     d.Device.RamArray,
                                                     d.CreatedAt,
                                                     d.Name);
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

public sealed class ConsoleOutputView : IOutputView
{
    public void AppendLine(string message, LogLevel level)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = level switch
        {
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => originalColor
        };
        Console.WriteLine($"[{level}] {message}");
        Console.ForegroundColor = originalColor;
    }

    public void Append(char message) => Console.Write(message.AsText);

    public void Clear() => Console.Clear();
}

public sealed class DefaultProjectFilesConfig : IProjectFilesConfig
{
    public string ProjectPath { get; init; } = Directory.GetCurrentDirectory();
    public string IncludePath { get; init; } = "include";
    public string[] ExtensionsAsm { get; init; } = [".asm", ".vma"];
    public string[] ExtensionsMiniC { get; init; } = [".c", ".mic"];
}

public sealed class DefaultFileService(string projectPath) : IFileService
{

    public string ProjectPath { get; } = projectPath;

    public IEnumerable<string> GetSourceFiles()
    {
        return Directory.EnumerateFiles(ProjectPath, "*.*", SearchOption.AllDirectories);
    }

    public string ReadFile(string fileName)
    {
        var fullPath = Path.Combine(ProjectPath, fileName);
        return File.ReadAllText(fullPath);
    }

    public void SaveFile(string fileName, string content)
    {
        var fullPath = Path.Combine(ProjectPath, fileName);
        File.WriteAllText(fullPath, content);
    }

    public void SaveBinaryFile(string fileName, byte[] content)
    {
        var fullPath = Path.Combine(ProjectPath, fileName);
        File.WriteAllBytes(fullPath, content);
    }

    public bool Exist(string fileName)
    {
        var fullPath = Path.Combine(ProjectPath, fileName);
        return File.Exists(fullPath);
    }

    public string CombinePath(string path1, string path2) => Path.Combine(path1, path2);
}

public static class VMHostFactory
{
    public static VMHost CreateDefault(
        IOutputView? outputView = null,
        IProjectFilesConfig? projectConfig = null,
        IFileService? fileService = null,
        SizePort portBusSize = SizePort.Size16KB,
        SizePortOnDevice portsPerDevice = SizePortOnDevice.Size16B,
        string loggerName = "default")
    {
        // Создаём логгер
        var loggerBuilder = new LoggerBuilder()
            .WithOutPut(outputView ?? new ConsoleOutputView())
            .WithNameLogger(loggerName);
        var logger = loggerBuilder.Build();

        // Создаём проект
        var res = projectConfig ?? new DefaultProjectFilesConfig();

        var projectBuilder = new VMHostProjectBuilder()
            .WithLogger(logger)
            .WithPaths(res)
            .WithFileSevice(fileService ?? new DefaultFileService(res.ProjectPath));
        var project = projectBuilder.Build();

        // Создаём эмулятор
        var emulatorBuilder = new VMEmulatorBuilder()
            .WithLogger(logger)
            .WithPortBusSize(portBusSize)
            .WithPortsPerDevice(portsPerDevice);
        var emulator = emulatorBuilder.Build();

        return new VMHost(project, emulator, logger);
    }
}

public sealed record VMHost(VMHostProject Project, VMEmulator Emulator, VMHostLogger Logger) : IDisposable
{
    public void Dispose() => Emulator.Dispose();
}


