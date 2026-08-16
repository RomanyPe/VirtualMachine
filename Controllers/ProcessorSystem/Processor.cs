using Kernel.Common;
using Kernel.ControllersData;
using Kernel.RamSystem;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using static Kernel.Common.InstructionDecoder;

namespace Kernel.ProcessorSystem;

public sealed class Processor(MemoryBus ram, NameDeviceToken nameDeviceToken, PortBus portBus, Lock regLock, ReadOnlySpan<char> name)
{
    [InlineArray(CountReg)]
    private struct ArrayRegisters
    {
        public ulong _element0;
    }

    private const int CountReg = 32;
    private const ulong NegativeMask = 0x8000000000000000;

    private ArrayRegisters _registers;

    private readonly ConcurrentQueue<uint> _externalCommands = new();
    private readonly ConcurrentQueue<BiosStatus> _statusFromSimulation = [];

    private NameDeviceToken _nameDevice = nameDeviceToken.CreateChild(name);
    private MemoryBus _ram = ram;
    private PortBus _portBus = portBus;
    private Lock _regLock = regLock;

    private volatile bool _isRunning = false;
    private volatile bool _externalCommandAdded = false;
    private volatile bool _hasSimulationStatus = false;
    private bool _sleeping = false;

    private Action<ulong, uint, OpCode>? _snowOpcodeCallback;

