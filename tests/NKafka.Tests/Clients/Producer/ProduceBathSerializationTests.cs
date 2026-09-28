//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Buffers.Binary;
using System.Text;

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Clients.Producer;
using NKafka.Clients.Producer.Internals;
using NKafka.Compressions;
using NKafka.Config;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;
using NKafka.Protocol.Records;

namespace NKafka.Tests.Clients.Producer;

public class ProduceBathSerializationTests
{
    private byte[] _testSerialization =
    [
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x3c,
        0x00,
        0x00,
        0x00,
        0x00,
        0x02,
        0x70,
        0x07,
        0x01,
        0x6d,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x01,
        0x86,
        0xcf,
        0x2a,
        0xa0,
        0x85,
        0x00,
        0x00,
        0x01,
        0x86,
        0xcf,
        0x2a,
        0xa0,
        0x85,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0x00,
        0x00,
        0x00,
        0x01,
        0x14,
        0x00,
        0x00,
        0x00,
        0x01,
        0x08,
        0x74,
        0x65,
        0x73,
        0x74,
        0x00
    ];

    [Fact]
    public void ProduceBatchSerializeTest()
    {
        // var buffer = new byte[_testSerialization.Length + 28]; //21 is possible overhead
        // var writer = new BufferWriter(new MemoryStream(buffer, 0, buffer.Length, true, true), 61); //61 is batch header size
        // var producerBatch =
        //     new ProducerBatch(new TopicPartition("test", 0), writer, NullLoggerFactory.Instance, 1678512922757); //1678512922757 - test timestamp
        // producerBatch.TryAppend(0, null, "test"u8.ToArray(), Headers.Empty, out _);
        // producerBatch.Close();
        // buffer[.._testSerialization.Length].Should().BeEquivalentTo(_testSerialization);
    }

    [Fact]
    public void ProducerBatch_StateMustFollowTheLifecycle()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.State.Should().Be(ProducerBatch.BatchState.Open);

