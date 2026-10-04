using Kernel.Contracts;
using System.Text;

namespace VMApplication.Project.IO;

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

    public byte ReadPort(ulong offset) => 0;

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

    public void WakeProcessor() => _writer.Flush();

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
