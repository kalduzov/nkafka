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

using System.Buffers;
using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;
using Microsoft.IO;

using NKafka.Collections;
using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Metrics;
using NKafka.Protocol;
using NKafka.Protocol.Records;
using NKafka.Resources;

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Manages the distribution of messages by batches
/// </summary>
internal sealed class RecordAccumulator: IRecordAccumulator
{
    private class TopicBatches
    {
        public ConcurrentDictionary<Partition, Deque<ProducerBatch>> Batches { get; }

        public TopicBatches()
        {
            Batches = new ConcurrentDictionary<Partition, Deque<ProducerBatch>>();
        }
    }

    private readonly ConcurrentDictionary<string, TopicBatches> _topicBatchesMap;
    private readonly int _batchSize;
    private readonly ArrayPool<byte> _bufferPool;
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    private readonly bool _closed;
    private readonly CompressionType _compressionType;
    private readonly int _deliveryTimeoutMs;
    private readonly double _lingerMs;
    private readonly ILogger _logger;
    private readonly long _retryBackoffMs;
    private readonly ITransactionManager _transactionManager;
    private volatile int _appendsInProgress;
    private volatile int _flushesInProgress = 0;
    private readonly IProducerMetrics _metrics;

    public RecordAccumulator(
        ProducerConfig config,
        ITransactionManager transactionManager,
        int deliveryTimeoutMs,
        ILoggerFactory loggerFactory)
    {
        _topicBatchesMap = new();
        _metrics = config.Metrics;
        _transactionManager = transactionManager;
        _deliveryTimeoutMs = deliveryTimeoutMs;
        _logger = loggerFactory.CreateLogger<RecordAccumulator>();
        _closed = false;
        _batchSize = Math.Max(1, config.BatchSize);
        _compressionType = config.CompressionType;
        _retryBackoffMs = config.RetryBackoffMs;
        _lingerMs = config.LingerMs;
        _bufferPool = ArrayPool<byte>.Create(config.BufferMemory, _batchSize);
        _memoryStreamManager = new RecyclableMemoryStreamManager(config.BufferMemory, _batchSize);

    }

    /// <summary>
    /// Append new message to accumulator
    /// </summary>
    /// <param name="topicPartition">Partition to which the message should be sent</param>
    /// <param name="timestamp">The timestamp when this message was created</param>
    /// <param name="key">Message key</param>
    /// <param name="value">Message value</param>
    /// <param name="headers">Message headers</param>
    /// <returns>Returns the result of adding the message to the accumulator</returns>
    public RecordAppendResult Append(
        TopicPartition topicPartition,
        long timestamp,
        byte[] key,
        byte[] value,
        Headers headers)
    {
        Interlocked.Increment(ref _appendsInProgress);

        // list of batches for a specific topic divided by partitions
        var topicBatches = _topicBatchesMap.GetOrAdd(topicPartition.Topic, new TopicBatches());

        var buffer = Stream.Null;

        try
        {
            while (true)
            {
                var effectivePartition = topicPartition.Partition.Value;

                // get a queue containing batches for adding records
                var deque = topicBatches.Batches.GetOrAdd(effectivePartition, _ => new Deque<ProducerBatch>());

                lock (deque) //only one thread can add data to the queue
                {
                    if (TryAppend(timestamp, key, value, headers, deque, out var appendResult))
                    {
                        //the data could be added because a suitable batch already existed
                        return appendResult!;
                    }
                }

                //Не было батча для добавления записи, так что продолжаем работу
                //подготавливаем буфер, в который будут писаться данные в батче
                if (buffer == Stream.Null)
                {
                    var size = Math.Max(_batchSize, Records.EstimateSizeInBytesUpperBound(key, value, headers));
                    buffer = _memoryStreamManager.GetStream();
                    buffer.SetLength(size);
                }

                lock (deque)
                {
                    var bufferWriter = new BufferWriter(buffer, false);
                    var appendResult = AppendNewBatch(topicPartition.Topic, effectivePartition, deque, timestamp, key, value, headers, bufferWriter);

                    if (appendResult.NewBatchCreated)
                    {
                        buffer = null; //не возвращаем буфер в пул
                    }

                    return appendResult;
                }
            }
        }
        finally
        {
            buffer?.Dispose();

            Interlocked.Decrement(ref _appendsInProgress);
        }
    }

