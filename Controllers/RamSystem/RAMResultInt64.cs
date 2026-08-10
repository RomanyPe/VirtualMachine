using Kernel.BiosSystem;
using Kernel.Common;

namespace Kernel.RamSystem;

public readonly struct RAMResultInt64
{
    public readonly ulong Data;
    private readonly ulong _statusAndAddress;
    public readonly NameDeviceToken NameDeviceToken;

    public BiosStatus Status => (BiosStatus)(_statusAndAddress >> 48);

    public ulong FaultAddress => _statusAndAddress & 0x0000FFFFFFFFFFFFUL;

    public bool IsSuccess => Status == BiosStatus.Success;

    /// <summary>
    /// Конструктор для успеха
    /// </summary>
    /// <param name="data"> Данные для передачи </param>
    public RAMResultInt64(ulong data)
    {
        Data = data;
        _statusAndAddress = 0;
    }

    /// <summary>
    /// Конструктор для ошибки
    /// </summary>
    /// <param name="status"> Статус ошибки </param>
    /// <param name="adress"> Адресс ошибки в памяти</param>
    public RAMResultInt64(BiosStatus status, ulong adress, NameDeviceToken nameDeviceToken)
    {
        NameDeviceToken = nameDeviceToken;
        Data = 0;
        _statusAndAddress = ((ulong)status << 48) | (adress & 0x0000FFFFFFFFFFFFUL);
    }
}


public readonly struct RAMResultInt32
{
    public readonly uint Data;
    private readonly ulong _statusAndAddress;
    public readonly NameDeviceToken NameDeviceToken;

    public BiosStatus Status => (BiosStatus)(_statusAndAddress >> 48);

    public ulong FaultAddress => _statusAndAddress & 0x0000FFFFFFFFFFFFUL;

    public bool IsSuccess => Status == BiosStatus.Success;

    /// <summary>
    /// Конструктор для успеха
    /// </summary>
    /// <param name="data"> Данные для передачи </param>
    public RAMResultInt32(uint data)
    {
        Data = data;
        _statusAndAddress = 0;
    }

    /// <summary>
    /// Конструктор для ошибки
    /// </summary>
    /// <param name="status"> Статус ошибки </param>
    /// <param name="adress"> Адресс ошибки в памяти</param>
    public RAMResultInt32(BiosStatus status, ulong adress, NameDeviceToken nameDeviceToken)
    {
        NameDeviceToken = nameDeviceToken;
        Data = 0;
        _statusAndAddress = ((ulong)status << 48) | (adress & 0x0000FFFFFFFFFFFFUL);
    }
}

public readonly struct RAMResultInt16
{
    public readonly ushort Data;
    private readonly ulong _statusAndAddress;
    public readonly NameDeviceToken NameDeviceToken;
    public BiosStatus Status => (BiosStatus)(_statusAndAddress >> 48);

    public ulong FaultAddress => _statusAndAddress & 0x0000FFFFFFFFFFFFUL;

    public bool IsSuccess => Status == BiosStatus.Success;

    /// <summary> Конструктор для успеха </summary>
    public RAMResultInt16(ushort data)
    {
        Data = data;
        _statusAndAddress = 0;
    }

    /// <summary> Конструктор для ошибки </summary>
    public RAMResultInt16(BiosStatus status, ulong faultAddress, NameDeviceToken nameDeviceToken)
    {
        NameDeviceToken = nameDeviceToken;
        Data = 0;
        _statusAndAddress = ((ulong)status << 48) | (faultAddress & 0x0000FFFFFFFFFFFFUL);

    }
}

public readonly struct RAMResultInt8
{
    public readonly byte Data;
    private readonly ulong _statusAndAddress;
    public readonly NameDeviceToken NameDeviceToken;

    public BiosStatus Status => (BiosStatus)(_statusAndAddress >> 48);

    public ulong FaultAddress => _statusAndAddress & 0x0000FFFFFFFFFFFFUL;

    public bool IsSuccess => Status == BiosStatus.Success;

    /// <summary> Конструктор для успеха </summary>
    public RAMResultInt8(byte data)
    {
        Data = data;
        _statusAndAddress = 0;
    }

    /// <summary> Конструктор для ошибки </summary>
    public RAMResultInt8(BiosStatus status, ulong faultAddress, NameDeviceToken nameDeviceToken)
    {
        NameDeviceToken = nameDeviceToken;
        Data = 0;
        _statusAndAddress = ((ulong)status << 48) | (faultAddress & 0x0000FFFFFFFFFFFFUL);
    }
}
