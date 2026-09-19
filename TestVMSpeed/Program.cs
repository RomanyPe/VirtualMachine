using Kernel.Common;
using System.Text;
using VMApplication;
using VMApplication.Emulator;
using VMApplication.Project;

const string text1 =
    """
    int main() {

        asm{
    LDI r0, 0;
    LDI r1, 0;
    LDI r6, 10000000;   // константа 10000000
    LDI r7, 1;         // константа 1

    loop:
        CALL Sum;       // rZ = sum + i  (Sum: ADD rZ, r0)
        ADD r1, r7;     // i++
        MOV r2, r1;     // копия i для сравнения
        SUB r2, r6;     // r1 = i - 10000000
        JL loop;        // если i < 10000000, продолжаем

    END;
    Sum:
        ADD r0, r1;
        RET;
        }
        return 0;
    }
    """;


const string text2 =
    """
    int main() {

        asm{
    LDI r0, 0;
    LDI r1, 0;
    LDI r6, 10000000;   // константа 10000000
    LDI r7, 1;         // константа 1

    loop:
        ADD r0, r1;       // rZ = sum + i  (Sum: ADD rZ, r0)
        ADD r1, r7;     // i++
        MOV r2, r1;     // копия i для сравнения
        SUB r2, r6;     // r1 = i - 10000000
        JL loop;        // если i < 10000000, продолжаем

    END;
        }
        return 0;
    }
    """;



File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "test.mic", text1);
List<double> logsWithCall = [];
List<double> logsOnlyMath = [];

KernelWarmup.WarmupAll();

using var host = VMHostFactory.CreateDefault();

CompilationToILResult il = host.Project.CompileToIL(ProjectBuilder.BaseAdressProgram, true);
var res = host.Project.Compile(il);
if (!res.Success)
{
    Console.WriteLine(res.Errors);
    Console.ReadLine();
    return;
}

int deviceId = host.Emulator.CreateDevice(
    bios: [], // биос 
    ramSize: RamSize.Size1KB,
    sector: 0,
    name: "ConsoleVM"
);

Console.WriteLine("Прогрев JIT");
var flowControl = Run([], host, deviceId, 2);
if (flowControl == null) return;

Console.WriteLine();
Console.WriteLine("Запущена проверка скорости на вызовах с Call");
flowControl = Run(logsWithCall, host, deviceId, 5);
if (flowControl == null) return;

File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "test.mic", text2);

Console.WriteLine();
Console.WriteLine("Запущена проверка скорости на только математику");
flowControl = Run(logsOnlyMath, host, deviceId, 5);
if (flowControl == null) return;

StringBuilder sb = new();

double l = Sum(logsOnlyMath);
sb.AppendLine("Test only Math");
sb.AppendLine(CreateLog(l));
sb.AppendLine();
l = Sum(logsWithCall);
sb.AppendLine("Test with Call");
sb.AppendLine(CreateLog(l));
var path = AppDomain.CurrentDomain.BaseDirectory + "resLogs.txt";
var logRes = sb.ToString();
File.WriteAllText(path, logRes);
Console.WriteLine(logRes);
Console.WriteLine();
Console.WriteLine($"Файл результатов логов сохранен по пути: \"{path}\"");

static void Launch(List<double> log, LaunchModeDevice? launchMode) =>
    launchMode?.Launch(
        showTimer: true,
        onStart: ctx => ctx.Log("Симуляция запустилась \n", LogLevel.Log),
        onEnd: (ctx, res) =>
        {
            ctx.Log("Симуляция завершена \n", LogLevel.Log);
            if (res != null) MetricTime(res, log);
        });

static void MetricTime(ISimulationResult res, List<double> log)
{
    if (res.Elapsed.TotalMilliseconds <= 0)
    {
        log.Add(0); // или не добавлять вовсе
        return;
    }
    double mips = res.Steps / res.Elapsed.TotalSeconds / 1_000_000.0;
    log.Add(mips);
}

static double Sum(List<double> logs)
{
    if (logs.Count <= 0)
    {
        Console.WriteLine("Метрика вернула 0");
        return 0;
    }
    double log = logs.Sum();
    return log / logs.Count;
}

static string CreateLog(double mips)
{
    return $"MIPS: {mips}";
}

static DeviceContext? Run(List<double> logsOnlyMath, VMHost host, int deviceId, int count = 5)
{
    var il = host.Project.CompileToIL(ProjectBuilder.BaseAdressProgram, true);
    var res = host.Project.Compile(il);
    if (!res.Success)
    {
        Console.WriteLine(res.Errors);
        Console.ReadLine();
        return null;
    }
    DeviceContext deviceContext = host.Emulator.CreateDeviceContext(deviceId)!;

    var launchModeMath = deviceContext.TryFastLoadProgram(res.Program, 0, out string? errorMath);

    if (launchModeMath == null)
    {
        Console.WriteLine("Ошибка при загрузке программы для метрики блока математики");
        Console.WriteLine(errorMath);
        Console.ReadLine();
        deviceContext?.Dispose();
        return null;
    }

    for (int i = 1; i < count + 1; i++)
    {
        Console.WriteLine($"Запуск номер {i}");
        Launch(logsOnlyMath, launchModeMath);
    }
    return deviceContext;
}