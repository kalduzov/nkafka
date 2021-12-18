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

    //
    // private ReadOnlySpan<byte> DeserializePartition(ReadOnlySpan<byte> span, out IReadOnlyCollection<PartitionInfo> partitions)
    // {
    //     var nextSpan = span.ReadInt32(out var partitionCount);
    //     var partitionsLocal = new List<PartitionInfo>(partitionCount);
    //
    //     for (var i = 0; i < partitionCount; i++)
    //     {
    //         nextSpan = nextSpan
    //             .ReadInt16(out var errorCode)
    //             .ReadInt32(out var partitionIndex)
    //             .ReadInt32(out var leaderId);
    //
    //         if (Version >= ApiVersions.Version7)
    //         {
    //             nextSpan = nextSpan.ReadInt32(out var leader_epoch);
    //         }
    //
    //         nextSpan = nextSpan.ReadInt32(out var replicasCount);
    //         var replicaNodes = new int[replicasCount];
    //
    //         for (var j = 0; j < replicasCount; j++)
    //         {
    //             nextSpan = nextSpan.ReadInt32(out replicaNodes[j]);
    //         }
    //
    //         nextSpan = nextSpan.ReadInt32(out var isrCount);
    //         var isrNodes = new int[isrCount];
    //
    //         try
    //         {
    //             for (var j = 0; j < isrCount; j++)
    //             {
    //                 nextSpan = nextSpan.ReadInt32(out isrNodes[j]);
    //             }
    //         }
    //         catch (Exception exc)
    //         {
    //             exc.ToString();
    //         }
    //
    //         var partitionInfo = new PartitionInfo();
    //         partitionsLocal.Add(partitionInfo);
    //     }
    //
    //     partitions = partitionsLocal;
    //
    //     return nextSpan;
    // }
}