using Compiller.ASM;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.Common;
using System.Text;
using Xunit.Abstractions;

namespace Tests;

public class LoggerOutPut : ILogger
{
    private readonly StringBuilder _output = new();
    private static readonly Lock _lock = new();

    public void Clear()
    {
        lock (_lock)
            _output.Clear();
    }

    public void Error(string message)
    {
        lock (_lock)
            _output.AppendLine(message);
    }

    public void Info(string message)
    {
        lock (_lock)
            _output.AppendLine(message);
    }

    public void Warning(string message)
    {
        lock (_lock)
            _output.AppendLine(message);
    }

    public override string ToString() => _output.ToString();
}


public class TestIOSleepAndWake(ITestOutputHelper outPut)
{
    private readonly ITestOutputHelper _outPut = outPut;


    [Theory]
    [InlineData(SizePort.Size16KB, SizePortOnDevice.Size16B)]
    [InlineData(SizePort.Size128KB, SizePortOnDevice.Size16B)]
    [InlineData(SizePort.Size512KB, SizePortOnDevice.Size32B)]
    public void TestCpu(SizePort totalPorts, SizePortOnDevice portsPerDevice)
    {
        const string NameLogger = "default";
        const string Code_First_Device_Test_1 = "NOP\r\nNOP\r\nHALT\r\nEND\r\nEND\r\nEND";
        const string Code_Second_Device_Test_1 =
            "\r\nNOP\r\nNOP\r\nNOP\r\nNOP\r\nNOP\r\nNOP\r\nNOP\r\nNOP" +
            "\r\nNOP\r\nNOP\r\nLDI r0, 0\r\nWAKE_INT r0\r\nEND\r\nEND\r\nEND";
        var log = new LoggerOutPut();
        LoggerProvider.RegistryLogger(NameLogger, log);
        LoggerProvider.ChoiceLogger(NameLogger);


        Emulator emu = new(totalPorts, portsPerDevice);

        Device? deviceFirst = null;
        Device? deviceSecond = null;

        try
        {
            int deviceIdFirst = emu.CreateDevice([], RamSize.Size1MB, 0, "Device First");
            Assert.InRange(deviceIdFirst, 0, int.MaxValue);

            int deviceIdSecond = emu.CreateDevice([], RamSize.Size1MB, 1, "Device Second");
            Assert.InRange(deviceIdSecond, 0, int.MaxValue);

            deviceFirst = emu.GetDevice(deviceIdFirst);
            deviceSecond = emu.GetDevice(deviceIdSecond);

            Assert.NotNull(deviceFirst);
            Assert.NotNull(deviceSecond);

            deviceFirst.ActionOnWake = OnWake;
            deviceSecond.ActionOnWake = OnWake;

            byte[]? programFirstDevice = GetByteCode(Code_First_Device_Test_1);
            Assert.NotNull(programFirstDevice);

            deviceFirst.LoadProgram(programFirstDevice, 0);
            deviceFirst.InitHeap((ulong)programFirstDevice.LongLength);

            byte[]? programSecondDevice = GetByteCode(Code_Second_Device_Test_1);
            Assert.NotNull(programSecondDevice);

            deviceSecond.LoadProgram(programSecondDevice, 0);
            deviceSecond.InitHeap((ulong)programSecondDevice.LongLength);

            // После загрузки программ, перед запуском:
            using var doneFirst = new ManualResetEventSlim(false);
            using var doneSecond = new ManualResetEventSlim(false);

            // Запускаем с колбэками, которые сигнализируют о завершении
            deviceFirst.LaunchDeviceOnDedicatedThread(0, false, 0, false, OnTitle, null,
                log => { OnEnd(log); doneFirst.Set(); });
            deviceSecond.LaunchDeviceOnDedicatedThread(0, false, 0, false, OnTitle, null,
                log => { OnEnd(log); doneSecond.Set(); });

            // Ждём завершения обоих устройств (максимум 10 секунд каждое)
            bool firstFinished = doneFirst.Wait(TimeSpan.FromSeconds(10));
            bool secondFinished = doneSecond.Wait(TimeSpan.FromSeconds(10));

            Assert.True(firstFinished, "Первое устройство не завершилось за отведённое время.");
            Assert.True(secondFinished, "Второе устройство не завершилось за отведённое время.");

            Assert.InRange(deviceFirst.CurrentIP, 0UL, (ulong)programFirstDevice.LongLength);
            Assert.InRange(deviceSecond.CurrentIP, 0UL, (ulong)programSecondDevice.LongLength);

            Assert.False(deviceFirst.IsRunning);
            Assert.False(deviceSecond.IsRunning);

            _outPut.WriteLine(log.ToString());
        }
        finally
        {
            deviceFirst?.Dispose();
            deviceSecond?.Dispose();
            emu.Reset(); // или emu.Dispose(), если есть
            _outPut.WriteLine(log.ToString());
            LoggerProvider.DeleteLogger(NameLogger);
        }

    }

    private static void OnEnd(IDeviceLoggerContext logger)
    {
        logger.Log("Устройство выключилось штатно", LogLevel.Log);
    }

    private static void OnTitle(IDeviceLoggerContext logger)
    {
        logger.Log("Устройство запущено", LogLevel.Log);
    }
    private static void OnWake(IDeviceLoggerContext logger)
    {
        logger.Log("Устройство проснулось", LogLevel.Log);
    }


    private static byte[]? GetByteCode(string text)
    {
        try
        {
            var code = new AssemblerParser();
            return code.Assemble(text);
        }
        catch (Exception ex)
        {
            LoggerKernel.LogFromSystem("Compiler", ex.Message, LogLevel.Error);
            return null;
        }
    }
}