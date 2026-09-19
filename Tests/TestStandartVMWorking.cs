using Compiller.ASM;
using Compiller.ASM.Optimizators;
using Compiller.ASM.Optimizators.Rules;
using Kernel.Common;
using System.Text;
using VMApplication;
using VMApplication.Emulator;
using VMApplication.Logger;
using Xunit.Abstractions;

namespace Tests;

public class TestStandartVMWorking(ITestOutputHelper output)
{
    private const string test1 = 
        """
        
        """;
    private readonly ITestOutputHelper _outPut = output;
    private readonly ReplaceSafeMemAccessWithUnsafeRule _rule = new();

    private static string GetText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("LDI r0, 0");
        sb.AppendLine("LDI r1, 1");
        for (int i = 0; i < 1_000_000; i++)
            sb.AppendLine("ADD r0, r1");
        sb.AppendLine("END");
        return sb.ToString();
    }

    private class VmOutPut(ITestOutputHelper output) : IOutputView
    {
        private readonly ITestOutputHelper _outPut = output;

        public void Append(char message)
        {
            _outPut.WriteLine(message.AsText);
        }

        public void AppendLine(string message, LogLevel level = LogLevel.Log)
        {
            _outPut.WriteLine($"[{level}] {message}");
        }

        public void Clear() { }
    }

    [Theory]
    [InlineData(test1)]
    [InlineData(" ")]
    public void Main(string asm)
    {
        if (string.IsNullOrEmpty(asm) ||  asm == " ")
        {
            asm = GetText();
        }

        VMHost? host = null;
        DeviceContext? deviceContext = null;

        try
        {
            host = VMHostFactory.CreateDefault(
                   new VmOutPut(_outPut),
                   portBusSize: SizePort.Size16KB,
                   portsPerDevice: SizePortOnDevice.Size16B
               );

            byte[]? program = GetByteCode(asm, RamSize.Size16MB);

            Assert.NotNull(program);

            int deviceId = host.Emulator.CreateDevice(
                bios: [], // биос 
                ramSize: RamSize.Size1MB,
                sector: 0,
                name: "ConsoleVM"
            );

            Assert.InRange(deviceId, 0, int.MaxValue);
            deviceContext = host.Emulator.CreateDeviceContext(deviceId);

            Assert.NotNull(deviceContext);
            LaunchModeDevice launchMode = deviceContext!.LoadProgram(program, 0);

            Assert.NotNull(launchMode);

            launchMode.Launch(
                startAddress: 0,
                debug: false,
                delayMs: 0,
                showTimer: true,
                onStart: ctx => ctx.Log("Симуляция началась", LogLevel.Log),
                onEnd: ctx => ctx.Log("Симуляция завершена", LogLevel.Log)
            );
        }
        catch (Exception ex)
        {
            _outPut.WriteLine(ex.Message);
        }
        finally
        {
            deviceContext?.Dispose();
            host?.Dispose();
        }

    }

    private byte[]? GetByteCode(string text, RamSize size)
    {
        try
        {
            var asm = new IRAssembler();
            var code = new AssemblerParser();
            code.Assemble(text, asm);
            var log = new PeepholeLog();
            var peep = asm.GetItems();
            _rule.RamSize = size;
            //PeepholeOptimizer.Optimize(peep, log, 5);
            LoggerKernel.LogFromSystem("Compiller", log.ToString());
            
            return asm.Build();
        }
        catch (Exception ex)
        {
            LoggerKernel.LogFromSystem("Compiler", ex.Message, LogLevel.Error);
            return null;
        }
    }
}
