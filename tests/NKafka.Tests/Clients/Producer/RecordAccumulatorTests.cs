using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Clients.Producer;
using NKafka.Clients.Producer.Internals;
using NKafka.Config;
using NKafka.Metrics;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;
using NKafka.Protocol.Records;

namespace NKafka.Tests.Clients.Producer;

public sealed class RecordAccumulatorTests
{
    [Fact]
    public void PullReadyBatches_RemovesFirstReadyBatchFromPartitionQueue()
    {
        var config = new ProducerConfig
        {
            BatchSize = 80,
            MaxRequestSize = 1024 * 1024,
            LingerMs = 0
        };
        var accumulator = new RecordAccumulator(
            config,
            Substitute.For<ITransactionManager>(),
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);
        var topicPartition = new TopicPartition("test", 0);

        accumulator.Append(topicPartition, 1_000, null, new byte[10], Headers.Empty);
        accumulator.Append(topicPartition, 1_001, null, new byte[10], Headers.Empty);
        Thread.Sleep(5);

        var firstReadyBatch = accumulator.PullReadyBatches(config.MaxRequestSize).First();
        var records = firstReadyBatch.GetAsRecords();
        var reader = new BufferReader(records.Buffer.DangerousGetFirstBuffer().AsSpan(0, records.SizeInBytes));
        var recordBatch = new RecordBatch(ref reader);

        firstReadyBatch.TopicPartition.Should().Be(topicPartition);
        recordBatch.BaseTimestamp.Should().Be(1_000);
    }

    [Fact]
    public void Requeue_ReturnsBatchToTheBeginningOfItsPartitionQueue()
    {
        var config = new ProducerConfig
        {
            BatchSize = 80,
            MaxRequestSize = 1024 * 1024,
            LingerMs = 0
        };
        var accumulator = new RecordAccumulator(
            config,
            Substitute.For<ITransactionManager>(),
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);
        var topicPartition = new TopicPartition("test", 0);

        accumulator.Append(topicPartition, 1_000, null, new byte[10], Headers.Empty);
        Thread.Sleep(5);

        var batch = accumulator.PullReadyBatches(config.MaxRequestSize).Single();
        accumulator.Requeue(batch);

        accumulator.PullReadyBatches(config.MaxRequestSize).Single().Should().BeSameAs(batch);
    }

    [Fact]
    public async Task FlushAllAsync_CancellationCancelsOnlyWaitingAndClosesAcceptedBatch()
    {
        var config = new ProducerConfig
        {
            BatchSize = 100,
            MaxRequestSize = 1024 * 1024,
            LingerMs = 60_000
        };
        var accumulator = new RecordAccumulator(
            config,
            Substitute.For<ITransactionManager>(),
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);
        var topicPartition = new TopicPartition("test", 0);
        accumulator.Append(topicPartition, 1_000, null, new byte[10], Headers.Empty);

        using var cancellation = new CancellationTokenSource();
        var flush = accumulator.FlushAllAsync(cancellation.Token);
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => flush);

        var flushedBatch = accumulator.PullReadyBatches(config.MaxRequestSize).Single();
        var records = flushedBatch.GetAsRecords();
        var reader = new BufferReader(records.Buffer.DangerousGetFirstBuffer().AsSpan(0, records.SizeInBytes));
        var recordBatch = new RecordBatch(ref reader);
        recordBatch.BaseTimestamp.Should().Be(1_000);
    }

    [Fact]
    public void Append_WhenBufferMemoryIsExhausted_ReturnsEnqueueTimeout()
    {
        var config = new ProducerConfig
        {
            BatchSize = 80,
            BufferMemory = 120,
            EnqueueTimeoutMs = 10,
            MaxRequestSize = 1024 * 1024
        };
        var accumulator = new RecordAccumulator(
            config,
            Substitute.For<ITransactionManager>(),
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var accepted = accumulator.Append(new TopicPartition("test", 0), 1_000, null, new byte[1], Headers.Empty);
        var rejected = accumulator.Append(new TopicPartition("test", 1), 1_001, null, new byte[1], Headers.Empty);

        accepted.Error.Should().BeNull();
        rejected.Error.Should().Be(new ProducerError(ErrorCodes.ClientError, ProducerLocalError.EnqueueTimedOut));

        Thread.Sleep(5);
        accumulator.PullReadyBatches(config.MaxRequestSize).Single();

        var acceptedAfterRelease = accumulator.Append(new TopicPartition("test", 1), 1_002, null, new byte[1], Headers.Empty);
        acceptedAfterRelease.Error.Should().BeNull();
    }

    [Fact]
    public void Append_WhenQueuedMessageLimitIsReached_ReleasesCapacityAfterBatchCompletion()
    {
        var config = new ProducerConfig
        {
            BatchSize = 80,
            MaxQueuedMessages = 1,
            EnqueueTimeoutMs = 10,
            MaxRequestSize = 1024 * 1024
        };
        var accumulator = new RecordAccumulator(
            config,
            Substitute.For<ITransactionManager>(),
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);

        var accepted = accumulator.Append(new TopicPartition("test", 0), 1_000, null, new byte[1], Headers.Empty);
        var rejected = accumulator.Append(new TopicPartition("test", 1), 1_001, null, new byte[1], Headers.Empty);

        accepted.Error.Should().BeNull();
        rejected.Error.Should().Be(new ProducerError(ErrorCodes.ClientError, ProducerLocalError.EnqueueTimedOut));

        Thread.Sleep(5);
        var batch = accumulator.PullReadyBatches(config.MaxRequestSize).Single();
        batch.Complete(0, 1_000);

        accumulator.Append(new TopicPartition("test", 1), 1_002, null, new byte[1], Headers.Empty).Error.Should().BeNull();
    }
}
