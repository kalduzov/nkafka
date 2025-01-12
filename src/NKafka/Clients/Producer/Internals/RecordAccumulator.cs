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

using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

using NKafka.Collections;
using NKafka.Compressions;
using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Metrics;
using NKafka.Protocol.Buffers;
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
    /// <summary>
    /// Коллекция пакетов в виде двухсторонней очереди
    /// </summary>
    private class ProducerBatchesDeque(): Deque<ProducerBatch>(ProducerBatch.Null);

    /// <summary>
    /// Коллекция пакетов распределенных по партициям
    /// </summary>
    private class PartitionedBatchCollection: ConcurrentDictionary<Partition, ProducerBatchesDeque>;

    //Пачки распределенные по топикам
    private readonly ConcurrentDictionary<string, PartitionedBatchCollection> _batchesByTopics;

    private readonly int _batchSize;
    private readonly bool _closed;
    private readonly ICompression _compression;
    private readonly int _deliveryTimeoutMs;
    private readonly double _lingerMs;
    private readonly ILogger _logger;
    private readonly long _retryBackoffMs;
    private readonly ITransactionManager _transactionManager;
    private volatile int _appendsInProgress;
    private volatile int _flushesInProgress = 0;
    private readonly IProducerMetrics _metrics;
    private readonly ILoggerFactory _loggerFactory;

    public RecordAccumulator(
        ProducerConfig config,
        ITransactionManager transactionManager,
        int deliveryTimeoutMs,
        IProducerMetrics metrics,
        ILoggerFactory loggerFactory)
    {
        _batchesByTopics = new ConcurrentDictionary<string, PartitionedBatchCollection>();
        _metrics = metrics;
        _loggerFactory = loggerFactory;
        _transactionManager = transactionManager;
        _deliveryTimeoutMs = deliveryTimeoutMs;
        _logger = loggerFactory.CreateLogger<RecordAccumulator>();
        _closed = false;
        _batchSize = Math.Max(1, config.BatchSize);
        _compression = GetCompression(config.Compression);
        _retryBackoffMs = config.RetryBackoffMs;
        _lingerMs = config.LingerMs;

    }

    private static ICompression GetCompression(CompressionConfig compression)
    {
        return compression.CompressionType switch
        {
            CompressionType.None => new NoCompression(),
            CompressionType.Gzip => new GZIPCompression(compression.GzipLevel),
            CompressionType.Lz4 => new LZ4Compression(compression.LZ4Level),
            CompressionType.ZStd => new ZStdCompression(compression.ZstdLevel),
            CompressionType.Snappy => new SnappyCompression(),
            _ => new NoCompression()
        };
    }

    /// <summary>
    /// Appends a record to the specified topic partition with the given timestamp, key, value, and headers.
    /// </summary>
    /// <param name="topicPartition">The topic partition to append the record to.</param>
    /// <param name="timestamp">The timestamp of the record.</param>
    /// <param name="key">The key of the record.</param>
    /// <param name="value">The value of the record.</param>
    /// <param name="headers">The headers of the record.</param>
    /// <returns>A <see cref="RecordAppendResult"/> object representing the result of the append operation.</returns>
    public RecordAppendResult Append(
        TopicPartition topicPartition,
        long timestamp,
        byte[] key,
        byte[] value,
        Headers headers)
    {
        Interlocked.Increment(ref _appendsInProgress);

        // list of batches for a specific topic divided by partitions
        var topicBatches = _batchesByTopics.GetOrAdd(topicPartition.Topic, _ => new PartitionedBatchCollection());

        ArrayBuffer? buffer = null;

        try
        {
            while (true)
            {
                var effectivePartition = topicPartition.Partition.Value;

                // get a queue containing batches for adding records
                var deque = topicBatches.GetOrAdd(effectivePartition, _ => new ProducerBatchesDeque());

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
                if (buffer is null)
                {
                    // We calculate what buffer size we need and try to get it 
                    var size = Math.Max(_batchSize, RecordsBatch.EstimateSizeInBytesUpperBound(key, value, headers));
                    buffer = ArrayBufferPool.Rent(size);
                }

                lock (deque)
                {
                    var recordAppendResult = AppendIntoNewBatch(topicPartition.Topic,
                        effectivePartition,
                        deque,
                        timestamp,
                        key,
                        value,
                        headers,
                        buffer);

                    // It is possible that the batch was already created in another thread while we were preparing the buffer
                    if (recordAppendResult.NewBatchCreated)
                    {
                        buffer = null; // We do not return the buffer to the pool. This buffer will be used in BufferWriter
                    }

                    return recordAppendResult;
                }
            }
        }
        finally
        {
            ArrayBufferPool.Return(buffer);

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
        ArrayBuffer buffer)
    {
        // We are trying to add, all of a sudden, while we were preparing to add, someone has already added a new batch
        if (TryAppend(timestamp, key, value, headers, deque, out var recordAppendResult))
        {
            return recordAppendResult;
        }

        var topicPartition = new TopicPartition(topic, partition);

        var batch = new ProducerBatch(topicPartition, buffer, _loggerFactory);

        _logger.AddNewBatchTrace(topicPartition);

        if (!batch.TryAppend(timestamp, key, value, headers, out var sendResultTask))
        {
            throw new ArgumentNullException(nameof(sendResultTask));
        }

        deque.AddLast(batch);

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

        if (!deque.TryPeekLast(out var lastBatch)) //get the last batch from the queue
        {
            return false;
        }

        var initialBytes = lastBatch.EstimatedSizeInBytes;

        if (!lastBatch.TryAppend(timestamp, key, value, headers, out var sendResultTask))
        {
            lastBatch.Close();
            lastBatch.SetReady();

            return false;
        }
        var appendedBytes = lastBatch.EstimatedSizeInBytes - initialBytes;

        recordAppendResult = new RecordAppendResult(sendResultTask, deque.Count > 1 || lastBatch.IsFull, false, appendedBytes);

        return true;
    }

    /// <inheritdoc/>
    public async Task FlushAllAsync(CancellationToken cancellationToken)
    {
        if (_flushesInProgress > 0)
        {
            return;
        }

        Interlocked.Increment(ref _flushesInProgress);

        try
        {
            // 1. Выбираем все батчи
            // 2. Помечаем их как готовые к отправке, вне зависимости от размера, времени и т.п.
            // 3. Ждем когда sender их отправит или выставит в ошибку

            var waitingTasks = new List<Task>();

            foreach (var batches in _batchesByTopics.Values)
            {
                foreach (var batchesByPartition in batches.Values)
                {
                    foreach (var batch in batchesByPartition)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        batch.Close();
                        batch.SetReady();
                        var completion = batch.CompletionTask;
                        waitingTasks.Add(completion);
                    }
                }
            }
            await Task.WhenAll(waitingTasks);
        }
        finally
        {
            Interlocked.Decrement(ref _flushesInProgress);
        }

    }

    /// <inheritdoc/>
    public IEnumerable<ProducerBatch> PullReadyBatches(int maxRequestSize)
    {
        var size = 0;

        foreach (var batches in _batchesByTopics.Values) //Обрабатываем все данные по всем топикам за раз
        {
            foreach (var deque in batches.Values) // Ищем все собранные очереди в разрезе партиций
            {
                ProducerBatch? firstBatch;

                lock (deque) // Блокируем очередную очередь
                {
                    firstBatch = deque.PeekFirst(); // Проверяем первый пакет перед извлечением

                    if (firstBatch == ProducerBatch.Null) // Нет готового батча - идем к следующей очереди
                    {
                        continue;
                    }

                    if (Timestamp.DateTimeToUnixTimestampMs(DateTime.UtcNow) - firstBatch.CreateTimestamp > _lingerMs)
                    {
                        firstBatch.SetReady();
                    }

                    // Пакет есть - проверяем, если мы добавим этот пакет в запрос, его размер превысит ограничение на запрос?
                    // Если это первый пакет, то мы игнорируем процесс отбора. Первый пакет отправляется всегда!
                    if (size + firstBatch.Size > maxRequestSize)
                    {
                        if (size == 0) //первый пакет для отправки
                        {
                            _logger.LogWarning(
                                "Пакет имеет размер больше чем требуется. Т.к. это первый пакет - он все равно будет отправлен. Пожалуйста повысьте значение MaxRequestSize");
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (firstBatch.IsReady)
                    {
                        firstBatch = deque.RemoveLast();
                    }
                    else
                    {
                        continue;
                    }
                }
                firstBatch.Close();
                size += firstBatch.Size;

                yield return firstBatch;
            }
        }
    }
}