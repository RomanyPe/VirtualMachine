using Compiller.ASM.Optimizators;
using Kernel.Common;

namespace ExtensionsVMApplication.Optimizators.IRAsmRules;

public sealed class RemoveNoopMovRule : IPeepholeRule
{
    public bool TryOptimize(List<AsmItem> items, int index, PeepholeLog log, out int newIndex)
    {
        newIndex = index;
        if (items[index] is not AsmInstruction instr)
            return false;

        if (instr.OpCode == OpCode.MOV &&
            instr.FirstReg == instr.SecondReg &&
            instr.Immediate == null && instr.TargetLabel == null &&
            !IsImmediatelyAfterLabel(items, index))
        {
            log.AppendLine(items[index]);

            items.RemoveAt(index);
            newIndex = index - 1; // откатываемся, чтобы проверить предыдущую инструкцию
            return true;
        }

        return false;
    }

    private static bool IsImmediatelyAfterLabel(List<AsmItem> items, int index)
        => index > 0 && items[index - 1] is AsmLabel;
}
