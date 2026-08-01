using Kernel.ProcessorSystem;
using Kernel.RamSystem;

namespace Compiller.ASM;

/// <summary>
/// Энкодер инструкций для процессора.
/// Формат инструкции (32 бита):
///   Bits 0-7:   OpCode
///   Bits 8-12:  Reg1 (целевой/основной регистр)
///   Bits 13-17: Reg2 (второй регистр-источник)
///   Bits 18-19: OpCodeSize (размер данных: 0=S8, 1=S16, 2=S32, 3=S64)
///   Bits 20-31: Зарезервированы
/// </summary>
public static class InstructionEncoder
{
    #region Методы кодирования

    /// <summary>
    /// Кодирует инструкцию формата R (регистр-регистр): ADD, SUB, MOV, AND, OR, XOR
    /// </summary>
    public static uint EncodeR(uint opcode, uint regDst, uint regSrc)
    {
        return opcode | (regDst << 8) | (regSrc << 13);
    }

    /// <summary>
    /// Кодирует инструкцию формата R (регистр-регистр): ADD, SUB, MOV, AND, OR, XOR
    /// </summary>
    public static uint EncodeRS(uint opcode, uint regDst, uint regSrc, uint sizeCode)
    {
        return opcode | (regDst << 8) | (regSrc << 13) | (sizeCode << 18);
    }
    /// <summary>
    /// Кодирует инструкцию формата I (регистр-константа/адрес): LDI, LOAD, STORE
    /// sizeCode - размер операнда (S8, S16, S32, S64)
    /// Для LDI: sizeCode определяет, сколько байт значащие
    /// Для LOAD/STORE: sizeCode определяет размер загружаемых/сохраняемых данных
    /// </summary>
    public static uint EncodeI(uint opcode, uint reg, uint sizeCode)
    {
        return opcode | (reg << 8) | (sizeCode << 18);
    }

    /// <summary>
    /// Кодирует инструкцию LDI (загрузка константы) с автоматическим S64
    /// </summary>
    public static uint EncodeLDI(uint reg)
    {
        return EncodeI((uint)Processor.OpCode.LDI, reg, (uint)Processor.OpCodeSize.S64);
    }

    /// <summary>
    /// Кодирует инструкцию LOAD с указанием размера данных
    /// </summary>
    public static uint EncodeLOAD(uint regDst, uint sizeCode)
    {
        return EncodeI((uint)Processor.OpCode.LOAD, regDst, sizeCode);
    }

    /// <summary>
    /// Кодирует инструкцию STORE с указанием размера данных
    /// </summary>
    public static uint EncodeSTORE(uint regSrc, uint sizeCode)
    {
        return EncodeI((uint)Processor.OpCode.STORE, regSrc, sizeCode);
    }

    /// <summary>
    /// Кодирует инструкцию формата U (один регистр): INC, DEC, NOT, PUSH, POP
    /// </summary>
    public static uint EncodeU(uint opcode, uint reg)
    {
        return opcode | (reg << 8);
    }

    /// <summary>
    /// Кодирует инструкцию перехода (JMP, JZ, JNZ, JG, JL)
    /// Адрес всегда 64-битный (S64)
    /// </summary>
    public static uint EncodeJ(uint opcode)
    {
        return opcode | ((uint)Processor.OpCodeSize.S64 << 18);
    }

    /// <summary>
    /// Кодирует инструкцию CALL (вызов подпрограммы)
    /// </summary>
    public static uint EncodeCALL()
    {
        return EncodeJ((uint)Processor.OpCode.CALL);
    }

    /// <summary>
    /// Кодирует инструкцию RET (возврат из подпрограммы)
    /// </summary>
    public static uint EncodeRET()
    {
        return (uint)Processor.OpCode.RET;  // Без аргументов, без размера
    }

    /// <summary>
    /// Кодирует HALT
    /// </summary>
    public static uint EncodeHALT()
    {
        return (uint)Processor.OpCode.HALT;
    }

    /// <summary>
    /// Кодирует NOP
    /// </summary>
    public static uint EncodeNOP()
    {
        return (uint)Processor.OpCode.NOP;
    }

    #endregion

    #region Вспомогательные методы для работы с буфером

    /// <summary>
    /// Записывает инструкцию и выравнивает поток под 8 байт (для последующего 64-битного данного)
    /// </summary>
    public static void WriteInstruction(BinaryWriter writer, uint instruction)
    {
        writer.Write(instruction);
    }

