using Kernel.Common;
using System.Buffers.Binary;

namespace Kernel.RamSystem;

public class MemoryBus(RamSize size, NameDeviceToken nameDevice, ReadOnlySpan<char> name, byte[] biosRom = null!) : IDisposable
{
    private readonly NameDeviceToken _nameDevice = nameDevice.CreateChild(name);

    private NativeMemoryBuffer _memory = new (size);
    private byte[] _biosRom = biosRom;
    private ulong _biosRomSize = (ulong)biosRom.Length;
    private readonly ulong _ramSize = (ulong)size;
    private readonly ulong _biosRomStartCode = (ulong)size;

    public bool HaveBios => _biosRom != null && _biosRom.Length > 0;
    public ulong RamSize => _ramSize;

    public Span<byte> Span => _memory.AsSpan();
    public Span<byte> AsSpan(int start, int length) => _memory.AsSpan(start, length);
    public Memory<byte> Memory => _memory.AsMemory();
    public Memory<byte> AsMemory() => _memory.AsMemory();
    public Memory<byte> AsMemory(int start, int length) => _memory.AsMemory(start, length);

    public ReadOnlyMemory<byte> ReadOnlyMemory => _memory.AsMemory();

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
        return biosAddress < _biosRomSize
            ? new RAMResultInt8(_biosRom[biosAddress])
            : new RAMResultInt8(BiosStatus.SegmentationFault, address, _nameDevice);
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
        return biosAddress + 2 <= _biosRomSize
            ? MemoryBusHelpers.GenerateInt16Le(_biosRom.AsSpan((int)biosAddress, 2))
            : new RAMResultInt16(BiosStatus.SegmentationFault, (uint)address, _nameDevice);
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
        return biosAddress + 4 <= _biosRomSize
            ? MemoryBusHelpers.GenerateInt32Le(_biosRom.AsSpan((int)biosAddress, 4))
            : new RAMResultInt32(BiosStatus.SegmentationFault, address, _nameDevice);
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
        return biosAddress + 8 <= _biosRomSize
            ? MemoryBusHelpers.GenerateInt64Le(_biosRom.AsSpan((int)biosAddress, 8))
            : new RAMResultInt64(BiosStatus.SegmentationFault, address, _nameDevice);
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

    /// <summary>
    /// Метод для чтения 16 битового целого числа (Int16)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

    public RAMResultInt16 ReadInt16LEUnSafe(ulong address)
    {
        if (address + 2 <= _ramSize)
        {
            return MemoryBusHelpers.GenerateInt16Le(_memory.AsSpan((int)address, 2));
        }

        ulong biosAddress = address - _biosRomStartCode;
        return biosAddress + 2 <= _biosRomSize
            ? MemoryBusHelpers.GenerateInt16Le(_biosRom.AsSpan((int)biosAddress, 2))
            : new RAMResultInt16(BiosStatus.SegmentationFault, (uint)address, _nameDevice);
    }

    /// <summary>
    /// Метод для чтения 32 битового целого числа (Int32)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>
    public RAMResultInt32 ReadInt32LEUnSafe(ulong address)
    {
        if (address + 4 <= _ramSize)
        {
            return MemoryBusHelpers.GenerateInt32Le(_memory.AsSpan((int)address, 4));
        }

        ulong biosAddress = address - _biosRomStartCode;
        return biosAddress + 4 <= _biosRomSize
            ? MemoryBusHelpers.GenerateInt32Le(_biosRom.AsSpan((int)biosAddress, 4))
            : new RAMResultInt32(BiosStatus.SegmentationFault, address, _nameDevice);
    }

    /// <summary>
    /// Метод для чтения 64 битового целого числа (Int64)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

    public RAMResultInt64 ReadInt64LEUnSafe(ulong address)
    {
        if (address + 8 <= _ramSize)
        {
            return MemoryBusHelpers.GenerateInt64Le(_memory.AsSpan((int)address, 8));
        }

        ulong biosAddress = address - _biosRomStartCode;
        return biosAddress + 8 <= _biosRomSize
            ? MemoryBusHelpers.GenerateInt64Le(_biosRom.AsSpan((int)biosAddress, 8))
            : new RAMResultInt64(BiosStatus.SegmentationFault, address, _nameDevice);
    }

    // ==============================
    //              API 
    // ==============================

    /// <summary>
    /// API для очистки памяти
    /// </summary>
    public void ClearMemory() => _memory.Clear();

    public void SetBios(byte[] bios)
    {
        _biosRom = bios ?? [];
        _biosRomSize = (ulong)_biosRom.Length;
    }

    private int _disposed;

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        if (_memory != null)
        {
            ClearMemory();
            _memory.Dispose();
            _memory = null!;
        }

        GC.SuppressFinalize(this);
    }
}
