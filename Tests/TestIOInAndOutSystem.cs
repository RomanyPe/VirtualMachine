using Compiller.ASM;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.Common;
using Xunit.Abstractions;

namespace Tests;

public class TestIOInAndOutSystem(ITestOutputHelper outPut)
{
    private readonly ITestOutputHelper _outPut = outPut;


    [Fact]
    public void TestReadingLocalIOMemory()
    {
        const string NameLogger = "default";
        const string Code_First_Device_Test_1 = @"
LDI r0, 0x42    // значение для записи
LDI r1, 0       // адрес порта 0 (глобальный адрес для сектора 0)
OUT r0, r1      // записать в порт 0

LDI r1, 0       // тот же порт
IN  r0, r1      // прочитать из порта 0 в r0

PRINT_INT r0    // выведет 66 (0x42)

END";

        var log = new LoggerOutPut();
        LoggerProvider.RegistryLogger(NameLogger, log);
        LoggerProvider.ChoiceLogger(NameLogger);

        Emulator emu = new(SizePort.Size16KB, SizePortOnDevice.Size16B);

        Device? deviceFirst = null;

        try
        {
            int deviceIdFirst = emu.CreateDevice([], RamSize.Size16KB, 0, "Device");
            Assert.InRange(deviceIdFirst, 0, int.MaxValue);

            deviceFirst = emu.GetDevice(deviceIdFirst);

            Assert.NotNull(deviceFirst);

            deviceFirst.ActionOnWake = OnWake;

            byte[]? programFirstDevice = GetByteCode(Code_First_Device_Test_1);
            Assert.NotNull(programFirstDevice);

            deviceFirst.LoadProgram(programFirstDevice, 0);
            deviceFirst.InitHeap((ulong)programFirstDevice.LongLength);

            using var doneFirst = new ManualResetEventSlim(false);

            deviceFirst.LaunchDevice(0, false, 0, false, OnTitle, null,
                log => { OnEnd(log); doneFirst.Set(); });

            bool firstFinished = doneFirst.Wait(TimeSpan.FromSeconds(10));

            Assert.True(firstFinished, "Первое устройство не завершилось за отведённое время.");
            Assert.InRange(deviceFirst.CurrentIP, 0UL, (ulong)programFirstDevice.LongLength);
            Assert.False(deviceFirst.IsRunning);

            _outPut.WriteLine(log.ToString());
        }
        finally
        {
            deviceFirst?.Dispose();
            emu.Reset(); // или emu.Dispose(), если есть
            _outPut.WriteLine(log.ToString());
            LoggerProvider.DeleteLogger(NameLogger);
        }

    }

    [Fact]
    public void TestIOMemory()
    {
        const string NameLogger = "default";
        const string Code_First_Device_Test_1 = 
    @"
    LDI r0, 0xAA    // значение
    LDI r1, 16      // глобальный адрес порта второго устройства
    OUT r0, r1      // записать

    END
    ";
        const string Code_Second_Device_Test_1 =
    @"
    LDI r1, 16      // тот же глобальный адрес порта
    IN  r0, r1      // прочитать в r0

    PRINT_INT r0    // выведет 170 (0xAA)

    END
    ";
        var log = new LoggerOutPut();
        LoggerProvider.RegistryLogger(NameLogger, log);
        LoggerProvider.ChoiceLogger(NameLogger);


        Emulator emu = new(SizePort.Size64KB, SizePortOnDevice.Size16B);

        Device? deviceFirst = null;
        Device? deviceSecond = null;

        try
        {
            int deviceIdFirst = emu.CreateDevice([], RamSize.Size16KB, 0, "Device First");
            Assert.InRange(deviceIdFirst, 0, int.MaxValue);

            int deviceIdSecond = emu.CreateDevice([], RamSize.Size16KB, 1, "Device Second");
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
            deviceFirst.LaunchDevice(0, false, 0, false, OnTitle, null,
                log => { OnEnd(log); doneFirst.Set(); });
            deviceSecond.LaunchDevice(0, false, 0, false, OnTitle, null,
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

    private static void OnEnd(Action<string, LogLevel> logger)
    {
        logger.Invoke("Устройство выключилось штатно", LogLevel.Log);
    }

    private static void OnTitle(Action<string, LogLevel> logger)
    {
        logger.Invoke("Устройство запущено", LogLevel.Log);
    }
    private static void OnWake(Action<string, LogLevel> logger)
    {
        logger.Invoke("Устройство проснулось", LogLevel.Log);
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
