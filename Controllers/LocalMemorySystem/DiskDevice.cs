using Kernel.Common;
using Kernel.ControllersData;
using System.Buffers.Binary;

namespace Kernel.LocalMemorySystem;

public sealed class DiskDevice : IPortUse, IDisposable
{
    public const int SectorSize = 512;

    // Смещения портов внутри сектора устройства
    public const byte PortData = 0;      // байт данных (чтение/запись с автоинкрементом)
    public const byte PortCommand = 1;   // команда (запись)
    public const byte PortStatus = 2;    // статус (чтение)
    public const byte PortLba0 = 3;      // младший байт LBA
    public const byte PortLba1 = 4;
    public const byte PortLba2 = 5;
    public const byte PortLba3 = 6;      // старший байт LBA
    public const byte PortErrorCode = 7; // код ошибки (чтение)

    // Команды
    private const byte CmdReadSector = 0x01;
    private const byte CmdWriteSector = 0x02;
    private const byte CmdIdentify = 0x03;

    private readonly FileStream _file;
    private readonly int _countSectors;
    private readonly long _sizeFile;
    private readonly byte[] _sectorBuffer = new byte[SectorSize];
    private int _bufferPos;
    private uint _currentLba;
    private bool _busy;
    private bool _error;
    private byte _errorCode;
    private readonly Lock _lock = new();

    public DiskDevice(string imagePath)
    {
        _file = new(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        ImagePath = imagePath;
        _sizeFile = _file.Length;
        _countSectors = (int)_sizeFile / SectorSize;
    }

    public DateTime CreatedAt { get; } = DateTime.Now;
    public long SizeDisk => _sizeFile;
    public int CountSectors => _countSectors;
    public string ImagePath { get; }


    public byte[] SectorBuffer => _sectorBuffer;
    public byte ReadPort(ulong offset)
    {
        lock (_lock)
        {
            return offset switch
            {
                PortData => ReadData(),
                PortStatus => GetStatus(),
                PortErrorCode => _errorCode,
                _ => 0
            };
        }
    }

    public void WritePort(ulong offset, byte value)
    {
        lock (_lock)
        {
            switch (offset)
            {
                case PortData: WriteData(value); break;
                case PortCommand: ExecuteCommand(value); break;
                case PortLba0: _currentLba = (_currentLba & 0xFFFFFF00) | value; break;
                case PortLba1: _currentLba = (_currentLba & 0xFFFF00FF) | ((uint)value << 8); break;
                case PortLba2: _currentLba = (_currentLba & 0xFF00FFFF) | ((uint)value << 16); break;
                case PortLba3: _currentLba = (_currentLba & 0x00FFFFFF) | ((uint)value << 24); break;
            }
        }
    }

    private void ExecuteCommand(byte command)
    {
        _busy = true;
        _error = false;
        _errorCode = 0;

        switch (command)
        {
            case CmdReadSector: ReadSector(); break;
            case CmdWriteSector: WriteSector(); break;
            case CmdIdentify: Identify(); break;
            default:
                _error = true;
                _errorCode = 0x01; // неизвестная команда
                break;
        }

        _busy = false;
    }

    private void ReadSector()
    {
        long offset = (long)_currentLba * SectorSize;
        if (offset + SectorSize > _file.Length)
        {
            _error = true;
            _errorCode = 0x02; // выход за границы диска
            return;
        }
        _file.Seek(offset, SeekOrigin.Begin);
        _file?.Read(_sectorBuffer, 0, SectorSize);
        _bufferPos = 0;
    }

    private void WriteSector()
    {
        long offset = (long)_currentLba * SectorSize;
        if (offset + SectorSize > _file.Length)
        {
            _error = true;
            _errorCode = 0x02;
            return;
        }
        _file.Seek(offset, SeekOrigin.Begin);
        _file.Write(_sectorBuffer, 0, SectorSize);
        _file.Flush(); // гарантируем запись на физический диск
        _bufferPos = 0;
    }

    private void Identify()
    {
        Array.Clear(_sectorBuffer, 0, SectorSize);
        uint totalSectors = (uint)(_file.Length / SectorSize);
        BinaryPrimitives.WriteUInt32LittleEndian(_sectorBuffer.AsSpan(0, 4), totalSectors);
        BinaryPrimitives.WriteUInt16LittleEndian(_sectorBuffer.AsSpan(4, 2), SectorSize);
        _bufferPos = 0;
    }

    private byte ReadData()
    {
        if (_bufferPos >= SectorSize) return 0;
        return _sectorBuffer[_bufferPos++];
    }

    private void WriteData(byte value)
    {
        if (_bufferPos < SectorSize)
            _sectorBuffer[_bufferPos++] = value;
    }

    private byte GetStatus()
    {
        byte status = 0;
        if (_busy) status |= 0x01;
        if (!_busy && !_error) status |= 0x02; // READY
        if (_error) status |= 0x04;
        return status;
    }

    public void WakeProcessor() { }

    public void Dispose()
    {
        _file?.Dispose();
        GC.SuppressFinalize(this);
    }

    public byte[]? ReadSectorDirect(uint lba)
    {
        var result = new byte[SectorSize];
        long offset = lba * SectorSize;
        if (offset + SectorSize > _file.Length)
        {
            _error = true;
            _errorCode = 0x02; // выход за границы диска
            return null;
        }
        _file.Seek(offset, SeekOrigin.Begin);
        _file?.Read(result, 0, SectorSize);
        return result;
    }

    public bool WriteSectorDirect(uint lba, ReadOnlySpan<byte> data)
    {
        if (data.Length != SectorSize)
            throw new ArgumentException($"Длина данных должна быть ровно {SectorSize} байт", nameof(data));

        long offset = (long)lba * SectorSize;
        if (offset + SectorSize > _file.Length)
        {
            _error = true;
            _errorCode = 0x02;
            return false;
        }

        lock (_lock)
        {
            _file.Seek(offset, SeekOrigin.Begin);
            _file.Write(data);
            _file.Flush();
        }
        return true;
    }

    public bool WriteSectorDirect(uint lba, byte[] data) => WriteSectorDirect(lba, data.AsSpan());
}