using Kernel.Common;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Kernel.RamSystem;

public unsafe sealed class NativeMemoryBuffer : IDisposable
{
    private readonly NativeMemoryManager _manager;
    private byte* _ptr;
    private readonly nuint _length;
    private int _isDisposed;

    public NativeMemoryBuffer(RamSize size)
    {
        _length = (nuint)size;
        _ptr = (byte*)NativeMemory.Alloc(_length);
        _manager = new(this);
    }

    public int Length => (int)_length; // для совместимости, но лучше использовать nuint
    public nuint LengthU => _length;
    public byte* Pointer => _ptr;

    public Span<byte> AsSpan() => new(_ptr, (int)_length);
    public ReadOnlySpan<byte> AsReadOnlySpan() => new(_ptr, (int)_length);

    public Span<byte> AsSpan(int start, int length) => new(_ptr + start, length);
    public ReadOnlySpan<byte> AsReadOnlySpan(int start, int length) => new(_ptr + start, length);

    public byte this[int index]
    {
        get => _ptr[index];
        set => _ptr[index] = value;
    }

    public byte this[ulong index]
    {
        get => _ptr[index];
        set => _ptr[index] = value;
    }

    public void Clear() => AsSpan().Clear();

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
        {
            NativeMemory.Free(_ptr);
            _ptr = null;
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

    public ReadOnlyMemory<byte> AsReadOnlyMemory()
    {
        return _manager.Memory;
    }
}

public sealed unsafe class NativeMemoryManager(NativeMemoryBuffer buffer) : MemoryManager<byte>
{
    private readonly int _length = buffer.Length;
    private byte* _ptr = buffer.Pointer; // копия указателя для быстрого доступа

    protected override void Dispose(bool disposing)
    {
        _ptr = null;
    }

    public override Span<byte> GetSpan()
    {
        return _ptr == null ? throw new ObjectDisposedException(nameof(NativeMemoryManager)) : new Span<byte>(_ptr, _length);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if ((uint)elementIndex >= (uint)_length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        return new MemoryHandle(_ptr + elementIndex);
    }

    public override void Unpin() { } // нативная память не перемещается
}