    private RecordAppendResult AppendNewBatch(
        string topic,
        int partition,
        Deque<ProducerBatch> deque,
        long timestamp,
        byte[] key,
        byte[] value,
        Headers headers,
        BufferWriter buffer)
    {
        //Пытаемся добавить, вдруг пока мы готовились к добавлению, уже кто-то добавил новый батч
        if (TryAppend(timestamp, key, value, headers, deque, out var appendResult))
        {
            return appendResult!;
        }

        var recordsBuilder = RecordBuilder(buffer);
        var batch = new ProducerBatch(new TopicPartition(topic, partition), recordsBuilder);

        if (!batch.TryAppend(timestamp, key, value, headers, out var recordMetadataTask))
        {
            throw new ArgumentNullException(nameof(recordMetadataTask));
        }

        deque.PushBack(batch);

        var batchIsFull = deque.Count > 1 || batch.IsFull;

        return new RecordAppendResult(recordMetadataTask, batchIsFull, true, batch.EstimatedSizeInBytes);
    }

    private RecordsBuilder RecordBuilder(BufferWriter buffer)
    {
        return Records.Builder(buffer, _compressionType, TimestampType.CreateTime, 0L);
    }

    /// <summary>
    /// Attempt to add data to any batch for a partiotion
    /// </summary>
    /// <exception cref="KafkaException"></exception>
    private bool TryAppend(
        long timestamp,
        byte[] key,
        byte[] value,
        Headers headers,
        Deque<ProducerBatch> deque,
        out RecordAppendResult recordAppendResult)
    {
        recordAppendResult = new NoneAppendResult();

        if (_closed) //The producer was closed, which means that all operations in it no longer went through
        {
            throw new ProduceException(ExceptionMessages.Producer_WasClosed);
        }

        var last = deque.PeekFront(); //get the last batch from the queue

        if (last is null)
        {
            return false;
        }

        var initialBytes = last.EstimatedSizeInBytes;

        if (!last.TryAppend(timestamp, key, value, headers, out var sendResultTask))
        {
            last.CloseForRecordAppends();

            return false;
        }
        var appendedBytes = last.EstimatedSizeInBytes - initialBytes;

        recordAppendResult = new RecordAppendResult(sendResultTask, deque.Count > 1 || last.IsFull, false, appendedBytes);

        return true;
    }

    /// <summary>
    /// Отправляет все скопившиеся батчи 
    /// </summary>
    /// <param name="timeSpan"></param>
    public void FlushAll(TimeSpan timeSpan)
    {
    }

    public Task FlushAllAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///  Get the list of partitions with data ready to send
    /// </summary>
    public ReadyCheckResult GetReadyBatches(IKafkaCluster kafkaCluster)
    {
        var readyNodes = new HashSet<Node>();
        var unknownLeaderTopics = new HashSet<string>();

        foreach (var topicInfo in _topicBatchesMap)
        {
            var topic = topicInfo.Key;
            PartitionReady(kafkaCluster, topic, topicInfo.Value, readyNodes, unknownLeaderTopics);
        }

        return new ReadyCheckResult(readyNodes, unknownLeaderTopics);
    }

    private void PartitionReady(IKafkaCluster cluster,
        string topic,
        TopicBatches topicBatches,
        HashSet<Node> readyNodes,
        HashSet<string> unknownLeaderTopics)
    {
        var batches = topicBatches.Batches;

        int[]? queueSizes = null;
        int[]? partitionIds = null;

        var queueSizesIndex = -1;

        foreach (var entry in batches)
        {
            var part = new TopicPartition(topic, entry.Key);
            var leader = cluster.LeaderFor(part);

            if (leader != Node.NoNode && queueSizes is not null)
            {
                ++queueSizesIndex;
                partitionIds[queueSizesIndex] = part.Partition;
            }
            var deque = entry.Value;

            long waitedTimeMs;
            bool backingOff;
            int dequeSize;
            bool full;

            lock (deque)
            {
                var batch = deque.PeekFront();

                if (batch is null)
                {
                    continue;
                }

                dequeSize = deque.Count;
                full = dequeSize > 1 || batch.IsFull;
            }

            if (leader is null)
            {
                unknownLeaderTopics.Add(part.Topic);
            }
            else
            {
                if (queueSizes is not null)
                {
                    queueSizes[queueSizesIndex] = dequeSize;
                }

                // if (_partitionAvailabilityTimeoutMs > 0)
                // {
                //     
                // }

                BatchReady(part, leader, full, readyNodes);
            }
        }
    }

    private void BatchReady(TopicPartition part, Node leader, bool full, HashSet<Node> readyNodes)
    {
        //if (!readyNodes.Contains(leader) && )
    }
}