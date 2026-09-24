using Kernel.Common;
using Kernel.ControllersData;
using Kernel.RamSystem;
using System.Collections.Concurrent;
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

    private ArrayRegisters _registers;

    private readonly ConcurrentQueue<uint> _externalCommands = new();
    private readonly ConcurrentQueue<BiosStatus> _statusFromSimulation = [];

    private NameDeviceToken _nameDevice = nameDeviceToken.CreateChild(name);
    private MemoryBus _ram = ram;
    private PortBus _portBus = portBus;
    private Lock _regLock = regLock;

    private int _isRunning = 0;
    private int _hasSimulationStatus = 0;
    private int _hasExternalCommand = 0;
    private int _sleeping = 0;
    public bool IsSleeping => _sleeping == 1;
    public bool IsRunning => _isRunning == 1;
    private bool _hasCommand = false;
    private const bool NeedLogEnd = false;
    private ulong _lastFaultAddress = 0;
    private bool ZeroFlagSet => (_registers[RegType.rFL.Int] & 1UL) != 0UL;
    private bool NegativeFlagSet => (_registers[RegType.rFL.Int] & 2UL) != 0UL;

    public bool TryInitInPool(MemoryBus ram, NameDeviceToken nameDeviceToken, PortBus portBus, Lock regLock, ReadOnlySpan<char> name)
    {
        if (IsRunning) return false;

        Reset();

        _nameDevice = nameDeviceToken.CreateChild(name);
        _ram = ram;
        _portBus = portBus;
        _regLock = regLock;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetRegValue(RegType reg) => _registers[reg.Int];

    public void EnqueueBiosStatus(BiosStatus status)
    {
        Volatile.Write(ref _hasCommand, true);
        Volatile.Write(ref _hasSimulationStatus, 1);
        _statusFromSimulation.Enqueue(status);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
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
    public void CopyRegisters(Span<ulong> destination)
    {
        if (destination.Length < CountReg)
            throw new ArgumentException($"Нужно минимум {CountReg} элементов", nameof(destination));
        for (int i = 0; i < CountReg; i++)
            destination[i] = _registers[i];
    }

    public ulong[] GetRegistersSnapshot()
    {
        var arr = new ulong[CountReg];
        CopyRegisters(arr);
        return arr;
    }
   

    private ulong Get64IntData()
    {
        ulong ip = _registers[RegType.rIP.Int];
        ip = (ip + 7) & ~7UL;
        ulong data = _ram.ReadInt64LEUnSafe(ip);
        ip += 8;
        _registers[RegType.rIP.Int] = ip;
        return data;
    }

    public void LaunchProgramm(ulong startAddress)
    {
        _sleeping = 0;
        _registers[RegType.rIP.Int] = startAddress;
        _isRunning = 1;

        _registers[RegType.rCL.Int] = 0;
        _registers[RegType.rCD.Int] = 0;
        _registers[RegType.rFL.Int] = 0;
        _registers[RegType.rTB.Int] = 0;

        ulong stackTop = _ram.RamSize;
        stackTop &= ~0x7UL;

        _registers[RegType.rSP.Int] = stackTop;
    }


    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ProcessExternalCommandsSlow()
    {
        while (_externalCommands.TryDequeue(out uint cmd))
        {
            ExecuteExternalCommand(cmd);
            if (!IsRunning) return;
        }
        Volatile.Write(ref _hasExternalCommand, 0);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ProcessSimulationStatusSlow()
    {
        while (_statusFromSimulation.TryDequeue(out BiosStatus result))
        {
            if (result != BiosStatus.Success)
            {
                Volatile.Write(ref _isRunning, ProcessorHelpers.TryContinueAfterStatus(result, 1UL, ulong.MaxValue, _regLock, NeedLogEnd));
                return;
            }
        }
        Volatile.Write(ref _hasSimulationStatus, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Step()
    {

        if (_hasCommand)
        {
            if (_hasExternalCommand == 1)
                ProcessExternalCommandsSlow();

            if (_hasSimulationStatus == 1)
                ProcessSimulationStatusSlow();
            Volatile.Write(ref _hasCommand, false);

            if (!IsRunning | IsSleeping) return;
        }

        ulong ip = _registers[RegType.rIP.Int];        

        uint rawInst = _ram.ReadInt32LEUnSafe(ip);
        OpCode opCode = GetOpCode(rawInst);

        ip += 4;

        _registers[RegType.rIP.Int] = ip;
        
        BiosStatus dat = opCode switch
        {
            // === 1. Системные команды ===
            OpCode.NOP => BiosStatus.Success,
            OpCode.END => EndProgramm(),
            OpCode.PRINT => InstructionPRINT(GetReg1(rawInst)),
            // === 2. Работа с памятью (Указатели и регистры) ===
            OpCode.MOV => InstructionMOV(GetReg1(rawInst), GetReg2(rawInst)),

            OpCode.LOAD => InstructionLOAD(GetDataSizeCode(rawInst), GetReg1(rawInst), Get64IntData()),
            OpCode.STORE => InstructionSTORE(GetDataSizeCode(rawInst), GetReg1(rawInst), Get64IntData()),
            OpCode.STORE_IND => InstructionSTORE_IND(GetDataSizeCode(rawInst), GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.LOAD_IND => InstructionLOAD_IND(GetDataSizeCode(rawInst), GetReg1(rawInst), GetReg2(rawInst)),

            OpCode.LOAD_UNSAFE => InstructionLOAD_UNSAFE(GetDataSizeCode(rawInst), GetReg1(rawInst), Get64IntData()),
            OpCode.STORE_UNSAFE => InstructionSTORE_UNSAFE(GetDataSizeCode(rawInst), GetReg1(rawInst), Get64IntData()),
            OpCode.STORE_IND_UNSAFE => InstructionSTORE_IND_UNSAFE(GetDataSizeCode(rawInst), GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.LOAD_IND_UNSAFE => InstructionLOAD_IND_UNSAFE(GetDataSizeCode(rawInst), GetReg1(rawInst), GetReg2(rawInst)),

            OpCode.LDI => InstructionLDI(GetReg1(rawInst), Get64IntData()),

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
            OpCode.JMP => InstructionJMP(Get64IntData()),
            OpCode.JZ => InstructionJZ(Get64IntData()),
            OpCode.JNZ => InstructionJNZ(Get64IntData()),
            OpCode.JG => InstructionJG(Get64IntData()),
            OpCode.JL => InstructionJL(Get64IntData()),

            // === 6. Работа со Стеком ===
            OpCode.PUSH => InstructionPUSH(GetReg1(rawInst)),
            OpCode.POP => InstructionPOP(GetReg1(rawInst)),
            OpCode.CALL => InstructionCALL(Get64IntData()),
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
            OpCode.CMP => InstructionCMP(GetReg1(rawInst), GetReg2(rawInst)),
            OpCode.TEST => InstructionTEST(GetReg1(rawInst), GetReg2(rawInst)),

            _ => SetStatusNotImplementedOpCode(opCode),
        };

        if (dat != BiosStatus.Success)
        {
            Volatile.Write(ref _isRunning, ProcessorHelpers.TryContinueAfterStatus(dat, _lastFaultAddress, ip, _regLock, NeedLogEnd));
        }
    }
    private BiosStatus InstructionCMP(RegType r1, RegType r2)
    {
        ulong res = _registers[r1.Int] - _registers[r2.Int];

        ulong zeroFlag = 1UL ^ ((res | (0UL - res)) >> 63);
        ulong negativeFlag = res >> 63;

        _registers[RegType.rFL.Int] = zeroFlag | (negativeFlag << 1);
        return BiosStatus.Success;
    }

    private BiosStatus InstructionTEST(RegType r1, RegType r2)
    {
        ulong res = _registers[r1.Int] & _registers[r2.Int];
        ulong zeroFlag = 1UL ^ ((res | (0UL - res)) >> 63);
        ulong negativeFlag = res >> 63;

        _registers[RegType.rFL.Int] = zeroFlag | (negativeFlag << 1);
        return BiosStatus.Success;
    }


    private BiosStatus SetStatusNotImplementedOpCode(OpCode code)
    {
        _lastFaultAddress = (ulong)code;
        return BiosStatus.NotImplementedOpCode;
    }

    private BiosStatus InstructionHALT()
    {
        Volatile.Write(ref _sleeping, 1);
        Volatile.Write(ref _hasCommand, true);
        return BiosStatus.Success;
    }

    private BiosStatus InstructionWAKE_INT(RegType reg1)
    {
        ulong regV = _registers[reg1.Int];
        if (!_portBus.WakeProcessor(regV))
        {
            ulong ip = _registers[RegType.rIP.Int];
            _lastFaultAddress = ip;
            return BiosStatus.NullDeviceOutput;
        }

        return BiosStatus.Success;
    }

    private BiosStatus InstructionDIV(RegType reg1, RegType reg2)
    {
        ulong divisor = _registers[reg2.Int];
        if (divisor == 0)
        {
            ulong ip = _registers[RegType.rIP.Int];
            _lastFaultAddress = ip;
            return BiosStatus.DivOnZero;
        }
        ulong dividend = _registers[reg1.Int];
        ulong res = dividend / divisor;
        _registers[reg1.Int] = res;
        return BiosStatus.Success;
    }

    private BiosStatus InstructionSHR(RegType reg1, RegType reg2)
    {
        ulong regv1 = _registers[reg1.Int];
        byte regv2 = (byte)_registers[reg2.Int];
        ulong res = regv1 >> regv2;
        _registers[reg1.Int] = res;
        return BiosStatus.Success;
    }

    private BiosStatus InstructionMULT_INT(RegType reg1, RegType reg2)
    {
        ulong regv1 = _registers[reg1.Int];
        ulong regv2 = _registers[reg2.Int];
        ulong res = regv1 * regv2;
        _registers[reg1.Int] = res;
        return BiosStatus.Success;
    }

    private BiosStatus InstructionALLOC(RegType reg1)
    {
        ulong size = _registers[RegType.r0.Int];
        ulong hp = _registers[RegType.rHP.Int];
        // выравниваем размер вверх до кратности 8
        if (size % 8 != 0) size = (size + 7) & ~7UL;
        ulong result = hp;
        ulong newHp = hp + size;
        if (newHp >= _registers[(int)RegType.rSP])   // коллизия с стеком
        {
            _lastFaultAddress = result;
            return BiosStatus.SegmentationFault;
        }
        _registers[RegType.rHP.Int] = newHp;

        if (reg1 != RegType.r0)
            _registers[reg1.Int] = result;
        else
            _registers[RegType.r0.Int] = result;

        return BiosStatus.Success;
    }

    private BiosStatus InstructionINT(RegType reg)
    {
        uint vector = (uint)_registers[reg.Int] & 0x1F; // 32 вектора
        ulong tableBase = _registers[RegType.rTB.Int];
        ulong handlerAddr;
        RAMResultInt64 readResult = _ram.ReadInt64LE(tableBase + (ulong)vector * 8);
        if (!readResult.IsSuccess)
        {
            _lastFaultAddress = readResult.FaultAddress;
            return readResult.Status;
        }

        handlerAddr = readResult.Data;

        // Сохраняем текущий IP в стек
        ulong ip = _registers[RegType.rIP.Int];
        ulong sp = _registers[RegType.rSP.Int];
        sp -= 8;
        RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, ip);

        if (!writeResult.IsSuccess)
        {
            _lastFaultAddress = sp;
            return writeResult.Status;
        }

        _registers[RegType.rSP.Int] = sp;
        if (handlerAddr == 0)
        {
            ConsoleLock("handlerAddr == 0");
            return EndProgramm();
        }
        // Переходим на обработчик
        _registers[RegType.rIP.Int] = handlerAddr;
        return BiosStatus.Success;
    }

    private BiosStatus InstructionIRET()
    {
        // Восстанавливаем IP из стека
        ulong sp = _registers[RegType.rSP.Int];
        RAMResultInt64 readResult = _ram.ReadInt64LE(sp);
        if (!readResult.IsSuccess)
        {
            _lastFaultAddress = sp;
            return readResult.Status;
        }

        sp += 8;
        _registers[RegType.rIP.Int] = readResult.Data;
        _registers[RegType.rSP.Int] = sp;
        return BiosStatus.Success;
    }

    public BiosStatus EndProgramm()
    {
        Volatile.Write(ref _isRunning, 0);
        return BiosStatus.EndProgramm;
    }

    public BiosStatus InstructionPRINT_INT(RegType reg)
    {
        ConsoleLock(_registers[reg.Int].ToString());
        return BiosStatus.Success;
    }

    private BiosStatus InstructionPRINT(RegType reg)
    {
        LoggerProvider.CharOutPut((char)_registers[reg.Int]);
        return BiosStatus.Success;
    }

    private BiosStatus InstructionMOV(RegType reg1, RegType reg2)
    {
        _registers[reg1.Int] = _registers[reg2.Int];
        return BiosStatus.Success;
    }
    private BiosStatus InstructionLOAD(OpCodeSize sizeT, RegType reg, ulong adress)
    {

        switch (sizeT)
        {
            case OpCodeSize.S8:
                RAMResultInt8 data8 = _ram.ReadInt8LE(adress);
                if (data8.IsSuccess)
                {
                    _registers[reg.Int] = data8.Data;
                    return BiosStatus.Success;
                }
                _lastFaultAddress = data8.FaultAddress;
                return data8.Status;


            case OpCodeSize.S16:
                RAMResultInt16 data16 = _ram.ReadInt16LE(adress);
                if (data16.IsSuccess)
                {
                    _registers[reg.Int] = data16.Data;

                    return BiosStatus.Success;
                }
                _lastFaultAddress = data16.FaultAddress;
                return data16.Status;

            case OpCodeSize.S32:
                RAMResultInt32 data32 = _ram.ReadInt32LE(adress);
                if (data32.IsSuccess)
                {
                    _registers[reg.Int] = data32.Data;
                    return BiosStatus.Success;
                }
                _lastFaultAddress = data32.FaultAddress;
                return data32.Status;

            case OpCodeSize.S64:
                RAMResultInt64 data64 = _ram.ReadInt64LE(adress);
                if (data64.IsSuccess)
                {
                    _registers[reg.Int] = data64.Data;
                    return BiosStatus.Success;
                }
                _lastFaultAddress = data64.FaultAddress;
                return data64.Status;

            default:
                // Защита на случай передачи некорректного или нереализованного OpCodeSize
                _lastFaultAddress = adress;
                return BiosStatus.SegmentationFault;
        }


    }

    private BiosStatus InstructionSTORE(OpCodeSize sizeT, RegType reg, ulong adress)
    {

        switch (sizeT)
        {
            case OpCodeSize.S8:
                RAMResultInt8 data8 = _ram.WriteInt8LE(adress, (byte)_registers[reg.Int]);
                if (data8.IsSuccess)
                {
                    return BiosStatus.Success;
                }
                _lastFaultAddress = data8.FaultAddress;
                return data8.Status;

            case OpCodeSize.S16:
                RAMResultInt16 data16 = _ram.WriteInt16LE(adress, (ushort)_registers[reg.Int]);
                if (data16.IsSuccess)
                {
                    return BiosStatus.Success;
                }
                _lastFaultAddress = data16.FaultAddress;
                return data16.Status;


            case OpCodeSize.S32:
                RAMResultInt32 data32 = _ram.WriteInt32LE(adress, (uint)_registers[reg.Int]);
                if (data32.IsSuccess)
                {
                    return BiosStatus.Success;
                }
                _lastFaultAddress = data32.FaultAddress;
                return data32.Status;


            case OpCodeSize.S64:
                RAMResultInt64 data64 = _ram.WriteInt64LE(adress, _registers[reg.Int]);
                if (data64.IsSuccess)
                {
                    return BiosStatus.Success;
                }
                _lastFaultAddress = data64.FaultAddress;
                return data64.Status;


            default:
                _lastFaultAddress = adress;
                return BiosStatus.AlignmentFault;
        }
    }

    private BiosStatus InstructionLOAD_IND(OpCodeSize sizeT, RegType reg1, RegType reg2)
    {
        return InstructionLOAD(sizeT, reg1, _registers[reg2.Int]);
    }

    private BiosStatus InstructionSTORE_IND(OpCodeSize sizeT, RegType reg1, RegType reg2)
    {
        return InstructionSTORE(sizeT, reg1, _registers[reg2.Int]);
    }
    private BiosStatus InstructionLOAD_UNSAFE(OpCodeSize sizeT, RegType reg, ulong adress)
    {

        switch (sizeT)
        {
            case OpCodeSize.S8:
                var data8 = _ram.ReadInt8LEUnSafe(adress);
                _registers[reg.Int] = data8;
                return BiosStatus.Success;


            case OpCodeSize.S16:
                var data16 = _ram.ReadInt16LEUnSafe(adress);
                _registers[reg.Int] = data16;
                return BiosStatus.Success;


            case OpCodeSize.S32:
                var data32 = _ram.ReadInt32LEUnSafe(adress);
                
                _registers[reg.Int] = data32;

                return BiosStatus.Success;


            case OpCodeSize.S64:
                var data64 = _ram.ReadInt64LEUnSafe(adress);
                _registers[reg.Int] = data64;

                return BiosStatus.Success;
        }
        _lastFaultAddress = adress;
        return BiosStatus.SegmentationFault;
    }

    private BiosStatus InstructionSTORE_UNSAFE(OpCodeSize sizeT, RegType reg, ulong adress)
    {

        switch (sizeT)
        {
            case OpCodeSize.S8:
                var data8 = (byte)_registers[reg.Int];
                _ram.WriteInt8LEUnSafe(adress, data8);
                return BiosStatus.Success;

            case OpCodeSize.S16:
                var data16 = (ushort)_registers[reg.Int];
                _ram.WriteInt16LEUnSafe(adress, data16);
                return BiosStatus.Success;


            case OpCodeSize.S32:
                var data32 = (uint)_registers[reg.Int];
                _ram.WriteInt32LEUnSafe(adress, data32);
                return BiosStatus.Success;


            case OpCodeSize.S64:
                var data64 = _registers[reg.Int];
                _ram.WriteInt64LEUnSafe(adress, data64);
                return BiosStatus.Success;
        }
        _lastFaultAddress = adress;
        return BiosStatus.AlignmentFault;
    }

    private BiosStatus InstructionLOAD_IND_UNSAFE(OpCodeSize sizeT, RegType reg1, RegType reg2)
    {
        return InstructionLOAD_UNSAFE(sizeT, reg1, _registers[reg2.Int]);
    }

    private BiosStatus InstructionSTORE_IND_UNSAFE(OpCodeSize sizeT, RegType reg1, RegType reg2)
    {
        return InstructionSTORE_UNSAFE(sizeT, reg1, _registers[reg2.Int]);
    }

    private BiosStatus InstructionLDI(RegType reg, ulong value)
    {
        _registers[reg.Int] = value;
        return BiosStatus.Success;
    }

    private BiosStatus InstructionADD(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 + value2;
        _registers[reg1.Int] = res;

        return BiosStatus.Success;
    }

    private BiosStatus InstructionSUB(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 - value2;
        _registers[reg1.Int] = res;

        return BiosStatus.Success;
    }

    private BiosStatus InstructionINC(RegType reg)
    {
        ulong value1 = _registers[reg.Int];
        ulong res = value1 + 1;
        _registers[reg.Int] = res;

        return BiosStatus.Success;
    }

    private BiosStatus InstructionDEC(RegType reg)
    {
        ulong value1 = _registers[reg.Int];
        ulong res = value1 - 1;
        _registers[reg.Int] = res;

        return BiosStatus.Success;
    }

    private BiosStatus InstructionAND(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 & value2;
        _registers[reg1.Int] = res;

        return BiosStatus.Success;
    }

    private BiosStatus InstructionOR(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 | value2;
        _registers[reg1.Int] = res;

        return BiosStatus.Success;
    }

    private BiosStatus InstructionXOR(RegType reg1, RegType reg2)
    {
        ulong value1 = _registers[reg1.Int];
        ulong value2 = _registers[reg2.Int];
        ulong res = value1 ^ value2;
        _registers[reg1.Int] = res;

        return BiosStatus.Success;
    }

    private BiosStatus InstructionNOT(RegType reg)
    {
        ulong value = _registers[reg.Int];
        ulong res = ~value;
        _registers[reg.Int] = res;

        return BiosStatus.Success;
    }
    private BiosStatus InstructionJMP(ulong targetAddress)
    {
        _registers[RegType.rIP.Int] = targetAddress;
        return BiosStatus.Success;
    }

    private BiosStatus InstructionJZ(ulong t) 
    { 
        if (ZeroFlagSet) _registers[RegType.rIP.Int] = t; 
        return BiosStatus.Success; 
    }
    private BiosStatus InstructionJNZ(ulong t) 
    { 
        if (!ZeroFlagSet) _registers[RegType.rIP.Int] = t;
        return BiosStatus.Success; 
    }
    private BiosStatus InstructionJL(ulong t) 
    { 
        if (NegativeFlagSet) _registers[RegType.rIP.Int] = t;
        return BiosStatus.Success; 
    }
    private BiosStatus InstructionJG(ulong t) 
    { 
        if (!NegativeFlagSet & !ZeroFlagSet) _registers[RegType.rIP.Int] = t;
        return BiosStatus.Success; 
    }

    private BiosStatus InstructionPUSH(RegType reg)
    {
        ulong value = _registers[reg.Int];
        ulong sp = _registers[RegType.rSP.Int];

        sp -= 8;

        RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, value);

        if (!writeResult.IsSuccess)
        {
            _lastFaultAddress = sp;
            return writeResult.Status;
        }

        if (_registers[RegType.rHP.Int] >= sp)   // коллизия с стеком
        {
            _lastFaultAddress = writeResult.Data;
            return BiosStatus.SegmentationFault;
        }

        _registers[RegType.rSP.Int] = sp;
        return BiosStatus.Success;
    }

    private BiosStatus InstructionPOP(RegType reg)
    {
        ulong sp = _registers[RegType.rSP.Int];

        RAMResultInt64 readResult = _ram.ReadInt64LE(sp);

        if (!readResult.IsSuccess)
        {
            _lastFaultAddress = sp;
            return readResult.Status;
        }

        sp += 8;

        _registers[reg.Int] = readResult.Data;
        _registers[RegType.rSP.Int] = sp;

        return BiosStatus.Success;
    }
    

    private BiosStatus InstructionCALL(ulong targetAddress)
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
            if (_registers[RegType.rHP.Int] >= sp)   // коллизия с стеком
            {
                return BiosStatus.SegmentationFault;
            }
            _ram.WriteInt64LEUnSafe(sp, currentLink);

            _registers[RegType.rSP.Int] = sp;
        }

        _registers[RegType.rCL.Int] = ip;

        callDepth++;

        ip = targetAddress;

        _registers[RegType.rCD.Int] = callDepth;
        _registers[RegType.rIP.Int] = ip;

        return BiosStatus.Success;
    }

    // Возврат из подпрограммы (RET)
    private BiosStatus InstructionRET()
    {
        ulong callDepth = _registers[RegType.rCD.Int];
        ulong ip = _registers[RegType.rIP.Int];

        if (callDepth == 0)
        {
            ConsoleLock("Ошибка: RET вызван без предшествующего CALL. Программа остановлена.");
            _lastFaultAddress = ip;
            return (BiosStatus)9;
        }

        ulong returnAddress = _registers[RegType.rCL.Int];
        ip = returnAddress;


        callDepth--;

        if (callDepth > 0)
        {
            ulong sp = _registers[RegType.rSP.Int];

            _registers[RegType.rCL.Int] = _ram.ReadInt64LEUnSafe(sp);
            sp += 8;
            _registers[RegType.rSP.Int] = sp;
        }
        else
        {
            _registers[RegType.rCL.Int] = 0;
        }

        _registers[RegType.rCD.Int] = callDepth;
        _registers[RegType.rIP.Int] = ip;
        return BiosStatus.Success;
    }

    private BiosStatus InstructionIN(RegType regDest, RegType regPort)
    {
        ulong portAddress = _registers[regPort.Int];
        RAMResultInt8 result = _portBus.ReadPort(portAddress);
        if (!result.IsSuccess)
            return result.Status;

        _registers[regDest.Int] = result.Data;
        return BiosStatus.Success;
    }

    private BiosStatus InstructionOUT(RegType regSrc, RegType regPort)
    {
        ulong portAddress = _registers[regPort.Int];
        byte value = (byte)(_registers[regSrc.Int] & 0xFF);
        RAMResultInt8 result = _portBus.WritePort(portAddress, value);
        if (!result.IsSuccess)
            return result.Status;

        // Флаги обычно не меняются при выводе, но можно обновить по желанию
        return BiosStatus.Success;
    }

    public void Reset()
    {
        lock (_regLock)
        {
            _lastFaultAddress = 0;
            _isRunning = 0;
            _sleeping = 0;
            ClearRegs();
            while (_externalCommands.TryDequeue(out _));
            while (_statusFromSimulation.TryDequeue(out _));
            _hasCommand = false;
            _hasExternalCommand = 0;
            _hasSimulationStatus = 0;
        }
    }

    private void ClearRegs()
    {
        for (int i = 0; i < CountReg; i++) _registers[i] = 0UL;
    }

    public void InitRegHP(ulong hpInit) => _registers[RegType.rHP.Int] = hpInit;

    public void EnqueueExternalCommand(uint instruction)
    {
        Volatile.Write(ref _hasExternalCommand, 1);
        Volatile.Write(ref _hasCommand, true);
        _externalCommands.Enqueue(instruction);
    }

    private void ExecuteExternalCommand(uint rawInst)
    {
        _hasCommand = true;
        OpCode opCode = GetOpCode(rawInst);
        switch (opCode)
        {
            case OpCode.END: _isRunning = 0; return;
            case OpCode.HALT: _sleeping = 1; return;
            case OpCode.WAKE: _sleeping = 0; return;

            default: return;
        }
    }
}