    public bool IsSleeping => _sleeping;
    public bool IsRunning => _isRunning;
    private ulong IsZero => _registers[RegType.rFL.Int] & 1UL;
    private ulong IsNegative => _registers[RegType.rFL.Int] & 2UL;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetRegValue(RegType reg)
    {
        return reg == RegType.rZ ? 0 : _registers[reg.Int];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetRegValue(RegType reg, ulong value)
    {
        if (reg == RegType.rZ) return;

        _registers[(int)reg] = (reg == RegType.rFL) ? (value & 0x3UL) : value;
    }



    public void EnqueueBiosStatus(BiosStatus status)
    {
        _hasSimulationStatus = true;
        _statusFromSimulation.Enqueue(status);
    }

    private void ConsoleLock(string text, LogLevel level = LogLevel.Log)
    {
        LoggerKernel.LogFromDevice(in _nameDevice, text, level);
    }

    public string DumbRegs()
    {
        StringBuilder stringBuilder = new(128);
        for (int i = 0; i < CountReg; i++)
        {
            stringBuilder.AppendLine($"[{(RegType)i}]= {_registers[i]}");
        }
        stringBuilder.AppendLine($"IP {_registers[RegType.rIP.Int]}");
        return stringBuilder.ToString();
    }
    private void UpdateFlags(ulong result)
    {
        ulong currentFlags = _registers[RegType.rFL.Int];

        currentFlags &= ~(1UL | 2UL);

        if (result == 0) currentFlags |= 1UL;
        if ((result & NegativeMask) != 0) currentFlags |= 2UL;

        _registers[RegType.rFL.Int] = currentFlags;
    }

    public void LaunchProgramm(ulong startAddress, Action<ulong, uint, OpCode>? act = null)
    {
        _snowOpcodeCallback = act;
        _sleeping = false;
        _registers[RegType.rIP.Int] = startAddress;
        _isRunning = true;

        _registers[RegType.rCL.Int] = 0;
        _registers[RegType.rCD.Int] = 0;
        _registers[RegType.rFL.Int] = 0;
        _registers[RegType.rTB.Int] = 0;

        ulong stackTop = _ram.RamSize;
        stackTop &= ~0x7UL;

        _registers[RegType.rSP.Int] = stackTop;
    }

    public void Step()
    {
        if (_sleeping) return;

        if (_externalCommandAdded)
        {
            while (_externalCommands.TryDequeue(out uint cmd))
            {
                ExecuteExternalCommand(cmd);
                if (!_isRunning) return;   // если команда остановила процессор
            }
            _externalCommandAdded = false;
        }
        if (_hasSimulationStatus)
        {
            while (_statusFromSimulation.TryDequeue(out BiosStatus result))
            {
                if (result != BiosStatus.Success)
                {
                    _isRunning = ProcessorHelpers.TryContinueAfterStatus(new ResultInstruction(result, 1UL), ulong.MaxValue, _regLock);
                    return;
                }
            }
            _hasSimulationStatus = false;
        }

        ulong ip = _registers[RegType.rIP.Int];

        RAMResultInt32 instResult = _ram.ReadInt32LE(ip);

        if (!instResult.IsSuccess)
        {
            _isRunning = ProcessorHelpers.TryContinueAfterStatus(new ResultInstruction(instResult.Status, instResult.FaultAddress), ip, _regLock);
            return;
        }

        uint rawInst = instResult.Data;
        OpCode opCode = (OpCode)(rawInst & 0xFF);

        _snowOpcodeCallback?.Invoke(ip, rawInst, opCode);
        ip += 4;

        ulong data = 0;

        if (HasNeed64IntData(opCode))
        {
            ip = (ip + 7) & ~7UL;
            RAMResultInt64 dataResult = _ram.ReadInt64LE(ip);
            if (!dataResult.IsSuccess)
            {
                _isRunning = ProcessorHelpers.TryContinueAfterStatus(new ResultInstruction(dataResult.Status, dataResult.FaultAddress), ip, _regLock);
                return;
            }

            data = dataResult.Data;
            ip += 8;
        }

        _registers[RegType.rIP.Int] = ip;

        ResultInstruction dat = DecodeInstruction(rawInst, data);

        if (dat.BiosStatus != BiosStatus.Success)
        {
            _isRunning = ProcessorHelpers.TryContinueAfterStatus(dat, ip, _regLock);
        }
    }

    
    public ResultInstruction DecodeInstruction(uint rawInst, ulong data)
    {
        return GetOpCode(rawInst) switch
        {
            // === 1. Системные команды ===
            OpCode.NOP => ResultInstruction.IsSucced,
            OpCode.END => EndProgramm(),
            OpCode.PRINT => InstructionPRINT(GetReg1(rawInst)),
            // === 2. Работа с памятью (Указатели и регистры) ===
            OpCode.MOV => InstructionMOV(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.LOAD => InstructionLOAD(GetDataSizeCode(rawInst), GetReg1(rawInst), data),
            OpCode.STORE => InstructionSTORE(GetDataSizeCode(rawInst), GetReg1(rawInst), data),
            OpCode.STORE_IND => InstructionSTORE_IND(GetDataSizeCode(rawInst), GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.LOAD_IND => InstructionLOAD_IND(GetDataSizeCode(rawInst), GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.LDI => InstructionLDI(GetReg1(rawInst), data),

            // === 3. Арифметика и Логика (Тьюринг-базис) ===
            OpCode.ADD => InstructionADD(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.SUB => InstructionSUB(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.INC => InstructionINC(GetReg1(rawInst)),
            OpCode.DEC => InstructionDEC(GetReg1(rawInst)),
            OpCode.MULT_INT => InstructionMULT_INT(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.SHR => InstructionSHR(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.DIV => InstructionDIV(GetReg1(rawInst), GetReg2(rawInst)),

            // === 4. Логика ===
            OpCode.AND => InstructionAND(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.OR => InstructionOR(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.XOR => InstructionXOR(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.NOT => InstructionNOT(GetReg1(rawInst)),

            // === 5. Управление потоком ===
            OpCode.JMP => InstructionJMP(data),
            OpCode.JZ => InstructionJZ(data),
            OpCode.JNZ => InstructionJNZ(data),
            OpCode.JG => InstructionJG(data),
            OpCode.JL => InstructionJL(data),

            // === 6. Работа со Стеком ===
            OpCode.PUSH => InstructionPUSH(GetReg1(rawInst)),
            OpCode.POP => InstructionPOP(GetReg1(rawInst)),
            OpCode.CALL => InstructionCALL(data),
            OpCode.RET => InstructionRET(),

            // === 7. Ввод-вывод ===
            OpCode.IN => InstructionIN(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.OUT => InstructionOUT(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.INT => InstructionINT(GetReg1(rawInst)),   // GetReg1(rawInst) содержит номер вектора
            OpCode.IRET => InstructionIRET(),

            OpCode.PRINT_INT => InstructionPRINT_INT(GetReg1(rawInst)),
            OpCode.ALLOC => InstructionALLOC(GetReg1(rawInst)),
            OpCode.WAKE_INT => InstructionWAKE_INT(GetReg1(rawInst)),
            OpCode.HALT => InstructionHALT(),
            _ => new ResultInstruction(BiosStatus.NotImplementedOpCode, (ulong)GetOpCode(rawInst))
        };
    }

    private ResultInstruction InstructionHALT()
    {
        _sleeping = true;
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionWAKE_INT(RegType reg1)
    {
        ulong regV = _registers[reg1.Int];
        if (!_portBus.WakeProcessor(regV))
        {
            ulong ip = _registers[RegType.rIP.Int];
            return new(BiosStatus.NullDeviceOutput, ip);
        }

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionDIV(RegType reg1, RegType reg2)
    {
        ulong divisor = _registers[reg2.Int];
        if (divisor == 0)
        {
            ulong ip = _registers[RegType.rIP.Int];
            return new ResultInstruction(BiosStatus.DivOnZero, ip);
        }
        ulong dividend = _registers[reg1.Int];
        ulong res = dividend / divisor;
        SetRegValue(reg1, res);
        UpdateFlags(res);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionSHR(RegType reg1, RegType reg2)
    {
        ulong regv1 = _registers[reg1.Int];
        byte regv2 = (byte)_registers[reg2.Int];
        ulong res = regv1 >> regv2;
        SetRegValue(reg1, res);
        UpdateFlags(res);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionMULT_INT(RegType reg1, RegType reg2)
    {
        ulong regv1 = _registers[reg1.Int];
        ulong regv2 = _registers[reg2.Int];
        ulong res = regv1 * regv2;
        SetRegValue(reg1, res);
        UpdateFlags(res);
        return ResultInstruction.IsSucced;
    }

#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
    private ResultInstruction InstructionALLOC(RegType reg1)
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр
    {
        ulong size = _registers[RegType.r0.Int];
        ulong hp = _registers[RegType.rHP.Int];
        // выравниваем размер вверх до кратности 8
        if (size % 8 != 0) size = (size + 7) & ~7UL;
        ulong result = hp;
        _registers[RegType.rHP.Int] = hp + size;
        _registers[RegType.r0.Int] = result;
        UpdateFlags(result);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionINT(RegType reg)
    {
        uint vector = (uint)_registers[reg.Int] & 0x1F; // 32 вектора
        ulong tableBase = _registers[RegType.rTB.Int];
        ulong handlerAddr;
        RAMResultInt64 readResult = _ram.ReadInt64LE(tableBase + (ulong)vector * 8);
        if (!readResult.IsSuccess)
            return new ResultInstruction(readResult.Status, readResult.FaultAddress);
        handlerAddr = readResult.Data;

        // Сохраняем текущий IP в стек
        ulong ip = _registers[RegType.rIP.Int];
        ulong sp = _registers[RegType.rSP.Int];
        sp -= 8;
        RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, ip);

        if (!writeResult.IsSuccess)
            return new ResultInstruction(writeResult.Status, sp);

        _registers[RegType.rSP.Int] = sp;
        if (handlerAddr == 0)
        {
            ConsoleLock("handlerAddr == 0");
            return EndProgramm();
        }
        // Переходим на обработчик
        _registers[RegType.rIP.Int] = handlerAddr;
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionIRET()
    {
        // Восстанавливаем IP из стека
        ulong sp = _registers[RegType.rSP.Int];
        RAMResultInt64 readResult = _ram.ReadInt64LE(sp);
        if (!readResult.IsSuccess)
            return new ResultInstruction(readResult.Status, sp);
        sp += 8;
        _registers[RegType.rIP.Int] = readResult.Data;
        _registers[RegType.rSP.Int] = sp;
        return ResultInstruction.IsSucced;
    }

    public ResultInstruction EndProgramm()
    {
        _isRunning = false;
        return new ResultInstruction(BiosStatus.EndProgramm, 0);
    }

    public ResultInstruction InstructionPRINT_INT(RegType reg)
    {
        ConsoleLock(_registers[reg.Int].ToString());
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionPRINT(RegType reg)
    {
        LoggerProvider.Info((char)_registers[reg.Int]);
        return ResultInstruction.IsSucced;
    }

    public readonly struct ResultInstruction(BiosStatus biosStatus, ulong adress)
    {
        public readonly BiosStatus BiosStatus = biosStatus;
        public readonly ulong Adress = adress;

        public static ResultInstruction IsSucced => new(BiosStatus.Success, 0);
    }

    private ResultInstruction InstructionMOV(RegType reg1, RegType reg2)
    {
        SetRegValue(reg1, _registers[reg2.Int]);
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
                RAMResultInt8 data8 = _ram.WriteInt8LE(adress, (byte)_registers[reg.Int]);
                if (data8.IsSuccess)
                {
                    UpdateFlags(data8.Data);
                    return ResultInstruction.IsSucced;
                }
                return new(data8.Status, data8.FaultAddress);

            case OpCodeSize.S16:
                RAMResultInt16 data16 = _ram.WriteInt16LE(adress, (ushort)_registers[reg.Int]);
                if (data16.IsSuccess)
                {
                    UpdateFlags(data16.Data);
                    return ResultInstruction.IsSucced;
                }
                return new(data16.Status, data16.FaultAddress);


            case OpCodeSize.S32:
                RAMResultInt32 data32 = _ram.WriteInt32LE(adress, (uint)_registers[reg.Int]);
                if (data32.IsSuccess)
                {
                    UpdateFlags(data32.Data);
                    return ResultInstruction.IsSucced;
                }
                return new(data32.Status, data32.FaultAddress);


            case OpCodeSize.S64:
                RAMResultInt64 data64 = _ram.WriteInt64LE(adress, _registers[reg.Int]);
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
        return InstructionLOAD(sizeT, reg1, _registers[reg2.Int]);
    }

    private ResultInstruction InstructionSTORE_IND(OpCodeSize sizeT, RegType reg1, RegType reg2)
    {
        return InstructionSTORE(sizeT, reg1, _registers[reg2.Int]);
    }

    private ResultInstruction InstructionLDI(RegType reg, ulong value)
    {
        SetRegValue(reg, value);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionADD(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 + value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionSUB(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 - value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionINC(RegType reg)
    {
        ulong value1 = _registers[reg.Int];
        ulong res = value1 + 1;
        SetRegValue(reg, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionDEC(RegType reg)
    {
        ulong value1 = _registers[reg.Int];
        ulong res = value1 - 1;
        SetRegValue(reg, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionAND(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 & value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionOR(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 | value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionXOR(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 ^ value2;
        SetRegValue(reg1, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionNOT(RegType reg)
    {
        ulong value = _registers[reg.Int];
        ulong res = ~value;
        SetRegValue(reg, res);
        UpdateFlags(res);

        return ResultInstruction.IsSucced;
    }
    private ResultInstruction InstructionJMP(ulong targetAddress)
    {
        _registers[RegType.rIP.Int] = targetAddress;
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
            _registers[RegType.rIP.Int] = targetAddress;
        }
        return ResultInstruction.IsSucced;
    }


    private ResultInstruction InstructionJG(ulong targetAddress)
    {
        bool isZero = IsZero != 0;
        bool isNegative = IsNegative != 0;

        if (!isZero && !isNegative)
        {
            _registers[RegType.rIP.Int] = targetAddress;
        }
        return ResultInstruction.IsSucced;
    }


    private ResultInstruction InstructionJL(ulong targetAddress)
    {
        if (IsNegative != 0)
        {
            _registers[RegType.rIP.Int] = targetAddress;
        }
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionPUSH(RegType reg)
    {
        ulong value = _registers[reg.Int];
        ulong sp = _registers[RegType.rSP.Int];

        sp -= 8;

        RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, value);

        if (!writeResult.IsSuccess)
        {
            return new ResultInstruction(writeResult.Status, sp);
        }

        _registers[RegType.rSP.Int] = sp;
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionPOP(RegType reg)
    {
        ulong sp = _registers[RegType.rSP.Int];

        RAMResultInt64 readResult = _ram.ReadInt64LE(sp);

        if (!readResult.IsSuccess)
        {
            return new ResultInstruction(readResult.Status, sp);
        }

        sp += 8;

        SetRegValue(reg, readResult.Data);
        _registers[RegType.rSP.Int] = sp;

        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionCALL(ulong targetAddress)
    {
        ulong currentLink = _registers[RegType.rCL.Int];
        ulong callDepth = _registers[RegType.rCD.Int];
        ulong ip = _registers[RegType.rIP.Int];
        // Если глубина вложенности > 0, значит rCL уже занят предыдущим методом.
        // Спасаем его значение в стек.
        if (callDepth > 0)
        {
            ulong sp = _registers[RegType.rSP.Int];
            sp -= 8;

            RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, currentLink);
            if (!writeResult.IsSuccess)
            {
                return new ResultInstruction(writeResult.Status, sp);
            }

            _registers[RegType.rSP.Int] = sp;
        }

        // Сохраняем адрес возврата в быстрый регистр rCL
        _registers[RegType.rCL.Int] = ip;

        // Увеличиваем счетчик вложенности
        callDepth++;

        // Переходим к коду вызванного метода
        ip = targetAddress;

        _registers[RegType.rCD.Int] = callDepth;
        _registers[RegType.rIP.Int] = ip;

        return ResultInstruction.IsSucced;
    }

    // Возврат из подпрограммы (RET)
    private ResultInstruction InstructionRET()
    {
        ulong callDepth = _registers[RegType.rCD.Int];
        ulong ip = _registers[RegType.rIP.Int];

        if (callDepth == 0)
        {
            ConsoleLock("Ошибка: RET вызван без предшествующего CALL. Программа остановлена.");
            return new ResultInstruction((BiosStatus)9, ip);
        }

        ulong returnAddress = _registers[RegType.rCL.Int];
        ip = returnAddress;


        callDepth--;

        if (callDepth > 0)
        {
            ulong sp = _registers[RegType.rSP.Int];

            RAMResultInt64 readResult = _ram.ReadInt64LE(sp);
            if (!readResult.IsSuccess)
            {
                return new ResultInstruction(readResult.Status, sp);
            }

            _registers[RegType.rCL.Int] = readResult.Data;
            sp += 8;
            _registers[RegType.rSP.Int] = sp;
        }
        else
        {
            _registers[RegType.rCL.Int] = 0;
        }

        _registers[RegType.rCD.Int] = callDepth;
        _registers[RegType.rIP.Int] = ip;
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionIN(RegType regDest, RegType regPort)
    {
        ulong portAddress = _registers[regPort.Int];
        RAMResultInt8 result = _portBus.ReadPort(portAddress);
        if (!result.IsSuccess)
            return new ResultInstruction(result.Status, result.FaultAddress);

        SetRegValue(regDest, result.Data);
        UpdateFlags(result.Data);
        return ResultInstruction.IsSucced;
    }

    private ResultInstruction InstructionOUT(RegType regSrc, RegType regPort)
    {
        ulong portAddress = _registers[regPort.Int];
        byte value = (byte)(_registers[regSrc.Int] & 0xFF);
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
            _sleeping = false;
            ClearRegs();
        }
    }

    private void ClearRegs()
    {
        for (int i = 0; i < CountReg; i++) _registers[i] = 0UL;
    }

    public void InitReg(ulong hpInit) => _registers[RegType.rHP.Int] = hpInit;

    public bool TryDequeueExternalCommand(out uint cmd)
    {
        return _externalCommands.TryDequeue(out cmd);
    }

    public void EnqueueExternalCommand(uint instruction)
    {
        _externalCommandAdded = true;
        _externalCommands.Enqueue(instruction);
    }

    public void ExecuteExternalCommand(uint rawInst)
    {
        OpCode opCode = (OpCode)(rawInst & 0xFF);
        switch (opCode)
        {
            case OpCode.END: _isRunning = false; break;
            case OpCode.HALT: _sleeping = true; break;
            case OpCode.WAKE: _sleeping = false; break;

            default: break;
        }
    }
}

