using Kernel.ControllersData;
using System.Text;
using System.Threading.Channels;

namespace VMApplication.Project.IO;

/// <summary>
/// Раскладка портов устройства вывода. Устройство занимает 8 байт.
/// Оффсеты — относительные (0..WindowSize-1) либо абсолютные, если
/// <see cref="IPortController.BaseAddress"/> != 0.
/// </summary>
public static class OutputPortLayout
{
    public const ulong WindowSize = 8;

    /// <summary>Запись байта на вывод. Чтение возвращает 0.</summary>
    public const ulong Data = 0;

    /// <summary>Статус: 0 — готово, 1 — есть необработанные данные в очереди.</summary>
    public const ulong Status = 1;

    /// <summary>Запись 1 — сбросить буфер вывода (для Queued — дождаться опустошения очереди).</summary>
    public const ulong Flush = 2;
}

/// <summary>
/// Способ кодирования символов при записи в порт данных.
/// </summary>
public enum PortCharEncoding
{
    /// <summary>1 Write = 1 символ (Latin-1). Простейший режим.</summary>
    BytePerChar,

    /// <summary>1..4 Write = 1 символ, полноценный UTF-8 (через Decoder).</summary>
    Utf8,

    /// <summary>2 Write = 1 символ, UTF-16 little-endian.</summary>
    Utf16
}

public sealed class SynchronousIOStream : IPortController
{
    private readonly TextWriter _writer;
    private readonly PortCharEncoding _encoding;
    private readonly Decoder? _utf8Decoder;
    private readonly char[] _charBuffer = new char[4];
    private byte? _pendingLowByte;
    private int _disposed;

    public SynchronousIOStream(TextWriter writer, PortCharEncoding encoding)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _encoding = encoding;

        if (encoding == PortCharEncoding.Utf8)
            _utf8Decoder = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetDecoder();
    }

    public byte ReadPort(ulong offset) => offset switch
    {
        OutputPortLayout.Status => 0, // синхронное устройство всегда готово
        _ => 0
    };

    public void WritePort(ulong offset, byte value)
    {
        ThrowIfDisposed();
        switch (offset)
        {
            case OutputPortLayout.Data:
                EmitByte(value);
                break;

            case OutputPortLayout.Flush:
                _writer.Flush();
                break;
        }
    }

    public void WakeProcessor()
    {
        // Устройству вывода будить CPU не нужно — это для устройств ввода.
    }

    private void EmitByte(byte value)
    {
        switch (_encoding)
        {
            case PortCharEncoding.BytePerChar:
                _writer.Write((char)value);
                _writer.Flush();
                break;

            case PortCharEncoding.Utf16:
                if (_pendingLowByte is null)
                {
                    _pendingLowByte = value;
                    return;
                }
                char c = (char)(_pendingLowByte.Value | (value << 8));
                _pendingLowByte = null;
                _writer.Write(c);
                _writer.Flush();
                break;

            case PortCharEncoding.Utf8:
                Span<byte> one = [value];
                int charsUsed = _utf8Decoder!.GetChars(one, _charBuffer, flush: false);
                if (charsUsed > 0)
                {
                    _writer.Write(_charBuffer, 0, charsUsed);
                    _writer.Flush();
                }
                break;
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        try { _writer.Flush(); } catch { /* writer не наш — не закрываем */ }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed != 0, this);
}


/// <summary>
/// Асинхронный драйвер устройства вывода.
/// WritePort кладёт байт в потокобезопасный канал и сразу возвращает
/// управление CPU. Фоновой таск разгружает очередь.
/// </summary>
public sealed class QueuedIOStream : IPortController
{
    private readonly TextWriter _writer;
    private readonly PortCharEncoding _encoding;
    private readonly Channel<byte> _channel;
    private readonly Task _drainTask;
    private readonly CancellationTokenSource _cts = new();

    // Ручной счётчик: ChannelReader.Count у unbounded-каналов не поддерживается.
    private int _pendingCount;
    private int _disposed;

