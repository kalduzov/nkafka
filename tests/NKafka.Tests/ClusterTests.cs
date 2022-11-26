using FluentAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;
using NKafka.Messages;

using Xunit;

namespace NKafka.Tests;

public class ClusterTests
{
    private readonly List<IBroker> _seedMockBrokers = new(2);

    public ClusterTests()
    {
        var broker1 = SetupBroker();
        var broker2 = SetupBroker();

        _seedMockBrokers.AddRange(
            new[]
            {
                broker1.Object,
                broker2.Object
            });
    }

    private static Mock<IBroker> SetupBroker()
    {
        var broker = new Mock<IBroker>();

        // broker
        //     .Setup(
        //         x => x.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(
        //             It.IsAny<MetadataRequestMessage>(),
        //             It.IsAny<CancellationToken>()))
        //     .ReturnsAsync(
        //         () => new MetadataResponseMessage
        //         {
        //             Brokers = new MetadataResponseMessage.MetadataResponseBrokerCollection(2)
        //             {
        //                 new()
        //                 {
        //                     NodeId = 1,
        //                     Host = "localhost1",
        //                     Port = 1000,
        //                     Rack = null
        //                 },
        //                 new()
        //                 {
        //                     NodeId = 2,
        //                     Host = "localhost2",
        //                     Port = 1000,
        //                     Rack = null
        //                 }
        //             },
        //             ControllerId = 1,
        //             ClusterId = "test_cluster",
        //         });

        return broker;
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
            ClusterInitTimeoutMs = 1
        };

        var awaiting = FluentActions.Awaiting(() => clusterConfig.CreateClusterInternalAsync(NullLoggerFactory.Instance));
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

        await using var kafkaCluster = await clusterConfig.CreateClusterInternalAsync(NullLoggerFactory.Instance, _seedMockBrokers);
        kafkaCluster.Brokers.Should().HaveCount(2);
        kafkaCluster.Controller.Should().NotBeNull();
        kafkaCluster.Controller?.Id.Should().BePositive();
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
            },
        };

        var kafkaCluster = await clusterConfig.CreateClusterInternalAsync(NullLoggerFactory.Instance, _seedMockBrokers); //no call dispose!
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
            },
        };

        await using var kafkaCluster = await clusterConfig.CreateClusterInternalAsync(NullLoggerFactory.Instance, _seedMockBrokers);
        await using var producer = kafkaCluster.BuildProducer<int, string>();

        producer.Name.Should().Be(KafkaCluster.DEFAULT_PRODUCER_NAME);
    }

    [Fact(DisplayName = "Build producer with custom name successful")]
    public async Task BuildProducer_WithCustomName_Successful()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:29091"
            },
        };

        await using var kafkaCluster = await clusterConfig.CreateClusterInternalAsync(NullLoggerFactory.Instance, _seedMockBrokers);
        await using var producer = kafkaCluster.BuildProducer<int, string>("test_producer");

        producer.Name.Should().Be("test_producer");
    }
}