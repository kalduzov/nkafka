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
    public async Task FailAllPending_WithInitializationError_FailsQueuedRecordWithInitializationError()
    {
        var config = new ProducerConfig();
        var accumulator = new RecordAccumulator(
            config,
            Substitute.For<ITransactionManager>(),
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);
        var topicPartition = new TopicPartition("test", 0);
        var append = accumulator.Append(topicPartition, 1_000, null, "value"u8.ToArray(), Headers.Empty);
        var error = new ProducerError(
            ErrorCodes.ClientError,
            ProducerLocalError.IdempotenceInitializationFailed);

        accumulator.FailAllPending(error);

        var exception = await Assert.ThrowsAsync<ProducerInitializationException>(
            async () => await append.SendResult!.Task);
        exception.Error.Should().Be(error);
    }

    [Fact]
    public async Task PullReadyBatches_ReturnsPartitionBatchesInOrderOneAtATime()
    {
        var config = new ProducerConfig
        {
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
        var flushFirstBatch = accumulator.FlushAllAsync(CancellationToken.None);
        accumulator.Append(topicPartition, 2_000, null, new byte[10], Headers.Empty);
        Thread.Sleep(5);

        var firstBatch = accumulator.PullReadyBatches(config.MaxRequestSize).Should().ContainSingle().Which;
        firstBatch.BaseTimestamp.Should().Be(1_000);
        firstBatch.Complete(0, 1_000);
        await flushFirstBatch;

        var secondBatch = accumulator.PullReadyBatches(config.MaxRequestSize).Should().ContainSingle().Which;
        secondBatch.BaseTimestamp.Should().Be(2_000);
        secondBatch.Complete(1, 2_000);
    }

    [Fact]
    public async Task PullReadyBatches_AssignsSequencesPerPartitionAndAllowsOnlyOneInFlightBatch()
    {
        var config = new ProducerConfig
        {
            EnableIdempotence = true,
            BatchSize = 80,
            MaxRequestSize = 1024 * 1024,
            LingerMs = 0
        };
        var transactionManager = Substitute.For<ITransactionManager>();
        transactionManager.CurrentProducerIdAndEpoch.Returns(new ProducerIdAndEpoch(42, 3));
        var accumulator = new RecordAccumulator(
            config,
            transactionManager,
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);
        var firstPartition = new TopicPartition("test", 0);
        var secondPartition = new TopicPartition("test", 1);

        accumulator.Append(firstPartition, 1_000, null, new byte[10], Headers.Empty);
        var flushTask = accumulator.FlushAllAsync(CancellationToken.None);
        accumulator.Append(firstPartition, 1_001, null, new byte[10], Headers.Empty);
        accumulator.Append(secondPartition, 1_002, null, new byte[10], Headers.Empty);
        Thread.Sleep(5);

        var initialBatches = accumulator.PullReadyBatches(config.MaxRequestSize).ToArray();
        initialBatches.Should().HaveCount(2);
        var firstPartitionBatch = initialBatches.Single(batch => batch.TopicPartition == firstPartition);
        var secondPartitionBatch = initialBatches.Single(batch => batch.TopicPartition == secondPartition);
        firstPartitionBatch.HasProducerState.Should().BeTrue();
        secondPartitionBatch.HasProducerState.Should().BeTrue();
        ReadRecordBatch(firstPartitionBatch).BaseSequence.Should().Be(0);
        ReadRecordBatch(secondPartitionBatch).BaseSequence.Should().Be(0);

        accumulator.PullReadyBatches(config.MaxRequestSize).Should().BeEmpty();

        accumulator.Requeue(firstPartitionBatch);
        var retriedBatch = accumulator.PullReadyBatches(config.MaxRequestSize).Should().ContainSingle().Which;
        retriedBatch.Should().BeSameAs(firstPartitionBatch);
        ReadRecordBatch(retriedBatch).BaseSequence.Should().Be(0);
        retriedBatch.Complete(0, 1_000);
        secondPartitionBatch.Complete(0, 1_002);
        retriedBatch.State.Should().Be(ProducerBatch.BatchState.Completed);

        var nextBatch = accumulator.PullReadyBatches(config.MaxRequestSize).Should().ContainSingle().Which;
        nextBatch.TopicPartition.Should().Be(firstPartition);
        ReadRecordBatch(nextBatch).BaseSequence.Should().Be(1);

        nextBatch.Complete(1, 1_001);
        await flushTask;
    }

    [Fact]
    public void PullReadyBatches_UsesIndependentProducerIdsForSeparateAccumulators()
    {
        var config = new ProducerConfig
        {
            EnableIdempotence = true,
            MaxRequestSize = 1024 * 1024,
            LingerMs = 0
        };
        var firstManager = Substitute.For<ITransactionManager>();
        firstManager.CurrentProducerIdAndEpoch.Returns(new ProducerIdAndEpoch(100, 0));
        var secondManager = Substitute.For<ITransactionManager>();
        secondManager.CurrentProducerIdAndEpoch.Returns(new ProducerIdAndEpoch(200, 0));
        var firstAccumulator = new RecordAccumulator(
            config,
            firstManager,
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);
        var secondAccumulator = new RecordAccumulator(
            config,
            secondManager,
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);
        var topicPartition = new TopicPartition("test", 0);

        firstAccumulator.Append(topicPartition, 1_000, null, new byte[10], Headers.Empty);
        secondAccumulator.Append(topicPartition, 1_000, null, new byte[10], Headers.Empty);
        Thread.Sleep(5);

        var firstBatch = firstAccumulator.PullReadyBatches(config.MaxRequestSize).Should().ContainSingle().Which;
        var secondBatch = secondAccumulator.PullReadyBatches(config.MaxRequestSize).Should().ContainSingle().Which;
        var firstRecordBatch = ReadRecordBatch(firstBatch);
        var secondRecordBatch = ReadRecordBatch(secondBatch);

        firstRecordBatch.ProducerId.Should().Be(100);
        secondRecordBatch.ProducerId.Should().Be(200);
        firstRecordBatch.BaseSequence.Should().Be(0);
        secondRecordBatch.BaseSequence.Should().Be(0);

        firstBatch.Complete(0, 1_000);
        secondBatch.Complete(0, 1_000);
    }

    [Fact]
    public async Task Append_SeparateCallsRemainSeparateRecordsInIdempotentBatch()
    {
        var config = new ProducerConfig
        {
            EnableIdempotence = true,
            MaxRequestSize = 1024 * 1024,
            LingerMs = 0
        };
        var transactionManager = Substitute.For<ITransactionManager>();
        transactionManager.CurrentProducerIdAndEpoch.Returns(new ProducerIdAndEpoch(42, 3));
        var accumulator = new RecordAccumulator(
            config,
            transactionManager,
            config.DeliveryTimeoutMs,
            Substitute.For<IProducerMetrics>(),
            NullLoggerFactory.Instance);
        var topicPartition = new TopicPartition("test", 0);
        var firstValue = "first"u8.ToArray();
        var secondValue = "second"u8.ToArray();

        accumulator.Append(topicPartition, 1_000, null, firstValue, Headers.Empty);
        accumulator.Append(topicPartition, 1_001, null, secondValue, Headers.Empty);
        var flushTask = accumulator.FlushAllAsync(CancellationToken.None);

        var batch = accumulator.PullReadyBatches(config.MaxRequestSize).Should().ContainSingle().Which;
        var recordBatch = ReadRecordBatch(batch);

        recordBatch.CountRecords.Should().Be(2);
        var records = recordBatch.Records.ToArray();
        records.Should().HaveCount(2);
        records[0].Value.Should().Equal(firstValue);
        records[1].Value.Should().Equal(secondValue);

        batch.Complete(0, 1_000);
        await flushTask;
    }

    private static RecordBatch ReadRecordBatch(ProducerBatch batch)
    {
        var records = batch.GetAsRecords();
        var reader = new BufferReader(records.Buffer.DangerousGetFirstBuffer().AsSpan(0, records.SizeInBytes));

        return new RecordBatch(ref reader);
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
