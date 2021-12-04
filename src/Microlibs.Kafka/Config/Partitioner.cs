namespace Microlibs.Kafka.Config;

/// <summary>
///     Partitioner enum values
/// </summary>
public enum Partitioner
{
    /// <summary>
    ///     Random
    /// </summary>
    Random,

    /// <summary>
    ///     Consistent
    /// </summary>
    Consistent,

    /// <summary>
    ///     ConsistentRandom
    /// </summary>
    ConsistentRandom,

    /// <summary>
    ///     Murmur2
    /// </summary>
    Murmur2,

    /// <summary>
    ///     Murmur2Random
    /// </summary>
    Murmur2Random
}