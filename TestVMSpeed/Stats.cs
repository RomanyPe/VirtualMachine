namespace TestVMSpeed;

public static class Stats
{
    public static double Mean(IReadOnlyList<double> v) => v.Count == 0 ? 0 : v.Average();
    public static double StdDev(IReadOnlyList<double> v)
    {
        if (v.Count < 2) return 0;
        double avg = v.Average();
        double sum = v.Sum(x => (x - avg) * (x - avg));
        return Math.Sqrt(sum / (v.Count - 1));
    }

    public static double Percentile(IReadOnlyList<double> v, double p)
    {
        if (v.Count == 0) return 0;
        var s = v.OrderBy(x => x).ToArray();
        double rank = (p / 100.0) * (s.Length - 1);
        int lo = (int)Math.Floor(rank);
        int hi = (int)Math.Ceiling(rank);
        if (lo == hi) return s[lo];
        return s[lo] + (s[hi] - s[lo]) * (rank - lo);
    }
    public static double Median(IReadOnlyList<double> values)
    {
        if (values.Count == 0) return 0;
        var sorted = values.OrderBy(x => x).ToArray();
        int n = sorted.Length;
        if (n % 2 == 1) return sorted[n / 2];
        return (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
    }
    public static double Min(IReadOnlyList<double> v) => v.Count == 0 ? 0 : v.Min();
    public static double Max(IReadOnlyList<double> v) => v.Count == 0 ? 0 : v.Max();
}