using System;
using System.Collections.Generic;
using System.IO;
using Microlibs.Kafka.Protocol.Extensions;
using static System.Buffers.Binary.BinaryPrimitives;

namespace Microlibs.Kafka.Protocol.Responses;

public class MetadataResponseMessage : KafkaResponseMessage
{
    private int _throttleTimeMs;
    private int _controllerId;
    private string? _clusterId;
    private int _clusterAuthorizedOperations;

    /// <summary>
    ///     The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request
    ///     did not violate any quota
    /// </summary>
    public int ThrottleTimeMs => _throttleTimeMs;

    /// <summary>
    ///     The ID of the controller broker
    /// </summary>
    public int ControllerId => _controllerId;

    /// <summary>
    ///     The cluster ID that responding broker belongs to
    /// </summary>
    public string? ClusterId => _clusterId;

    /// <summary>
    ///     32-bit bitfield to represent authorized operations for this cluster
    /// </summary>
    public int ClusterAuthorizedOperations => _clusterAuthorizedOperations;

    /// <summary>
    ///     Each broker in the response
    /// </summary>
    public IReadOnlyCollection<BrokerInfo> Brokers { get; private set; } = Array.Empty<BrokerInfo>();

    /// <summary>
    ///     Each topic in the response
    /// </summary>
    public IReadOnlyCollection<TopicInfo> Topics { get; private set; } = Array.Empty<TopicInfo>();

    public override void DeserializeFromStream(ReadOnlySpan<byte> span)
    {
        var nextSpan = span;

        if (Version >= ApiVersions.Version3)
        {
            nextSpan = nextSpan.ReadInt32(out _throttleTimeMs);
        }

        nextSpan = DeserializeBrokers(nextSpan);

        if (Version >= ApiVersions.Version3)
        {
            nextSpan = nextSpan.ReadNullableString(out _clusterId);
            nextSpan = nextSpan.ReadInt32(out _controllerId);
        }

        nextSpan = DeserializeTopics(nextSpan);

        //
        // ClusterAuthorizedOperations = Version switch
        // {
        //     >= ApiVersions.Version8 and <= ApiVersions.Version10 => reader.ReadInt32().Swap(),
        //     _ => ClusterAuthorizedOperations
        // };
        //
        // switch (Version)
        // {
        //     case >= ApiVersions.Version11:
        //     {
        //         //TAG_BUFFER
        //         break;
        //     }
        // }
    }

    private ReadOnlySpan<byte> DeserializeTopics(ReadOnlySpan<byte> span)
    {
        var nextSpan = span.ReadInt32(out var topicsCount);
        var topics = new List<TopicInfo>(topicsCount);

        for (var i = 0; i < topicsCount; i++)
        {
            nextSpan = nextSpan
                .ReadInt16(out var errorCode)
                .ReadString(out var name);

            var isInternal = false;

            if (Version >= ApiVersions.Version1)
            {
                nextSpan = nextSpan.ReadBoolean(out isInternal);
            }

            nextSpan = DeserializePartition(nextSpan, out var partitions);

            var topicAuthorizedOperations = -1;

            if (Version > ApiVersions.Version7)
            {
                nextSpan = nextSpan.ReadInt32(out topicAuthorizedOperations);
            }

            var topic = new TopicInfo((StatusCodes)errorCode, name, isInternal, topicAuthorizedOperations, partitions, Guid.Empty);

            topics.Add(topic);
        }

        Topics = topics;

        return nextSpan;
    }

    private ReadOnlySpan<byte> DeserializePartition(ReadOnlySpan<byte> span, out IReadOnlyCollection<PartitionInfo> partitions)
    {
        var nextSpan = span.ReadInt32(out var partitionCount);
        var partitionsLocal = new List<PartitionInfo>(partitionCount);

        for (var i = 0; i < partitionCount; i++)
        {
            nextSpan = nextSpan
                .ReadInt16(out var errorCode)
                .ReadInt32(out var partitionIndex)
                .ReadInt32(out var leaderId);

            if (Version >= ApiVersions.Version7)
            {
                nextSpan = nextSpan.ReadInt32(out var leader_epoch);
            }

            nextSpan = nextSpan.ReadInt32(out var replicasCount);
            var replicaNodes = new int[replicasCount];

            for (var j = 0; j < replicasCount; j++)
            {
                nextSpan = nextSpan.ReadInt32(out replicaNodes[j]);
            }

            nextSpan = nextSpan.ReadInt32(out var isrCount);
            var isrNodes = new int[isrCount];

            for (var j = 0; j < isrCount; j++)
            {
                nextSpan = nextSpan.ReadInt32(out isrNodes[j]);
            }

            var partitionInfo = new PartitionInfo();
            partitionsLocal.Add(partitionInfo);
        }

        partitions = partitionsLocal;

        return nextSpan;
    }

    private IReadOnlyCollection<PartitionInfo> DeserializePartition(BinaryReader reader)
    {
        var partitionCount = reader.ReadInt32().Swap();
        var partitions = new List<PartitionInfo>(partitionCount);

        return partitions;
    }

    private ReadOnlySpan<byte> DeserializeBrokers(ReadOnlySpan<byte> span)
    {
        var brokersCount = 0;

        var nextSpan = span;

        if (Version < ApiVersions.Version9)
        {
            nextSpan = nextSpan.ReadInt32(out brokersCount);
        }

        var brokers = new List<BrokerInfo>(brokersCount);

        for (var i = 0; i < brokersCount; i++)
        {
            nextSpan = nextSpan.ReadInt32(out var id);
            nextSpan = nextSpan.ReadString(out var host);
            nextSpan = nextSpan.ReadInt32(out var port);

            string rack = null!;

            if (Version >= ApiVersions.Version3)
            {
                nextSpan = nextSpan.ReadNullableString(out rack);
            }

            var brokerInfo = new BrokerInfo(id, host, port, rack);
            brokers.Add(brokerInfo);
        }

        Brokers = brokers;

        return nextSpan;
    }
}