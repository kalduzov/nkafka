namespace Microlibs.Kafka.Compressions;

public enum CompressionCodec : byte
{
    None = 0,
    GZIP = 1,
    Snappy = 2,
    LZ4 = 3,
    ZSTD = 4
}