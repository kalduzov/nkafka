using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Clients.Producer;
using NKafka.Clients.Producer.Internals;
using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;
using NKafka.Metrics;
using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;
using NKafka.Protocol.Records;

namespace NKafka.Tests.Clients.Producer;

public sealed class MessagesSenderTests
{
    [Fact]
    public async Task RunOnceAsync_IdempotentProducerRetriesInitializationBeforePullingBatches()
    {
        var config = new ProducerConfig { EnableIdempotence = true };
        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.HasPendingRecords.Returns(true);
        var transactionManager = Substitute.For<ITransactionManager>();
        transactionManager.IsTransactional.Returns(false);
        transactionManager.EnsureIdempotentProducerIdAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        var sender = new MessagesSender(
            config,
            accumulator,
            transactionManager,
            Substitute.For<IKafkaCluster>(),
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var result = await sender.RunOnceAsync(CancellationToken.None);

        result.Should().Be(MessagesSender.SendCycleResult.RetryScheduled);
        _ = accumulator.DidNotReceive().PullReadyBatches(Arg.Any<int>());
    }

    [Fact]
    public async Task RunOnceAsync_PermanentInitializationFailureFailsQueuedBatchesAndStopsSending()
    {
        var config = new ProducerConfig { EnableIdempotence = true };
        var initializationError = new ProducerError(
            ErrorCodes.ClientError,
            ProducerLocalError.IdempotenceInitializationFailed);
        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.HasPendingRecords.Returns(true);
        var transactionManager = Substitute.For<ITransactionManager>();
        transactionManager.IsTransactional.Returns(false);
        transactionManager.EnsureIdempotentProducerIdAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        transactionManager.IdempotenceInitializationError.Returns(initializationError);
        var kafkaCluster = Substitute.For<IKafkaCluster>();
        var sender = new MessagesSender(
            config,
            accumulator,
            transactionManager,
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var result = await sender.RunOnceAsync(CancellationToken.None);

        result.Should().Be(MessagesSender.SendCycleResult.WorkCompleted);
        accumulator.Received(1).FailAllPending(initializationError);
        _ = accumulator.DidNotReceive().PullReadyBatches(Arg.Any<int>());
        await kafkaCluster.DidNotReceive()
            .SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunOnceAsync_IdempotentProducerInitializesBeforePullingBatches()
    {
        var config = new ProducerConfig { EnableIdempotence = true };
        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.HasPendingRecords.Returns(true);
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([]);
        var transactionManager = Substitute.For<ITransactionManager>();
        transactionManager.IsTransactional.Returns(false);
        transactionManager.EnsureIdempotentProducerIdAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        var sender = new MessagesSender(
            config,
            accumulator,
            transactionManager,
            Substitute.For<IKafkaCluster>(),
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var result = await sender.RunOnceAsync(CancellationToken.None);

        result.Should().Be(MessagesSender.SendCycleResult.NoWork);
        await transactionManager.Received(1).EnsureIdempotentProducerIdAsync(Arg.Any<CancellationToken>());
        _ = accumulator.Received(1).PullReadyBatches(config.MaxRequestSize);
    }

    [Fact]
    public async Task RunOnceAsync_DoesNotInitializeIdempotenceWhenNoRecordsArePending()
    {
        var config = new ProducerConfig { EnableIdempotence = true };
        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.HasPendingRecords.Returns(false);
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([]);
        var transactionManager = Substitute.For<ITransactionManager>();
        transactionManager.IsTransactional.Returns(false);
        var sender = new MessagesSender(
            config,
            accumulator,
            transactionManager,
            Substitute.For<IKafkaCluster>(),
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var result = await sender.RunOnceAsync(CancellationToken.None);

        result.Should().Be(MessagesSender.SendCycleResult.NoWork);
        await transactionManager.DidNotReceive().EnsureIdempotentProducerIdAsync(Arg.Any<CancellationToken>());
    }

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
    public async Task SendProducerDataAsync_WithNetworkFailure_RetriesBatch()
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
        kafkaCluster.LeaderFor(topicPartition).Returns(new Node(1, "localhost", 9092));
        kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<ProduceResponseMessage>>(_ => throw new ProtocolKafkaException(ErrorCodes.NetworkException));

        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var sendResult = await sender.SendProducerDataAsync(CancellationToken.None);

        sendResult.Should().Be(MessagesSender.SendCycleResult.RetryScheduled);
        accumulator.Received(1).Requeue(batch);
        resultTask!.Task.IsCompleted.Should().BeFalse();

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public async Task SendProducerDataAsync_AfterLostResponse_RetriesIdenticalIdempotentBatch()
    {
        var config = new ProducerConfig { Acks = Acks.Leader };
        var topicPartition = new TopicPartition("test", 0);
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(topicPartition, buffer, NullLoggerFactory.Instance);
        batch.TryAppend(1_000, null, "value"u8.ToArray(), Headers.Empty, out var resultTask).Should().BeTrue();
        batch.SetProducerState(new ProducerIdAndEpoch(123, 4), 17);
        batch.Close();

        var accumulator = Substitute.For<IRecordAccumulator>();
        accumulator.PullReadyBatches(config.MaxRequestSize).Returns([batch]);
        var kafkaCluster = Substitute.For<IKafkaCluster>();
        kafkaCluster.LeaderFor(topicPartition).Returns(new Node(1, "localhost", 9092));
        var requests = new List<byte[]>();
        var attempts = 0;
        kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                var request = call.Arg<ProduceRequestMessage>();
                var records = request.TopicData.Single().PartitionData.Single().Records!;
                requests.Add(records.Buffer.DangerousGetFirstBuffer().AsSpan(0, records.SizeInBytes).ToArray());

                if (++attempts == 1)
                {
                    throw new TimeoutException("The Produce response was lost.");
                }

                return Task.FromResult(new ProduceResponseMessage
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
                                    BaseOffset = 42
                                }
                            ]
                        }
                    ]
                });
            });
        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var firstAttempt = await sender.SendProducerDataAsync(CancellationToken.None);
        firstAttempt.Should().Be(MessagesSender.SendCycleResult.RetryScheduled);
        batch.State.Should().Be(ProducerBatch.BatchState.Closed);
        resultTask!.Task.IsCompleted.Should().BeFalse();

        var retryAttempt = await sender.SendProducerDataAsync(CancellationToken.None);

        retryAttempt.Should().Be(MessagesSender.SendCycleResult.WorkCompleted);
        requests.Should().HaveCount(2);
        requests[1].Should().Equal(requests[0]);
        var reader = new BufferReader(requests[1]);
        var retriedRecordBatch = new RecordBatch(ref reader);
        retriedRecordBatch.ProducerId.Should().Be(123);
        retriedRecordBatch.ProducerEpoch.Should().Be(4);
        retriedRecordBatch.BaseSequence.Should().Be(17);
        (await resultTask.Task).Offset.Should().Be(new Offset(42));
        accumulator.Received(1).Requeue(batch);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public async Task SendProducerDataAsync_WithPermanentRequestFailure_FailsBatchWithoutRetry()
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
        kafkaCluster.LeaderFor(topicPartition).Returns(new Node(1, "localhost", 9092));
        kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<ProduceResponseMessage>>(_ => throw new ProtocolKafkaException(ErrorCodes.InvalidRequiredAcks));

        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        await sender.SendProducerDataAsync(CancellationToken.None);

        var exception = await Assert.ThrowsAsync<ProtocolKafkaException>(() => resultTask!.Task);
        exception.InternalError.Should().Be(ErrorCodes.InvalidRequiredAcks);
        accumulator.DidNotReceive().Requeue(batch);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public async Task SendProducerDataAsync_WithPartitionsOnDifferentBrokers_SendsSeparateRequests()
    {
        static ProduceResponseMessage CreateResponse(
            Partition partition,
            long baseOffset,
            ErrorCodes code = ErrorCodes.None)
            => new()
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
                                Index = partition,
                                BaseOffset = baseOffset,
                                ErrorCode = (short)code
                            }
                        ]
                    }
                ]
            };

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
        var firstResponse = new TaskCompletionSource<ProduceResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondResponse = new TaskCompletionSource<ProduceResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        var firstRequestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondRequestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                var nodeId = call.ArgAt<int>(1);

                if (nodeId == firstNode.Id)
                {
                    firstRequestStarted.TrySetResult();

                    return firstResponse.Task;
                }

                secondRequestStarted.TrySetResult();

                return secondResponse.Task;
            });

        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var sendTask = sender.SendProducerDataAsync(CancellationToken.None);
        var requestsStartedConcurrently = false;
        var secondCompletedWhileFirstWasWaiting = false;

        try
        {
            await Task.WhenAll(firstRequestStarted.Task, secondRequestStarted.Task)
                .WaitAsync(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);
            requestsStartedConcurrently = true;
            secondResponse.TrySetResult(CreateResponse(secondPartition.Partition, 20));
            await secondResult!.Task.WaitAsync(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);
            secondCompletedWhileFirstWasWaiting = !firstResult!.Task.IsCompleted;
        }
        catch (TimeoutException)
        {
            // Complete the first response below so a sequential implementation can finish cleanly.
        }
        finally
        {
            firstResponse.TrySetResult(
                CreateResponse(firstPartition.Partition, 10, ErrorCodes.InvalidRequiredAcks));
            secondResponse.TrySetResult(CreateResponse(secondPartition.Partition, 20));
        }

        var result = await sendTask;

        requestsStartedConcurrently.Should().BeTrue();
        secondCompletedWhileFirstWasWaiting.Should().BeTrue();
        result.Should().Be(MessagesSender.SendCycleResult.WorkCompleted);
        await kafkaCluster.Received(1).SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
            Arg.Any<ProduceRequestMessage>(),
            firstNode.Id,
            Arg.Any<CancellationToken>());
        await kafkaCluster.Received(1).SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
            Arg.Any<ProduceRequestMessage>(),
            secondNode.Id,
            Arg.Any<CancellationToken>());
        var firstException = await Assert.ThrowsAsync<ProtocolKafkaException>(() => firstResult!.Task);
        firstException.InternalError.Should().Be(ErrorCodes.InvalidRequiredAcks);
        (await secondResult!.Task).Offset.Should().Be(new Offset(20));

        ArrayBufferPool.Return(firstBuffer);
        ArrayBufferPool.Return(secondBuffer);
    }

    [Fact]
    public async Task SendProducerDataAsync_WhenCancelledWithMultipleBrokerRequests_FailsEveryOwnedBatch()
    {
        static async Task<ProduceResponseMessage> WaitForCancellationAsync(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token);

            return new ProduceResponseMessage();
        }

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
        var firstRequestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondRequestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                Arg.Any<ProduceRequestMessage>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                var token = call.Arg<CancellationToken>();

                if (call.ArgAt<int>(1) == firstNode.Id)
                {
                    firstRequestStarted.TrySetResult();
                }
                else
                {
                    secondRequestStarted.TrySetResult();
                }

                return WaitForCancellationAsync(token);
            });

        var sender = new MessagesSender(
            config,
            accumulator,
            Substitute.For<ITransactionManager>(),
            kafkaCluster,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);
        using var cancellation = new CancellationTokenSource();

        var sendTask = sender.SendProducerDataAsync(cancellation.Token);
        await Task.WhenAll(firstRequestStarted.Task, secondRequestStarted.Task)
            .WaitAsync(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => sendTask);

        var firstException = await Assert.ThrowsAsync<ProducerClosingException>(() => firstResult!.Task);
        var secondException = await Assert.ThrowsAsync<ProducerClosingException>(() => secondResult!.Task);
        firstException.Status.Should().Be(PersistenceStatus.PossiblyPersisted);
        secondException.Status.Should().Be(PersistenceStatus.PossiblyPersisted);

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
