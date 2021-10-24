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
/// 
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
    private List<RecordMetadataTask> _recordTasks = new();

    public int EstimatedSizeInBytes { get; set; }

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

    public bool TryAppend(
        long timestamp,
        byte[]? key,
        byte[]? value,
        Headers headers,
        out RecordMetadataTask? recordMetadataTask)
    {
        recordMetadataTask = null;

        if (!_recordsBuilder.HasRoomFor(timestamp, key, value, headers))
        {
            return false;
        }

        _recordsBuilder.Append(timestamp, key, value, headers);
        var estimateSizeInBytesUpperBound = Records.EstimateSizeInBytesUpperBound(key, value, headers);
        _maxRecordSize = Math.Max(_maxRecordSize, estimateSizeInBytesUpperBound);
        recordMetadataTask = new RecordMetadataTask(_produceRequestResult, _recordsCount, timestamp, key?.Length ?? -1, value?.Length ?? -1);
        _recordTasks.Add(recordMetadataTask);
        _recordsCount++;

        return true;
    }

    public void CloseForRecordAppends()
    {
        _recordsBuilder.CloseForRecordAppends();
    }

    /// <summary>
    /// Успешно завершает 
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