using Kernel.Common;
using VMApplication;
using VMApplication.Emulator;
using VMApplication.Project;

namespace TestVMSpeed;

public class BenchRunner(VMHost host, int deviceId, BenchCase benchCase, IFileService fileService)
{
    private readonly VMHost _host = host;
    private readonly int _deviceId = deviceId;
    private readonly BenchCase _case = benchCase;
    private readonly List<double> _mipsLogs = [];
    private readonly List<double> _elapsedSecondsLogs = [];
    private readonly List<long> _stepsLogs = [];
    private readonly IFileService _fileService = fileService;

    public LaunchModeDevice? LaunchMode { get; private set; }
    public BenchCase Case => _case;
    public IReadOnlyList<double> MipsLogs => _mipsLogs;
    public IReadOnlyList<double> ElapsedSecondsLog => _elapsedSecondsLogs;
    public IReadOnlyList<long> StepsLogs => _stepsLogs;
    public bool IsPrepared { get; private set; } = false;

    /// <summary>Пиковая скорость: максимум выполненных инструкций / минимальное время.</summary>
    public double PeakMips => _elapsedSecondsLogs.Count == 0
        ? 0
        : StepsOnce / Stats.Min(_elapsedSecondsLogs) / 1_000_000.0;

    /// <summary>Число инструкций в одном прогоне (должно быть стабильно).</summary>
    public long StepsOnce => _stepsLogs.Count == 0 ? 0 : _stepsLogs[0];

    /// <summary>Средне-квадратичное число инструкций за прогон.</summary>
    public double StepsVariance => _stepsLogs.Count < 2
        ? 0
        : _stepsLogs.Select(s => (double)s).ToList().ComputeVariance();

    public byte[]? Program { get; private set; }

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
        var ctx = _host.Emulator.GetDeviceContext(_deviceId)!;
        Program = res.Program;
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

    private static void Metric(ISimulationResult res, List<double> elapsed, List<double> mips, List<long> steps)
    {
        mips.Add(res.Steps / res.Elapsed.TotalSeconds / 1_000_000.0);
        elapsed.Add(res.Elapsed.TotalSeconds);
        steps.Add(res.Steps);
    }

    public void RunOnce() => Launch(_mipsLogs, _elapsedSecondsLogs, _stepsLogs, LaunchMode);
    public void Warmup() => Launch([], [], [], LaunchMode);

    public static void Launch(
        List<double> log, List<double> logTime, List<long> logSteps, LaunchModeDevice? launchMode) =>
        launchMode?.Launch(
            showTimer: true,
            onEnd: (ctx, res) =>
            {
                if (res != null) Metric(res, logTime, log, logSteps);
            });
}

file static class ListExtensions
{
    public static double ComputeVariance(this IReadOnlyList<double> v)
    {
        if (v.Count < 2) return 0;
        double avg = v.Average();
        return v.Sum(x => (x - avg) * (x - avg)) / (v.Count - 1);
    }
}