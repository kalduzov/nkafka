namespace Microlibs.Kafka.Config;

/// <summary>
///     Acks enum values
/// </summary>
public enum Acks : short
{
    /// <summary>
    ///     no set
    /// </summary>
    NoSet = -2,

    /// <summary>
    ///     None
    /// </summary>
    None = 0,

    /// <summary>
    ///     Leader
    /// </summary>
    Leader = 1,

    /// <summary>
    ///     All
    /// </summary>
    All = -1
}