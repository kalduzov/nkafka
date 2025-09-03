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

using NKafka.Exceptions;
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
internal class ProducerBatch(TopicPartition topicPartition, ArrayBuffer buffer, ILoggerFactory loggerFactory)
{
    /// <summary>
    /// This default bath 
    /// </summary>
    public static readonly ProducerBatch Null = new(TopicPartition.Null,
        ArrayBuffer.Null,
        NullLoggerFactory.Instance);

    internal const int RECORD_BATCH_OVERHEAD = 61;

    private ArrayBuffer _buffer = buffer;

    /// <summary>
    /// Batch header length
    /// </summary>
    internal const int BATCH_HEADER_LEN = 54;

    private const int _BATCH_OVERHEAD_WITHOUT_RECORDS_OFFSET = RECORD_BATCH_OVERHEAD - 4;

    private const int _ATTRIBUTES_OFFSET = 17;

    private readonly TaskCompletionSource _produceRequestResult = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _maxRecordSize;
    private int _recordsCount;
    private readonly List<SendResultTask> _recordTasks = [];
    private int _lastOffset = -1;
    private readonly List<Record> _records = new(16);
    private readonly ILogger<ProducerBatch> _logger = loggerFactory.CreateLogger<ProducerBatch>();

    /// <summary>
    /// How many bytes are left to add so that the batch is complete?
    /// </summary>
    public int EstimatedSizeInBytes { get; set; }

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

    internal ProducerBatch(TopicPartition topicPartition, ArrayBuffer buffer, ILoggerFactory loggerFactory, long timestamp)
        : this(topicPartition, buffer, loggerFactory)
    {
        BaseTimestamp = timestamp;
        MaxTimestamp = timestamp;
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
        var estimateSizeInBytesUpperBound = RecordExtensions.EstimateSizeInBytesUpperBound(key, value, headers);

        if (_buffer.Remaining - estimateSizeInBytesUpperBound < 0)
        {
            sendResultTask = null;

            return false;
        }

        var offset = Interlocked.Increment(ref _lastOffset);

        var record = new Record(headers, key, value, timestamp, offset);
        //_baseTimestamp = timestamp;
        // _maxTimestamp = Math.Max(_baseTimestamp, timestamp);

        _records.Add(record);

        _maxRecordSize = Math.Max(_maxRecordSize, estimateSizeInBytesUpperBound);
        sendResultTask = new SendResultTask(_produceRequestResult, _recordsCount, timestamp, key?.Length ?? -1, value?.Length ?? -1);
        _recordTasks.Add(sendResultTask);
        _recordsCount++;
        EstimatedSizeInBytes += estimateSizeInBytesUpperBound;

        return true;
    }

    /// <summary>
    /// Closes the current batch by writing any pending records and the batch header.
    /// Sets the IsFull flag to true indicating that the batch is no longer open for writing.
    /// </summary>
    public void Close()
    {
        var bufferWriter = new BufferWriter(ref _buffer);
        WriteRecords(ref bufferWriter);
        WriteHeader(ref bufferWriter);
        IsFull = true;
    }

    private void WriteRecords(ref BufferWriter bufferWriter)
    {
        // bufferWriter.Position = _BATCH_OVERHEAD_WITHOUT_RECORDS_OFFSET;
        //
        // bufferWriter.WriteInt(_records.Count);
        //
        // var size = 0;
        //
        // foreach (var record in _records)
        // {
        //     size += record.WriteTo(ref bufferWriter);
        // }
        // //Length += size;
        // _buffer.Position = 0;
    }

    private void WriteHeader(ref BufferWriter bufferWriter)
    {
        // bufferWriter.Position = 0;
        // // https://kafka.apache.org/documentation/#recordbatch
        // bufferWriter.WriteLong(BaseOffset);
        // bufferWriter.WriteInt(Length - 12);
        // bufferWriter.WriteInt(PartitionLeaderEpoch);
        // bufferWriter.WriteByte(Magic);
        // bufferWriter.WriteUInt(Crc); //reserve
        // bufferWriter.WriteShort(Attributes);
        // bufferWriter.WriteInt(_lastOffset);
        // bufferWriter.WriteLong(BaseTimestamp);
        // bufferWriter.WriteLong(MaxTimestamp);
        // bufferWriter.WriteLong(ProducerId);
        // bufferWriter.WriteShort(ProducerEpoch);
        // bufferWriter.WriteInt(BaseSequence);
        // Crc = CrcUtils.Calculate(bufferWriter.AsSpan(_ATTRIBUTES_OFFSET + 4, Length));
        // bufferWriter.PutUInt(_ATTRIBUTES_OFFSET, Crc); //
        // _bufferWriter.Position = 0;
    }

    /// <summary>
    /// Retrieves the data as a Records object.
    /// </summary>
    /// <returns>A new Records object containing the data.</returns>
    public Records GetAsRecords()
    {
        var list = new[]
        {
            this
        };

        return new Records(_buffer);
        //return new Records(Length, list);
    }

    /// <summary>
    /// Successfully completes batch processing
    /// </summary>
    /// <param name="baseOffset">The base offset to be incremented for each record</param>
    /// <param name="appendTime">The appended time of the batch</param>
    public void Complete(long baseOffset, long appendTime)
    {
        foreach (var recordTask in _recordTasks)
        {
            recordTask.SetResult(new RecordMetadata
            {
                TopicPartition = TopicPartition,
                Offset = baseOffset++
            });
        }
        _produceRequestResult.SetResult();
    }

    /// <summary>
    /// Method to handle failure by setting exception for all record tasks and produce request result.
    /// </summary>
    /// <param name="errorCode">The error code for the failure.</param>
    public void Fail(ErrorCodes errorCode)
    {
        var exception = new ProtocolKafkaException(errorCode);

        foreach (var recordTask in _recordTasks)
        {
            recordTask.SetException(exception);
        }
        _produceRequestResult.SetException(exception);
    }

    public void SetReady()
    {
        IsReady = true;
    }
}