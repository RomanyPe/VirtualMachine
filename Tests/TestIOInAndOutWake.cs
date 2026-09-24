using Compiller.ASM;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.Common;
using Xunit.Abstractions;

namespace Tests;

[Collection("StrictlySingleExecutionCollection")]
public class TestIOInAndOutWake(ITestOutputHelper outPut)
{
    private readonly ITestOutputHelper _outPut = outPut;

    [Fact]
    public void TestIOSincWAKE_INT()
    {
        const string NameLogger = "default";
        const string Code_First_Device_Test_1 =
        """

    LDI r0, 16
    LDI r1, 16
    OUT r0, r1
    NOP
    LDI r0, 16      // адрес порта, через который будим
    WAKE_INT r0     // разбудить устройство, чей порт 16
    PRINT_INT r0

    END
    """;
        const string Code_Second_Device_Test_1 =
        """
    HALT            // уснуть

    LDI r1, 16
    IN  r0, r1
    PRINT_INT r0

    END
    """;
        var log = new LoggerOutPut();
        LoggerProvider.RegistryLogger(NameLogger, log);
        LoggerProvider.ChoiceLogger(NameLogger);


        Emulator emu = new(SizePort.Size64KB, SizePortOnDevice.Size16B);

        Device? deviceFirst = null;
        Device? deviceSecond = null;

        try
        {
            int deviceIdFirst = emu.CreateDevice([], RamSize.Size16KB, RamSize.Size64KB, 0, "Device First");
            Assert.InRange(deviceIdFirst, 0, int.MaxValue);

            int deviceIdSecond = emu.CreateDevice([], RamSize.Size16KB, RamSize.Size64KB, 1, "Device Second");
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

            byte[]? programSecondDevice = GetByteCode(Code_Second_Device_Test_1);
            Assert.NotNull(programSecondDevice);

            deviceSecond.LoadProgram(programSecondDevice, 0);

            // После загрузки программ, перед запуском:
            using var doneFirst = new ManualResetEventSlim(false);
            using var doneSecond = new ManualResetEventSlim(false);

            // Запускаем с колбэками, которые сигнализируют о завершении
            var threadFirst = new Thread(() => deviceFirst.RunSimulation(0, false, 0, false,
                           title => title.Log("Запуск", LogLevel.Log),
                           null,
                           (end, _) => { end.Log("Завершено", LogLevel.Log); doneFirst.Set(); }));
            var threadSecond = new Thread(() => deviceSecond.RunSimulation(0, false, 0, false,
                           title => title.Log("Запуск", LogLevel.Log),
                           null,
                           (end, _) => { end.Log("Завершено", LogLevel.Log); doneSecond.Set(); }));
            threadFirst.Start();
            threadSecond.Start();

            // Ждём завершения обоих устройств (максимум 10 секунд каждое)
            bool firstFinished = doneFirst.Wait(TimeSpan.FromSeconds(10));
            bool secondFinished = doneSecond.Wait(TimeSpan.FromSeconds(10));

            var firstIp = deviceFirst.CurrentIP;
            var secondIp = deviceSecond.CurrentIP;

            Assert.InRange(firstIp, 0UL, (ulong)programFirstDevice.LongLength);
            Assert.InRange(secondIp, 0UL, (ulong)programSecondDevice.LongLength);

            Assert.True(firstFinished, $"Первое устройство не завершилось за отведённое время. Ip [{firstIp}]");
            Assert.True(secondFinished, $"Второе устройство не завершилось за отведённое время. Ip [{secondIp}]");

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