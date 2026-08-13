using Kernel.Common;
using Kernel.ControllersData;
using Kernel.RamSystem;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kernel.ProcessorSystem;

public class Processor(MemoryBus ram, NameDeviceToken nameDeviceToken, PortBus portBus, Lock regLock, ReadOnlySpan<char> name)
{
    [InlineArray(CountReg)]
    public struct ArrayRegisters
    {
        public ulong _element0;
    }
    const int CountReg = 32;
    private const ulong NegativeMask = 0x8000000000000000;

    

    private ArrayRegisters _registers;
    private readonly ConcurrentStack<BiosStatus> _statusFromSimulation = [];
    private NameDeviceToken _nameDevice = nameDeviceToken.CreateChild(name);
    private MemoryBus _ram = ram;
    private PortBus _portBus = portBus;
    private Lock _regLock = regLock;
    private volatile bool _isRunning = false;

    public bool IsRunning => _isRunning;
    private ulong IsZero => GetRegValue(RegType.rFL) & 1UL;
    private ulong IsNegative => GetRegValue(RegType.rFL) & 2UL;

    public bool TryInitInPool(MemoryBus ram, NameDeviceToken nameDeviceToken, PortBus portBus, Lock regLock, ReadOnlySpan<char> name)
    {
        if (_isRunning) return false;

        Reset();

        _nameDevice = nameDeviceToken.CreateChild(name);
        _ram = ram;
        _portBus = portBus;
        _regLock = regLock;

        return true;
    }

    public void PushBiosStatus(BiosStatus status) => _statusFromSimulation.Push(status);

    private void ConsoleLock(string text, LogLevelKernel level = LogLevelKernel.Log)
    {
        LoggerKernel.LogFromDevice(in _nameDevice, text, level);
    }

    public string GetAllData()
    {
        StringBuilder stringBuilder = new(128);
        for (int i = 0; i < CountReg; i++)
        {
            stringBuilder.AppendLine($"Регистр [{(RegType)i}]= {_registers[i]}");
        }
        stringBuilder.AppendLine($"IP {GetRegValue(RegType.rIP)}");
        return stringBuilder.ToString();
    }
    private void UpdateFlags(ulong result)
    {
        ulong currentFlags = GetRegValue(RegType.rFL);

        currentFlags &= ~(1UL | 2UL);

        if (result == 0) currentFlags |= 1UL;
        if ((result & NegativeMask) != 0) currentFlags |= 2UL;
        
        SetRegValue(RegType.rFL, currentFlags);
    }

    public void LaunchProgramm(ulong startAddress)
    {
        SetRegValue(RegType.rIP, startAddress);
        _isRunning = true;

        // Сбрасываем управляющие регистры связи и вложенности
        SetRegValue(RegType.rCL, 0);
        SetRegValue(RegType.rCD, 0);
        SetRegValue(RegType.rFL, 0);
        SetRegValue(RegType.rTB, 0);
        // Инициализируем стек на самый конец RAM, выравнивая по границе 8 байт
        ulong stackTop = _ram.RamSize;

        // Безопасное выравнивание (округляем вниз до ближайшего кратного 8)
        stackTop &= ~0x7UL;

        SetRegValue(RegType.rSP, stackTop);
    }

