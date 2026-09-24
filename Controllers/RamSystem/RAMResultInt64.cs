using Kernel.Common;

namespace Kernel.RamSystem;

public readonly struct RAMResultInt64
{
    private const ulong ErrorFlag = 1UL << 63;
    private const int StatusShift = 48;
    private const ulong AddressMask = 0x0000FFFFFFFFFFFFUL;

    private readonly ulong _packed;

    public bool IsSuccess => (_packed & ErrorFlag) == 0;

    public ulong Data => IsSuccess ? _packed : 0;

    public BiosStatus Status =>
        IsSuccess ? BiosStatus.Success : (BiosStatus)((_packed >> StatusShift) & 0xFF);

    public ulong FaultAddress => IsSuccess ? 0 : (_packed & AddressMask);

    /// <summary> Успех. data должно быть меньше 2^63. </summary>
    public RAMResultInt64(ulong data)
    {
        if ((data & ErrorFlag) != 0)
            throw new ArgumentOutOfRangeException(nameof(data),
                "Data must fit in 63 bits (0 .. 2^63-1).");
        _packed = data;
    }

    /// <summary> Ошибка. </summary>
    public RAMResultInt64(BiosStatus status, ulong address)
    {
        if (status == BiosStatus.Success)
            throw new ArgumentException("Use success constructor for Success.", nameof(status));
        if ((ulong)status > 0xFF)
            throw new ArgumentOutOfRangeException(nameof(status), "Status must fit in 8 bits.");
        _packed = ErrorFlag | ((ulong)status << StatusShift) | (address & AddressMask);
    }
}

public readonly struct RAMResultInt32
{
    private const ulong ErrorFlag = 1UL << 63;
    private const int StatusShift = 48;
    private const ulong AddressMask = 0x0000FFFFFFFFFFFFUL;

    private readonly ulong _packed;

    public bool IsSuccess => (_packed & ErrorFlag) == 0;

    public uint Data => IsSuccess ? (uint)_packed : 0;

    public BiosStatus Status =>
        IsSuccess ? BiosStatus.Success : (BiosStatus)((_packed >> StatusShift) & 0xFF);

    public ulong FaultAddress => IsSuccess ? 0 : (_packed & AddressMask);

    public RAMResultInt32(uint data) => _packed = data;

    public RAMResultInt32(BiosStatus status, ulong address)
    {
        if (status == BiosStatus.Success)
            throw new ArgumentException("Use success constructor for Success.", nameof(status));
        if ((ulong)status > 0xFF)
            throw new ArgumentOutOfRangeException(nameof(status), "Status must fit in 8 bits.");
        _packed = ErrorFlag | ((ulong)status << StatusShift) | (address & AddressMask);
    }
}

public readonly struct RAMResultInt16
{
    private const ulong ErrorFlag = 1UL << 63;
    private const int StatusShift = 48;
    private const ulong AddressMask = 0x0000FFFFFFFFFFFFUL;

    private readonly ulong _packed;

    public bool IsSuccess => (_packed & ErrorFlag) == 0;

    public ushort Data => IsSuccess ? (ushort)_packed : (ushort)0;

    public BiosStatus Status =>
        IsSuccess ? BiosStatus.Success : (BiosStatus)((_packed >> StatusShift) & 0xFF);

    public ulong FaultAddress => IsSuccess ? 0 : (_packed & AddressMask);

    public RAMResultInt16(ushort data) => _packed = data;

    public RAMResultInt16(BiosStatus status, ulong address)
    {
        if (status == BiosStatus.Success)
            throw new ArgumentException("Use success constructor for Success.", nameof(status));
        if ((ulong)status > 0xFF)
            throw new ArgumentOutOfRangeException(nameof(status), "Status must fit in 8 bits.");
        _packed = ErrorFlag | ((ulong)status << StatusShift) | (address & AddressMask);
    }
}

public readonly struct RAMResultInt8
{
    private const ulong ErrorFlag = 1UL << 63;
    private const int StatusShift = 48;
    private const ulong AddressMask = 0x0000FFFFFFFFFFFFUL;

    private readonly ulong _packed;

    public bool IsSuccess => (_packed & ErrorFlag) == 0;

    public byte Data => IsSuccess ? (byte)_packed : (byte)0;

    public BiosStatus Status =>
        IsSuccess ? BiosStatus.Success : (BiosStatus)((_packed >> StatusShift) & 0xFF);

    public ulong FaultAddress => IsSuccess ? 0 : (_packed & AddressMask);

    public RAMResultInt8(byte data) => _packed = data;

    public RAMResultInt8(BiosStatus status, ulong address)
    {
        if (status == BiosStatus.Success)
            throw new ArgumentException("Use success constructor for Success.", nameof(status));
        if ((ulong)status > 0xFF)
            throw new ArgumentOutOfRangeException(nameof(status), "Status must fit in 8 bits.");
        _packed = ErrorFlag | ((ulong)status << StatusShift) | (address & AddressMask);
    }
}