    public QueuedIOStream(TextWriter writer, PortCharEncoding encoding)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _encoding = encoding;

        _channel = Channel.CreateUnbounded<byte>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });

        _drainTask = Task.Run(DrainAsync);
    }

    public byte ReadPort(ulong offset) => offset switch
    {
        // 0 — очередь пуста, 1 — есть необработанные байты
        OutputPortLayout.Status => (byte)(Volatile.Read(ref _pendingCount) > 0 ? 1 : 0),
        _ => 0
    };

    public void WritePort(ulong offset, byte value)
    {
        ThrowIfDisposed();
        switch (offset)
        {
            case OutputPortLayout.Data:
                if (!_channel.Writer.TryWrite(value))
                    throw new InvalidOperationException("Output queue is closed.");
                Interlocked.Increment(ref _pendingCount);
                break;

            case OutputPortLayout.Flush:
                // Best-effort: ждём, пока очередь опустеет, и флашим writer.
                var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(2);
                while (Volatile.Read(ref _pendingCount) > 0 && DateTime.UtcNow < deadline)
                    Thread.Sleep(1);
                _writer.Flush();
                break;
        }
    }

    public void WakeProcessor()
    {
        // Устройству вывода будить CPU не нужно.
    }

    private async Task DrainAsync()
    {
        byte? pendingLowByte = null;
        Decoder? utf8Decoder = _encoding == PortCharEncoding.Utf8
            ? new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetDecoder()
            : null;

        byte[] single = new byte[1];
        char[] chars = new char[4];

        try
        {
            await foreach (byte b in _channel.Reader.ReadAllAsync(_cts.Token))
            {
                switch (_encoding)
                {
                    case PortCharEncoding.BytePerChar:
                        _writer.Write((char)b);
                        break;

                    case PortCharEncoding.Utf16:
                        if (pendingLowByte is null)
                        {
                            // Ждём второй байт — но счётчик уже уменьшаем:
                            // первый байт фактически "поглощён" буфером драйвера.
                            pendingLowByte = b;
                            Interlocked.Decrement(ref _pendingCount);
                            continue;
                        }
                        char c = (char)(pendingLowByte.Value | (b << 8));
                        pendingLowByte = null;
                        _writer.Write(c);
                        break;

                    case PortCharEncoding.Utf8:
                        single[0] = b;
                        int charsUsed = utf8Decoder!.GetChars(single, chars, flush: false);
                        if (charsUsed > 0)
                            _writer.Write(chars, 0, charsUsed);
                        break;
                }
                _writer.Flush();
                Interlocked.Decrement(ref _pendingCount);
            }
        }
        catch (OperationCanceledException) { /* нормальное завершение */ }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        _channel.Writer.TryComplete();
        try { _drainTask.Wait(TimeSpan.FromSeconds(5)); }
        catch (AggregateException) { }

        try { _writer.Flush(); } catch { }
        _cts.Dispose();
    }
    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed != 0, this);
}

public static class PortControllerExtensions
{
    extension(IPortUse controller)
    {
        /// <summary>
        /// Записать строку в устройство вывода через порт Data.
        /// Один символ может потребовать 1..4 записей в зависимости от кодировки.
        /// </summary>
        public void WriteString(ReadOnlySpan<char> text, PortCharEncoding encoding)
        {
            foreach (char c in text)
            {
                switch (encoding)
                {
                    case PortCharEncoding.BytePerChar:
                        controller.WritePort(OutputPortLayout.Data, (byte)c);
                        break;

                    case PortCharEncoding.Utf16:
                        controller.WritePort(OutputPortLayout.Data, (byte)(c & 0xFF));
                        controller.WritePort(OutputPortLayout.Data, (byte)(c >> 8));
                        break;

                    case PortCharEncoding.Utf8:
                        foreach (byte b in Encoding.UTF8.GetBytes(c.ToString()))
                            controller.WritePort(OutputPortLayout.Data, b);
                        break;
                }
            }
        }
    }
}