        batch.TryAppend(1, null, "value"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.Close();
        batch.State.Should().Be(ProducerBatch.BatchState.Closed);

        batch.MarkCompressed();
        batch.State.Should().Be(ProducerBatch.BatchState.Compressed);
        batch.MarkFinalized();
        batch.State.Should().Be(ProducerBatch.BatchState.Finalized);
        batch.MarkSent();
        batch.State.Should().Be(ProducerBatch.BatchState.Sent);

        batch.Complete(10, 1);
        batch.State.Should().Be(ProducerBatch.BatchState.Completed);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_MustRejectWritesAndRepeatedTransitionsAfterClose()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.Close();

        batch.TryAppend(1, null, "value"u8.ToArray(), Headers.Empty, out var task).Should().BeFalse();
        (task is null).Should().BeTrue();
        var close = () => batch.Close();
        close.Should().Throw<InvalidOperationException>();

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_PrepareForRetry_ReturnsSentBatchToClosedState()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.TryAppend(1, null, "value"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.Close();
        batch.MarkFinalized();
        batch.MarkSent();

        batch.PrepareForRetry(60_000).Should().BeTrue();
        batch.State.Should().Be(ProducerBatch.BatchState.Closed);
        batch.IsReady.Should().BeTrue();
        batch.PrepareForRetry(60_000).Should().BeTrue();

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_PrepareForRetry_AllowsRetryBeforeSending()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.TryAppend(1, null, "value"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.Close();

        batch.PrepareForRetry(60_000).Should().BeTrue();
        batch.State.Should().Be(ProducerBatch.BatchState.Closed);
        batch.IsReady.Should().BeTrue();

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_PrepareForRetry_DoesNotRetryFinalizedBatch()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.TryAppend(1, null, "value"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.Close();
        batch.MarkFinalized();

        batch.PrepareForRetry(60_000).Should().BeFalse();

        ArrayBufferPool.Return(buffer);
    }

    [Theory]
    [InlineData(ErrorCodes.LeaderNotAvailable, true)]
    [InlineData(ErrorCodes.NotLeaderOrFollower, true)]
    [InlineData(ErrorCodes.RequestTimedOut, true)]
    [InlineData(ErrorCodes.BrokerNotAvailable, true)]
    [InlineData(ErrorCodes.ReplicaNotAvailable, true)]
    [InlineData(ErrorCodes.NotEnoughReplicas, true)]
    [InlineData(ErrorCodes.NotEnoughReplicasAfterAppend, true)]
    [InlineData(ErrorCodes.UnknownTopicOrPartition, true)]
    [InlineData(ErrorCodes.FencedLeaderEpoch, true)]
    [InlineData(ErrorCodes.UnknownLeaderEpoch, true)]
    [InlineData(ErrorCodes.PreferredLeaderNotAvailable, true)]
    [InlineData(ErrorCodes.NetworkException, true)]
    [InlineData(ErrorCodes.MessageTooLarge, false)]
    [InlineData(ErrorCodes.TopicAuthorizationFailed, false)]
    public void MessagesSender_ClassifiesProduceErrors(ErrorCodes errorCode, bool expectedRetriable)
    {
        MessagesSender.IsRetriableProduceError(errorCode).Should().Be(expectedRetriable);
    }

    [Fact]
    public async Task ProducerBatch_FailForClosing_ReportsNotPersistedBeforeSend()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.TryAppend(1, null, "value"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.FailForClosing();

        var exception = await Assert.ThrowsAsync<ProducerClosingException>(() => batch.CompletionTask);
        exception.Status.Should().Be(PersistenceStatus.NotPersisted);
        batch.State.Should().Be(ProducerBatch.BatchState.Completed);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public async Task ProducerBatch_CompleteWithoutAcknowledgement_ReturnsUnknownOffsets()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.TryAppend(1, null, "value"u8.ToArray(), Headers.Empty, out var sendTask).Should().BeTrue();
        batch.CompleteWithoutAcknowledgement();

        (await sendTask!.Task).Offset.Should().Be(Offset.Unset);
        batch.State.Should().Be(ProducerBatch.BatchState.Completed);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void RecordBatch_EstimateMustIncludeBatchHeader()
    {
        var value = "value"u8.ToArray();

        RecordBatch.EstimateSizeInBytesUpperBound(value, value, Headers.Empty)
            .Should()
            .Be(RecordBatch.RECORD_BATCH_OVERHEAD + RecordExtensions.EstimateSizeInBytesUpperBound(value, value, Headers.Empty));
    }

    [Fact]
    public void ProducerBatch_CloseMustWriteReadableRecordBatch()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);
        batch.TryAppend(1678512922757, null, "test"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();

        batch.Close();

        var bytes = buffer.DangerousGetFirstBuffer().AsSpan(0, batch.Size).ToArray();
        var reader = new BufferReader(bytes);
        var recordBatch = new RecordBatch(ref reader);

        recordBatch.CountRecords.Should().Be(1);
        recordBatch.Records.Single().Value.Should().BeEquivalentTo("test"u8.ToArray());
        recordBatch.BaseTimestamp.Should().Be(1678512922757);
        recordBatch.LastOffsetDelta.Should().Be(0);
        recordBatch.Crc.Should().Be(global::NKafka.Crc.Crc.Calculate(bytes[21..]));

        var recordsReader = new BufferReader(bytes);
        var records = recordsReader.ReadRecords(bytes.Length);
        records.Should().NotBeNull();
        records!.SizeInBytes.Should().Be(bytes.Length);
        records.Batches.Should().ContainSingle();

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_MustWriteRecordDeltasAndMaximumTimestamp()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.TryAppend(1000, "key-1"u8.ToArray(), "value-1"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.TryAppend(1010, "key-2"u8.ToArray(), "value-2"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.Close();

        var bytes = buffer.DangerousGetFirstBuffer().AsSpan(0, batch.Size).ToArray();
        var reader = new BufferReader(bytes);
        var recordBatch = new RecordBatch(ref reader);

        recordBatch.CountRecords.Should().Be(2);
        recordBatch.BaseTimestamp.Should().Be(1000);
        recordBatch.MaxTimestamp.Should().Be(1010);
        recordBatch.LastOffsetDelta.Should().Be(1);
        recordBatch.Records.Select(record => record.TimestampDelta).Should().Equal(0, 10);
        recordBatch.Records.Select(record => record.OffsetDelta).Should().Equal(0, 1);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_MustWriteHeadersAndNullValues()
    {
        var headers = new Headers(
        [
            new Header("trace-id", "abc"u8.ToArray()),
            new Header("empty", null)
        ]);
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.TryAppend(1000, null, null, headers, out _).Should().BeTrue();
        batch.Close();

        var bytes = buffer.DangerousGetFirstBuffer().AsSpan(0, batch.Size).ToArray();
        var reader = new BufferReader(bytes);
        var record = new RecordBatch(ref reader).Records.Single();

        record.Key.Should().BeNull();
        record.Value.Should().BeNull();
        record.Headers.Count.Should().Be(2);
        record.Headers[0].Key.Should().Be("trace-id");
        record.Headers[0].Value.Should().BeEquivalentTo("abc"u8.ToArray());
        record.Headers[1].Key.Should().Be("empty");
        record.Headers[1].Value.Should().BeNull();

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_CrcMustChangeWhenRecordDataChanges()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);
        batch.TryAppend(1000, null, "value"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.Close();

        var bytes = buffer.DangerousGetFirstBuffer().AsSpan(0, batch.Size).ToArray();
        var originalCrc = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(17, sizeof(uint)));
        bytes[^1] ^= 0x01;

        global::NKafka.Crc.Crc.Calculate(bytes[21..]).Should().NotBe(originalCrc);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_EmptyBatchMustHaveAValidHeader()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.Close();

        batch.Size.Should().Be(RecordBatch.RECORD_BATCH_OVERHEAD);
        var reader = new BufferReader(buffer.DangerousGetFirstBuffer().AsSpan(0, batch.Size));
        var recordBatch = new RecordBatch(ref reader);

        recordBatch.CountRecords.Should().Be(0);
        recordBatch.Length.Should().Be(RecordBatch.RECORD_BATCH_OVERHEAD - 12);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_MustPreserveUnicodeAndEmptyArrays()
    {
        var headers = new Headers([new Header("ключ", [])]);
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.TryAppend(1000, [], "Привет, Kafka"u8.ToArray(), headers, out _).Should().BeTrue();
        batch.Close();

        var reader = new BufferReader(buffer.DangerousGetFirstBuffer().AsSpan(0, batch.Size));
        var record = new RecordBatch(ref reader).Records.Single();

        record.Key.Should().BeEmpty();
        record.Value.Should().BeEquivalentTo("Привет, Kafka"u8.ToArray());
        record.Headers[0].Key.Should().Be("ключ");
        record.Headers[0].Value.Should().BeEmpty();

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_MustWriteProtocolHeaderFields()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);
        batch.TryAppend(1000, null, "value"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.Close();

        var reader = new BufferReader(buffer.DangerousGetFirstBuffer().AsSpan(0, batch.Size));
        var recordBatch = new RecordBatch(ref reader);

        recordBatch.Magic.Should().Be(2);
        recordBatch.PartitionLeaderEpoch.Should().Be(-1);
        recordBatch.ProducerId.Should().Be(-1);
        recordBatch.ProducerEpoch.Should().Be(-1);
        recordBatch.BaseSequence.Should().Be(-1);
        recordBatch.Length.Should().Be(batch.Size - 12);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_BytesMustRemainStableAfterClose()
    {
        var buffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);
        batch.TryAppend(1000, null, "value"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.Close();

        var first = buffer.DangerousGetFirstBuffer().AsSpan(0, batch.Size).ToArray();
        _ = batch.GetAsRecords();
        var second = buffer.DangerousGetFirstBuffer().AsSpan(0, batch.Size).ToArray();

        second.Should().Equal(first);

        ArrayBufferPool.Return(buffer);
    }

    [Fact]
    public void ProducerBatch_RecordsViewMustKeepSizeAndSupportRepeatedSerialization()
    {
        var sourceBuffer = ArrayBufferPool.Rent(1024);
        var batch = new ProducerBatch(new TopicPartition("test", 0), sourceBuffer, NullLoggerFactory.Instance);
        batch.TryAppend(1000, null, "value"u8.ToArray(), Headers.Empty, out _).Should().BeTrue();
        batch.Close();

        var records = batch.GetAsRecords();
        records.SizeInBytes.Should().Be(batch.Size);

        var firstRequestBuffer = ArrayBufferPool.Rent(batch.Size);
        var firstWriter = new BufferWriter(ref firstRequestBuffer);
        firstWriter.WriteRecords(records);
        firstRequestBuffer.TotalWritten.Should().Be(batch.Size);

        var secondRequestBuffer = ArrayBufferPool.Rent(batch.Size);
        var secondWriter = new BufferWriter(ref secondRequestBuffer);
        secondWriter.WriteRecords(records);
        secondRequestBuffer.TotalWritten.Should().Be(batch.Size);

        sourceBuffer.TotalWritten.Should().Be(batch.Size);

        ArrayBufferPool.Return(firstRequestBuffer);
        ArrayBufferPool.Return(secondRequestBuffer);
        ArrayBufferPool.Return(sourceBuffer);
    }

    [Fact]
    public void ProducerBatch_CompressesOnlyRecordsAndRecalculatesHeader()
    {
        var buffer = ArrayBufferPool.Rent(4096);
        var batch = new ProducerBatch(
            new TopicPartition("test", 0),
            buffer,
            NullLoggerFactory.Instance,
            new ZStdCompression(3),
            CompressionType.ZStd);

        batch.TryAppend(1000, null, Encoding.UTF8.GetBytes(new string('x', 512)), Headers.Empty, out _).Should().BeTrue();
        batch.Close();
        batch.Compress();

        batch.State.Should().Be(ProducerBatch.BatchState.Compressed);
        var serialized = batch.GetAsRecords().Buffer;
        var reader = new BufferReader(serialized.DangerousGetFirstBuffer().AsSpan(0, batch.Size));
        var recordBatch = new RecordBatch(ref reader);

        recordBatch.Attributes.Should().Be((short)CompressionType.ZStd);
        recordBatch.Length.Should().Be(batch.Size - 12);
        recordBatch.Crc.Should().Be(global::NKafka.Crc.Crc.Calculate(serialized.WrittenFirstSpan[21..batch.Size]));

        ArrayBufferPool.Return(serialized);
    }

    [Fact]
    public void ProducerBatch_MustRejectRecordWhenBufferCannotFitHeaderAndRecord()
    {
        var buffer = new ArrayBuffer(true, false, RecordBatch.RECORD_BATCH_OVERHEAD);
        var batch = new ProducerBatch(new TopicPartition("test", 0), buffer, NullLoggerFactory.Instance);

        batch.TryAppend(1000, null, "value"u8.ToArray(), Headers.Empty, out var sendResult).Should().BeFalse();
        (sendResult is null).Should().BeTrue();
    }

}
