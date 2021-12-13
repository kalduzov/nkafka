using System.Collections.Generic;

namespace Microlibs.Kafka.Protocol.Responses;

public class MetadataResponseMessage : KafkaResponseMessage
{
    /// <summary>
    ///     The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request
    ///     did not violate any quota
    /// </summary>
    public int ThrottleTimeMs { get; }

    /// <summary>
    ///     The ID of the controller broker
    /// </summary>
    public int ControllerId { get; }

    /// <summary>
    ///     The cluster ID that responding broker belongs to
    /// </summary>
    public string? ClusterId { get; }

    /// <summary>
    ///     32-bit bitfield to represent authorized operations for this cluster
    /// </summary>
    public int ClusterAuthorizedOperations { get; }

    /// <summary>
    ///     Each broker in the response
    /// </summary>
    public IReadOnlyCollection<BrokerInfo> Brokers { get; }

    /// <summary>
    ///     Each topic in the response
    /// </summary>
    public IReadOnlyCollection<TopicInfo> Topics { get; }
}