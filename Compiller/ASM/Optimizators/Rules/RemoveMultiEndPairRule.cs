using Kernel.Common;

namespace Compiller.ASM.Optimizators.Rules;

/// <summary>
/// Удаляет дублирующиеся инструкции END в конце программы,
/// оставляя только одну.
/// </summary>
public sealed class RemoveMultiEndPairRule : IPeepholeRule
{
    public bool TryOptimize(List<AsmItem> items, int index, PeepholeLog log, out int newIndex)
    {
        newIndex = index;

        // Текущий элемент должен быть END
        if (index >= items.Count || items[index] is not AsmInstruction current ||
            current.OpCode != OpCode.END)
            return false;

        // Проверяем, есть ли следующий элемент и является ли он тоже END
        if (index + 1 >= items.Count)
            return false;

        if (items[index + 1] is not AsmInstruction next || next.OpCode != OpCode.END)
            return false;

        // Если между ними метка (маловероятно, но для безопасности), не удаляем
        if (items[index] is AsmLabel || items[index + 1] is AsmLabel)
            return false;

        // Удаляем второй END
        items.RemoveAt(index + 1);
        log.AppendLine($"Removed extra END at position {index + 1}");

        // Возвращаем тот же индекс, чтобы проверить, не остались ли ещё END подряд.
        newIndex = index;
        return true;
    }
}