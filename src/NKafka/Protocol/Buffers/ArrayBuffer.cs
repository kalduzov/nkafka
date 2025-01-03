using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NKafka.Protocol.Buffers;

using static GC;

// This class fork from https://github.com/Cysharp/MemoryPack
internal sealed class ArrayBuffer(bool useFirstBuffer, bool pinned, int bufferSize): IBufferWriter<byte>
{
    private static readonly byte[] _noUseFirstBufferSentinel = [];

    private readonly List<BufferSegment> _buffers = []; // add freezed buffer.

    private readonly byte[] _firstBuffer = useFirstBuffer
        ? AllocateUninitializedArray<byte>(bufferSize, pinned)
        : _noUseFirstBufferSentinel; // cache firstBuffer to avoid call ArrayPoo.Rent/Return

    private int _firstBufferWritten;

    private BufferSegment _current;
    private int _nextBufferSize = bufferSize;

    public int TotalWritten { get; private set; }

    private bool UseFirstBuffer => _firstBuffer != _noUseFirstBufferSentinel;

    public int Remaining => bufferSize - TotalWritten;

    public static ArrayBuffer Null => new(true, false, 0);

    public byte[] DangerousGetFirstBuffer() => _firstBuffer;

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        throw new NotSupportedException();
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        if (_current.IsNull)
        {
            // use firstBuffer
            var free = _firstBuffer.Length - _firstBufferWritten;
            if (free != 0 && sizeHint <= free)
            {
                return _firstBuffer.AsSpan(_firstBufferWritten);
            }
        }
        else
        {
            var buffer = _current.FreeBuffer;
            if (buffer.Length > sizeHint)
            {
                return buffer;
            }
        }

        BufferSegment next;
        if (sizeHint <= _nextBufferSize)
        {
            next = new BufferSegment(_nextBufferSize);
            _nextBufferSize = MathEx.NewArrayCapacity(_nextBufferSize);
        }
        else
        {
            next = new BufferSegment(sizeHint);
        }

        if (_current.WrittenCount != 0)
        {
            _buffers.Add(_current);
        }
        _current = next;
        return next.FreeBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        if (_current.IsNull)
        {
            _firstBufferWritten += count;
        }
        else
        {
            _current.Advance(count);
        }
        TotalWritten += count;
    }

    public byte[] ToArrayAndReset()
    {
        if (TotalWritten == 0) return [];

        var result = AllocateUninitializedArray<byte>(TotalWritten);
        var dest = result.AsSpan();

        if (UseFirstBuffer)
        {
            _firstBuffer.AsSpan(0, _firstBufferWritten).CopyTo(dest);
            dest = dest.Slice(_firstBufferWritten);
        }

        if (_buffers.Count > 0)
        {
            foreach (ref var item in CollectionsMarshal.AsSpan(_buffers))
            {
                item.WrittenBuffer.CopyTo(dest);
                dest = dest.Slice(item.WrittenCount);
                item.Clear(); // reset buffer-segment in this loop to avoid iterate twice for Reset
            }
        }

        if (!_current.IsNull)
        {
            _current.WrittenBuffer.CopyTo(dest);
            _current.Clear();
        }

        ResetCore();
        return result;
    }

    public async ValueTask WriteToAndResetAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (TotalWritten == 0) return;

        if (UseFirstBuffer)
        {
            await stream.WriteAsync(_firstBuffer.AsMemory(0, _firstBufferWritten), cancellationToken).ConfigureAwait(false);
        }

        if (_buffers.Count > 0)
        {
            foreach (var item in _buffers)
            {
                await stream.WriteAsync(item.WrittenMemory, cancellationToken).ConfigureAwait(false);
                item.Clear(); // reset
            }
        }

        if (!_current.IsNull)
        {
            await stream.WriteAsync(_current.WrittenMemory, cancellationToken).ConfigureAwait(false);
            _current.Clear();
        }

        ResetCore();
    }

    // reset without list's BufferSegment element
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ResetCore()
    {
        _firstBufferWritten = 0;
        _buffers.Clear();
        TotalWritten = 0;
        _current = default;
        _nextBufferSize = bufferSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        if (TotalWritten == 0) return;
        foreach (ref var item in CollectionsMarshal.AsSpan(_buffers))
        {
            item.Clear();
        }
        _current.Clear();
        ResetCore();
    }

    public struct Enumerator: IEnumerator<Memory<byte>>
    {
        private ArrayBuffer _parent;
        private State _state;
        private Memory<byte> _current;
        private List<BufferSegment>.Enumerator _buffersEnumerator;

        public Enumerator(ArrayBuffer parent)
        {
            _parent = parent;
            _state = default;
            _current = default;
            _buffersEnumerator = default;
        }

        public Memory<byte> Current => _current;

        object IEnumerator.Current => throw new NotSupportedException();

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_state == State.FirstBuffer)
            {
                _state = State.BuffersInit;

                if (_parent.UseFirstBuffer)
                {
                    _current = _parent._firstBuffer.AsMemory(0, _parent._firstBufferWritten);
                    return true;
                }
            }

            if (_state == State.BuffersInit)
            {
                _state = State.BuffersIterate;

                _buffersEnumerator = _parent._buffers.GetEnumerator();
            }

            if (_state == State.BuffersIterate)
            {
                if (_buffersEnumerator.MoveNext())
                {
                    _current = _buffersEnumerator.Current.WrittenMemory;
                    return true;
                }

                _buffersEnumerator.Dispose();
                _state = State.Current;
            }

            if (_state == State.Current)
            {
                _state = State.End;

                _current = _parent._current.WrittenMemory;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        private enum State
        {
            FirstBuffer,
            BuffersInit,
            BuffersIterate,
            Current,
            End
        }
    }
}