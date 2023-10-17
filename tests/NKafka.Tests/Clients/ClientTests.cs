//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Clients.Producer;
using NKafka.Clients.Producer.Internals;
using NKafka.Config;
using NKafka.Connection;
using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Serialization;

namespace NKafka.Tests.Clients;

public abstract class ClientTests
{
    protected static IKafkaCluster CreateKafkaClusterForTests()
    {
        var clusterConfig = new ClusterConfig();

        var connectionPool = CreateKafkaConnectorPool();
        var clusterMetadata = new ClusterMetadata
        {
            AggregationApiByVersion =
            {
                [ApiKeys.FindCoordinator] = new ApiMetadata
                {
                    MinVersion = ApiVersion.Version0,
                    MaxVersion = ApiVersion.Version4
                },
                [ApiKeys.OffsetFetch] = new ApiMetadata
                {
                    MinVersion = ApiVersion.Version0,
                    MaxVersion = ApiVersion.Version8
                },
                [ApiKeys.JoinGroup] = new ApiMetadata
                {
                    MinVersion = ApiVersion.Version0,
                    MaxVersion = ApiVersion.Version9
                }
            }
        };

        return new KafkaCluster(clusterConfig, NullLoggerFactory.Instance, connectionPool, clusterMetadata);
    }

    protected static IProducer<TKey, TValue> CreateProducerForTests<TKey, TValue>(ProducerConfig config)
        where TKey : notnull
        where TValue : notnull
    {
        var kafkaCluster = Substitute.For<IKafkaCluster>();
        kafkaCluster.Closed.Returns(true);

        var transactionManager = Substitute.For<ITransactionManager>();
        var recordAccumulator = Substitute.For<IRecordAccumulator>();

        recordAccumulator
            .Append(Arg.Is<TopicPartition>(tp => tp.Partition == 1 && tp.Topic == "test_topic"),
                Arg.Any<long>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<Headers>())
            .Returns(new RecordAppendResult(new SendResultTask(new TaskCompletionSource(), 1, 0, 0, 0), false, true, 10));

        var messagesSender = Substitute.For<IMessagesSender>();

        return new Producer<TKey, TValue>(kafkaCluster,
            "test_producer",
            config,
            NoneSerializer<TKey>.Instance,
            NoneSerializer<TValue>.Instance,
            transactionManager,
            recordAccumulator,
            messagesSender,
            NullLoggerFactory.Instance);
    }

    private static IKafkaConnectorPool CreateKafkaConnectorPool()
    {
        var connectionPool = Substitute.For<IKafkaConnectorPool>();

        var kafkaConnector1 = Substitute.For<IKafkaConnector>();
        kafkaConnector1.NodeId.Returns(1);

        var kafkaConnector2 = Substitute.For<IKafkaConnector>();
        kafkaConnector2.NodeId.Returns(2);

        SetupMetadataRequests(kafkaConnector1);
        SetupMetadataRequests(kafkaConnector2);

        SetupApiRequests(kafkaConnector1);
        SetupApiRequests(kafkaConnector2);

        SetupFindCoordinatorRequests(kafkaConnector1);
        SetupJointToGroupRequests(kafkaConnector1);
        SetupOffsetFetchRequests(kafkaConnector1);

        connectionPool.GetConnector().Returns(kafkaConnector1);
        connectionPool.TryGetConnector(1, true, connector: out Arg.Any<IKafkaConnector>())
            .Returns(x =>
            {
                x[2] = kafkaConnector1;

                return true;
            });

        return connectionPool;
    }

    private static void SetupOffsetFetchRequests(IKafkaConnector kafkaConnector)
    {
        kafkaConnector.SendAsync<OffsetFetchRequestMessage, OffsetFetchResponseMessage>(Arg.Any<OffsetFetchRequestMessage>(),
                false,
                Arg.Any<CancellationToken>())
            .Returns(new OffsetFetchResponseMessage
            {
                Groups = new List<OffsetFetchResponseMessage.OffsetFetchResponseGroupMessage>
                {
                    new()
                    {
                        groupId = "good_test",
                        Topics = new List<OffsetFetchResponseMessage.OffsetFetchResponseTopicsMessage>
                        {
                            new()
                            {
                                Name = "test",
                                Partitions = new List<OffsetFetchResponseMessage.OffsetFetchResponsePartitionsMessage>
                                {
                                    new()
                                    {
                                        CommittedOffset = 0,
                                        PartitionIndex = 0,
                                        CommittedLeaderEpoch = -1
                                    }
                                }
                            }
                        }
                    }
                }
            });
    }

    private static void SetupApiRequests(IKafkaConnector kafkaConnector)
    {
        kafkaConnector.SendAsync<ApiVersionsRequestMessage, ApiVersionsResponseMessage>(Arg.Any<ApiVersionsRequestMessage>(),
                false,
                Arg.Any<CancellationToken>())
            .Returns(new ApiVersionsResponseMessage
            {
                ApiKeys = new ApiVersionsResponseMessage.ApiVersionCollection
                {
                    new()
                    {
                        ApiKey = (short)ApiKeys.FindCoordinator,
                        MaxVersion = (short)ApiVersion.Version4,
                        MinVersion = (short)ApiVersion.Version0,
                    }
                }
            });
    }

    private static void SetupJointToGroupRequests(IKafkaConnector kafkaConnector)
    {
        kafkaConnector.SendAsync<JoinGroupRequestMessage, JoinGroupResponseMessage>(
                Arg.Is<JoinGroupRequestMessage>(
                    r => r.GroupId == "good_test"
                         && string.IsNullOrWhiteSpace(r.MemberId)
                         && r.ProtocolType == "consumer"),
                false,
                Arg.Any<CancellationToken>())
            .Returns(new JoinGroupResponseMessage
            {
                MemberId = "1",
                Leader = "1",
                GenerationId = 1,
                SkipAssignment = false,
                ProtocolName = "roundrobin"
            });
    }

    private static void SetupFindCoordinatorRequests(IKafkaConnector kafkaConnector)
    {
        kafkaConnector.SendAsync<FindCoordinatorRequestMessage, FindCoordinatorResponseMessage>(
                Arg.Is<FindCoordinatorRequestMessage>(
                    r => r.KeyType == 0 && (r.CoordinatorKeys.Contains("good_test") || r.Key == "good_test")),
                false,
                Arg.Any<CancellationToken>())
            .Returns(new FindCoordinatorResponseMessage
            {
                Host = "127.0.0.1",
                Port = 9091,
                NodeId = 1,
                Coordinators = new List<FindCoordinatorResponseMessage.CoordinatorMessage>
                {
                    new()
                    {
                        Host = "127.0.0.1",
                        Port = 9091,
                        NodeId = 1,
                        Key = "good_test"
                    }
                }
            });
    }

    private static void SetupMetadataRequests(IKafkaConnector kafkaConnector)
    {
        kafkaConnector.SendAsync<MetadataRequestMessage, MetadataResponseMessage>(
                Arg.Any<MetadataRequestMessage>(),
                true,
                Arg.Any<CancellationToken>())
            .Returns(new MetadataResponseMessage());
    }
}