    /// <summary>
    /// Выравнивает поток до границы 8 байт (для 64-битных операндов)
    /// </summary>
    public static void Align8(BinaryWriter writer)
    {
        long pos = writer.BaseStream.Position;
        long pad = ((pos + 7) & ~7) - pos;
        if (pad > 0)
            writer.Write(new byte[pad]);
    }

    /// <summary>
    /// Записывает инструкцию + 64-битный операнд (с выравниванием)
    /// </summary>
    public static void WriteInstructionWithData64(BinaryWriter writer, uint instruction, ulong data)
    {
        writer.Write(instruction);
        Align8(writer);
        writer.Write(data);
    }

    public static uint EncodeIN(uint regDst, uint regPort) =>
        EncodeR((uint)Processor.OpCode.IN, regDst, regPort);

    public static uint EncodeOUT(uint regSrc, uint regPort) =>
        EncodeR((uint)Processor.OpCode.OUT, regSrc, regPort);

    public static uint EncodeLOAD_IND(uint regDst, uint regAddr, uint sizeCode)
    {
        return EncodeRS((uint)Processor.OpCode.LOAD_IND, regDst, regAddr, sizeCode);
    }

    public static uint EncodeSTORE_IND(uint regSrc, uint regAddr, uint sizeCode)
    {
        return EncodeRS((uint)Processor.OpCode.STORE_IND, regSrc, regAddr, sizeCode);
    }
    #endregion

    #region Декодирование (для отладки)

    /// <summary>
    /// Декодирует инструкцию в читаемый вид (для отладки)
    /// </summary>
    public static string Decode(uint instruction)
    {
        uint opcode = instruction & 0xFF;
        uint reg1 = (instruction >> 8) & 0x1F;
        uint reg2 = (instruction >> 13) & 0x1F;
        uint size = (instruction >> 18) & 0x3;

        string sizeStr = size switch
        {
            (uint)Processor.OpCodeSize.S8 => "S8",
            (uint)Processor.OpCodeSize.S16 => "S16",
            (uint)Processor.OpCodeSize.S32 => "S32",
            (uint)Processor.OpCodeSize.S64 => "S64",
            _ => "???"
        };

        return opcode switch
        {
            (uint)Processor.OpCode.NOP => "NOP",
            (uint)Processor.OpCode.HALT => "HALT",

            (uint)Processor.OpCode.MOV => $"MOV {RegName(reg1)}, {RegName(reg2)}",
            (uint)Processor.OpCode.LOAD => $"LOAD.{sizeStr} {RegName(reg1)}, [data64]",
            (uint)Processor.OpCode.STORE => $"STORE.{sizeStr} [data64], {RegName(reg1)}",
            (uint)Processor.OpCode.LOAD_IND => $"LOAD_IND.{sizeStr} {RegName(reg1)}, {RegName(reg2)}",
            (uint)Processor.OpCode.STORE_IND => $"STORE_IND.{sizeStr} {RegName(reg1)}, {RegName(reg2)}",
            (uint)Processor.OpCode.LDI => $"LDI {RegName(reg1)}, data64",

            (uint)Processor.OpCode.ADD => $"ADD {RegName(reg1)}, {RegName(reg2)}",
            (uint)Processor.OpCode.SUB => $"SUB {RegName(reg1)}, {RegName(reg2)}",
            (uint)Processor.OpCode.INC => $"INC {RegName(reg1)}",
            (uint)Processor.OpCode.DEC => $"DEC {RegName(reg1)}",

            (uint)Processor.OpCode.AND => $"AND {RegName(reg1)}, {RegName(reg2)}",
            (uint)Processor.OpCode.OR => $"OR {RegName(reg1)}, {RegName(reg2)}",
            (uint)Processor.OpCode.XOR => $"XOR {RegName(reg1)}, {RegName(reg2)}",
            (uint)Processor.OpCode.NOT => $"NOT {RegName(reg1)}",

            (uint)Processor.OpCode.JMP => $"JMP data64",
            (uint)Processor.OpCode.JZ => $"JZ data64",
            (uint)Processor.OpCode.JNZ => $"JNZ data64",
            (uint)Processor.OpCode.JG => $"JG data64",
            (uint)Processor.OpCode.JL => $"JL data64",

            (uint)Processor.OpCode.PUSH => $"PUSH {RegName(reg1)}",
            (uint)Processor.OpCode.POP => $"POP {RegName(reg1)}",
            (uint)Processor.OpCode.CALL => $"CALL data64",
            (uint)Processor.OpCode.RET => $"RET",

            _ => $"UNKNOWN 0x{opcode:X2}"
        };
    }

