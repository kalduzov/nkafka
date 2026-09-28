// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Clients.Producer;
using NKafka.Clients.Producer.Internals;
using NKafka.Config;
using NKafka.Messages;
using NKafka.Protocol;

using NSubstitute;

namespace NKafka.Tests.Clients.Producer;

public sealed class TransactionManagerTests
{
    [Fact]
    public async Task EnsureIdempotentProducerIdAsync_SendsInitProducerIdWithoutTransactionalId()
    {
        var cluster = Substitute.For<IKafkaCluster>();
        SetProduceApiVersion(cluster, ApiVersion.Version3);
        InitProducerIdRequestMessage? capturedRequest = null;
        cluster.SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(
                Arg.Do<InitProducerIdRequestMessage>(request => capturedRequest = request),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new InitProducerIdResponseMessage
            {
                ProducerId = 123,
                ProducerEpoch = 0
            }));
        var manager = new TransactionManager(
            new ProducerConfig { EnableIdempotence = true },
            NullLoggerFactory.Instance,
            cluster);

        var initialized = await manager.EnsureIdempotentProducerIdAsync(CancellationToken.None);

        initialized.Should().BeTrue();
        manager.HasProducerId.Should().BeTrue();
        capturedRequest.Should().NotBeNull();
        capturedRequest!.TransactionalId.Should().BeNull();
    }

    [Fact]
    public async Task EnsureIdempotentProducerIdAsync_DoesNotSendRequestWhenIdempotenceIsDisabled()
    {
        var cluster = Substitute.For<IKafkaCluster>();
        var manager = new TransactionManager(new ProducerConfig(), NullLoggerFactory.Instance, cluster);

        var initialized = await manager.EnsureIdempotentProducerIdAsync(CancellationToken.None);

        initialized.Should().BeTrue();
        await cluster.DidNotReceive()
            .SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(
                Arg.Any<InitProducerIdRequestMessage>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EnsureIdempotentProducerIdAsync_RetriesRetriableResponse()
    {
        var cluster = Substitute.For<IKafkaCluster>();
        SetProduceApiVersion(cluster, ApiVersion.Version3);
        cluster.SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(
                Arg.Any<InitProducerIdRequestMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(
                _ => Task.FromResult(new InitProducerIdResponseMessage
                {
                    ErrorCode = (short)ErrorCodes.CoordinatorNotAvailable
                }),
                _ => Task.FromResult(new InitProducerIdResponseMessage
                {
                    ProducerId = 456,
                    ProducerEpoch = 0
                }));
        var manager = new TransactionManager(
            new ProducerConfig { EnableIdempotence = true },
            NullLoggerFactory.Instance,
            cluster);

        var firstAttempt = await manager.EnsureIdempotentProducerIdAsync(CancellationToken.None);
        var secondAttempt = await manager.EnsureIdempotentProducerIdAsync(CancellationToken.None);

        firstAttempt.Should().BeFalse();
        secondAttempt.Should().BeTrue();
        manager.IdempotenceInitializationError.Should().BeNull();
        await cluster.Received(2)
            .SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(
                Arg.Any<InitProducerIdRequestMessage>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EnsureIdempotentProducerIdAsync_PermanentFailureIsSticky()
    {
        var cluster = Substitute.For<IKafkaCluster>();
        cluster.SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(
                Arg.Any<InitProducerIdRequestMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new InitProducerIdResponseMessage
            {
                ErrorCode = (short)ErrorCodes.UnsupportedVersion
            }));
        var manager = new TransactionManager(
            new ProducerConfig { EnableIdempotence = true },
            NullLoggerFactory.Instance,
            cluster);

        var firstAttempt = await manager.EnsureIdempotentProducerIdAsync(CancellationToken.None);
        var secondAttempt = await manager.EnsureIdempotentProducerIdAsync(CancellationToken.None);

        firstAttempt.Should().BeFalse();
        secondAttempt.Should().BeFalse();
        manager.IdempotenceInitializationError.Should().Be(
            new ProducerError(ErrorCodes.ClientError, ProducerLocalError.IdempotenceInitializationFailed));
        await cluster.Received(1)
            .SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(
                Arg.Any<InitProducerIdRequestMessage>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EnsureIdempotentProducerIdAsync_StopsRetryingAfterDeliveryTimeout()
    {
        var cluster = Substitute.For<IKafkaCluster>();
        cluster.SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(
                Arg.Any<InitProducerIdRequestMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new InitProducerIdResponseMessage
            {
                ErrorCode = (short)ErrorCodes.CoordinatorNotAvailable
            }));
        var manager = new TransactionManager(
            new ProducerConfig { EnableIdempotence = true, DeliveryTimeoutMs = 1 },
            NullLoggerFactory.Instance,
            cluster);

        var firstAttempt = await manager.EnsureIdempotentProducerIdAsync(CancellationToken.None);
        Thread.Sleep(5);
        var secondAttempt = await manager.EnsureIdempotentProducerIdAsync(CancellationToken.None);

        firstAttempt.Should().BeFalse();
        secondAttempt.Should().BeFalse();
        manager.IdempotenceInitializationError.Should().NotBeNull();
        await cluster.Received(1)
            .SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(
                Arg.Any<InitProducerIdRequestMessage>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EnsureIdempotentProducerIdAsync_FailsWhenProduceCannotCarryIdempotenceFields()
    {
        var cluster = Substitute.For<IKafkaCluster>();
        SetProduceApiVersion(cluster, ApiVersion.Version2);
        cluster.SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(
                Arg.Any<InitProducerIdRequestMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new InitProducerIdResponseMessage
            {
                ProducerId = 789,
                ProducerEpoch = 0
            }));
        var manager = new TransactionManager(
            new ProducerConfig { EnableIdempotence = true },
            NullLoggerFactory.Instance,
            cluster);

        var initialized = await manager.EnsureIdempotentProducerIdAsync(CancellationToken.None);

        initialized.Should().BeFalse();
        manager.IdempotenceInitializationError.Should().Be(
            new ProducerError(ErrorCodes.ClientError, ProducerLocalError.IdempotenceInitializationFailed));
        await cluster.DidNotReceive()
            .SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    private static void SetProduceApiVersion(IKafkaCluster cluster, ApiVersion maxVersion)
    {
        var metadata = new ClusterMetadata();
        metadata.AggregationApiByVersion[ApiKeys.Produce] = new ApiMetadata
        {
            MinVersion = ApiVersion.Version0,
            MaxVersion = maxVersion
        };
        cluster.GetClusterMetadata().Returns(metadata);
    }
}