    public void Step(bool isDebug = false)
    {
        ulong _ip = GetRegValue(RegType.rIP);
        RAMResultInt32 instResult = _ram.ReadInt32LE(_ip);

        while (_statusFromSimulation.TryPop(out BiosStatus result))
        {
            if (result != BiosStatus.Success)
            {
                _isRunning = ProcessorHelpers.TryContinueAfterStatus(new ResultInstruction(instResult.Status, 1UL), _ip, _regLock);
                return;
            }
        }

        if (!instResult.IsSuccess)
        {
            _isRunning = ProcessorHelpers.TryContinueAfterStatus(new ResultInstruction(instResult.Status, instResult.FaultAddress), _ip, _regLock);
            return;
        }

        uint rawInst = instResult.Data;
        OpCode opCode = (OpCode)(rawInst & 0xFF);

        // ВРЕМЕННЫЙ ТЕСТ: Выводим в консоль, что именно прочитал процессор из памяти
        if (isDebug)
        {
            ConsoleLock($"IP: {_ip} | rawInst в HEX: 0x{rawInst:X8} | Распознанный OpCode: {opCode} (0x{(byte)opCode:X2})");
        }
        _ip += 4;

        ulong data = 0;

        if (opCode == OpCode.LDI || opCode == OpCode.LOAD || opCode == OpCode.STORE ||
            opCode == OpCode.CALL || opCode == OpCode.JMP ||
            opCode == OpCode.JZ || opCode == OpCode.JNZ || opCode == OpCode.JG || opCode == OpCode.JL)
        {
            _ip = (_ip + 7) & ~7UL;
            RAMResultInt64 dataResult = _ram.ReadInt64LE(_ip);
            if (!dataResult.IsSuccess)
            {
                _isRunning = ProcessorHelpers.TryContinueAfterStatus(new ResultInstruction(dataResult.Status, dataResult.FaultAddress), _ip, _regLock);
                return;
            }

            data = dataResult.Data;
            _ip += 8;
        }

        SetRegValue(RegType.rIP, _ip);
        ResultInstruction dat = DecodeInstruction(rawInst, data);

        if (dat.BiosStatus != BiosStatus.Success)
        {
            _isRunning = ProcessorHelpers.TryContinueAfterStatus(dat, _ip, _regLock);
        }
    }

