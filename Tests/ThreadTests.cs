using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Diagnostics;
using System.Text;
using ThreadingSystem;
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadControl;
using Xunit.Abstractions;

namespace Tests;


public class ThreadTests(ITestOutputHelper output)
{

    private class CpuLoadTask(long iterations) : ITask
    {
        private readonly long _iterations = iterations;
        public long Result; // чтобы компилятор не удалил цикл

        public TaskPriority Priority => TaskPriority.High;
        public CancellationToken Token => new();

        public void Execute()
        {
            long sum = 0;
            for (long i = 0; i < _iterations; i++)
            {
                // Несложные арифметические операции
                sum += i * i ^ (i >> 3);
            }
            Result = sum;
        }
    }

    private class HeavyMathTask(int iterations) : ITask
    {
        private readonly int _iterations = iterations;
        public double Result;

        public TaskPriority Priority => TaskPriority.High;
        public CancellationToken Token => new();

        public void Execute()
        {
            double accumulator = 0.0;
            for (int i = 1; i <= _iterations; i++)
            {
                accumulator += Math.Sqrt(i) * Math.Log(i + 1) / (i % 7 + 1);
            }
            Result = accumulator;
        }
    }

    private const int count = 1000;
    private readonly ITestOutputHelper _outPut = output;

    [Fact]
    public void Test()
    {
        ThreadScheduler? scheduler = null;

        try
        {
            scheduler = new(2, logger: Log);

            HeavyMathTask? task = new(100000000);
            task.Execute();
            var res = task.Result;
            CpuLoadTask? task1 = new(10000000);
            task1.Execute();
            var res1 = task1.Result;
            {
                using ThreadHandle hundle1 = scheduler.Schedule(task);
                using ThreadHandle hundle2 = scheduler.Schedule(task1);

                Assert.True(hundle1.Wait(TimeSpan.FromSeconds(100)), $"Первая задача не выполнилась за 100 секунд");
                Assert.True(res == task.Result);

                Assert.True(hundle2.Wait(TimeSpan.FromSeconds(100)), $"Вторая задача не выполнилась за 100 секунд");
                Assert.True(res1 == task1.Result);
            }
            var times1 = scheduler.GetThreadMetric(0);
            var times2 = scheduler.GetThreadMetric(1);

            StringBuilder str = new();
            var length1 = times1.Index;
            for (int i = 0; i < length1; i++)
            {
                str.AppendLine($"id 0 = {times1[i].Milliseconds} мс");
            }
            var length2 = times2.Index;
            for (int i = 0; i < length2; i++)
            {
                str.AppendLine($"id 1 = {times2[i].Milliseconds} мс");
            }

            _outPut.WriteLine(str.ToString());
        }
        finally
        {
            scheduler?.Stop();

        }
    }

    private void Log(string? log)
    {
        _outPut.WriteLine(log);
    }


}


public class Test(ITestOutputHelper outPut)
{
    private readonly ITestOutputHelper _outPut = outPut;

    private class HeavyMathTask(int iterations) : ITask
    {
        private readonly int _iterations = iterations;
        private double _result;
        public double Result => _result;
        public TaskPriority Priority => TaskPriority.High;
        public CancellationToken Token => new();

        public void Execute()
        {
            double accumulator = 0.0;
            for (int i = 1; i <= _iterations; i++)
            {
                accumulator += Math.Sqrt(i) * Math.Log(i + 1) / (i % 7 + 1);
            }
            _result = accumulator;
        }
    }

    // Задача с целочисленными битовыми операциями
    private class IntegerBitTask(long iterations) : ITask
    {
        private readonly long _iterations = iterations;
        private long _result;
        public long Result => _result;
        public TaskPriority Priority => TaskPriority.High;
        public CancellationToken Token => new();

        public void Execute()
        {
            long sum = 0;
            for (long i = 0; i < _iterations; i++)
            {
                sum += i * i ^ (i >> 3);
            }
            _result = sum;
        }
    }

