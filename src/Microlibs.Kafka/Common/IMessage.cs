namespace Microlibs.Kafka.Common;

public interface IMessage
{
    /// <summary>
    ///     Returns the lowest supported API key of this message, inclusive
    /// </summary>
    short LowestSupportedVersion { get; }

    /// <summary>
    ///     Returns the highest supported API key of this message, inclusive
    /// </summary>
    short HighestSupportedVersion { get; }

    // /// <summary>
    // /// 
    // /// </summary>
    //List<RawTaggedField> UnknownTaggedFields { get; }
}