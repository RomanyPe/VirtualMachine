namespace Kernel.Common;

public interface IReadOnlyLogOptimization
{
    TypeOptimization Id { get; }
    string GetLogs();
}

public enum TypeOptimization
{
    Peephole,
    ASTNodeRemovedBeforeInline,
    ASTNodeInlinedFunc,
    ASTNodeConstPropagate,
    ASTNodeConstFold,
    ASTNodeRemovedAfterInline
}


public readonly record struct ProcessorFault(
    BiosStatus Status,
    ulong FaultAddress,
    ulong InstructionPointer,
    OpCode OpCode);

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