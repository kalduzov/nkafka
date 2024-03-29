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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Records;

using CrcUtils = NKafka.Crc.Crc;

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Contains batch data and metadata
/// </summary>
internal class ProducerBatch: RecordsBatch
{
    /// <summary>
    /// Batch header length
    /// </summary>
    internal const int BATCH_HEADER_LEN = 54;

    internal const int BATCH_OVERHEAD_WITHOUT_RECORDS_OFFSET = RECORD_BATCH_OVERHEAD - 4;

    internal const int ATTRIBUTES_OFFSET = 17;

    private readonly TaskCompletionSource _produceRequestResult;
    private readonly BufferWriter _bufferWriter;
    private int _maxRecordSize;
    private int _recordsCount;
    private List<SendResultTask> _recordTasks = new();
    private int _lastOffset;
    private List<IRecord> _records = new(16);

    /// <summary>
    /// How many bytes are left to add so that the batch is complete
    /// </summary>
    public int EstimatedSizeInBytes { get; set; }

    /// <summary>
    /// Indicates that no more data can be added to the batch
    /// </summary>
    public bool IsFull { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public TopicPartition TopicPartition { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsReady => true;

    public int Size { get; set; }

    public ProducerBatch(TopicPartition topicPartition, BufferWriter bufferWriter)
    {
        _lastOffset = -1;
        TopicPartition = topicPartition;
        _bufferWriter = bufferWriter;
        _produceRequestResult = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        BaseTimestamp = Timestamp.DateTimeToUnixTimestampMs(Timestamp.UnixTimeEpoch);
    }

    internal ProducerBatch(TopicPartition topicPartition, BufferWriter bufferWriter, long timestamp)
    {
        _lastOffset = -1;
        TopicPartition = topicPartition;
        _bufferWriter = bufferWriter;
        _produceRequestResult = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
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
        var estimateSizeInBytesUpperBound = EstimateSizeInBytesUpperBound(key, value, headers);

        if (_bufferWriter.Remaining - estimateSizeInBytesUpperBound < 0)
        {
            sendResultTask = null;

            return false;
        }

        var offset = Interlocked.Increment(ref _lastOffset);

        var record = new Record
        {
            Headers = headers,
            Value = value,
            Key = key,
            OffsetDelta = offset,
            TimestampDelta = timestamp
        };
        // _baseTimestamp = timestamp;
        // _maxTimestamp = Math.Max(_baseTimestamp, timestamp);

        _records.Add(record);

        _maxRecordSize = Math.Max(_maxRecordSize, estimateSizeInBytesUpperBound);
        sendResultTask = new SendResultTask(_produceRequestResult, _recordsCount, timestamp, key?.Length ?? -1, value?.Length ?? -1);
        _recordTasks.Add(sendResultTask);
        _recordsCount++;

        return true;
    }

    public void Close()
    {
        WriteRecords();
        WriteHeader();
        IsFull = true;
    }

    private void WriteRecords()
    {
        _bufferWriter.Position = BATCH_OVERHEAD_WITHOUT_RECORDS_OFFSET;

        _bufferWriter.WriteInt(_records.Count);

        var size = 0;

        foreach (var record in _records)
        {
            size += record.WriteTo(_bufferWriter);
        }
        Length += size;
        _bufferWriter.Position = 0;
    }

    private void WriteHeader()
    {
        _bufferWriter.Position = 0;
        // https://kafka.apache.org/documentation/#recordbatch
        _bufferWriter.WriteLong(BaseOffset);
        _bufferWriter.WriteInt(Length - 12);
        _bufferWriter.WriteInt(PartitionLeaderEpoch);
        _bufferWriter.WriteByte(Magic);
        _bufferWriter.WriteUInt(Crc); //reserve
        _bufferWriter.WriteShort(Attributes);
        _bufferWriter.WriteInt(LastOffsetDelta);
        _bufferWriter.WriteLong(BaseTimestamp);
        _bufferWriter.WriteLong(MaxTimestamp);
        _bufferWriter.WriteLong(ProducerId);
        _bufferWriter.WriteShort(ProducerEpoch);
        _bufferWriter.WriteInt(BaseSequence);
        Crc = CrcUtils.Calculate(_bufferWriter.AsSpan(ATTRIBUTES_OFFSET + 4, Length));
        _bufferWriter.PutUInt(ATTRIBUTES_OFFSET, Crc); //
        _bufferWriter.Position = 0;
    }

    public Records GetAsRecords()
    {
        return new Records(Length);
    }

    /// <summary>
    /// Successfully completes batch processing 
    /// </summary>
    /// <param name="baseOffset"></param>
    /// <param name="appendTime"></param>
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

    public void Fail(ErrorCodes errorCode)
    {
        var exception = new ProtocolKafkaException(errorCode);

        foreach (var recordTask in _recordTasks)
        {
            recordTask.SetException(exception);
        }
        _produceRequestResult.SetException(exception);
    }
}