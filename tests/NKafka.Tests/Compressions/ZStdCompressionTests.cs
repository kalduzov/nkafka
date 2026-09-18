using System.Text;

using System.IO.Compression;

using NKafka.Config;
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

    [Theory]
    [InlineData(CompressionType.None)]
    [InlineData(CompressionType.Gzip)]
    [InlineData(CompressionType.Snappy)]
    [InlineData(CompressionType.Lz4)]
    [InlineData(CompressionType.ZStd)]
    public void EveryCompressionType_EncodeAndDecodeStream_ReturnsOriginalData(CompressionType type)
    {
        var data = Encoding.UTF8.GetBytes("record batch payload ".PadRight(8192, 'x'));
        var compression = CreateCompression(type);
        using var source = new MemoryStream(data);
        using var encoded = new MemoryStream();

        var encoder = compression.Encode(encoded);
        source.CopyTo(encoder);
        if (ReferenceEquals(encoder, encoded))
        {
            encoder.Flush();
        }
        else
        {
            encoder.Dispose();
        }

        encoded.Position = 0;
        var decoder = compression.Decode(encoded);
        using var decoded = new MemoryStream();
        decoder.CopyTo(decoded);
        if (ReferenceEquals(decoder, encoded))
        {
            decoder.Flush();
        }
        else
        {
            decoder.Dispose();
        }

        decoded.ToArray().Should().Equal(data);
        encoded.CanRead.Should().BeTrue();
        encoded.CanWrite.Should().BeTrue();
    }

    private static ICompression CreateCompression(CompressionType type)
        => type switch
        {
            CompressionType.None => new NoCompression(),
            CompressionType.Gzip => new GZIPCompression(CompressionLevel.Fastest),
            CompressionType.Snappy => new SnappyCompression(),
            CompressionType.Lz4 => new LZ4Compression(0),
            CompressionType.ZStd => new ZStdCompression(3),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}
