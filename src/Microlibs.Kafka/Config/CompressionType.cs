namespace Microlibs.Kafka.Config;

/// <summary>
///     CompressionType enum values
/// </summary>
public enum CompressionType
{
    /// <summary>
    ///     None
    /// </summary>
    None,

    /// <summary>
    ///     Gzip
    /// </summary>
    Gzip,

    /// <summary>
    ///     Snappy
    /// </summary>
    Snappy,

    /// <summary>
    ///     Lz4
    /// </summary>
    Lz4,

    /// <summary>
    ///     Zstd
    /// </summary>
    Zstd
}