using Kernel.RamSystem;
using static Kernel.ProcessorSystem.Processor;

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
        return EncodeI((uint)OpCode.LDI, reg, (uint)OpCodeSize.S64);
    }

    /// <summary>
    /// Кодирует инструкцию LOAD с указанием размера данных
    /// </summary>
    public static uint EncodeLOAD(uint regDst, uint sizeCode)
    {
        return EncodeI((uint)OpCode.LOAD, regDst, sizeCode);
    }

    /// <summary>
    /// Кодирует инструкцию STORE с указанием размера данных
    /// </summary>
    public static uint EncodeSTORE(uint regSrc, uint sizeCode)
    {
        return EncodeI((uint)OpCode.STORE, regSrc, sizeCode);
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
        return opcode | ((uint)OpCodeSize.S64 << 18);
    }

    /// <summary>
    /// Кодирует инструкцию CALL (вызов подпрограммы)
    /// </summary>
    public static uint EncodeCALL()
    {
        return EncodeJ((uint)OpCode.CALL);
    }

    /// <summary>
    /// Кодирует инструкцию RET (возврат из подпрограммы)
    /// </summary>
    public static uint EncodeRET()
    {
        return (uint)OpCode.RET;  // Без аргументов, без размера
    }

    /// <summary>
    /// Кодирует HALT
    /// </summary>
    public static uint EncodeHALT()
    {
        return (uint)OpCode.HALT;
    }

    /// <summary>
    /// Кодирует NOP
    /// </summary>
    public static uint EncodeNOP()
    {
        return (uint)OpCode.NOP;
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

    public static uint EncodeIN(uint regDst, uint regPort) => EncodeR((uint)OpCode.IN, regDst, regPort);

    public static uint EncodeOUT(uint regSrc, uint regPort) => EncodeR((uint)OpCode.OUT, regSrc, regPort);

    public static uint EncodeLOAD_IND(uint regDst, uint regAddr, uint sizeCode)
    {
        return EncodeRS((uint)OpCode.LOAD_IND, regDst, regAddr, sizeCode);
    }

    public static uint EncodeSTORE_IND(uint regSrc, uint regAddr, uint sizeCode)
    {
        return EncodeRS((uint)OpCode.STORE_IND, regSrc, regAddr, sizeCode);
    }

    public static uint EncodeSHR(uint regDst, uint regSrc) => EncodeR((uint)OpCode.SHR, regDst, regSrc);
    public static uint EncodeMULT_INT(uint regDst, uint regSrc) => EncodeR((uint)OpCode.MULT_INT, regDst, regSrc);
    #endregion

    #region Декодирование (для отладки)

    /// <summary>
    /// Декодирует инструкцию в читаемый вид (для отладки)
    /// </summary>
    public static string Decode(uint instruction)
    {
        OpCode opcode = (OpCode)(instruction & 0xFF);
        RegType reg1 = (RegType)((instruction >> 8) & 0x1F);
        RegType reg2 = (RegType)((instruction >> 13) & 0x1F);
        uint size = (instruction >> 18) & 0x3;

        string sizeStr = size switch
        {
            (uint)OpCodeSize.S8 => "S8",
            (uint)OpCodeSize.S16 => "S16",
            (uint)OpCodeSize.S32 => "S32",
            (uint)OpCodeSize.S64 => "S64",
            _ => "???"
        };

        return opcode switch
        {
            OpCode.NOP => "NOP",
            OpCode.HALT => "HALT",
            OpCode.PRINT => $"PRINT {reg1.RegName()}", // ТОЛЬКО ДЛЯ ОТЛАДКИ

            OpCode.MOV => $"MOV {reg1.RegName()}, {reg2.RegName()}",
            OpCode.LOAD => $"LOAD.{sizeStr} {reg1.RegName()}, [data64]",
            OpCode.STORE => $"STORE.{sizeStr} [data64], {reg1.RegName()}",
            OpCode.LOAD_IND => $"LOAD_IND.{sizeStr} {reg1.RegName()}, {reg2.RegName()}",
            OpCode.STORE_IND => $"STORE_IND.{sizeStr} {reg1.RegName()}, {reg2.RegName()}",
            OpCode.LDI => $"LDI {reg1.RegName()}, data64",

            OpCode.ADD => $"ADD {reg1.RegName()}, {reg2.RegName()}",
            OpCode.SUB => $"SUB {reg1.RegName()}, {reg2.RegName()}",
            OpCode.INC => $"INC {reg1.RegName()}",
            OpCode.DEC => $"DEC {reg1.RegName()}",

            OpCode.AND => $"AND {reg1.RegName()}, {reg2.RegName()}",
            OpCode.OR => $"OR {reg1.RegName()}, {reg2.RegName()}",
            OpCode.XOR => $"XOR {reg1.RegName()}, {reg2.RegName()}",
            OpCode.NOT => $"NOT {reg1.RegName()}",

            OpCode.JMP => $"JMP data64",
            OpCode.JZ => $"JZ data64",
            OpCode.JNZ => $"JNZ data64",
            OpCode.JG => $"JG data64",
            OpCode.JL => $"JL data64",

            OpCode.PUSH => $"PUSH {reg1.RegName()}",
            OpCode.POP => $"POP {reg1.RegName()}",
            OpCode.CALL => $"CALL data64",
            OpCode.RET => $"RET",

            OpCode.IN => $"IN {reg1.RegName()}, {reg2.RegName()}",
            OpCode.OUT => $"OUT {reg1.RegName()}, {reg2.RegName()}",

            OpCode.PRINT_INT => $"PRINT_INT {reg1.RegName()}", // ТОЛЬКО ДЛЯ ОТЛАДКИ
            OpCode.ALLOC => $"ALLOC {reg1.RegName()}",
            OpCode.INT => $"INT {reg1.RegName()}",
            OpCode.IRET => "IRET",

            _ => $"UNKNOWN 0x{opcode:X2}"
        };
    }

    private static string RegName(this RegType r) => r switch
    {
        RegType.rZ => "rZ",
        RegType.r0 => "r0",
        RegType.r1 => "r1",
        RegType.r2 => "r2",
        RegType.r3 => "r3",
        RegType.r4 => "r4",
        RegType.r5 => "r5",
        RegType.r6 => "r6",
        RegType.r7 => "r7",
        RegType.r8 => "r8",
        RegType.r9 => "r9",
        RegType.r10 => "r10",
        RegType.r11 => "r11",
        RegType.r12 => "r12",
        RegType.r13 => "r13",
        RegType.r14 => "r14",
        RegType.r15 => "r15",
        RegType.r16 => "r16",
        RegType.r17 => "r17",
        RegType.r18 => "r18",
        RegType.r19 => "r19",
        RegType.r20 => "r20",
        RegType.r21 => "r21",
        RegType.rTB => "rTB",
        RegType.rCD => "rCD",
        RegType.rFL => "rFL",
        RegType.rLP => "rLP",
        RegType.rCL => "rCL",
        RegType.rRT => "rRT",
        RegType.rSP => "rSP",
        RegType.rHP => "rHP",
        RegType.rIP => "rIP",
        _ => $"r?"
    };

    #endregion

}
