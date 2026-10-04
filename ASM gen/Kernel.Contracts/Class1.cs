using Kernel.Common;
using System.Reflection.Emit;

namespace Kernel.Contracts;

public interface IProcessorFaultPolicy
{
    public static IProcessorFaultPolicy Default => new DefaultProcessorFaultPolicy();
    private class DefaultProcessorFaultPolicy : IProcessorFaultPolicy
    {
        public bool ShouldContinue(in ProcessorFault fault) => fault.Status switch
        {
            BiosStatus.Success => true,
            BiosStatus.NullDeviceOutput => true,
            _ => false
        };
    }

    bool ShouldContinue(in ProcessorFault fault);
}

public interface IDeviceLoggerContext
{
    void Log(string message, LogLevel logLevel = LogLevel.Log);

}

public interface ISimulationResult
{
    TimeSpan Elapsed { get; }
    long Steps { get; }
}

public interface IReadOnlyLogOptimization
{
    TypeOptimization Id { get; }
    string GetLogs();
}


// удалить эту хуйню и заменить на структуры с строками
public enum TypeOptimization
{
    Peephole,
    ASTNodeRemovedBeforeInline,
    ASTNodeInlinedFunc,
    ASTNodeConstPropagate,
    ASTNodeConstFold,
    ASTNodeRemovedAfterInline
}


