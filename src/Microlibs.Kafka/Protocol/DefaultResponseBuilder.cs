using System;
using System.Collections.Generic;
using Microlibs.Kafka.Protocol.Responses;

namespace Microlibs.Kafka.Protocol
{
    internal static partial class DefaultResponseBuilder
    {
        public static KafkaResponseMessage Build(ApiKeys apiKey, ApiVersions apiVersion, int responseLength, ReadOnlySpan<byte> span)
        {
            return apiKey switch
            {
                ApiKeys.Metadata => BuildMetadataResponse(span, apiVersion, responseLength),
            };
        }

        private static KafkaResponseMessage BuildMetadataResponse(ReadOnlySpan<byte> span, ApiVersions apiVersion, int responseLength)
        {
            var reader = new KafkaBufferReader(span);

            var throttleTimeMs = apiVersion switch
            {
                >= ApiVersions.Version3 => reader.ReadInt(),
                _ => -1
            };

            var brokers = DeserializeBrokers(ref reader);

            var clusterId = apiVersion switch
            {
                >= ApiVersions.Version2 => ReadStringByVersion(ref reader, apiVersion, ApiVersions.Version9),
                _ => null,
            };

            var controllerId = apiVersion switch
            {
                >= ApiVersions.Version1 => reader.ReadInt(),
                _ => -1
            };

            var topics = DeserializeTopics(ref reader);

            var clusterAuthorizedOperations = apiVersion switch
            {
                >= ApiVersions.Version8 and <= ApiVersions.Version10 => reader.ReadInt(),
                _ => -2147483648
            };

            var _ = apiVersion switch
            {
                >= ApiVersions.Version9 => reader.ReadEmptyTaggedFieldArray(),
                _ => -1
            };

            return new MetadataResponseMessage
            {
                ThrottleTimeMs = throttleTimeMs,
                Brokers = brokers,
                Version = apiVersion,
                ClusterId = clusterId,
                ControllerId = controllerId,
                Topics = topics,
                ClusterAuthorizedOperations = clusterAuthorizedOperations
            };

            IReadOnlyCollection<BrokerInfo> DeserializeBrokers(ref KafkaBufferReader reader)
            {
                var brokersCount = apiVersion switch
                {
                    >= ApiVersions.Version9 => reader.ReadCompactArrayLength(),
                    _ => reader.ReadInt()
                };

                var deserializeBrokers = new List<BrokerInfo>(brokersCount);

                for (var i = 0; i < brokersCount; i++)
                {
                    var id = reader.ReadInt();
                    var host = ReadStringByVersion(ref reader, apiVersion, ApiVersions.Version9);
                    var port = reader.ReadInt();
                    var rack = apiVersion switch
                    {
                        >= ApiVersions.Version3 => ReadNullableStringByVersion(ref reader, apiVersion, ApiVersions.Version9),
                        _ => null!
                    };

                    var brokerInfo = new BrokerInfo(id, host, port, rack);
                    deserializeBrokers.Add(brokerInfo);
                }

                return deserializeBrokers;
            }

            IReadOnlyCollection<TopicInfo> DeserializeTopics(ref KafkaBufferReader reader)
            {
                var topicsCount = apiVersion switch
                {
                    >= ApiVersions.Version9 => reader.ReadCompactArrayLength(),
                    _ => reader.ReadInt()
                };

                var deserializeTopics = new List<TopicInfo>(topicsCount);

                for (var i = 0; i < topicsCount; i++)
                {
                    var errorCode = reader.ReadShort();
                    var name = apiVersion switch
                    {
                        >= ApiVersions.Version12 => ReadStringByVersion(ref reader, apiVersion, ApiVersions.Version9),
                        _ => ReadNullableStringByVersion(ref reader, apiVersion, ApiVersions.Version9)
                    };

                    var topicId = Guid.Empty;

                    // var topicId = Guid.Parse(
                    //     apiVersion switch
                    //     {
                    //         >= ApiVersions.Version10 => localReader.readb
                    //     });

                    var isInternal = apiVersion switch
                    {
                        >= ApiVersions.Version1 => reader.ReadBoolean(),
                        _ => false
                    };

                    var partitions = DeserializePartitions(ref reader);

                    var topicAuthorizedOperations = apiVersion switch
                    {
                        >= ApiVersions.Version8 => reader.ReadInt(),
                        _ => -2147483648
                    };

                    var topic = new TopicInfo((StatusCodes)errorCode, name, isInternal, topicAuthorizedOperations, partitions, topicId);
                    deserializeTopics.Add(topic);
                }

                return deserializeTopics;

                IReadOnlyCollection<PartitionInfo> DeserializePartitions(ref KafkaBufferReader reader)
                {
                    var partitionsCount = apiVersion switch
                    {
                        >= ApiVersions.Version9 => reader.ReadCompactArrayLength(),
                        _ => reader.ReadInt()
                    };
                    var partitions = new List<PartitionInfo>(partitionsCount);

                    for (var i = 0; i < partitionsCount; i++)
                    {
                        var errorCode = reader.ReadShort();
                        var partitionIndex = reader.ReadInt();
                        var leaderId = reader.ReadInt();
                        var leaderEpoch = apiVersion switch
                        {
                            >= ApiVersions.Version7 => reader.ReadInt(),
                            _ => -1
                        };

                        var replicaNodes = apiVersion switch
                        {
                            >= ApiVersions.Version9 => reader.ReadCompactIntArray(),
                            _ => reader.ReadIntArray(),
                        };
                        var isrNodes = apiVersion switch
                        {
                            >= ApiVersions.Version9 => reader.ReadCompactIntArray(),
                            _ => reader.ReadIntArray(),
                        };
                        var offlineReplicas = apiVersion switch
                        {
                            >= ApiVersions.Version5 and <= ApiVersions.Version8 => reader.ReadIntArray(),
                            >= ApiVersions.Version9 => reader.ReadCompactIntArray(),
                            _ => null
                        };
                        var partition = new PartitionInfo(
                            (StatusCodes)errorCode,
                            partitionIndex,
                            leaderId,
                            leaderEpoch,
                            replicaNodes,
                            isrNodes,
                            offlineReplicas);

                        partitions.Add(partition);
                    }

                    return partitions;
                }
            }
        }

        private static string ReadStringByVersion(ref KafkaBufferReader reader, ApiVersions currentVersion, ApiVersions flexibleVersion)
        {
            return currentVersion < flexibleVersion ? reader.ReadString() : reader.ReadCompactString();
        }

        private static string? ReadNullableStringByVersion(ref KafkaBufferReader reader, ApiVersions currentVersion, ApiVersions flexibleVersion)
        {
            return currentVersion < flexibleVersion ? reader.ReadNullableString() : reader.ReadCompactNullableString();
        }
    }
}