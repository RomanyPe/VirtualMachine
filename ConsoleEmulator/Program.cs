using Kernel.Common;
using VMApplication;
using VMApplication.Emulator;

namespace ConsoleEmulator;

public class Program
{
    private static VMHost? host = null;
    private static byte[]? program = null;
    private static DeviceContext? deviceContext = null;

    private static void Main(string[] args)
    {
        try
        {
            if (TryInitArgs(args))
            {
                KernelWarmup.WarmupAll();
                Run();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            EndEmulator();
            deviceContext?.Dispose();
            host?.Dispose();
        }

    }

    private static bool TryInitArgs(string[] args)
    {
        if (args.Length < 2 || args[0] != "-prog" || string.IsNullOrEmpty(args[1]))
        {
            Console.WriteLine("аргемент '-prog' отсутсвует или данные переданы неверно");
            return false;
        }

        if (File.Exists(args[1]))
        {
            Console.WriteLine($"Файл по пути '{args[1]}' обнаружен");
            program = File.ReadAllBytes(args[1]);
            return true;
        }

        Console.WriteLine($"Файл по пути '{args[1]}' отсутствует");
        return false;
    }

    private static void Run()
    {
        if (program == null)
        {
            Console.WriteLine("Программа отсутсвует");
            return;
        }
        host = VMHostFactory.CreateDefault(
                    portBusSize: SizePort.Size16KB,
                    portsPerDevice: SizePortOnDevice.Size16B
                );

        // 3. Создаём устройство (ВМ) с BIOS (можно без BIOS передать пустой массив)
        int deviceId = host.Emulator.CreateDevice(
            bios: [], // биос 
            ramSize: RamSize.Size1MB, 
            biosSize: RamSize.Size64KB,
            sector: 0,
            name: "ConsoleVM"
        );

        // 4. Загружаем программу в память устройства
        deviceContext = host.Emulator.CreateDeviceContext(deviceId);

        LaunchModeDevice? launchMode = deviceContext!.TryFastLoadProgram(program, 0, out string? error);

        if (launchMode == null)
        {
            Console.WriteLine(error);
            return;
        }
        // 5. Запускаем эмуляцию (в том же потоке или через планировщик)
        launchMode?.Launch(
            onStart: ctx => ctx.Log("Симуляция началась", LogLevel.Log),
            onEnd: (ctx, _) => ctx.Log("Симуляция завершена", LogLevel.Log)
        );
    }
    private static void EndEmulator()
    {
        Console.WriteLine("Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}
