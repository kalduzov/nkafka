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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Records;

using CrcUtils = NKafka.Crc.Crc;

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Contains batch data and metadata
/// </summary>
internal class ProducerBatch
{
    internal const int RECORD_BATCH_OVERHEAD = 61;
    private readonly BufferWriter _bufferWriter;

    /// <summary>
    /// Batch header length
    /// </summary>
    internal const int BATCH_HEADER_LEN = 54;

    private const int _BATCH_OVERHEAD_WITHOUT_RECORDS_OFFSET = RECORD_BATCH_OVERHEAD - 4;

    private const int _ATTRIBUTES_OFFSET = 17;

    private readonly TaskCompletionSource _produceRequestResult;
    private int _maxRecordSize;
    private int _recordsCount;
    private readonly List<SendResultTask> _recordTasks = [];
    private int _lastOffset;
    private readonly List<Record> _records = new(16);
    private readonly ILogger<ProducerBatch> _logger;

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
    public TopicPartition TopicPartition { get; }

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
    public long CreateTimestamp { get; private set; }

    public long BaseTimestamp { get; set; }

    public long MaxTimestamp { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProducerBatch"/> class with the specified <see cref="TopicPartition"/> and <see cref="BufferWriter"/>.
    /// </summary>
    /// <param name="topicPartition">The <see cref="TopicPartition"/> associated with the batch.</param>
    /// <param name="bufferWriter">The <see cref="BufferWriter"/> used for writing the batch data.</param>
    /// <param name="loggerFactory"></param>
    public ProducerBatch(TopicPartition topicPartition, BufferWriter bufferWriter, ILoggerFactory loggerFactory)
    {
        _bufferWriter = bufferWriter;
        _logger = loggerFactory.CreateLogger<ProducerBatch>();
        _lastOffset = -1;
        TopicPartition = topicPartition;
        _produceRequestResult = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        BaseTimestamp = Timestamp.DateTimeToUnixTimestampMs(Timestamp.UnixTimeEpoch);
        CreateTimestamp = Timestamp.DateTimeToUnixTimestampMs(DateTime.UtcNow);
    }

    internal ProducerBatch(TopicPartition topicPartition, BufferWriter bufferWriter, ILoggerFactory loggerFactory, long timestamp)
        : this(topicPartition, bufferWriter, loggerFactory)
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

        if (_bufferWriter.Remaining - estimateSizeInBytesUpperBound < 0)
        {
            sendResultTask = null;

            return false;
        }

        var offset = Interlocked.Increment(ref _lastOffset);

        var record = new Record
        {
            // Headers = headers,
            // Value = value,
            // Key = key,
            // OffsetDelta = offset,
            // TimestampDelta = timestamp
        };
        // _baseTimestamp = timestamp;
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
        WriteRecords();
        WriteHeader();
        IsFull = true;
    }

    private void WriteRecords()
    {
        _bufferWriter.Position = _BATCH_OVERHEAD_WITHOUT_RECORDS_OFFSET;

        _bufferWriter.WriteInt(_records.Count);

        var size = 0;

        foreach (var record in _records)
        {
            size += record.WriteTo(_bufferWriter);
        }
        //Length += size;
        _bufferWriter.Position = 0;
    }

    private void WriteHeader()
    {
        // _bufferWriter.Position = 0;
        // // https://kafka.apache.org/documentation/#recordbatch
        // _bufferWriter.WriteLong(BaseOffset);
        // _bufferWriter.WriteInt(Length - 12);
        // _bufferWriter.WriteInt(PartitionLeaderEpoch);
        // _bufferWriter.WriteByte(Magic);
        // _bufferWriter.WriteUInt(Crc); //reserve
        // _bufferWriter.WriteShort(Attributes);
        // _bufferWriter.WriteInt(_lastOffset);
        // _bufferWriter.WriteLong(BaseTimestamp);
        // _bufferWriter.WriteLong(MaxTimestamp);
        // _bufferWriter.WriteLong(ProducerId);
        // _bufferWriter.WriteShort(ProducerEpoch);
        // _bufferWriter.WriteInt(BaseSequence);
        // Crc = CrcUtils.Calculate(_bufferWriter.AsSpan(_ATTRIBUTES_OFFSET + 4, Length));
        // _bufferWriter.PutUInt(_ATTRIBUTES_OFFSET, Crc); //
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

        return new Records(0, new List<IRecordsBatch>());
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