using System.Buffers;

namespace NKafka.Protocol.Buffers;

/// <summary>
/// 
/// </summary>
internal sealed class NullBufferWriter: IBufferWriter<byte>
{
    public static IBufferWriter<byte> Instance { get; } = new NullBufferWriter();

    public void Advance(int count)
    {
        throw new NotImplementedException();
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        throw new NotImplementedException();
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        throw new NotImplementedException();
    }
}