    private static string RegName(uint r) => r switch
    {
        (uint)Processor.RegType.rZ => "rZ",
        (uint)Processor.RegType.r0 => "r0",
        (uint)Processor.RegType.r1 => "r1",
        (uint)Processor.RegType.r2 => "r2",
        (uint)Processor.RegType.r3 => "r3",
        (uint)Processor.RegType.r4 => "r4",
        (uint)Processor.RegType.r5 => "r5",
        (uint)Processor.RegType.r6 => "r6",
        (uint)Processor.RegType.r7 => "r7",
        (uint)Processor.RegType.r8 => "r8",
        (uint)Processor.RegType.r9 => "r9",
        (uint)Processor.RegType.r10 => "r10",
        (uint)Processor.RegType.r11 => "r11",
        (uint)Processor.RegType.r12 => "r12",
        (uint)Processor.RegType.r13 => "r13",
        (uint)Processor.RegType.r14 => "r14",
        (uint)Processor.RegType.r15 => "r15",
        (uint)Processor.RegType.r16 => "r16",
        (uint)Processor.RegType.r17 => "r17",
        (uint)Processor.RegType.r18 => "r18",
        (uint)Processor.RegType.r19 => "r19",
        (uint)Processor.RegType.r20 => "r20",
        (uint)Processor.RegType.r21 => "r21",
        (uint)Processor.RegType.r22 => "r22",
        (uint)Processor.RegType.rCD => "rCD",
        (uint)Processor.RegType.rFL => "rFL",
        (uint)Processor.RegType.rLP => "rLP",
        (uint)Processor.RegType.rCL => "rCL",
        (uint)Processor.RegType.rRT => "rRT",
        (uint)Processor.RegType.rSP => "rSP",
        (uint)Processor.RegType.rHP => "rHP",
        (uint)Processor.RegType.rIP => "rIP",
        _ => $"r?"
    };

    #endregion

    private static byte[] Exe(RamSize size)
    {
        BiosBuilder builder = new(size);
        builder.EmitInstruction64(EncodeLDI((uint)Processor.RegType.r0), 5);
        builder.EmitInstruction64(EncodeLDI((uint)Processor.RegType.r1), 3);
        builder.EmitInstruction(EncodeR((uint)Processor.OpCode.ADD, (uint)Processor.RegType.r0, (uint)Processor.RegType.r1));
        builder.EmitJump(EncodeJ((uint)Processor.OpCode.JG), "halt");
        builder.MarkLabel("halt");
        builder.EmitInstruction(EncodeHALT());
        return builder.Build();
    }
    private static byte[] BuildOsProgram()
    {
        var builder = new BiosBuilder(baseAddress: 0); // начало ОЗУ
        builder.EmitInstruction64(EncodeLDI((uint)Processor.RegType.r0), 10);
        builder.EmitInstruction64(EncodeLDI((uint)Processor.RegType.r1), 20);
        builder.EmitInstruction(EncodeR((uint)Processor.OpCode.ADD, (uint)Processor.RegType.r0, (uint)Processor.RegType.r1));  // r0 = 30
        builder.EmitInstruction(EncodeHALT());
        return builder.Build();
    }
    private static byte[] BuildBiosLoader(RamSize size)
    {
        var builder = new BiosBuilder(baseAddress: size);
        builder.EmitJumpToAbsolute(EncodeJ((uint)Processor.OpCode.JMP), 0x0000); // JMP 0x0000
        builder.EmitInstruction(EncodeHALT());            // никогда не выполнится
        return builder.Build();
    }
    private static byte[] ExampleUsage(RamSize size)
    {

        ulong biosBase = (ulong)size;

        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        // LDI r0, 5
        WriteInstructionWithData64(writer,
            EncodeLDI((uint)Processor.RegType.r0), 5);

        // LDI r1, 3
        WriteInstructionWithData64(writer,
            EncodeLDI((uint)Processor.RegType.r1), 3);

        // ADD r0, r1
        WriteInstruction(writer,
            EncodeR((uint)Processor.OpCode.ADD, (uint)Processor.RegType.r0, (uint)Processor.RegType.r1));

        // JG -> HALT (временно пишем 0)
        WriteInstruction(writer, EncodeJ((uint)Processor.OpCode.JG));
        Align8(writer);
        long addrPos = ms.Position;
        writer.Write(0UL);

        // Запоминаем позицию HALT
        long haltPos = ms.Position;
        WriteInstruction(writer, EncodeHALT());

        // Патчим
        byte[] bios = ms.ToArray();
        ulong haltAddr = biosBase + (ulong)haltPos;
        BitConverter.GetBytes(haltAddr).CopyTo(bios, (int)addrPos);

        return bios;
        
    }
}
