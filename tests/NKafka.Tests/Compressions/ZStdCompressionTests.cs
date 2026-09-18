using System.Text;

using NKafka.Compressions;

namespace NKafka.Tests.Compressions;

public sealed class ZStdCompressionTests
{
    private readonly ICompression _compression = new ZStdCompression(3);

    [Fact]
    public void EncodeAndDecode_ByteArray_ReturnsOriginalData()
    {
        var data = Encoding.UTF8.GetBytes("repeated Kafka record payload ".PadRight(4096, 'x'));

        var encoded = _compression.Encode(data);
        var decoded = _compression.Decode(encoded);

        decoded.Should().Equal(data);
        encoded.Should().NotEqual(data);
    }

    [Fact]
    public void EncodeAndDecode_Stream_ReturnsOriginalDataAndLeavesStreamsOpen()
    {
        var data = Encoding.UTF8.GetBytes("stream payload ".PadRight(4096, 'x'));
        using var source = new MemoryStream(data);
        using var compressed = new MemoryStream();

        using (var encoder = _compression.Encode(compressed))
        {
            source.CopyTo(encoder);
        }

        compressed.Position = 0;
        using var decoder = _compression.Decode(compressed);
        using var restored = new MemoryStream();
        decoder.CopyTo(restored);

        restored.ToArray().Should().Equal(data);
        compressed.CanRead.Should().BeTrue();
        compressed.CanWrite.Should().BeTrue();
    }
}
