using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadControl;
using ThreadingSystem.ThreadPool;

namespace ThreadingSystem;

internal static class ThreadSchedulerHelper
{

    public static ContextScheduler CreateContextScheduler(int workerCount, SynchronizationContext uiContext, Action<string>? logger)
    {
        ManualResetEventSlimPool innerScheduler = new(workerCount);
        return new ContextScheduler(innerScheduler, uiContext, logger);
    }

    public static int DetermineWorkerCount(int? requestedWorkerCount)
    {
        if (requestedWorkerCount <= 0)
            ThrowArgumentOutOfRangeException(nameof(requestedWorkerCount), "Worker count must be positive.");

        int processorCount = Environment.ProcessorCount;
        int effectiveCount = requestedWorkerCount ?? processorCount;

        if (effectiveCount < processorCount)
            effectiveCount = processorCount;

        effectiveCount &= ~1;

        if (effectiveCount < 2)
            effectiveCount = 2;

        return effectiveCount;
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static SynchronizationContext GenerateNullExp() => throw new ArgumentNullException();

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentOutOfRangeException(string paramName, string message)
        => throw new ArgumentOutOfRangeException(paramName, message);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidOperationException(string message)
        => throw new InvalidOperationException(message);
        
    public static SynchronizationContext GetRequiredSynchronizationContext()
    {
        var context = SynchronizationContext.Current;
        if (context == null)
        {
            ThrowInvalidOperationException("SynchronizationContext is required. Create ThreadScheduler on a thread with a synchronization context (e.g., UI thread).");
        }
        return context;
    }
}