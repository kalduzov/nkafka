using System;
using System.Collections.Generic;

namespace Microlibs.Kafka.Protocol.Responses;

public record MetadataResponseMessage : KafkaResponseMessage
{
    /// <summary>
    ///     The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request
    ///     did not violate any quota
    /// </summary>
    public int ThrottleTimeMs { get; init; }

    /// <summary>
    ///     The ID of the controller broker
    /// </summary>
    public int ControllerId { get; init; }

    /// <summary>
    ///     The cluster ID that responding broker belongs to
    /// </summary>
    public string? ClusterId { get; init; }

    /// <summary>
    ///     32-bit bitfield to represent authorized operations for this cluster
    /// </summary>
    public int ClusterAuthorizedOperations { get; init; }

    /// <summary>
    ///     Each broker in the response
    /// </summary>
    public IReadOnlyCollection<BrokerInfo> Brokers { get; init; } = Array.Empty<BrokerInfo>();

    /// <summary>
    ///     Each topic in the response
    /// </summary>
    public IReadOnlyCollection<TopicInfo> Topics { get; init; } = Array.Empty<TopicInfo>();
}