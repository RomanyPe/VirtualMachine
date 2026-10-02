using Kernel.Common;
using System.Buffers.Binary;

namespace Kernel.RamSystem;

public class MemoryBus : IDisposable
{
    private NativeMemoryBuffer _memory;

    public bool HaveBios => _memory.HaveBios;
    public ulong RamSize => _memory.LengthRam;

    public MemoryBus(RamSize size, byte[] biosRom = null!, RamSize sizeBios = Common.RamSize.Size128KB)
    {
        _memory = new(size, sizeBios);
        _memory.SetBios(biosRom.AsSpan());
    }

    public Memory<byte> Memory => _memory.AsMemory();
    public Memory<byte> AsMemory(int start, int length) => _memory.AsMemory(start, length);

    public ReadOnlyMemory<byte> ReadOnlyMemory => _memory.AsMemory();

    /// <summary>
    /// Метод для чтения 8 битового целого числа (Int8)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>
    public RAMResultInt8 ReadInt8LE(ulong address)
    {
        return address < _memory.Length 
            ? new RAMResultInt8(_memory[address]) 
            : new RAMResultInt8(BiosStatus.SegmentationFault, address);
    }

    /// <summary>
    /// Метод для чтения 16 битового целого числа (Int16)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

    public RAMResultInt16 ReadInt16LE(ulong address)
    {
        if ((address & 0x01) != 0) return new RAMResultInt16(BiosStatus.AlignmentFault, (uint)address);

        return address <= _memory.Length - 2
            ? new RAMResultInt16(_memory.ReadUInt16(address))
            : new RAMResultInt16(BiosStatus.SegmentationFault, address);
    }

    /// <summary>
    /// Метод для чтения 32 битового целого числа (Int32)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>
    public RAMResultInt32 ReadInt32LE(ulong address)
    {
        if ((address & 0x03) != 0) return new RAMResultInt32(BiosStatus.AlignmentFault, address);

        return address <= _memory.Length - 4
            ? new RAMResultInt32(_memory.ReadUInt32(address))
            : new RAMResultInt32(BiosStatus.SegmentationFault, address);
    }

    /// <summary>
    /// Метод для чтения 64 битового целого числа (Int64)
    /// </summary>
    /// <param name="address"> Адресс в виртуальной памяти </param>
    /// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

    public RAMResultInt64 ReadInt64LE(ulong address)
    {
        if ((address & 0x07) != 0) return new RAMResultInt64(BiosStatus.AlignmentFault, address);

        return address <= _memory.Length - 8
            ? new RAMResultInt64(_memory.ReadUInt64(address))
            : new RAMResultInt64(BiosStatus.SegmentationFault, address);
    }

    /// <summary>
    /// Метод для записи в память 8 битового целого числа (Int8)
    /// </summary>
    /// <param name="address"> Адрес в виртуальной памяти </param>
    /// <param name="value"> 16 битовое целое число (Int8) </param>
    /// <returns> Успешность операции, при возврате False программа остановится и выведится ошибка </returns>
    public RAMResultInt8 WriteInt8LE(ulong address, byte value)
    {
        if (address >= _memory.Length) return new RAMResultInt8(BiosStatus.SegmentationFault, address);

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
        if ((address & 0x01) != 0) return new RAMResultInt16(BiosStatus.AlignmentFault, address);
        if (address > _memory.Length - 2) return new RAMResultInt16(BiosStatus.SegmentationFault, address);
        
        _memory.WriteUInt16(address, value);

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
        if ((address & 0x03) != 0) return new RAMResultInt32(BiosStatus.AlignmentFault, address);
        if (address > _memory.Length - 4) return new RAMResultInt32(BiosStatus.SegmentationFault, address);

        _memory.WriteUInt32(address, value);
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
        if ((address & 0x07) != 0) return new RAMResultInt64(BiosStatus.AlignmentFault, address);
        if (address > _memory.Length - 8) return new RAMResultInt64(BiosStatus.SegmentationFault, address);

        _memory.WriteUInt64(address, value);
        return new RAMResultInt64(value);
    }

    public void WriteInt8LEUnSafe(ulong address, byte value) => _memory[address] = value;

    public void WriteInt16LEUnSafe(ulong address, ushort value) => _memory.WriteUInt16(address, value);

    public void WriteInt32LEUnSafe(ulong address, uint value) => _memory.WriteUInt32(address, value);

    public void WriteInt64LEUnSafe(ulong address, ulong value) => _memory.WriteUInt64(address, value);

    public byte ReadInt8LEUnSafe(ulong address) => _memory[address];

    public ushort ReadInt16LEUnSafe(ulong address) => _memory.ReadUInt16(address);

    public uint ReadInt32LEUnSafe(ulong address) => _memory.ReadUInt32(address);

    public ulong ReadInt64LEUnSafe(ulong address) => _memory.ReadUInt64(address);

    // ==============================
    //              API 
    // ==============================

    /// <summary>
    /// API для очистки памяти
    /// </summary>
    public void ClearMemory() => _memory.Clear();

    public void SetBios(byte[] bios) => _memory.SetBios(bios.AsSpan());

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
