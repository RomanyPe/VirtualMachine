using Kernel.ProcessorSystem;
using System.Text;

namespace Compiller.ASM;

public static class Disassembler
{
    /// <summary>
    /// Метод для декодировани и представления программы Assembler в текстовом виде
    /// </summary>
    /// <param name="program"> Байт код программы </param>
    /// <param name="length"> [OUTPUT] количество строк кода программы </param>
    /// <param name="size"> [OUTPUT] размер программы в byte</param>
    /// <param name="baseAddress"> Начальный адресс чтения программы </param>
    /// <returns> Текстовое представление Assembler</returns>
    public static string Disassemble(byte[] program, out int length, out int size, ulong baseAddress = 0)
    {
        StringBuilder sb = new();
        int pos = 0;
        size = program.Length;
        length = 0;

        // Имитируем программный счетчик (IP) как в CPU
        ulong currentIp = baseAddress;

        while (pos < size)
        {
            // Проверяем, хватает ли байт на саму инструкцию в массиве
            if (pos + 4 > size)
            {
                sb.AppendLine($"{currentIp:X8}: <неполная инструкция, пропущено {size - pos} байт>");
                break;
            }

            uint instruction = BitConverter.ToUInt32(program, pos);
            uint opcode = instruction & 0xFF;

            bool hasData = opcode is ((uint)Processor.OpCode.LDI) or
                           ((uint)Processor.OpCode.LOAD) or
                           ((uint)Processor.OpCode.STORE) or
                           ((uint)Processor.OpCode.CALL) or
                           ((uint)Processor.OpCode.JMP) or
                           ((uint)Processor.OpCode.JZ) or
                           ((uint)Processor.OpCode.JNZ) or
                           ((uint)Processor.OpCode.JG) or
                           ((uint)Processor.OpCode.JL);

            string decoded = InstructionEncoder.Decode(instruction);
            sb.Append($"{currentIp:X8}: {instruction:X8}   {decoded}");

            // Симулируем шаг процессора: прочитали инструкцию, продвинули IP и POS
            ulong nextIp = currentIp + 4;
            int nextPos = pos + 4;

            if (hasData)
            {
                // Выравнивание строго по абсолютному адресу IP, как в CPU
                ulong alignedIp = (nextIp + 7) & ~7UL;
                long pad = (long)(alignedIp - nextIp);

                // Сдвигаем позицию в массиве на величину паддинга
                nextPos += (int)pad;
                nextIp = alignedIp;

                if (nextPos + 8 <= size)
                {
                    ulong data = BitConverter.ToUInt64(program, nextPos);
                    sb.Append($"  data = 0x{data:X16} ({data})");
                    nextPos += 8;
                    nextIp += 8;
                }
                else
                {
                    sb.Append($"  <недостаточно байт для данных>");
                    sb.AppendLine();
                    break;
                }
            }

            currentIp = nextIp;
            pos = nextPos;
            length++;
            sb.AppendLine();
        }

        return sb.ToString();
    }

}
