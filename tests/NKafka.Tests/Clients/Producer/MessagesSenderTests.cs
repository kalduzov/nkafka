using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Clients.Producer;
using NKafka.Clients.Producer.Internals;
using NKafka.Config;
using NKafka.Connection;
using NKafka.Metrics;
using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;
using NKafka.Protocol.Records;

namespace NKafka.Tests.Clients.Producer;

public sealed class MessagesSenderTests
{
    [Fact]
    public async Task SendProducerDataAsync_WithAcksNone_WritesWithoutWaitingForResponse()
    {
        var config = new ProducerConfig { Acks = Acks.None };
        var topicPartition = new TopicPartition("test", 0);
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(topicPartition, buffer, NullLoggerFactory.Instance);
        batch.TryAppend(1_000, null, "value"u8.ToArray(), Headers.Empty, out var sendResultTask).Should().BeTrue();
        batch.Close();

        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([batch]);

        var transactionManager = Substitute.For<ITransactionManager>();
        var kafkaCluster = Substitute.For<IKafkaCluster>();
        var node = new Node(1, "localhost", 9092);
        kafkaCluster.LeaderFor(topicPartition).Returns(node);
        kafkaCluster.SendAsync<ProduceRequestMessage>(
                Arg.Any<ProduceRequestMessage>(),
                node.Id,
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var sender = new MessagesSender(
            config,
            accumulator,
            transactionManager,
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var result = await sender.SendProducerDataAsync(CancellationToken.None);

        result.Should().Be(MessagesSender.SendCycleResult.WorkCompleted);
        batch.State.Should().Be(ProducerBatch.BatchState.Completed);
        (await sendResultTask!.Task).Offset.Should().Be(Offset.Unset);
        await kafkaCluster.Received(1).SendAsync<ProduceRequestMessage>(
            Arg.Any<ProduceRequestMessage>(),
            node.Id,
            Arg.Any<CancellationToken>());
        await kafkaCluster.DidNotReceive().SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
            Arg.Any<ProduceRequestMessage>(),
            node.Id,
            Arg.Any<CancellationToken>());

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public async Task SendProducerDataAsync_WithAcksNoneAndFailureBeforeWrite_DoesNotRetryAndReportsNotPersisted()
    {
        var config = new ProducerConfig { Acks = Acks.None };
        var topicPartition = new TopicPartition("test", 0);
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(topicPartition, buffer, NullLoggerFactory.Instance);
        batch.TryAppend(1_000, null, "value"u8.ToArray(), Headers.Empty, out var sendResultTask).Should().BeTrue();
        batch.Close();

        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([batch]);

        var transactionManager = Substitute.For<ITransactionManager>();
        var kafkaCluster = Substitute.For<IKafkaCluster>();
        kafkaCluster.LeaderFor(topicPartition).Returns(new Node(1, "localhost", 9092));
        kafkaCluster.SendAsync<ProduceRequestMessage>(
                Arg.Any<ProduceRequestMessage>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new IOException("Write failed"));

        var sender = new MessagesSender(
            config,
            accumulator,
            transactionManager,
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        await sender.SendProducerDataAsync(CancellationToken.None);

        var exception = await Assert.ThrowsAsync<ProducerTransportException>(() => sendResultTask!.Task);
        exception.Status.Should().Be(PersistenceStatus.NotPersisted);
        exception.Error.Should().Be(new ProducerError(ErrorCodes.ClientError, ProducerLocalError.TransportFailure));
        accumulator.DidNotReceive().Requeue(batch);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public async Task SendProducerDataAsync_WithAcksNoneAndFailureDuringWrite_DoesNotRetryAndReportsPossiblyPersisted()
    {
        var config = new ProducerConfig { Acks = Acks.None };
        var topicPartition = new TopicPartition("test", 0);
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(topicPartition, buffer, NullLoggerFactory.Instance);
        batch.TryAppend(1_000, null, "value"u8.ToArray(), Headers.Empty, out var sendResultTask).Should().BeTrue();
        batch.Close();

        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([batch]);

        var kafkaCluster = Substitute.For<IKafkaCluster>();
        kafkaCluster.LeaderFor(topicPartition).Returns(new Node(1, "localhost", 9092));
        kafkaCluster.SendAsync<ProduceRequestMessage>(
                Arg.Any<ProduceRequestMessage>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new RequestWriteException(new IOException("Connection closed during write")));

        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        await sender.SendProducerDataAsync(CancellationToken.None);

        var exception = await Assert.ThrowsAsync<ProducerTransportException>(() => sendResultTask!.Task);
        exception.Status.Should().Be(PersistenceStatus.PossiblyPersisted);
        accumulator.DidNotReceive().Requeue(batch);

        ArrayBufferPool.Return(buffer);
    }
}