    // Задача с обработкой массива (нагрузка на память и кэш)
    private class ArrayProcessingTask(int size, int passes = 5) : ITask
    {
        private readonly int _size = size;
        private readonly int _passes = passes;
        private int[] _array = null!;
        private long _checksum;
        public long Checksum => _checksum;
        public TaskPriority Priority => TaskPriority.High;
        public CancellationToken Token => new();

        public void Execute()
        {
            _array = new int[_size];
            var rng = new Random(12345); // фиксированное зерно для детерминизма
            for (int i = 0; i < _array.Length; i++)
                _array[i] = rng.Next();

            long sum = 0;
            for (int pass = 0; pass < _passes; pass++)
            {
                for (int i = 0; i < _array.Length; i++)
                {
                    _array[i] = (_array[i] * 31) ^ (_array[i] >> 7);
                    sum += _array[i];
                }
            }
            _checksum = sum;
        }
    }

    // Задача с вычислением чисел Фибоначчи (рекурсивно, но с мемоизацией для увеличения нагрузки)
    private class FibonacciTask(int n) : ITask
    {
        private readonly int _n = n;
        private long _result;
        public long Result => _result;
        public TaskPriority Priority => TaskPriority.High;
        public CancellationToken Token => new();

        public void Execute()
        {
            _result = Fib(_n);
        }

        private static long Fib(int n)
        {
            if (n <= 1) return n;
            long a = 0, b = 1;
            for (int i = 2; i <= n; i++)
            {
                long temp = a + b;
                a = b;
                b = temp;
            }
            return b;
        }
    }
    [Fact]
    public void TestMultipleCpuBoundTasks()
    {
        ThreadScheduler? scheduler = null;

        try
        {
            // Создаём задачи с разной нагрузкой
            var tasks = new ITask[]
            {
                new HeavyMathTask(50_000_000),          // ~200-300 мс
                new IntegerBitTask(20_000_000),         // ~150-250 мс
                new ArrayProcessingTask(1_000_000, 3),  // ~300-500 мс
                new FibonacciTask(10_000_000)           // ~100-200 мс
            };

            // Эталонные результаты (последовательное выполнение)
            var expectedResults = new object?[tasks.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i].Execute();
                expectedResults[i] = tasks[i] switch
                {
                    HeavyMathTask t => t.Result,
                    IntegerBitTask t => t.Result,
                    ArrayProcessingTask t => t.Checksum,
                    FibonacciTask t => t.Result,
                    _ => null
                };
            }

            scheduler = new ThreadScheduler( logger: Log);

            var handles = new ThreadHandle[tasks.Length];
            for (int i = 0; i < tasks.Length; i++)
                handles[i] = scheduler.Schedule(tasks[i]);

            for (int i = 0; i < handles.Length; i++)
            {
                Assert.True(handles[i].Wait(TimeSpan.FromSeconds(120)),
                    $"Задача {i} не выполнилась за 120 секунд");
            }

            // Сравниваем результаты
            for (int i = 0; i < tasks.Length; i++)
            {
                object? actual = tasks[i] switch
                {
                    HeavyMathTask t => t.Result,
                    IntegerBitTask t => t.Result,
                    ArrayProcessingTask t => t.Checksum,
                    FibonacciTask t => t.Result,
                    _ => null
                };

                // Для double можно указать точность (количество знаков после запятой)
                if (actual is double d1 && expectedResults[i] is double d2)
                    Assert.Equal(d2, d1, 5); // 5 знаков точности
                else
                    Assert.Equal(expectedResults[i], actual);
            }

            foreach (var h in handles)
                h.Dispose();

            var output = new StringBuilder();
            int threadCount = scheduler.ThreadCount;
            for (int threadId = 0; threadId < threadCount; threadId++)
            {
                var times = scheduler.GetThreadMetric(threadId);
                for (int i = 0; i < times.Index; i++)
                    output.AppendLine($"Поток {threadId}, задача {i}: {times[i].Milliseconds} мс");
            }

            _outPut.WriteLine(output.ToString());
        }
        finally
        {
            scheduler?.Stop();
        }
    }
    private void Log(string? log)
    {
        _outPut.WriteLine(log);
    }
}


