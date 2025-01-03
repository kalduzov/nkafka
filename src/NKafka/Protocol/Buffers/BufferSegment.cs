using System.Buffers;
using System.Runtime.CompilerServices;

namespace NKafka.Protocol.Buffers;

internal struct BufferSegment(int size)
{
    private byte[] _buffer = ArrayPool<byte>.Shared.Rent(size);
    private int _written = 0;

    public bool IsNull => _buffer == null;

    public int WrittenCount => _written;
    public Span<byte> WrittenBuffer => _buffer.AsSpan(0, _written);
    public Memory<byte> WrittenMemory => _buffer.AsMemory(0, _written);
    public Span<byte> FreeBuffer => _buffer.AsSpan(_written);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        _written += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        if (_buffer is not null)
        {
            ArrayPool<byte>.Shared.Return(_buffer);
        }
        _buffer = null!;
        _written = 0;
    }
}