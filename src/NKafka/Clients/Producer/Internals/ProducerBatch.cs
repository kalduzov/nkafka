// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 *
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System.Buffers.Binary;

using NKafka.Exceptions;
using NKafka.Compressions;
using NKafka.Config;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;
using NKafka.Protocol.Records;

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Contains batch data and metadata
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ProducerBatch"/> class with the specified <see cref="TopicPartition"/> and <see cref="BufferWriter"/>.
/// </remarks>
/// <param name="topicPartition">The <see cref="TopicPartition"/> associated with the batch.</param>
/// <param name="buffer">The <see cref="BufferWriter"/> used for writing the batch data.</param>
/// <param name="loggerFactory"></param>
/// <param name="compression">The compression implementation used after the batch is closed.</param>
/// <param name="compressionType">The compression type written to the batch attributes.</param>
internal class ProducerBatch(
    TopicPartition topicPartition,
    ArrayBuffer buffer,
    ILoggerFactory loggerFactory,
    ICompression? compression = null,
    CompressionType compressionType = CompressionType.None)
{
    internal enum BatchState
    {
        Open,
        Closed,
        Compressed,
        Finalized,
        Sent,
        Completed
    }

    /// <summary>
    /// This default bath 
    /// </summary>
    public static readonly ProducerBatch Null = new(TopicPartition.Null,
        ArrayBuffer.Null,
        NullLoggerFactory.Instance);

    private ArrayBuffer _buffer = buffer;

    // Batch header length
    internal const int BATCH_HEADER_LEN = RecordBatch.RECORD_BATCH_OVERHEAD;

    // RecordBatch.length follows the eight-byte baseOffset field.
    private const int _LENGTH_OFFSET = sizeof(long);

    // The CRC follows baseOffset, length, partitionLeaderEpoch, and magic.
    private const int _CRC_OFFSET = _LENGTH_OFFSET + sizeof(int) + sizeof(int) + sizeof(byte);

    // Kafka calculates the CRC over the bytes after the CRC field, starting with attributes.
    private const int _CRC_DATA_OFFSET = _CRC_OFFSET + sizeof(uint);

    // The attributes field starts after the CRC in the RecordBatch header.
    private const int _ATTRIBUTES_OFFSET = _CRC_OFFSET + sizeof(uint);

    // The compression type occupies the lowest three bits of attributes.
    private const short _COMPRESSION_MASK = 0b111;

    // The length field excludes baseOffset and the length field itself.
    private const int _RECORD_BATCH_PREFIX_LENGTH = _LENGTH_OFFSET + sizeof(int);

    private readonly TaskCompletionSource _produceRequestResult = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _maxRecordSize;
    private int _recordsCount;
    private readonly List<SendResultTask> _recordTasks = [];
    private int _lastOffset = -1;
    private readonly List<Record> _records = new(16);
    private readonly ILogger<ProducerBatch> _logger = loggerFactory.CreateLogger<ProducerBatch>();
    private readonly ICompression _compression = compression ?? new NoCompression();
    private readonly CompressionType _compressionType = compressionType;
    private int _memoryReservation;

    internal BatchState State { get; private set; } = BatchState.Open;

    internal int RecordsCount => _recordsCount;

    /// <summary>
    /// How many bytes are left to add so that the batch is complete?
    /// </summary>
    public int EstimatedSizeInBytes { get; private set; } = BATCH_HEADER_LEN;

    /// <summary>
    /// Indicates that no more data can be added to the batch
    /// </summary>
    public bool IsFull { get; set; }

    /// <summary>
    /// Represents a specific partition of a topic in a Kafka cluster.
    /// </summary>
    public TopicPartition TopicPartition { get; } = topicPartition;

    /// <summary>
    /// Gets a value indicating whether the property is ready.
    /// </summary>
    /// <value>
    /// <c>true</c> if the property is ready; otherwise, <c>false</c>.
    /// </value>
    public bool IsReady { get; private set; }

    /// <summary>
    /// Gets or sets the size of the object.
    /// </summary>
    /// <value>
    /// The size of the object.
    /// </value>
    public int Size { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Task CompletionTask => _produceRequestResult.Task;

    /// <summary>
    /// 
    /// </summary>
    public long CreateTimestamp { get; private set; } = Timestamp.DateTimeToUnixTimestampMs(DateTime.UtcNow);

    public long BaseTimestamp { get; set; } = Timestamp.DateTimeToUnixTimestampMs(Timestamp.UnixTimeEpoch);

    public long MaxTimestamp { get; set; }

    internal uint Crc { get; private set; }

    internal void SetMemoryReservation(int size)
    {
        if (size <= 0 || Interlocked.CompareExchange(ref _memoryReservation, size, 0) != 0)
        {
            throw new InvalidOperationException("The batch memory reservation is already set or invalid.");
        }
    }

    internal int ReleaseMemoryReservation()
        => Interlocked.Exchange(ref _memoryReservation, 0);

    internal ProducerBatch(TopicPartition topicPartition, ArrayBuffer buffer, ILoggerFactory loggerFactory, long timestampNow)
        : this(topicPartition, buffer, loggerFactory)
    {
        BaseTimestamp = timestampNow;
        MaxTimestamp = timestampNow;
    }

    /// <summary>
    /// Try to add new data to batch
    /// </summary>
    /// <returns>true if it was possible to add an entry to the batch, false otherwise</returns>
    public bool TryAppend(
        long timestamp,
        byte[]? key,
        byte[]? value,
        Headers headers,
        out SendResultTask? sendResultTask)
    {
        if (State != BatchState.Open)
        {
            sendResultTask = null;

            return false;
        }

        var estimateSizeInBytesUpperBound = RecordExtensions.EstimateSizeInBytesUpperBound(key, value, headers);

        var requiredSize = checked(estimateSizeInBytesUpperBound + (_recordsCount == 0 ? BATCH_HEADER_LEN : 0));

        if (_buffer.Remaining < requiredSize)
        {
            sendResultTask = null;

            return false;
        }

        var offset = Interlocked.Increment(ref _lastOffset);

        if (_recordsCount == 0)
        {
            BaseTimestamp = timestamp;
            MaxTimestamp = timestamp;
        }
        else
        {
            MaxTimestamp = Math.Max(MaxTimestamp, timestamp);
        }

        var record = new Record(headers, key, value, timestamp, offset);

        _records.Add(record);

        _maxRecordSize = Math.Max(_maxRecordSize, estimateSizeInBytesUpperBound);
        sendResultTask = new SendResultTask(_produceRequestResult, _recordsCount, timestamp, key?.Length ?? -1, value?.Length ?? -1);
        _recordTasks.Add(sendResultTask);
        _recordsCount++;
        EstimatedSizeInBytes = checked(EstimatedSizeInBytes + estimateSizeInBytesUpperBound);

        return true;
    }

    /// <summary>
    /// Closes the current batch by writing any pending records and the batch header.
    /// Sets the IsFull flag to true indicating that the batch is no longer open for writing.
    /// </summary>
    public void Close()
    {
        EnsureState(BatchState.Open, "close");

        _buffer.Reset();
        var bufferWriter = new BufferWriter(ref _buffer);
        WriteHeader(ref bufferWriter);

        for (var index = 0; index < _records.Count; index++)
        {
            var record = _records[index];
            record.WriteTo(ref bufferWriter, record.Timestamp - BaseTimestamp, index);
        }

        Size = _buffer.TotalWritten;
        Span<byte> integerBytes = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(integerBytes, Size - _RECORD_BATCH_PREFIX_LENGTH);
        _buffer.WriteAt(_LENGTH_OFFSET, integerBytes);

        Crc = global::NKafka.Crc.Crc.Calculate(_buffer.WrittenFirstSpan[_CRC_DATA_OFFSET..]);
        BinaryPrimitives.WriteUInt32BigEndian(integerBytes, Crc);
        _buffer.WriteAt(_CRC_OFFSET, integerBytes);

        IsFull = true;
        State = BatchState.Closed;
    }

    internal void MarkCompressed()
    {
        EnsureState(BatchState.Closed, "compress");
        State = BatchState.Compressed;
    }

    internal void Compress()
    {
        EnsureState(BatchState.Closed, "compress");

        if (_compressionType == CompressionType.None)
        {
            return;
        }

        var serializedBatch = new byte[Size];
        _buffer.CopyWrittenTo(serializedBatch);

        try
        {
            var records = serializedBatch.AsSpan(BATCH_HEADER_LEN).ToArray();
            var compressedRecords = CompressRecords(records);
            var compressedSize = checked(BATCH_HEADER_LEN + compressedRecords.Length);
            var compressedBuffer = ArrayBufferPool.Rent(compressedSize);
            var writer = new BufferWriter(ref compressedBuffer);
            writer.WriteBytes(serializedBatch.AsSpan(0, BATCH_HEADER_LEN));
            writer.WriteBytes(compressedRecords);

            Span<byte> integerBytes = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(integerBytes, compressedSize - _RECORD_BATCH_PREFIX_LENGTH);
            compressedBuffer.WriteAt(_LENGTH_OFFSET, integerBytes);

            Span<byte> shortBytes = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(shortBytes, (short)((short)_compressionType & _COMPRESSION_MASK));
            compressedBuffer.WriteAt(_ATTRIBUTES_OFFSET, shortBytes);

            var crc = global::NKafka.Crc.Crc.Calculate(compressedBuffer.WrittenFirstSpan[_CRC_DATA_OFFSET..]);
            BinaryPrimitives.WriteUInt32BigEndian(integerBytes, crc);
            compressedBuffer.WriteAt(_CRC_OFFSET, integerBytes);

            ArrayBufferPool.Return(_buffer);
            _buffer = compressedBuffer;
            Size = compressedSize;
            Crc = crc;
            State = BatchState.Compressed;
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            _logger.LogWarning(exception, "Не удалось сжать пакет {TopicPartition}; будет отправлена несжатая версия", TopicPartition);
        }
    }

    private byte[] CompressRecords(byte[] records)
    {
        using var input = new MemoryStream(records, writable: false);
        using var output = new MemoryStream(records.Length);
        using (var compressionStream = _compression.Encode(output))
        {
            input.CopyTo(compressionStream);
        }

        return output.ToArray();
    }

    internal void MarkFinalized()
    {
        if (State is not (BatchState.Closed or BatchState.Compressed))
        {
            throw new InvalidOperationException($"Cannot finalize a batch in state {State}.");
        }

        State = BatchState.Finalized;
    }

    internal void MarkSent()
    {
        if (State is not (BatchState.Closed or BatchState.Compressed or BatchState.Finalized))
        {
            throw new InvalidOperationException($"Cannot send a batch in state {State}.");
        }

        State = BatchState.Sent;
    }

    internal bool PrepareForRetry(int deliveryTimeoutMs)
    {
        // A batch can be retried before sending (for example, while its partition has no leader)
        // or after sending (for a retriable broker/network failure). A finalized batch has not
        // entered the send path and must not hide a local request-construction error.
        if (State is not (BatchState.Closed or BatchState.Compressed or BatchState.Sent) ||
            Timestamp.DateTimeToUnixTimestampMs(DateTime.UtcNow) - CreateTimestamp >= deliveryTimeoutMs)
        {
            return false;
        }

        State = _compressionType == CompressionType.None
            ? BatchState.Closed
            : BatchState.Compressed;
        IsReady = true;

        return true;
    }

    private void WriteHeader(ref BufferWriter bufferWriter)
    {
        bufferWriter.WriteLong(0);
        bufferWriter.WriteInt(0);
        bufferWriter.WriteInt(-1);
        bufferWriter.WriteByte(2);
        bufferWriter.WriteUInt(0);
        bufferWriter.WriteShort(0);
        bufferWriter.WriteInt(_lastOffset);
        bufferWriter.WriteLong(BaseTimestamp);
        bufferWriter.WriteLong(MaxTimestamp);
        bufferWriter.WriteLong(-1);
        bufferWriter.WriteShort(-1);
        bufferWriter.WriteInt(-1);
        bufferWriter.WriteInt(_recordsCount);
    }

    /// <summary>
    /// Retrieves the data as a Records object.
    /// </summary>
    /// <returns>A new Records object containing the data.</returns>
    public Records GetAsRecords()
    {
        return new Records(_buffer, Size);
    }

    /// <summary>
    /// Successfully completes batch processing
    /// </summary>
    /// <param name="baseOffset">The base offset to be incremented for each record</param>
    /// <param name="appendTime">The appended time of the batch</param>
    public void Complete(long baseOffset, long appendTime)
    {
        EnsureNotCompleted("complete");

        foreach (var recordTask in _recordTasks)
        {
            recordTask.SetResult(new RecordMetadata
            {
                TopicPartition = TopicPartition,
                Offset = baseOffset++
            });
        }
        _produceRequestResult.SetResult();
        State = BatchState.Completed;
    }

    internal void CompleteWithoutAcknowledgement()
    {
        EnsureNotCompleted("complete");

        foreach (var recordTask in _recordTasks)
        {
            recordTask.SetResult(new RecordMetadata
            {
                TopicPartition = TopicPartition,
                Offset = Offset.Unset
            });
        }

        _produceRequestResult.SetResult();
        State = BatchState.Completed;
    }

    /// <summary>
    /// Method to handle failure by setting exception for all record tasks and produce request result.
    /// </summary>
    /// <param name="errorCode">The error code for the failure.</param>
    public void Fail(ErrorCodes errorCode)
    {
        EnsureNotCompleted("fail");

        var exception = new ProtocolKafkaException(errorCode);

        foreach (var recordTask in _recordTasks)
        {
            recordTask.SetException(exception);
        }
        _produceRequestResult.SetException(exception);
        State = BatchState.Completed;
    }

    internal void Fail(ProducerError error)
    {
        EnsureNotCompleted("fail");

        var exception = new ProducerInitializationException(error);

        foreach (var recordTask in _recordTasks)
        {
            recordTask.SetException(exception);
        }

        _produceRequestResult.SetException(exception);
        State = BatchState.Completed;
    }

    internal void FailForClosing()
    {
        EnsureNotCompleted("fail");

        var status = State == BatchState.Sent
            ? PersistenceStatus.PossiblyPersisted
            : PersistenceStatus.NotPersisted;
        var exception = new ProducerClosingException(status);

        foreach (var recordTask in _recordTasks)
        {
            recordTask.SetException(exception);
        }

        _produceRequestResult.SetException(exception);
        State = BatchState.Completed;
    }

    internal void FailForTransport(PersistenceStatus status)
    {
        EnsureNotCompleted("fail");

        var exception = new ProducerTransportException(status);

        foreach (var recordTask in _recordTasks)
        {
            recordTask.SetException(exception);
        }

        _produceRequestResult.SetException(exception);
        State = BatchState.Completed;
    }

    public void SetReady()
    {
        IsReady = true;
    }

    private void EnsureState(BatchState expected, string operation)
    {
        if (State != expected)
        {
            throw new InvalidOperationException($"Cannot {operation} a batch in state {State}.");
        }
    }

    private void EnsureNotCompleted(string operation)
    {
        if (State == BatchState.Completed)
        {
            throw new InvalidOperationException($"Cannot {operation} a completed batch.");
        }
    }
}
