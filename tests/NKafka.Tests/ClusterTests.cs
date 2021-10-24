using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;
using NKafka.Messages;

namespace NKafka.Tests;

public class ClusterTests
{
    private readonly Mock<IKafkaConnectorPool> _mockConnectorPool;

    public ClusterTests()
    {
        _mockConnectorPool = new Mock<IKafkaConnectorPool>();
        var connector1 = SetupConnector(1);
        var connector2 = SetupConnector(2);

        _mockConnectorPool.Setup(pool => pool.TryGetConnector(-1, out connector1))
            .Returns(true);

        _mockConnectorPool.Setup(pool => pool.TryGetConnector(1, out connector1))
            .Returns(true);

        _mockConnectorPool.Setup(pool => pool.TryGetConnector(2, out connector2))
            .Returns(true);
    }

    private static IKafkaConnector SetupConnector(int nodeId)
    {
        var connector = new Mock<IKafkaConnector>();

        connector.Setup(
                x => x.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(
                    It.IsAny<MetadataRequestMessage>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new MetadataResponseMessage
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
                },
                TimeSpan.FromMilliseconds(10)); //используем задержку как имитацию сетевого вызова

        return connector.Object;
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
                _mockConnectorPool.Object,
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
            _mockConnectorPool.Object,
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
            _mockConnectorPool.Object,
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
            _mockConnectorPool.Object,
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
            _mockConnectorPool.Object,
            CancellationToken.None);
        await using var producer = kafkaCluster.BuildProducer<int, string>("test_producer");

        producer.Name.Should().Be("test_producer");
    }
}