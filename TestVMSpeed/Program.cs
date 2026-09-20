using Kernel.Common;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using VMApplication;
using VMApplication.Emulator;
using VMApplication.Project;

namespace TestVMSpeed;

internal class Program
{
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


    private static readonly int _runs = 15;
    private static readonly int _runsWarmup = 3;

    private static void Main()
    {
        KernelWarmup.WarmupAll();
        FileService fileService = new();
        using var host = VMHostFactory.CreateDefault(fileService: fileService);

        int deviceId = host.Emulator.CreateDevice(
            bios: [], // биос 
            ramSize: RamSize.Size1KB,
            sector: 0,
            name: "ConsoleVM"
        );

        var cases = new[]
        {
            new BenchCase("Math", text2, Runs: _runs, WarmupRuns: _runsWarmup),
            new BenchCase("Call", text1, Runs: _runs, WarmupRuns: _runsWarmup),
        };

        var sb = new StringBuilder();

        var runners = cases.Select(c => new BenchRunner(host, deviceId, c, fileService)).ToArray();
        foreach (var runner in runners)
        {
            if (!runner.TryPrepare())
            {
                sb.AppendLine($"Test '{runner.Case.Name}': prepare failed");
                continue;
            }

            int warmRuns = runner.Case.WarmupRuns;
            for (int i = 0; i < warmRuns; i++)
            {
                runner.Warmup();
                PrintProgress(runner.Case.Name, "warm run", i + 1, warmRuns);
            }

            int runs = runner.Case.Runs;
            for (int i = 0; i < runs; i++)
            {
                runner.RunOnce();
                PrintProgress(runner.Case.Name, "run", i + 1, runs);
            }

            sb.AppendLine($"Test '{runner.Case.Name}':");
            sb.AppendLine();

            sb.AppendLine("  Elapsed (s) per iteration:");
            AppendIterations(sb, runner.ElapsedSecondsLog, "F4");
            
            sb.AppendLine();
            sb.AppendLine("  MIPS per iteration:");
            AppendIterations(sb, runner.MipsLogs, "F2");

            sb.AppendLine();
            sb.AppendLine("  Elapsed stats (s):");
            AppendStats(sb, StatsResult.From(runner.ElapsedSecondsLog));

            sb.AppendLine();
            sb.AppendLine("  MIPS stats:");
            AppendStats(sb, StatsResult.From(runner.MipsLogs));

            sb.AppendLine();
        }

        Console.Clear();
        var path = AppDomain.CurrentDomain.BaseDirectory + "result_Benchmark_Logs.txt";
        var logRes = sb.ToString();
        //File.WriteAllText(path, logRes);
        Console.WriteLine(logRes);
        Console.WriteLine($"Файл результатов логов сохранен по пути: \"{path}\"");
        Console.ReadLine();

    }
    private static void PrintProgress(string test, string phase, int done, int total)
    {
        if (Console.IsOutputRedirected) return;
        Console.Write($"\r[{test}] {phase} {done}/{total}               ");
    }

    private static void AppendStats(StringBuilder sb, StatsResult s)
    {
        sb.AppendLine($"    mean:   {s.Mean:F4}");
        sb.AppendLine($"    median: {s.Median:F4}");
        sb.AppendLine($"    min:    {s.Min:F4}");
        sb.AppendLine($"    p95:    {s.P95:F4}");
        sb.AppendLine($"    max:    {s.Max:F4}");
        sb.AppendLine($"    stddev: {s.StdDev:F4}");
        sb.AppendLine($"    N:           {s.N}");
    }

    private static void AppendIterations(StringBuilder sb, IReadOnlyList<double> logs, string format)
    {
        sb.AppendLine($"  Iterations:");
        if (logs.Count == 0)
        {
            sb.AppendLine("    <нет данных>");
            return;
        }

        for (int i = 0; i < logs.Count; i++)
        {
            sb.Append("    [")
              .Append((i + 1).ToString("D3"))
              .Append("] ")
              .AppendLine(logs[i].ToString(format, CultureInfo.InvariantCulture));
        }
    }
}


public class FileService : IFileService
{
    private readonly Dictionary<string, string> _files = [];
    public string ProjectPath => "this";

    public string CombinePath(string path1, string path2) => Path.Combine(path1, path2);

    public bool Exist(string fileName) => _files.ContainsKey(fileName);

    public IEnumerable<string> GetSourceFiles() => _files.Keys;

    public string ReadFile(string fileName) => _files[fileName];

    public void SaveFile(string fileName, string content) => _files[fileName] = content;
}


public class BenchRunner(VMHost host, int deviceId, BenchCase benchCase, IFileService fileService)
{
    private readonly VMHost _host = host;
    private readonly int _deviceId = deviceId;
    private readonly BenchCase _case = benchCase;
    private readonly List<double> _mipsLogs = []; 
    private readonly List<double> _elapsedSecondsLogs = [];
    private readonly IFileService _fileService = fileService;
    public LaunchModeDevice? LaunchMode { get; private set; } = null; 
    public BenchCase Case => _case;
    public IReadOnlyList<double> MipsLogs => _mipsLogs;
    public IReadOnlyList<double> ElapsedSecondsLog => _elapsedSecondsLogs;
    public bool IsPrepared { get; private set; } = false;

    public bool TryPrepare()
    {
        IsPrepared = false;
        _fileService.SaveFile($"test.mic", _case.ProgramText);

        var il = _host.Project.CompileToIL(ProjectBuilder.BaseAdressProgram, true);
        var res = _host.Project.Compile(il);
        if (!res.Success)
        {
            Console.WriteLine(res.Errors);
            Console.ReadLine();
            return false;
        }
        var ctx = _host.Emulator.CreateDeviceContext(_deviceId)!;

        var launchModeMath = ctx.TryFastLoadProgram(res.Program, 0, out string? errorMath);
        LaunchMode = launchModeMath;

        if (launchModeMath == null)
        {
            Console.WriteLine($"Ошибка при загрузке программы для метрики блока {_case.Name}");
            Console.WriteLine(errorMath);
            Console.ReadLine();
            ctx?.Dispose();
            return false;
        }
        GC.Collect(2, GCCollectionMode.Forced, true);
        GC.WaitForPendingFinalizers();
        IsPrepared = true;
        return true;
    }

    private static void Metric(ISimulationResult res, List<double> elapsetSecondsLong, List<double> mipsLogs)
    {
        mipsLogs.Add(res.Steps / res.Elapsed.TotalSeconds / 1_000_000.0);
        elapsetSecondsLong.Add(res.Elapsed.TotalSeconds);
    }


    public void RunOnce()
    {
        Launch(_elapsedSecondsLogs, _mipsLogs, LaunchMode);
    }

    public void Warmup()
    {
        Launch([],[], LaunchMode); // warmup не пишем в лог
    }

    public static void Launch(List<double> log, List<double> logTime, LaunchModeDevice? launchMode) => launchMode?.Launch(
                    showTimer: true,
                    onEnd: (ctx, res) =>
                    {
                        if (res != null) Metric(res, logTime, log);
                    });
}

public record BenchCase(
    string Name,
    string ProgramText,
    int Runs,
    int WarmupRuns);