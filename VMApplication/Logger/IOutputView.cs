using Kernel.Common;

namespace VMApplication.Logger;

public interface IOutputView
{
    void AppendLine(string message, LogLevel level = LogLevel.Log);    // LogLevel = Info, Warning, Error
    void Clear();
}

public sealed class NullOutputView : IOutputView
{
    public static readonly NullOutputView Instance = new();
    public void AppendLine(string message, LogLevel level = LogLevel.Log) { }
    public void Clear() { }
}

public class ProcessorFaultPolicyBase : IProcessorFaultPolicy
{
    private static void LogFault(in ProcessorFault fault, IOutputView output)
    {
        if (output is null) return;
        output.AppendLine(FormatFault(fault), GetLogLevel(fault.Status));
    }

    private static LogLevel GetLogLevel(BiosStatus status) => status switch
    {
        BiosStatus.Success => LogLevel.Log,
        BiosStatus.EndProgramm => LogLevel.Log,
        BiosStatus.InfinityLoopWarning => LogLevel.Warning,
        BiosStatus.NullDeviceOutput => LogLevel.Warning,
        _ => LogLevel.Error,
    };

    private static string FormatFault(in ProcessorFault fault)
    {
        // Build a detailed description: short reason + possible cause/recommendation
        (string reason, string hint) = fault.Status switch
        {
            BiosStatus.DivOnZero => (
                "division by zero",
                "the divisor is 0 — check the operand of the DIV instruction; the register or memory location may be uninitialized"),

            BiosStatus.SegmentationFault => (
                "memory segmentation violation",
                "access went beyond the allowed segment — check the segment base address and offset; a pointer overflow is possible"),

            BiosStatus.AlignmentFault => (
                "memory access alignment violation",
                "the address is not aligned to the required boundary (e.g. 2/4/8 bytes) — check how the address is formed for instructions that require alignment"),

            BiosStatus.ReadViolation => (
                "memory read permission violation",
                "attempted to read from a protected or inaccessible region — check page/segment flags and the correctness of the address"),

            BiosStatus.NotImplementedOpCode => (
                "unimplemented opcode",
                "the CPU encountered an opcode that is not yet implemented in this BIOS version — check the opcode table and emulator version"),

            BiosStatus.NotSupportedOpCode => (
                "unsupported opcode",
                "the opcode is not supported on this CPU configuration — check the CPU operating mode and available extensions"),

            BiosStatus.NullDeviceInput => (
                "input from a non-existent/null device",
                "the program accessed an input port that is not connected — check device initialization and the port number"),

            BiosStatus.NullDeviceOutput => (
                "output to a non-existent/null device",
                "the program accessed an output port that is not connected — check device initialization and the port number"),

            BiosStatus.InfinityLoopWarning => (
                "infinite loop warning",
                "a loop without state changes was detected — an exit condition is probably missing, or a counter is not being updated"),

            BiosStatus.EndProgramm => (
                "program termination",
                "the CPU reached the end of the program or an HLT/RET instruction with no target — this is not an error, but a termination signal"),

            _ => (
                "unknown CPU error",
                "the status was not recognized — check that BiosStatus is up to date and review the emulator logic"),
        };

        // Details for specific statuses (addresses, opcodes, registers, etc.)
        string details = fault.Status switch
        {
            BiosStatus.DivOnZero =>
                $"IP=0x{fault.InstructionPointer:X}",

            BiosStatus.SegmentationFault or
            BiosStatus.AlignmentFault or
            BiosStatus.ReadViolation =>
                $"address=0x{fault.FaultAddress:X}, IP=0x{fault.InstructionPointer:X}",

            BiosStatus.NotImplementedOpCode =>
                $"OpCode=0x{fault.OpCode:X}, data=0x{fault.FaultAddress:X}, IP=0x{fault.InstructionPointer:X}",

            BiosStatus.NotSupportedOpCode =>
                $"OpCode=0x{fault.OpCode:X}, IP=0x{fault.InstructionPointer:X}",

            BiosStatus.NullDeviceInput =>
                $"port=0x{fault.FaultAddress:X}, IP=0x{fault.InstructionPointer:X}",

            BiosStatus.NullDeviceOutput =>
                $"port=0x{fault.FaultAddress:X}, IP=0x{fault.InstructionPointer:X}",

            BiosStatus.InfinityLoopWarning or
            BiosStatus.EndProgramm =>
                $"IP=0x{fault.InstructionPointer:X}",

            _ =>
                $"data=0x{fault.FaultAddress:X}, OpCode=0x{fault.OpCode:X}, IP=0x{fault.InstructionPointer:X}",
        };

        return $"CPU fault [{fault.Status}]: {reason}. {hint}. Details: {details}";
    }

    public bool ShouldContinue(in ProcessorFault fault)
    {
        throw new NotImplementedException();
    }
}