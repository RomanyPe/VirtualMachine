using Compiller.ASM;
using Compiller.C.Optimizators;
using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.Contracts;
using Kernel.ControllersData;
using VMApplication.Default;
using VMApplication.Emulator;
using VMApplication.Logger;
using VMApplication.Project;

namespace VMApplication;

public static class VMHostHelper
{
    public static DeviceView ConvertDeviceInfo(this DeviceInfo d) => new(d.Id,
                                                     d.Sector,
                                                     d.Name);

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
        IFileService? fileService = null,
        IProjectFilesConfig? projectFilesConfig = null,
        IOutputView? outputView = null,
        bool optimize = false,
        RamSize? ram = null)
    {
        ram ??= RamSize.MB16;
        outputView ??= new ConsoleOutputView();
        try
        {
            ProjectBuilder projectBuilder = new(
            fileService ?? new DefaultFileService(),
            projectFilesConfig ?? new DefaultProjectFilesConfig());

            Assembler assembler = new(ProjectBuilder.ZeroAdressProgram);
            AstOptimizer astOptimizer = new();
            projectBuilder.Build([new("source", source, lang)], optimize, assembler, astOptimizer);
            var resCompile = assembler.Build();

            PortBus portBus = new(PortSize.B64, DevicePortSize.B8);
            using Device device = new(portBus, ram.Value, new DeviceLoggerSingleObj(), null);
            portBus.RegisterDevice(device, 0);
            if (resCompile != null && device.TryLoadProgramFast(resCompile, 0))
            {
                device.RunSimulation(0, 0, false, null, null, null);
                var sp = device.GetRegistersSnapshot();
                long steps = device.StepCount;
                device.Dispose();
                return new(sp, resCompile, steps, true);
            }
            device.Dispose();
        }
        catch (Exception ex)
        {
            outputView.AppendLine(ex.Message);
        }
        return new();
    }
}
public record struct ResultSimulation(ulong[]? Registers = null, byte[]? ByteCode = null, long StepCount = 0, bool Succes = false);
