using System;
using System.Collections.Generic;
using System.IO;
using Microlibs.Kafka.Protocol.Extensions;

namespace Microlibs.Kafka.Protocol.Responses;

public class MetadataResponseMessage : KafkaResponseMessage
{
    /// <summary>
    ///     The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request
    ///     did not violate any quota
    /// </summary>
    public int ThrottleTimeMs { get; private set; }

    /// <summary>
    ///     The ID of the controller broker
    /// </summary>
    public int ControllerId { get; private set; }

    /// <summary>
    ///     The cluster ID that responding broker belongs to
    /// </summary>
    public string? ClusterId { get; private set; }

    /// <summary>
    ///     32-bit bitfield to represent authorized operations for this cluster
    /// </summary>
    public int ClusterAuthorizedOperations { get; private set; }

    /// <summary>
    ///     Each broker in the response
    /// </summary>
    public IReadOnlyCollection<BrokerInfo> Brokers { get; private set; }

    /// <summary>
    ///     Each topic in the response
    /// </summary>
    public IReadOnlyCollection<TopicInfo> Topics { get; private set; }

    public override void DeserializeFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream);

        Code = (StatusCodes)reader.ReadInt16().Swap();

        if (!IsSuccessStatusCode) //Нет смысла дальше считывать данные - уже была ошибка
        {
            return;
        }

        ThrottleTimeMs = Version switch
        {
            >= ApiVersions.Version3 => reader.ReadInt32().Swap(),
            _ => ThrottleTimeMs
        };

        DeserializeBrokers(reader);

        if (Version >= ApiVersions.Version3)
        {
            ClusterId = reader.ReadCompactNullableString();
            ControllerId = reader.ReadInt32().Swap();
        }

        DeserializeTopics(reader);

        ClusterAuthorizedOperations = Version switch
        {
            >= ApiVersions.Version8 and <= ApiVersions.Version10 => reader.ReadInt32().Swap(),
            _ => ClusterAuthorizedOperations
        };

        switch (Version)
        {
            case >= ApiVersions.Version11:
            {
                //TAG_BUFFER
                break;
            }
        }
    }

    private void DeserializeTopics(BinaryReader reader)
    {
        var topicsCount = reader.ReadInt32().Swap();

        var topics = new List<TopicInfo>(topicsCount);

        for (var i = 0; i < topicsCount; i++)
        {
            var errorCode = (StatusCodes)reader.ReadInt16().Swap();
            var name = reader.ReadNormalString();

            var topicId = Guid.Empty;

            if (Version > ApiVersions.Version9)
            {
                var tid = reader.ReadBytes(16); //todo надо скорректировать чтение guid
            }

            var isInternal = reader.ReadBoolean();

            var partitions = DeserializePartition(reader);

            var topicAuthorizedOperations = -1;

            if (Version > ApiVersions.Version7)
            {
                topicAuthorizedOperations = reader.ReadInt32().Swap();
            }

            var topic = new TopicInfo(errorCode, name, isInternal, topicAuthorizedOperations, partitions, topicId);

            topics.Add(topic);
        }

        Topics = topics;
    }

    private IReadOnlyCollection<PartitionInfo> DeserializePartition(BinaryReader reader)
    {
        var partitionCount = reader.ReadInt32().Swap();
        var partitions = new List<PartitionInfo>(partitionCount);

        return partitions;
    }

    private void DeserializeBrokers(BinaryReader reader)
    {
        var brokersCount = reader.ReadInt16().Swap();

        var brokers = new List<BrokerInfo>(brokersCount);

        for (var i = 0; i < brokersCount; i++)
        {
            var id = reader.ReadInt32().Swap();
            var host = reader.ReadNormalString();
            var port = reader.ReadInt32().Swap();

            string rack = null!;

            if (Version >= ApiVersions.Version3)
            {
                rack = reader.ReadCompactNullableString();
            }

            var brokerInfo = new BrokerInfo(id, host, port, rack);
            brokers.Add(brokerInfo);
        }

        Brokers = brokers;
    }
}