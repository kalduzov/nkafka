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
    public async Task SendProducerDataAsync_WithNoLeaderAfterMetadataRefresh_RequeuesBatch()
    {
        var config = new ProducerConfig { Acks = Acks.Leader };
        var topicPartition = new TopicPartition("test", 0);
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(topicPartition, buffer, NullLoggerFactory.Instance);
        batch.TryAppend(1_000, null, "value"u8.ToArray(), Headers.Empty, out var resultTask).Should().BeTrue();
        batch.Close();

        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([batch]);
        var kafkaCluster = Substitute.For<IKafkaCluster>();
        kafkaCluster.LeaderFor(topicPartition).Returns(Node.NoNode);
        kafkaCluster.RefreshMetadataAsync([topicPartition.Topic], Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var sendResult = await sender.SendProducerDataAsync(CancellationToken.None);

        sendResult.Should().Be(MessagesSender.SendCycleResult.RetryScheduled);
        batch.State.Should().Be(ProducerBatch.BatchState.Closed);
        accumulator.Received(1).Requeue(batch);
        (resultTask!.Task.IsCompleted).Should().BeFalse();

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public async Task SendProducerDataAsync_WithMultiplePartitionsOnSameBroker_SendsOneRequestAndMatchesResponses()
    {
        var config = new ProducerConfig { Acks = Acks.Leader };
        var firstPartition = new TopicPartition("test", 0);
        var secondPartition = new TopicPartition("test", 1);
        var firstBuffer = ArrayBufferPool.Rent(1024);
        var secondBuffer = ArrayBufferPool.Rent(1024);
        var firstBatch = new ProducerBatch(firstPartition, firstBuffer, NullLoggerFactory.Instance);
        var secondBatch = new ProducerBatch(secondPartition, secondBuffer, NullLoggerFactory.Instance);
        firstBatch.TryAppend(1_000, null, "first"u8.ToArray(), Headers.Empty, out var firstResult).Should().BeTrue();
        secondBatch.TryAppend(1_000, null, "second"u8.ToArray(), Headers.Empty, out var secondResult).Should().BeTrue();
        firstBatch.Close();
        secondBatch.Close();

        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([firstBatch, secondBatch]);

        var node = new Node(7, "localhost", 9092);
        var kafkaCluster = Substitute.For<IKafkaCluster>();
        kafkaCluster.LeaderFor(firstPartition).Returns(node);
        kafkaCluster.LeaderFor(secondPartition).Returns(node);
        ProduceRequestMessage? capturedRequest = null;
        kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(),
                node.Id,
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                capturedRequest = call.Arg<ProduceRequestMessage>();
                return new ProduceResponseMessage
                {
                    Responses =
                    [
                        new ProduceResponseMessage.TopicProduceResponseMessage
                        {
                            Name = "test",
                            PartitionResponses =
                            [
                                new ProduceResponseMessage.PartitionProduceResponseMessage
                                {
                                    Index = 0,
                                    BaseOffset = 10
                                },
                                new ProduceResponseMessage.PartitionProduceResponseMessage
                                {
                                    Index = 1,
                                    BaseOffset = 20
                                }
                            ]
                        }
                    ]
                };
            });

        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var result = await sender.SendProducerDataAsync(CancellationToken.None);

        result.Should().Be(MessagesSender.SendCycleResult.WorkCompleted);
        capturedRequest.Should().NotBeNull();
        capturedRequest!.TopicData.Should().ContainSingle();
        capturedRequest.TopicData.Single().PartitionData.Select(partition => partition.Index)
            .Should().BeEquivalentTo([0, 1]);
        await kafkaCluster.Received(1).SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
            Arg.Any<ProduceRequestMessage>(),
            node.Id,
            Arg.Any<CancellationToken>());
        (await firstResult!.Task).Offset.Should().Be(new Offset(10));
        (await secondResult!.Task).Offset.Should().Be(new Offset(20));

        ArrayBufferPool.Return(firstBuffer);
        ArrayBufferPool.Return(secondBuffer);
    }

    [Fact]
    public async Task SendProducerDataAsync_WithMissingPartitionResponse_RetriesOnlyThatPartition()
    {
        var config = new ProducerConfig { Acks = Acks.Leader };
        var firstPartition = new TopicPartition("test", 0);
        var secondPartition = new TopicPartition("test", 1);
        var firstBuffer = ArrayBufferPool.Rent(1024);
        var secondBuffer = ArrayBufferPool.Rent(1024);
        var firstBatch = new ProducerBatch(firstPartition, firstBuffer, NullLoggerFactory.Instance);
        var secondBatch = new ProducerBatch(secondPartition, secondBuffer, NullLoggerFactory.Instance);
        firstBatch.TryAppend(1_000, null, "first"u8.ToArray(), Headers.Empty, out var firstResult).Should().BeTrue();
        secondBatch.TryAppend(1_000, null, "second"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        firstBatch.Close();
        secondBatch.Close();

        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([firstBatch, secondBatch]);

        var node = new Node(7, "localhost", 9092);
        var kafkaCluster = Substitute.For<IKafkaCluster>();
        kafkaCluster.LeaderFor(firstPartition).Returns(node);
        kafkaCluster.LeaderFor(secondPartition).Returns(node);
        kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(),
                node.Id,
                Arg.Any<CancellationToken>())
            .Returns(new ProduceResponseMessage
            {
                Responses =
                [
                    new ProduceResponseMessage.TopicProduceResponseMessage
                    {
                        Name = "test",
                        PartitionResponses =
                        [
                            new ProduceResponseMessage.PartitionProduceResponseMessage
                            {
                                Index = 0,
                                BaseOffset = 10
                            }
                        ]
                    }
                ]
            });

        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var result = await sender.SendProducerDataAsync(CancellationToken.None);

        result.Should().Be(MessagesSender.SendCycleResult.RetryScheduled);
        (await firstResult!.Task).Offset.Should().Be(new Offset(10));
        accumulator.Received(1).Requeue(secondBatch);

        ArrayBufferPool.Return(firstBuffer);
        ArrayBufferPool.Return(secondBuffer);
    }

    [Fact]
    public async Task SendProducerDataAsync_WithPartitionsOnDifferentBrokers_SendsSeparateRequests()
    {
        var config = new ProducerConfig { Acks = Acks.Leader };
        var firstPartition = new TopicPartition("test", 0);
        var secondPartition = new TopicPartition("test", 1);
        var firstBuffer = ArrayBufferPool.Rent(1024);
        var secondBuffer = ArrayBufferPool.Rent(1024);
        var firstBatch = new ProducerBatch(firstPartition, firstBuffer, NullLoggerFactory.Instance);
        var secondBatch = new ProducerBatch(secondPartition, secondBuffer, NullLoggerFactory.Instance);
        firstBatch.TryAppend(1_000, null, "first"u8.ToArray(), Headers.Empty, out var firstResult).Should().BeTrue();
        secondBatch.TryAppend(1_000, null, "second"u8.ToArray(), Headers.Empty, out var secondResult).Should().BeTrue();
        firstBatch.Close();
        secondBatch.Close();

        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([firstBatch, secondBatch]);

        var firstNode = new Node(7, "localhost", 9092);
        var secondNode = new Node(8, "localhost", 9093);
        var kafkaCluster = Substitute.For<IKafkaCluster>();
        kafkaCluster.LeaderFor(firstPartition).Returns(firstNode);
        kafkaCluster.LeaderFor(secondPartition).Returns(secondNode);
        kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                var request = call.Arg<ProduceRequestMessage>();
                var nodeId = call.ArgAt<int>(1);

                return new ProduceResponseMessage
                {
                    Responses =
                    [
                        new ProduceResponseMessage.TopicProduceResponseMessage
                        {
                            Name = "test",
                            PartitionResponses =
                            [
                                new ProduceResponseMessage.PartitionProduceResponseMessage
                                {
                                    Index = request.TopicData.Single().PartitionData.Single().Index,
                                    BaseOffset = nodeId == firstNode.Id ? 10 : 20
                                }
                            ]
                        }
                    ]
                };
            });

        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var result = await sender.SendProducerDataAsync(CancellationToken.None);

        result.Should().Be(MessagesSender.SendCycleResult.WorkCompleted);
        await kafkaCluster.Received(1).SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
            Arg.Any<ProduceRequestMessage>(),
            firstNode.Id,
            Arg.Any<CancellationToken>());
        await kafkaCluster.Received(1).SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
            Arg.Any<ProduceRequestMessage>(),
            secondNode.Id,
            Arg.Any<CancellationToken>());
        (await firstResult!.Task).Offset.Should().Be(new Offset(10));
        (await secondResult!.Task).Offset.Should().Be(new Offset(20));

        ArrayBufferPool.Return(firstBuffer);
        ArrayBufferPool.Return(secondBuffer);
    }

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
        var firstPartition = new TopicPartition("test", 0);
        var secondPartition = new TopicPartition("test", 1);
        var firstBuffer = ArrayBufferPool.Rent(1024);
        var secondBuffer = ArrayBufferPool.Rent(1024);
        var firstBatch = new ProducerBatch(firstPartition, firstBuffer, NullLoggerFactory.Instance);
        var secondBatch = new ProducerBatch(secondPartition, secondBuffer, NullLoggerFactory.Instance);
        firstBatch.TryAppend(1_000, null, "first"u8.ToArray(), Headers.Empty, out var firstResult).Should().BeTrue();
        secondBatch.TryAppend(1_000, null, "second"u8.ToArray(), Headers.Empty, out var secondResult).Should().BeTrue();
        firstBatch.Close();
        secondBatch.Close();

        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([firstBatch, secondBatch]);

        var kafkaCluster = Substitute.For<IKafkaCluster>();
        var node = new Node(1, "localhost", 9092);
        kafkaCluster.LeaderFor(firstPartition).Returns(node);
        kafkaCluster.LeaderFor(secondPartition).Returns(node);
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

        var firstException = await Assert.ThrowsAsync<ProducerTransportException>(() => firstResult!.Task);
        var secondException = await Assert.ThrowsAsync<ProducerTransportException>(() => secondResult!.Task);
        firstException.Status.Should().Be(PersistenceStatus.PossiblyPersisted);
        secondException.Status.Should().Be(PersistenceStatus.PossiblyPersisted);
        accumulator.DidNotReceive().Requeue(firstBatch);
        accumulator.DidNotReceive().Requeue(secondBatch);

        ArrayBufferPool.Return(firstBuffer);
        ArrayBufferPool.Return(secondBuffer);
    }
}
