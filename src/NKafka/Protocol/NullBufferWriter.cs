using System.Buffers;

namespace NKafka.Protocol;

/// <summary>
/// 
/// </summary>
internal sealed class NullBufferWriter(): BufferWriter(Stream.Null, 0)
{
    public static BufferWriter Instance { get; } = new NullBufferWriter();
}