    public ResultInstruction DecodeInstruction(uint rawInst, ulong data)
    {
        OpCode opCode = (OpCode)(rawInst & 0xFF);

        // Вытягиваем регистры по новой плотной сетке битов
        RegType reg1 = (RegType)((rawInst >> 8) & 0x1F);
        RegType reg2 = (RegType)((rawInst >> 13) & 0x1F);

        // Вытягиваем размер данных из 18 и 19 битов
        OpCodeSize dataSizeCode = (OpCodeSize)((rawInst >> 18) & 0x3);

        return opCode switch
        {
            // === 1. Системные команды ===
            OpCode.NOP => ResultInstruction.IsSucced,
            OpCode.HALT => EndProgramm(),
            OpCode.PRINT => InstructionPRINT(reg1),

            // === 2. Работа с памятью (Указатели и регистры) ===
            OpCode.MOV => InstructionMOV(reg1, reg2),
            OpCode.LOAD => InstructionLOAD(dataSizeCode, reg1, data),
            OpCode.STORE => InstructionSTORE(dataSizeCode, reg1, data),
            OpCode.STORE_IND => InstructionSTORE_IND(dataSizeCode, reg1, reg2),
            OpCode.LOAD_IND => InstructionLOAD_IND(dataSizeCode, reg1, reg2),
            OpCode.LDI => InstructionLDI(reg1, data),

            // === 3. Арифметика и Логика (Тьюринг-базис) ===
            OpCode.ADD => InstructionADD(reg1, reg2),
            OpCode.SUB => InstructionSUB(reg1, reg2),
            OpCode.INC => InstructionINC(reg1),
            OpCode.DEC => InstructionDEC(reg1),
            OpCode.MULT_INT => InstructionMULT_INT(reg1, reg2),
            OpCode.SHR => InstructionSHR(reg1, reg2),
            OpCode.DIV => InstructionDIV(reg1, reg2),

            // === 4. Логика ===
            OpCode.AND => InstructionAND(reg1, reg2),
            OpCode.OR => InstructionOR(reg1, reg2),
            OpCode.XOR => InstructionXOR(reg1, reg2),
            OpCode.NOT => InstructionNOT(reg1),

            // === 5. Управление потоком ===
            OpCode.JMP => InstructionJMP(data),
            OpCode.JZ => InstructionJZ(data),
            OpCode.JNZ => InstructionJNZ(data),
            OpCode.JG => InstructionJG(data),
            OpCode.JL => InstructionJL(data),

            // === 6. Работа со Стеком ===
            OpCode.PUSH => InstructionPUSH(reg1),
            OpCode.POP => InstructionPOP(reg1),
            OpCode.CALL => InstructionCALL(data),
            OpCode.RET => InstructionRET(),

            // === 7. Ввод-вывод ===
            OpCode.IN => InstructionIN(reg1, reg2),
            OpCode.OUT => InstructionOUT(reg1, reg2),
            OpCode.INT => InstructionINT(reg1),   // reg1 содержит номер вектора
            OpCode.IRET => InstructionIRET(),

            OpCode.PRINT_INT => InstructionPRINT_INT(reg1),
            OpCode.ALLOC => InstructionALLOC(reg1),
            _ => new ResultInstruction(BiosStatus.NotImplementedOpCode, (ulong)opCode)
        };
    }
    private ResultInstruction InstructionDIV(RegType reg1, RegType reg2)
    {
        ulong divisor = GetRegValue(reg2);
        if (divisor == 0)
        {
            ulong ip = GetRegValue(RegType.rIP);
            return new ResultInstruction(BiosStatus.DivOnZero, ip);
        }
        ulong dividend = GetRegValue(reg1);
        ulong res = dividend / divisor;
        SetRegValue(reg1, res);
        UpdateFlags(res);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionSHR(RegType reg1, RegType reg2)
    {
        ulong regv1 = GetRegValue(reg1);
        byte regv2 = (byte)GetRegValue(reg2);
        ulong res = regv1 >> regv2;
        SetRegValue(reg1, res);
        UpdateFlags(res);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionMULT_INT(RegType reg1, RegType reg2)
    {
        ulong regv1 = GetRegValue(reg1);
        ulong regv2 = GetRegValue(reg2);
        ulong res = regv1 * regv2;
        SetRegValue(reg1, res);
        UpdateFlags(res);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionALLOC(RegType reg1)
    {
        ulong size = GetRegValue(RegType.r0);
        ulong hp = GetRegValue(RegType.rHP);
        // выравниваем размер вверх до кратности 8
        if (size % 8 != 0) size = (size + 7) & ~7UL;
        ulong result = hp;
        SetRegValue(RegType.rHP, hp + size);
        SetRegValue(RegType.r0, result);
        UpdateFlags(result);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionINT(RegType reg)
    {
        uint vector = (uint)GetRegValue(reg) & 0x1F; // 32 вектора
        ulong tableBase = GetRegValue(RegType.rTB);
        ulong handlerAddr;
        RAMResultInt64 readResult = _ram.ReadInt64LE(tableBase + (ulong)vector * 8);
        if (!readResult.IsSuccess)
            return new ResultInstruction(readResult.Status, readResult.FaultAddress);
        handlerAddr = readResult.Data;

        // Сохраняем текущий IP в стек
        ulong sp = GetRegValue(RegType.rSP);
        sp -= 8;
        RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, GetRegValue(RegType.rIP));

        if (!writeResult.IsSuccess) 
            return new ResultInstruction(writeResult.Status, sp);

        SetRegValue(RegType.rSP, sp);
        if (handlerAddr == 0)
        {
            ConsoleLock("handlerAddr == 0");
            return EndProgramm();
        }
        // Переходим на обработчик
        SetRegValue(RegType.rIP, handlerAddr);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionIRET()
    {
        // Восстанавливаем IP из стека
        ulong sp = GetRegValue(RegType.rSP);
        RAMResultInt64 readResult = _ram.ReadInt64LE(sp);
        if (!readResult.IsSuccess)
            return new ResultInstruction(readResult.Status, sp);
        sp += 8;
        SetRegValue(RegType.rIP, readResult.Data);
        SetRegValue(RegType.rSP, sp);
        return ResultInstruction.IsSucced;
    }

    public ResultInstruction EndProgramm()
    {
        _isRunning = false;
        return new ResultInstruction(BiosStatus.EndProgramm, 0);
    }

    public ResultInstruction InstructionPRINT_INT(RegType reg)
    {
        ConsoleLock(GetRegValue(reg).ToString());
        return ResultInstruction.IsSucced;
    }
    public ulong GetRegValue(RegType reg)
    {
        return reg switch
        {
            RegType.rZ => 0,
            _ => _registers[(byte)reg]
        };
    }

    private ResultInstruction InstructionPRINT(RegType reg)
    {
        LoggerProvider.Info((char)GetRegValue(reg));
        return ResultInstruction.IsSucced;
    }
    private void SetRegValue(RegType reg, ulong value)
    {
        switch (reg)
        {
            case RegType.rZ: return;

            case RegType.rIP:
                //_ip = value;
                _registers[(byte)reg] = value;
                return;

            case RegType.rFL: _registers[(byte)reg] = value & 0x3UL; return;

            default: _registers[(byte)reg] = value; return;
        }
    }


    public readonly struct ResultInstruction(BiosStatus biosStatus, ulong adress)
    {
        public readonly BiosStatus BiosStatus = biosStatus;
        public readonly ulong Adress = adress;

        public static ResultInstruction IsSucced => new(BiosStatus.Success, 0);
    }

    private ResultInstruction InstructionMOV(RegType reg1, RegType reg2)
    {
        SetRegValue(reg1, GetRegValue(reg2));
        return ResultInstruction.IsSucced;
    }
    private ResultInstruction InstructionLOAD(OpCodeSize sizeT, RegType reg, ulong adress)
    {

        switch (sizeT)
        {
            case OpCodeSize.S8:
                RAMResultInt8 data8 = _ram.ReadInt8LE(adress);
                if (data8.IsSuccess)
                {
                    SetRegValue(reg, data8.Data);
                    UpdateFlags(data8.Data);
                    return ResultInstruction.IsSucced;
                }
                return new(data8.Status, data8.FaultAddress);


            case OpCodeSize.S16:
                RAMResultInt16 data16 = _ram.ReadInt16LE(adress);
                if (data16.IsSuccess)
                {
                    SetRegValue(reg, data16.Data);
                    UpdateFlags(data16.Data);

                    return ResultInstruction.IsSucced;
                }
                return new(data16.Status, data16.FaultAddress);


            case OpCodeSize.S32:
                RAMResultInt32 data32 = _ram.ReadInt32LE(adress);
                if (data32.IsSuccess)
                {
                    SetRegValue(reg, data32.Data);
                    UpdateFlags(data32.Data);

                    return ResultInstruction.IsSucced;
                }
                return new(data32.Status, data32.FaultAddress);


            case OpCodeSize.S64:
                RAMResultInt64 data64 = _ram.ReadInt64LE(adress);
                if (data64.IsSuccess)
                {
                    SetRegValue(reg, data64.Data);
                    UpdateFlags(data64.Data);

                    return ResultInstruction.IsSucced;
                }
                return new(data64.Status, data64.FaultAddress);


            default:
                // Защита на случай передачи некорректного или нереализованного OpCodeSize
                return new(BiosStatus.SegmentationFault, adress);
        }

        
    }

    private ResultInstruction InstructionSTORE(OpCodeSize sizeT, RegType reg, ulong adress)
    {

        switch (sizeT)
        {
            case OpCodeSize.S8:
                RAMResultInt8 data8 = _ram.WriteInt8LE(adress, (byte)GetRegValue(reg));
                if (data8.IsSuccess)
                {
                    UpdateFlags(data8.Data);
                    return ResultInstruction.IsSucced;
                }
                return new(data8.Status, data8.FaultAddress);

            case OpCodeSize.S16:
                RAMResultInt16 data16 = _ram.WriteInt16LE(adress, (ushort)GetRegValue(reg));
                if (data16.IsSuccess)
                {
                    UpdateFlags(data16.Data);
                    return ResultInstruction.IsSucced;
                }
                return new(data16.Status, data16.FaultAddress);


            case OpCodeSize.S32:
                RAMResultInt32 data32 = _ram.WriteInt32LE(adress, (uint)GetRegValue(reg));
                if (data32.IsSuccess)
                {
                    UpdateFlags(data32.Data);
                    return ResultInstruction.IsSucced;
                }
                return new(data32.Status, data32.FaultAddress);


            case OpCodeSize.S64:
                RAMResultInt64 data64 = _ram.WriteInt64LE(adress, GetRegValue(reg));
                if (data64.IsSuccess)
                {
                    UpdateFlags(data64.Data);
                    return ResultInstruction.IsSucced;
                }
                return new(data64.Status, data64.FaultAddress);


            default:
                return new(BiosStatus.AlignmentFault, adress);
        }
    }

    private ResultInstruction InstructionLOAD_IND(OpCodeSize sizeT, RegType reg1, RegType reg2)
    {
        return InstructionLOAD(sizeT, reg1, GetRegValue(reg2));
    }

    private ResultInstruction InstructionSTORE_IND(OpCodeSize sizeT, RegType reg1, RegType reg2)
    {
        return InstructionSTORE(sizeT, reg1, GetRegValue(reg2));
    }

    private ResultInstruction InstructionLDI(RegType reg, ulong value)
    {
        SetRegValue(reg, value);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionADD(RegType reg1, RegType reg2)
    {
        ulong value1 = GetRegValue(reg1);
        ulong value2 = GetRegValue(reg2);
        ulong res = value1 + value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionSUB(RegType reg1, RegType reg2)
    {
        ulong value1 = GetRegValue(reg1);
        ulong value2 = GetRegValue(reg2);
        ulong res = value1 - value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionINC(RegType reg)
    {
        ulong value1 = GetRegValue(reg);
        ulong res = value1 + 1;
        SetRegValue(reg, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionDEC(RegType reg)
    {
        ulong value1 = GetRegValue(reg);
        ulong res = value1 - 1;
        SetRegValue(reg, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionAND(RegType reg1, RegType reg2)
    {
        ulong value1 = GetRegValue(reg1);
        ulong value2 = GetRegValue(reg2);
        ulong res = value1 & value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionOR(RegType reg1, RegType reg2)
    {
        ulong value1 = GetRegValue(reg1);
        ulong value2 = GetRegValue(reg2);
        ulong res = value1 | value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionXOR(RegType reg1, RegType reg2)
    {
        ulong value1 = GetRegValue(reg1);
        ulong value2 = GetRegValue(reg2);
        ulong res = value1 ^ value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionNOT(RegType reg)
    {
        ulong value = GetRegValue(reg);
        ulong res = ~value;
        SetRegValue(reg, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }
    private ResultInstruction InstructionJMP(ulong targetAddress)
    {
        SetRegValue(RegType.rIP, targetAddress);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionJZ(ulong targetAddress)
    {
        if (IsZero != 0) return InstructionJMP(targetAddress);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionJNZ(ulong targetAddress)
    {
        if (IsZero == 0)
        {
            SetRegValue(RegType.rIP, targetAddress);
        }
        return ResultInstruction.IsSucced;
    }


    private ResultInstruction InstructionJG(ulong targetAddress)
    {
        bool isZero = IsZero != 0;
        bool isNegative = IsNegative != 0;

        if (!isZero && !isNegative)
        {
            SetRegValue(RegType.rIP, targetAddress);
        }
        return ResultInstruction.IsSucced;
    }


    private ResultInstruction InstructionJL(ulong targetAddress)
    {
        if (IsNegative != 0)
        {
            SetRegValue(RegType.rIP, targetAddress);
        }
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionPUSH(RegType reg)
    {
        ulong value = GetRegValue(reg);
        ulong sp = GetRegValue(RegType.rSP);

        sp -= 8;

        RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, value);

        if (!writeResult.IsSuccess)
        {
            return new ResultInstruction(writeResult.Status, sp);
        }

        SetRegValue(RegType.rSP, sp);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionPOP(RegType reg)
    {
        ulong sp = GetRegValue(RegType.rSP);

        RAMResultInt64 readResult = _ram.ReadInt64LE(sp);

        if (!readResult.IsSuccess)
        {
            return new ResultInstruction(readResult.Status, sp);
        }

        sp += 8;

        SetRegValue(reg, readResult.Data);
        SetRegValue(RegType.rSP, sp);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionCALL(ulong targetAddress)
    {
        ulong currentLink = GetRegValue(RegType.rCL);
        ulong callDepth = GetRegValue(RegType.rCD);
        ulong _ip = GetRegValue(RegType.rIP);
        // Если глубина вложенности > 0, значит rCL уже занят предыдущим методом.
        // Спасаем его значение в стек.
        if (callDepth > 0)
        {
            ulong sp = GetRegValue(RegType.rSP);
            sp -= 8;

            RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, currentLink);
            if (!writeResult.IsSuccess)
            {
                return new ResultInstruction(writeResult.Status, sp);
            }

            SetRegValue(RegType.rSP, sp);
        }

        // Сохраняем адрес возврата в быстрый регистр rCL
        SetRegValue(RegType.rCL, _ip);

        // Увеличиваем счетчик вложенности
        callDepth++;

        // Переходим к коду вызванного метода
        _ip = targetAddress;

        SetRegValue(RegType.rCD, callDepth);
        SetRegValue(RegType.rIP, _ip);

        return ResultInstruction.IsSucced;
    }

    // Возврат из подпрограммы (RET)
    private ResultInstruction InstructionRET()
    {
        ulong callDepth = GetRegValue(RegType.rCD);
        ulong _ip = GetRegValue(RegType.rIP);

        if (callDepth == 0)
        {
            ConsoleLock("Ошибка: RET вызван без предшествующего CALL. Программа остановлена.");
            return new ResultInstruction((BiosStatus)9, _ip);
        }

        ulong returnAddress = GetRegValue(RegType.rCL);
        _ip = returnAddress;


        callDepth--;

        if (callDepth > 0)
        {
            ulong sp = GetRegValue(RegType.rSP);

            RAMResultInt64 readResult = _ram.ReadInt64LE(sp);
            if (!readResult.IsSuccess)
            {
                return new ResultInstruction(readResult.Status, sp);
            }

            SetRegValue(RegType.rCL, readResult.Data);
            sp += 8;
            SetRegValue(RegType.rSP, sp);
        }
        else
        {
            SetRegValue(RegType.rCL, 0);
        }

        SetRegValue(RegType.rCD, callDepth);
        SetRegValue(RegType.rIP, _ip);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionIN(RegType regDest, RegType regPort)
    {
        ulong portAddress = GetRegValue(regPort);
        RAMResultInt8 result = _portBus.ReadPort(portAddress);
        if (!result.IsSuccess)
            return new ResultInstruction(result.Status, result.FaultAddress);

        SetRegValue(regDest, result.Data);
        UpdateFlags(result.Data);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionOUT(RegType regSrc, RegType regPort)
    {
        ulong portAddress = GetRegValue(regPort);
        byte value = (byte)(GetRegValue(regSrc) & 0xFF);
        RAMResultInt8 result = _portBus.WritePort(portAddress, value);
        if (!result.IsSuccess)
            return new ResultInstruction(result.Status, result.FaultAddress);

        // Флаги обычно не меняются при выводе, но можно обновить по желанию
        return ResultInstruction.IsSucced;
    }

    public void Reset()
    {
        lock (_regLock)
        {
            _isRunning = false;
            ClearRegs();
        }
    }

    private void ClearRegs()
    {
        for (int i = 0; i < CountReg; i++) _registers[i] = 0UL;
    }

    public void InitReg(ulong hpInit) => SetRegValue(RegType.rHP, hpInit);
}
