using Kernel.BiosSystem;
using System.Buffers.Binary;

namespace Kernel.RamSystem;

public class MemoryBus(RamSize size, NameDeviceToken nameDevice, ReadOnlySpan<char> name, byte[] biosRom = null!) : IDisposable
{
    private readonly NameDeviceToken _nameDevice = nameDevice.CreateChild(name);

    private byte[] _memory = MemoryPoolEmulator.Rent(size);
    private readonly byte[] _biosRom = biosRom;
    private readonly RamSize _ramSizeEn = size;
    private readonly ulong _ramSize = (ulong)size;
    private readonly ulong _biosRomSize = (ulong)biosRom.Length;
    private readonly ulong _biosRomStartCode = (ulong)size;

    public bool HaveBios => _biosRom != null && _biosRom.Length > 0;
    public ulong RamSize => _ramSize;
    public byte[] Memory => _memory;

    /// <summary>
    /// Метод для чтения 8 битового целого числа (Int8)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>
    public RAMResultInt8 ReadInt8LE(ulong address)
    {
        if (address < _ramSize)
        {
            return new RAMResultInt8(_memory[address]);
        }

        ulong biosAddress = address - _biosRomStartCode;
        if (biosAddress < _biosRomSize)
        {
            return new RAMResultInt8(_biosRom[biosAddress]);
        }

        return new RAMResultInt8(BiosStatus.SegmentationFault, address, _nameDevice);
    }

    /// <summary>
    /// Метод для чтения 16 битового целого числа (Int16)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

    public RAMResultInt16 ReadInt16LE(ulong address)
    {
        if ((address & 0x01) != 0) return new RAMResultInt16(BiosStatus.AlignmentFault, (uint)address, _nameDevice);

        if (address + 2 <= _ramSize)
        {
            return MemoryBusHelpers.GenerateInt16Le(_memory.AsSpan((int)address, 2));
        }

        ulong biosAddress = address - _biosRomStartCode;
        if (biosAddress + 2 <= _biosRomSize)
        {
            return MemoryBusHelpers.GenerateInt16Le(_biosRom.AsSpan((int)biosAddress, 2));
        }

        return new RAMResultInt16(BiosStatus.SegmentationFault, (uint)address, _nameDevice);
    }


    /// <summary>
    /// Метод для чтения 32 битового целого числа (Int32)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>
    public RAMResultInt32 ReadInt32LE(ulong address)
    {
        if ((address & 0x03) != 0) return new RAMResultInt32(BiosStatus.AlignmentFault, address, _nameDevice);

        if (address + 4 <= _ramSize)
            return MemoryBusHelpers.GenerateInt32Le(_memory.AsSpan((int)address, 4));
        

        ulong biosAddress = address - _biosRomStartCode;
        if (biosAddress + 4 <= _biosRomSize)
            return MemoryBusHelpers.GenerateInt32Le(_biosRom.AsSpan((int)biosAddress, 4));
        

        return new RAMResultInt32(BiosStatus.SegmentationFault, address, _nameDevice);
    }

    /// <summary>
    /// Метод для чтения 64 битового целого числа (Int64)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

    public RAMResultInt64 ReadInt64LE(ulong address)
    {
        if ((address & 0x07) != 0) return new RAMResultInt64(BiosStatus.AlignmentFault, address, _nameDevice);

        if (address + 8 <= _ramSize)
        {
            return MemoryBusHelpers.GenerateInt64Le(_memory.AsSpan((int)address, 8));
        }

        ulong biosAddress = address - _biosRomStartCode;
        if (biosAddress + 8 <= _biosRomSize)
        {
            return MemoryBusHelpers.GenerateInt64Le(_biosRom.AsSpan((int)biosAddress, 8));
        }

        return new RAMResultInt64(BiosStatus.SegmentationFault, address, _nameDevice);
    }

    /// <summary>
    /// Метод для записи в память 8 битового целого числа (Int8)
    /// </summary>
    /// <param name="address"> Адрес в виртуальной памяти </param>
    /// <param name="value"> 16 битовое целое число (Int8) </param>
    /// <returns> Успешность операции, при возврате False программа остановится и выведится ошибка </returns>
    public RAMResultInt8 WriteInt8LE(ulong address, byte value)
    {
        if (address >= _ramSize) return new RAMResultInt8(BiosStatus.SegmentationFault, address, _nameDevice);

        _memory[address] = value;
        return new RAMResultInt8(value);
    }

    /// <summary>
    /// Метод для записи в память 16 битового целого числа (Int16)
    /// </summary>
    /// <param name="address"> Адрес в виртуальной памяти </param>
    /// <param name="value"> 16 битовое целое число (Int16) </param>
    /// <returns> Успешность операции, при возврате False программа остановится и выведится ошибка </returns>
    public RAMResultInt16 WriteInt16LE(ulong address, ushort value)
    {
        if ((address & 0x01) != 0) return new RAMResultInt16(BiosStatus.AlignmentFault, address, _nameDevice);
        if (address + 2 > _ramSize) return new RAMResultInt16(BiosStatus.SegmentationFault, address, _nameDevice);

        BinaryPrimitives.WriteUInt16LittleEndian(_memory.AsSpan((int)address, 2), value);
        return new RAMResultInt16(value);
    }


    /// <summary>
    /// Метод для записи в память 32 битового целого числа (Int32)
    /// </summary>
    /// <param name="address"> Адрес в виртуальной памяти </param>
    /// <param name="value"> 32 битовое целое число (Int32) </param>
    /// <returns> Успешность операции, при возврате False программа остановится и выведится ошибка </returns>
    public RAMResultInt32 WriteInt32LE(ulong address, uint value)
    {
        if ((address & 0x03) != 0) return new RAMResultInt32(BiosStatus.AlignmentFault, address, _nameDevice);
        if (address + 4 > _ramSize) return new RAMResultInt32(BiosStatus.SegmentationFault, address, _nameDevice);

        BinaryPrimitives.WriteUInt32LittleEndian(_memory.AsSpan((int)address, 4), value);
        return new RAMResultInt32(value);
    }
    /// <summary>
    /// Метод для записи в память 64 битового целого числа (Int64)
    /// </summary>
    /// <param name="address"> Адрес в виртуальной памяти </param>
    /// <param name="value"> 64 битовое целое число (Int64) </param>
    /// <returns> Успешность операции, при возврате False программа остановится и выведится ошибка </returns>

    public RAMResultInt64 WriteInt64LE(ulong address, ulong value)
    {
        if ((address & 0x07) != 0) return new RAMResultInt64(BiosStatus.AlignmentFault, address, _nameDevice);
        if (address + 8 > _ramSize) return new RAMResultInt64(BiosStatus.SegmentationFault, address, _nameDevice);

        BinaryPrimitives.WriteUInt64LittleEndian(_memory.AsSpan((int)address, 8), value);
        return new RAMResultInt64(value);
    }



    // ==============================
    //              API 
    // ==============================

    /// <summary>
    /// API для очистки памяти
    /// </summary>
    public void Clear()
    {
        Array.Clear(_memory, 0, _memory.Length);
        MemoryPoolEmulator.Return(_memory, _ramSizeEn);
    }


    public void Dispose()
    {
        if (_memory != null)
        {
            MemoryPoolEmulator.Return(_memory, _ramSizeEn);
            _memory = null!;
        }
        GC.SuppressFinalize(this);
    }
}
