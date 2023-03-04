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

using NKafka.Protocol.Records;

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Contains batch data and metadata
/// </summary>
internal class ProducerBatch
{
    private readonly bool _isSplitBatch;
    private readonly TaskCompletionSource _produceRequestResult;
    private readonly RecordsBuilder _recordsBuilder;
    private readonly TopicPartition _topicPartition;
    private int _maxRecordSize;
    private int _recordsCount;
    private bool _retry;
    private List<SendResultTask> _recordTasks = new();

    /// <summary>
    /// How many bytes are left to add so that the batch is complete
    /// </summary>
    public int EstimatedSizeInBytes { get; set; }

    /// <summary>
    /// Indicates that no more data can be added to the batch
    /// </summary>
    public bool IsFull { get; set; }

    public ProducerBatch(TopicPartition topicPartition, RecordsBuilder recordsBuilder)
        : this(topicPartition, recordsBuilder, false)
    {
    }

    public ProducerBatch(TopicPartition topicPartition, RecordsBuilder recordsBuilder, bool isSplitBatch)
    {
        _topicPartition = topicPartition;
        _recordsBuilder = recordsBuilder;
        _isSplitBatch = isSplitBatch;
        _produceRequestResult = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _retry = false;
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
        sendResultTask = null;

        if (!_recordsBuilder.HasRoomFor(timestamp, key, value, headers))
        {
            return false;
        }

        _recordsBuilder.Append(timestamp, key, value, headers);
        var estimateSizeInBytesUpperBound = Records.EstimateSizeInBytesUpperBound(key, value, headers);
        _maxRecordSize = Math.Max(_maxRecordSize, estimateSizeInBytesUpperBound);
        sendResultTask = new SendResultTask(_produceRequestResult, _recordsCount, timestamp, key?.Length ?? -1, value?.Length ?? -1);
        _recordTasks.Add(sendResultTask);
        _recordsCount++;

        return true;
    }

    public void CloseForRecordAppends()
    {
        _recordsBuilder.CloseForRecordAppends();
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
            recordTask.SetResult(new RecordMetadata());
        }
        _produceRequestResult.SetResult();
    }
}