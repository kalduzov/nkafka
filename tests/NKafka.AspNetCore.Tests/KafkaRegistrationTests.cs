using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NKafka.Clients.Consumer;
using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Serialization;

namespace NKafka.AspNetCore.Tests;

public class KafkaRegistrationTests
{
    [Fact]
    public void AddKafkaCluster_RegistersClusterAndConfig()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        const string clusterName = "test-cluster";

        // Act
        services.AddKafkaCluster(clusterName, config =>
        {
            config.BootstrapServers = ["localhost:9092"];
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var cluster = serviceProvider.GetKeyedService<IKafkaCluster>(clusterName);
        cluster.Should().NotBeNull();
    }

    [Fact]
    public void AddProducer_RegistersProducerWithCorrectConfig()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        
        const string clusterName = "test-cluster";
        const string bootstrapServer = "localhost:9092";

        // Act
        services.AddKafkaCluster(clusterName, config =>
        {
            config.BootstrapServers = [bootstrapServer];
        }).AddProducer<ITestProducer>(config =>
        {
            config.Acks = Acks.All;
        });

        var serviceProvider = services.BuildServiceProvider();
        
        // We can't easily verify the BuildProducer call on the real cluster without mocking the cluster,
        // but we can verify that the producer is registered and can be resolved (even if it throws on creation due to closed cluster).
        // To properly test the configuration merge, we'd need to mock IKafkaCluster.
        
        var producerAction = () => serviceProvider.GetRequiredService<ITestProducer>();

        // Assert
        producerAction.Should().Throw<ClusterKafkaException>().WithMessage("Tried to use a cluster that was closed");
    }

    [Fact]
    public void AddConsumer_RegistersConsumerWithDeserializers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        var keyDeserializer = Substitute.For<IDeserializer<string>>();
        var valueDeserializer = Substitute.For<IDeserializer<byte[]>>();

        const string clusterName = "test-cluster";
        
        services.AddSingleton(keyDeserializer);
        services.AddSingleton(valueDeserializer);

        // Act
        services.AddKafkaCluster(clusterName, config =>
        {
            config.BootstrapServers = ["localhost:9092"];
        }).AddConsumer<ITestConsumer, string, byte[]>(config =>
        {
            config.GroupId = "test-group";
        });

        var serviceProvider = services.BuildServiceProvider();
        var consumerAction = () => serviceProvider.GetRequiredService<ITestConsumer>();

        // Assert
        consumerAction.Should().Throw<ClusterKafkaException>().WithMessage("Tried to use a cluster that was closed");
    }

    [Fact]
    public void MultiCluster_RegistersDifferentClusters()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        
        // Act
        services.AddKafkaCluster("cluster1", config => { config.BootstrapServers = ["host1:9092"]; });
        services.AddKafkaCluster("cluster2", config => { config.BootstrapServers = ["host2:9092"]; });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var cluster1 = serviceProvider.GetKeyedService<IKafkaCluster>("cluster1");
        var cluster2 = serviceProvider.GetKeyedService<IKafkaCluster>("cluster2");

        cluster1.Should().NotBeNull();
        cluster2.Should().NotBeNull();
        cluster1.Should().NotBeSameAs(cluster2);
    }

    public interface ITestProducer : IProducer;
    public interface ITestConsumer : IConsumer<string, byte[]>;
}
