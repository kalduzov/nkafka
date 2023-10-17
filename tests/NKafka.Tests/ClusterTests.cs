// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 *
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol;

namespace NKafka.Tests;

public partial class ClusterTests
{
    private readonly IKafkaConnectorPool _connectorPool;

    public ClusterTests()
    {
        _connectorPool = Substitute.For<IKafkaConnectorPool>();
        var connector1 = SetupConnector(1);
        var connector2 = SetupConnector(2);

        _connectorPool.TryGetConnector(-1, false, out Arg.Any<IKafkaConnector>())
            .Returns(x =>
            {
                x[2] = connector1;

                return true;
            });

        _connectorPool.TryGetConnector(1, false, out Arg.Any<IKafkaConnector>())
            .Returns(x =>
            {
                x[2] = connector1;

                return true;
            });

        _connectorPool.TryGetConnector(2, false, out Arg.Any<IKafkaConnector>())
            .Returns(x =>
            {
                x[2] = connector2;

                return true;
            });

        _connectorPool.GetAllOpenedConnectors()
            .Returns(new[]
            {
                connector1,
                connector2
            });
    }

    [Fact(DisplayName = "Create new cluster when timeout must be throw ClusterKafkaException")]
    public async Task CreateNewClusterAsync_WhenTimeout_MustBeException()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:64000"
            },
            ClusterInitTimeoutMs = 1 //из-за имитации сетевого вызова - этот код отработает корректно
        };

        var awaiting = FluentActions.Awaiting(
            () => clusterConfig.CreateClusterInternalAsync(
                NullLoggerFactory.Instance,
                true,
                _connectorPool,
                CancellationToken.None));

        await awaiting.Should().ThrowAsync<ClusterKafkaException>();
    }

    [Fact(DisplayName = "Create new cluster broker updated")]
    public async Task CreateNewClusterAsync_BrokerUpdated()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:29091"
            },
            ClusterInitTimeoutMs = 10000
        };

        await using var kafkaCluster = await clusterConfig.CreateClusterInternalAsync(
            NullLoggerFactory.Instance,
            true,
            _connectorPool,
            CancellationToken.None);
        kafkaCluster.Brokers.Should().HaveCount(2);
        kafkaCluster.Controller.Should().NotBeNull();
        kafkaCluster.Controller.Id.Should().BePositive();
        kafkaCluster.ClusterId.Should().NotBeNull();
        kafkaCluster.ClusterId.Should().Be("test_cluster");
    }

    [Fact(DisplayName = "Build producer without exceptions")]
    public async Task BuildProducer_NoThrowException_Successful()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:29091"
            }
        };

        var kafkaCluster = await clusterConfig.CreateClusterInternalAsync(
            NullLoggerFactory.Instance,
            true,
            _connectorPool,
            CancellationToken.None); //no call dispose!
        var func = FluentActions.Invoking(() => kafkaCluster.BuildProducer<int, string>());
        func.Should().NotThrow();
    }

    [Fact(DisplayName = "Build producer without name successful")]
    public async Task BuildProducer_WithoutName_Successful()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:29091"
            }
        };

        await using var kafkaCluster = await clusterConfig.CreateClusterInternalAsync(
            NullLoggerFactory.Instance,
            true,
            _connectorPool,
            CancellationToken.None);
        await using var producer = kafkaCluster.BuildProducer<int, string>();

        producer.Name.Should().Be("__DefaultProducer<Int32,String>");
    }

    [Fact(DisplayName = "Build producer with custom name successful")]
    public async Task BuildProducer_WithCustomName_Successful()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:29091"
            }
        };

        await using var kafkaCluster = await clusterConfig.CreateClusterInternalAsync(
            NullLoggerFactory.Instance,
            true,
            _connectorPool,
            CancellationToken.None);
        await using var producer = kafkaCluster.BuildProducer<int, string>("test_producer");

        producer.Name.Should().Be("test_producer");
    }

    private static IKafkaConnector SetupConnector(int nodeId)
    {
        var connector = Substitute.For<IKafkaConnector>();
        connector.NodeId.Returns(nodeId);
        connector.SendAsync<MetadataRequestMessage, MetadataResponseMessage>(
                Arg.Any<MetadataRequestMessage>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(async _ =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10)); //используем задержку как имитацию сетевого вызова

                    return new MetadataResponseMessage
                    {
                        Brokers = new MetadataResponseMessage.MetadataResponseBrokerCollection(2)
                        {
                            new()
                            {
                                NodeId = 1,
                                Host = "localhost1",
                                Port = 1000,
                                Rack = null
                            },
                            new()
                            {
                                NodeId = 2,
                                Host = "localhost2",
                                Port = 1000,
                                Rack = null
                            }
                        },
                        ControllerId = 1,
                        ClusterId = "test_cluster"
                    };
                }
            );

        connector.SupportVersions.Returns(_apiVersions[nodeId - 1]);

        return connector;
    }
}