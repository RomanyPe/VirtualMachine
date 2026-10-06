using Compiller.ASM;
using Compiller.ASM.Optimizators;
using Compiller.C;
using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.Contracts;
using VMApplication.Emulator;
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
                                                     d.Sector,
                                                     d.Name);
    }

    public static ResultDeCompilation DisassemblCode(ReadOnlySpan<byte> prog, ulong baseAddress = 0UL)
    {
        var text = Disassembler.Disassemble(prog, out int lenght, out int size, baseAddress);
        return new(text, lenght, size);
    }

    private class DeviceLoggerSingleObj : IDeviceLoggerContext
    {
        public void Log(string message, LogLevel level = LogLevel.Log)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = level switch
            {
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => originalColor
            };
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }
    }

    public static ResultSimulation RunIsolated(
        string source,
        SourceLanguage lang, 
        bool optimize = false,
        RamSize? ram = null)
    {
        ram ??= RamSize.MB16;
        ProjectBuilder projectBuilder = new(null, null);
        IRAssembler assembler = new(0);
        projectBuilder.Build([new("sourse", source, lang)], optimize, assembler);
        var resCompile = assembler.Build();
        
        using Device device = new(null!, ram.Value, new DeviceLoggerSingleObj(), null);
        if (resCompile != null && device.TryLoadProgramFast(resCompile, 0))
        {
            device.RunSimulation(0, false, 0, false, null, null, null);
            Span<ulong> sp = stackalloc ulong[32];
            device.CopyRegisters(sp);
            long steps = device.StepCount;
            return new(sp.ToArray(), resCompile, steps, true);
        }
        return new();
    }
}
public record struct ResultSimulation(ulong[]? Registers = null, byte[]? ByteCode = null, long StepCount = 0, bool Succes = false);
