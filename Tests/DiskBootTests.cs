using Compiller.ASM;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.ProcessorSystem;
using System.Text;
using Xunit.Abstractions;

namespace Tests;

public class DiskBootTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _outPut = output;

    private class TestLogger : ILogger
    {
        private readonly StringBuilder _sb = new();
        private static readonly Lock _lock = new();

        public void Clear() { lock (_lock) _sb.Clear(); }
        public void Error(string message) => Append("ERR", message);
        public void Info(string message) => Append("INF", message);
        public void Warning(string message) => Append("WRN", message);

        private void Append(string level, string message)
        {
            lock (_lock) _sb.AppendLine($"[{level}] {message}");
        }

        public override string ToString() => _sb.ToString();
        public bool Contains(string text) => _sb.ToString().Contains(text);
    }

    [Fact]
    public void BiosLoadsProgramFromDisk()
    {
        const string BiosAsm = 
            """
            // BIOS bootloader
            // Диск расположен в секторе 0 портов (базовый адрес портов диска = 0)
            // Порты диска:
            //   0 – данные (автоинкремент)
            //   1 – команда
            //   2 – статус
            //   3..6 – LBA (little-endian)

            // --- Устанавливаем LBA = 0 ---
            LDI r0, 0
            LDI r1, 3
            OUT r0, r1          // PortLba0
            LDI r1, 4
            OUT r0, r1          // PortLba1
            LDI r1, 5
            OUT r0, r1          // PortLba2
            LDI r1, 6
            OUT r0, r1          // PortLba3

            // --- Команда чтения сектора (CmdReadSector = 0x01) ---
            LDI r0, 1
            LDI r1, 1
            OUT r0, r1          // PortCommand

            // --- Ожидание готовности (busy-бит сброшен) ---
            wait_ready:
                LDI r1, 2       // PortStatus
                IN  r0, r1
                LDI r2, 1       // маска busy
                AND r0, r2
                JNZ wait_ready  // если busy != 0, ждём

            // --- Читаем 8 байт размера программы во временный буфер 0x200..0x207 ---
            LDI r10, 0x200     // адрес буфера
            LDI r11, 0         // счётчик
            LDI r12, 8         // количество байт
            read_size_loop:
                LDI r1, 0      // PortData
                IN  r0, r1
                STORE_IND.S8 r0, r10
                INC r10
                INC r11
                SUB r12, r12, 1
                JNZ read_size_loop

            // --- Загружаем размер из буфера в r8 ---
            LDI r10, 0x200     // сбрасываем указатель на буфер
            LOAD_IND.S64 r8, r10

            // --- Копируем программу из порта данных в ОЗУ, начиная с адреса 0 ---
            LDI r10, 0         // адрес назначения в ОЗУ
            copy_loop:
                // проверка: если r8 == 0, завершить
                LDI r12, 0
                SUB r12, r8, r12   // r12 = r8, флаги обновляются
                JZ boot_done

                // читаем байт из порта данных
                LDI r1, 0
                IN  r0, r1
                // записываем байт в ОЗУ
                STORE_IND.S8 r0, r10

                INC r10
                DEC r8
                JMP copy_loop

            boot_done:
                JMP 0

            """;
        const string TestProgramAsm = 
            """
            LDI r0, 65
            PRINT_INT r0
            END
            """;

        const string loggerName = "disk-test";
        var logger = new TestLogger();
        LoggerProvider.RegistryLogger(loggerName, logger);
        LoggerProvider.ChoiceLogger(loggerName);

        SizePortOnDevice sizePOD = SizePortOnDevice.Size16B;

        var emu = new Emulator(SizePort.Size16KB, sizePOD);
        Device? device = null;
        Lock loc = new();
        uint sectorDevice = 4;
        uint idSectorDisk = 0;
        int diskCodeError = 0;
        string? imagePath = null;
        try
        {
            // 1. Создаём устройство с BIOS (пока без программы)
            byte[] biosCode = new AssemblerParser().Assemble(BiosAsm, (ulong)SizePort.Size16KB);
            int deviceId = emu.CreateDevice(biosCode, RamSize.Size16KB, sectorDevice, "BootDevice");
            Assert.InRange(deviceId, 0, int.MaxValue);
            logger.Info($"Device Id = {deviceId}");
            device = emu.GetDevice(deviceId);
            Assert.NotNull(device);

            // 2. Готовим образ диска
            byte[] program = new AssemblerParser().Assemble(TestProgramAsm);
            Span<byte> diskImage = CreateBootableImage(program, sectorCount: 1);

            imagePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".vmg");
            File.WriteAllBytes(imagePath, diskImage);

            // 3. Создаём диск, передавая уже готовый файл
            diskCodeError = emu.CreateDisk(imagePath, idSectorDisk);

            if (diskCodeError != 0)
                _outPut.WriteLine($"Code Error: {diskCodeError}");

            Assert.InRange(diskCodeError,0, int.MaxValue);
            Assert.NotNull(emu.GetDisk(idSectorDisk));
            
            for (ulong i = 0; i < 8; i++)
            {
                var res1 = emu.PortBus.ReadPort(i);
                if (res1.IsSuccess)
                {
                    logger.Info($"1 Порт[{i}] устройство, владелец:{res1.NameDeviceToken}");
                }
                else
                {
                    Assert.True(res1.IsSuccess, $"Port [{i}], {ProcessorHelpers.TryContinueAfterStatus(new(res1.Status, res1.FaultAddress), i, loc)}");
                }
            }

            // 4. Запускаем устройство с BIOS (стартовый адрес = RamSize)
            using var done = new ManualResetEventSlim(false);
            device.LaunchDeviceOnDedicatedThread(device.MaxRamSize, false, 0, false,
                title => title.Log("Запуск", LogLevel.Log),
                null,
                end => { end.Log("Завершено", LogLevel.Log); done.Set(); });

            // 5. Ждём завершения
            bool finished = done.Wait(TimeSpan.FromSeconds(10));
            Assert.True(finished, "Устройство не завершило выполнение за 10 секунд.");

            device.StopDevice(200, 
                log => log.Log("Остановка устройства", LogLevel.Log),
                log => log.Log("Устройство уже остановлено или не запускалось", LogLevel.Log), 
                log => log.Log("Устройство успешно остановлено", LogLevel.Log), 
                log => log.Log("По неизвестной причине устрйоство продолжает работу", LogLevel.Log));


            logger.Info(imagePath);
            Assert.Contains("65", logger.ToString());
        }
        finally
        {
            if (diskCodeError == 0)
                emu.RemoveDisk(idSectorDisk);
            if (imagePath != null && File.Exists(imagePath))
                File.Delete(imagePath);
            device?.Dispose();
            emu.Reset();
            _outPut.WriteLine(logger.ToString());
            LoggerProvider.DeleteLogger(loggerName);
        }
    }

    private static byte[] CreateBootableImage(byte[] program, int sectorCount = 1)
    {
        int sectorSize = 512; // DiskDevice.SectorSize

        Span<byte> image = stackalloc byte[sectorSize * sectorCount];
        // Записываем размер программы в первые 8 байт
        BitConverter.TryWriteBytes(image[..8], (ulong)program.Length);
        // Копируем программу
        program.CopyTo(image[8..]);
        return image.ToArray();
    }


    [Fact]
    public void DiskReadSingleByte()
    {
        const string ReadDiskProgramAsm = @"
LDI r0, 0
LDI r1, 19
OUT r0, r1
LDI r1, 20
OUT r0, r1
LDI r1, 21
OUT r0, r1
LDI r1, 22
OUT r0, r1
LDI r0, 1
LDI r1, 17
OUT r0, r1
wait_ready:
    LDI r1, 18
    IN  r0, r1
    LDI r2, 1
    AND r0, r2
    JNZ wait_ready
    LDI r1, 16
    IN  r0, r1
    PRINT_INT r0
    IN  r0, r1
    PRINT_INT r0
    END
";

    const string loggerName = "disk-single-byte";
        var logger = new TestLogger();
        LoggerProvider.RegistryLogger(loggerName, logger);
        LoggerProvider.ChoiceLogger(loggerName);

        var emu = new Emulator(SizePort.Size16KB, SizePortOnDevice.Size16B);
        Device? device = null;
        uint diskSector = 1;
        int diskCodeError = 0;
        string? imagePath = null;

        try
        {
            // Устройство в секторе 0
            int deviceId = emu.CreateDevice([], RamSize.Size16KB, 0, "ReadDevice");
            Assert.InRange(deviceId, 0, int.MaxValue);
            device = emu.GetDevice(deviceId);
            Assert.NotNull(device);

            // Диск в секторе 1
            Span<byte> diskImage = stackalloc byte[512];
            diskImage[0] = 65;
            diskImage[1] = 66;
            imagePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".vmg");
            File.WriteAllBytes(imagePath, diskImage);

            diskCodeError = emu.CreateDisk(imagePath, diskSector);
            Assert.InRange(diskCodeError, 0, int.MaxValue);

            // Программа чтения байта
            byte[] program = new AssemblerParser().Assemble(ReadDiskProgramAsm);
            device.LoadProgram(program, 0);
            device.InitHeap((ulong)program.Length);

            using var done = new ManualResetEventSlim(false);
            device.LaunchDeviceOnDedicatedThread(0, false, 0, false,
                title => title.Log("Запуск", LogLevel.Log),
                null,
                end => { end.Log("Завершено", LogLevel.Log); done.Set(); });

            Assert.True(done.Wait(TimeSpan.FromSeconds(10)), "Устройство не завершилось.");

            Assert.Contains("65", logger.ToString());
            Assert.Contains("66", logger.ToString());
        }
        finally
        {
            device?.Dispose();
            if (diskCodeError == 0) emu.RemoveDisk(diskSector);
            if (imagePath != null && File.Exists(imagePath)) File.Delete(imagePath);
            emu.Reset();
            _outPut.WriteLine(logger.ToString());
            LoggerProvider.DeleteLogger(loggerName);
        }
    }

}