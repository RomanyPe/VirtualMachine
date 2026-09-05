using System.Runtime.CompilerServices;

namespace ThreadingSystem.ThreadMetrics;

public class MetricsCollector
{
    [InlineArray(BufferSize)]
    private struct TimeBuffer
    {
        public const int BufferSize = 1024;
        private TimeSpan _element0; // Меняем тип базового элемента
    }

    private const int BufferMask = TimeBuffer.BufferSize - 1;

    private TimeBuffer _buffer;
    private int _index = 0;

    public TimeSpan[] GetMetricsTime()
    {
        var result = new TimeSpan[TimeBuffer.BufferSize];
        for (int i = 0; i < TimeBuffer.BufferSize; i++)
        {
            result[i] = _buffer[i];
        }
        return result;
    }

    public TimeSpan this[int i] => _buffer[i];
    public TimeSpan Last => _buffer[_index];
    public int Index => _index;
    public void Clear() => _index = 0;

    public void Add(TimeSpan elapsed)
    {
        ref TimeSpan baseRef = ref Unsafe.As<TimeBuffer, TimeSpan>(ref _buffer);
        Unsafe.Add(ref baseRef, _index & BufferMask) = elapsed;

        _index++;
    }
}
