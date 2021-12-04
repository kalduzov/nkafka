namespace Microlibs.Kafka;

/// <summary>
///     Enumerates the different meanings of a message timestamp value.
/// </summary>
public enum TimestampType
{
    /// <summary>
    ///     Timestamp type is unknown.
    /// </summary>
    NotAvailable = 0,

    /// <summary>
    ///     Timestamp relates to message creation time as set by a Kafka client.
    /// </summary>
    CreateTime = 1,

    /// <summary>
    ///     Timestamp relates to the time a message was appended to a Kafka log.
    /// </summary>
    LogAppendTime = 2
}