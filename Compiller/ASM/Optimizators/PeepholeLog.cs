using Kernel.Common;
using System.Text;

namespace Compiller.ASM.Optimizators;

public class PeepholeLog
{
    private readonly StringBuilder _log = new();

    public static string DeleteText { get; set; } = "Deleted: ";

    public void Append(string text) => _log.Append(text);

    public void Append(AsmItem item) => _log.Append(FormatAsmItem(item));

    public void AppendLine(AsmItem item)
    {
        _log.AppendLine(FormatAsmItem(item));
    }
    public void AppendLine(string item)
    {
        _log.AppendLine(item);
    }
    private static string FormatAsmItem(AsmItem item)
    {
        if (item is AsmLabel label)
            return $"{label.Name}:";

        if (item is AsmInstruction inst)
        {
            var sb = new StringBuilder();
            sb.Append(DeleteText);
            string opName = inst.OpCode.OpName;
            sb.Append(opName);

            // Суффикс размера для инструкций работы с памятью
            if (inst.OpCode is OpCode.LOAD or OpCode.STORE or OpCode.LOAD_IND or OpCode.STORE_IND)
                sb.Append('.').Append(inst.Size.SizeName);

            sb.Append(' ');

            switch (inst.OpCode)
            {
                // Без операндов
                case OpCode.NOP:
                case OpCode.END:
                case OpCode.RET:
                case OpCode.HALT:
                case OpCode.WAKE:
                case OpCode.ALLOC:
                case OpCode.IRET:
                    return sb.ToString().TrimEnd();

                // Один регистр
                case OpCode.PRINT:
                case OpCode.INC:
                case OpCode.DEC:
                case OpCode.NOT:
                case OpCode.PUSH:
                case OpCode.POP:
                case OpCode.PRINT_INT:
                case OpCode.WAKE_INT:
                case OpCode.INT:
                    return sb.Append(inst.FirstReg).ToString();

                // Два регистра (обычные)
                case OpCode.MOV:
                case OpCode.ADD:
                case OpCode.SUB:
                case OpCode.AND:
                case OpCode.OR:
                case OpCode.XOR:
                case OpCode.IN:
                case OpCode.OUT:
                case OpCode.SHR:
                case OpCode.DIV:
                case OpCode.MULT_INT:
                    return sb.Append(inst.FirstReg).Append(", ").Append(inst.SecondReg).ToString();

                // Два регистра (с размером)
                case OpCode.LOAD_IND:
                case OpCode.STORE_IND:
                    return sb.Append(inst.FirstReg).Append(", ").Append(inst.SecondReg).ToString();

                // LDI: регистр, immediate/метка
                case OpCode.LDI:
                    sb.Append(inst.FirstReg).Append(", ");
                    AppendImmediateOrLabel(sb, inst);
                    return sb.ToString();

                // LOAD/STORE: регистр, адрес (immediate/метка)
                case OpCode.LOAD:
                case OpCode.STORE:
                    sb.Append(inst.FirstReg).Append(", ");
                    AppendImmediateOrLabel(sb, inst);
                    return sb.ToString();

                // Переходы и вызовы
                case OpCode.JMP:
                case OpCode.JZ:
                case OpCode.JNZ:
                case OpCode.JG:
                case OpCode.JL:
                case OpCode.CALL:
                    if (inst.TargetLabel != null)
                        return sb.Append(inst.TargetLabel).ToString();
                    if (inst.Immediate.HasValue)
                        return sb.Append("0x").Append(inst.Immediate.Value.ToString("X")).ToString();
                    return sb.Append("???").ToString();

                default:
                    return "???";
            }
        }
        return "???";
    }

    private static void AppendImmediateOrLabel(StringBuilder sb, AsmInstruction inst)
    {
        if (inst.TargetLabel != null)
            sb.Append(inst.TargetLabel);
        else if (inst.Immediate.HasValue)
            sb.Append("0x").Append(inst.Immediate.Value.ToString("X"));
        else
            sb.Append("???");
    }
    public override string ToString() => _log.ToString();
}

