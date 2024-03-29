﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

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
/// <remarks>
/// The main implementation is borrowed from the Java client
/// </remarks>
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

    /// <inheritdoc/>
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

        var stream = Stream.Null;

        try
        {
            while (true)
            {
                var effectivePartition = topicPartition.Partition.Value;

                // get a queue containing batches for adding records
                var deque = topicBatches.Batches.GetOrAdd(effectivePartition, _ => new Deque<ProducerBatch>());

                lock (deque) // only one thread can add data to the queue
                {
                    if (TryAppend(timestamp, key, value, headers, deque, out var appendResult))
                    {
                        // the data could be added because a suitable batch already existed
                        return appendResult;
                    }
                }

                // There was no batch to add a record, so we continue to work,
                // prepare the buffer into which the data in the batch will be written
                if (stream == Stream.Null)
                {
                    // We calculate what buffer size we need and try to get it 
                    var size = Math.Max(_batchSize, RecordsBatch.EstimateSizeInBytesUpperBound(key, value, headers));
                    stream = _memoryStreamManager.GetStream();
                    stream.SetLength(size);
                }

                lock (deque)
                {
                    var bufferWriter = new BufferWriter(stream, ProducerBatch.BATCH_HEADER_LEN);
                    var recordAppendResult = AppendIntoNewBatch(topicPartition.Topic, effectivePartition, deque, timestamp, key, value, headers, bufferWriter);

                    // It is possible that the batch was already created in another thread while we were preparing the buffer
                    if (recordAppendResult.NewBatchCreated)
                    {
                        stream = null; // We do not return the buffer to the pool. This buffer will be used in BufferWriter
                    }

                    return recordAppendResult;
                }
            }
        }
        finally
        {
            stream?.Dispose(); // If the buffer has not been used, then return it to the pool

            Interlocked.Decrement(ref _appendsInProgress);
        }
    }

    private RecordAppendResult AppendIntoNewBatch(
        string topic,
        int partition,
        Deque<ProducerBatch> deque,
        long timestamp,
        byte[] key,
        byte[] value,
        Headers headers,
        BufferWriter buffer)
    {
        // We are trying to add, all of a sudden, while we were preparing to add, someone has already added a new batch
        if (TryAppend(timestamp, key, value, headers, deque, out var recordAppendResult))
        {
            return recordAppendResult;
        }

        var topicPartition = new TopicPartition(topic, partition);

        var batch = new ProducerBatch(topicPartition, buffer);

        _logger.AddNewBatchTrace(topicPartition);

        if (!batch.TryAppend(timestamp, key, value, headers, out var sendResultTask))
        {
            throw new ArgumentNullException(nameof(sendResultTask));
        }

        deque.PushBack(batch);

        var batchIsFull = deque.Count > 1 || batch.IsFull;

        return new RecordAppendResult(sendResultTask, batchIsFull, true, batch.EstimatedSizeInBytes);
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

        var lastProducerBatch = deque.PeekFront(); //get the last batch from the queue

        if (lastProducerBatch is null)
        {
            return false;
        }

        var initialBytes = lastProducerBatch.EstimatedSizeInBytes;

        if (!lastProducerBatch.TryAppend(timestamp, key, value, headers, out var sendResultTask))
        {
            lastProducerBatch.Close();

            return false;
        }
        var appendedBytes = lastProducerBatch.EstimatedSizeInBytes - initialBytes;

        recordAppendResult = new RecordAppendResult(sendResultTask, deque.Count > 1 || lastProducerBatch.IsFull, false, appendedBytes);

        return true;
    }

    /// <inheritdoc/>
    public void FlushAll(TimeSpan timeSpan)
    {
    }

    /// <inheritdoc/>
    public Task FlushAllAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IEnumerable<ProducerBatch> PullBathes(IKafkaCluster kafkaCluster, int maxRequestSize)
    {
        var size = 0;

        foreach (var topicBatches in _topicBatchesMap.Values)
        {
            foreach (var topicBatch in topicBatches.Batches.Values)
            {
                ProducerBatch? batch;

                lock (topicBatch)
                {
                    batch = topicBatch.PeekFront();

                    if (batch is null)
                    {
                        continue;
                    }

                    if (size + batch.Size > maxRequestSize)
                    {
                        break;
                    }

                    if (batch.IsReady) //todo всегда true, надо что-то с этим сделать
                    {
                        batch = topicBatch.PopFront();
                    }
                    else
                    {
                        continue;
                    }
                }
                batch.Close();
                size += batch.Size;

                yield return batch;
            }
        }
    }
}