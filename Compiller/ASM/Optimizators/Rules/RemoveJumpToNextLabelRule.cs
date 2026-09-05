using Kernel.Common;
using System.Text;

namespace Compiller.ASM.Optimizators.Rules;

public sealed class RemoveJumpToNextLabelRule : IPeepholeRule
{
    public bool TryOptimize(List<AsmItem> items, int index, PeepholeLog log, out int newIndex)
    {
        newIndex = index;
        if (items[index] is not AsmInstruction instr)
            return false;

        if (instr.OpCode == OpCode.JMP &&
            instr.TargetLabel != null &&
            index + 1 < items.Count &&
            items[index + 1] is AsmLabel nextLabel &&
            nextLabel.Name == instr.TargetLabel)
        {
            log.AppendLine(items[index]);

            items.RemoveAt(index);
            newIndex = index - 1;
            return true;
        }

        return false;
    }
}
