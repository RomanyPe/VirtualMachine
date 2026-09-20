namespace TestVMSpeed;

public record struct StatsResult(double Mean, double Median, double Min, double Max, double P95, double StdDev, int N)
{
    public static StatsResult From(IReadOnlyList<double> v) => new(
        Mean: v.Average(),
        Median: Stats.Median(v),
        Min: v.Min(),
        Max: v.Max(),
        P95: Stats.Percentile(v, 95),
        StdDev: Stats.StdDev(v),
        N: v.Count);
}
