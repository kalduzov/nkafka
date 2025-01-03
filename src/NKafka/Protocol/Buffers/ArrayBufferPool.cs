using System.Collections.Concurrent;

namespace NKafka.Protocol.Buffers;

internal static class ArrayBufferPool
{
    private static readonly ConcurrentQueue<ArrayBuffer> _queue = new();

    public static ArrayBuffer Rent(int size)
    {
        return _queue.TryDequeue(out var writer)
            ? writer
            : new ArrayBuffer(useFirstBuffer: true, pinned: false, size);
    }

    public static void Return(ArrayBuffer writer)
    {
        writer.Reset();
        _queue.Enqueue(writer);
    }
}