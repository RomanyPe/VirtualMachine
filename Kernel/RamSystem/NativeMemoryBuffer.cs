using Kernel.Common;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Kernel.RamSystem;

public unsafe sealed class NativeMemoryBuffer : IDisposable
{
    private readonly NativeMemoryManager _manager;

    private nuint _sizeBios;
    private readonly nuint _maxSizeBios;

    private byte* _ptrRam;
    private readonly nuint _sizeRam;

    private int _isDisposed;

    public NativeMemoryBuffer(RamSize sizeRam, RamSize sizeBios = RamSize.Size128KB)
    {
        nuint ram = (nuint)sizeRam;
        nuint bios = (nuint)sizeBios;

        if (bios > int.MaxValue)
            throw new InvalidOperationException(
                $"The maximum BIOS size [{bios}] exceeds the maximum 32-bit number.");

        byte* ptr = (byte*)NativeMemory.AllocZeroed(ram + bios);
        if (ptr == null)
            throw new OutOfMemoryException(
                $"Failed to allocate {ram + bios} bytes for RAM+BIOS.");

        try
        {
            _sizeRam = ram;
            _maxSizeBios = bios;
            _ptrRam = ptr;

            // _manager создаём здесь, чтобы при его падении освободить ptr
            _manager = new NativeMemoryManager(this);
        }
        catch
        {
            NativeMemory.Free(ptr);
            _ptrRam = null;
            throw;
        }
    }


    public bool HaveBios => _sizeBios > 0;
    public nuint LengthRam => _sizeRam;
    public nuint LengthBios => _sizeBios;
    public nuint Length => _sizeRam + _sizeBios; // для совместимости, но лучше использовать nuint
    public nuint MaxLenghtBios => _maxSizeBios;

    public byte* PointerRAM => _ptrRam;
    public byte* PointerBios => _ptrRam + _sizeRam;


    public void SetBios(ReadOnlySpan<byte> bios)
    {
        if (bios.IsEmpty)
        {
            _sizeBios = 0;
            return;
        }
        nuint currentBiosSize = (nuint)bios.Length;
        if (currentBiosSize > _maxSizeBios) throw new InvalidOperationException(
            $"BIOS size {bios.Length} exceeds reserved {_maxSizeBios}.");

        

        Span<byte> span = new(_ptrRam + _sizeRam, (int)_maxSizeBios);
        span.Clear();
        bios.CopyTo(span);
        _sizeBios = currentBiosSize;
    }


    public ushort ReadUInt16(ulong address)
    {
        var value = Unsafe.ReadUnaligned<ushort>(_ptrRam + address);
        return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public uint ReadUInt32(ulong address)
    {
        var value = Unsafe.ReadUnaligned<uint>(_ptrRam + address);
        return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public ulong ReadUInt64(ulong address)
    {
        var value = Unsafe.ReadUnaligned<ulong>(_ptrRam + address);
        return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }


    public void WriteUInt16(ulong address, ushort value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Unsafe.WriteUnaligned(_ptrRam + address, value);
    }

    public void WriteUInt32(ulong address, uint value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Unsafe.WriteUnaligned(_ptrRam + address, value);
    }

    public void WriteUInt64(ulong address, ulong value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Unsafe.WriteUnaligned(_ptrRam + address, value);
    }

    public Span<byte> AsSpan() => new(_ptrRam, (int)_sizeRam);

    public byte this[ulong index]
    {
        get => *(_ptrRam + index);
        set => *(_ptrRam + index) = value;
    }

    public void Clear() => AsSpan().Clear();

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
        {
            NativeMemory.Free(_ptrRam);
            _ptrRam = null;
        }
    }

    public Memory<byte> AsMemory()
    {
        return _manager.Memory;
    }

    public Memory<byte> AsMemory(int start, int length)
    {
        return _manager.Memory.Slice(start, length);
    }
}

public sealed unsafe class NativeMemoryManager(NativeMemoryBuffer buffer) : MemoryManager<byte>
{
    private readonly int _length = (int)buffer.Length;
    private byte* _ptr = buffer.PointerRAM; // копия указателя для быстрого доступа

    protected override void Dispose(bool disposing)
    {
        _ptr = null;
    }

    public override Span<byte> GetSpan()
    {
        return _ptr != null ? 
            new(_ptr, _length) 
            : throw new ObjectDisposedException(nameof(NativeMemoryManager));
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if ((uint)elementIndex >= (uint)_length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));

        return new MemoryHandle(_ptr + elementIndex);
    }

    public override void Unpin() { } // нативная память не перемещается
}