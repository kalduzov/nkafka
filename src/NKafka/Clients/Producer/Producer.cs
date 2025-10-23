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

using System.Diagnostics;

using Microsoft.Extensions.Logging;

using NKafka.Clients.Producer.Internals;
using NKafka.Config;
using NKafka.Diagnostics;
using NKafka.Exceptions;
using NKafka.Metrics;
using NKafka.Protocol.Records;
using NKafka.Serialization;

using EM = NKafka.Resources.ExceptionMessages;

namespace NKafka.Clients.Producer;

/// <summary>
/// A Kafka client that publishes messages to the Kafka cluster
/// </summary>
internal sealed partial class Producer: Client<ProducerConfig>, IProducer
{
    private readonly IRecordAccumulator _accumulator;
    private readonly ILogger _logger;
    private readonly int _maxRequestSize;
    private readonly string _name;
    private readonly IProducerMetrics _producerMetrics;
    private readonly IPartitioner _partitioner;
    private readonly CancellationTokenSource _tokenSource = new();
    private readonly int _totalMemorySize;
    private readonly ITransactionManager _transactionManager;
    private bool _closed;
    private readonly Task _senderTask;
    private readonly IMessagesSender _messagesSender;
    private readonly int _deliveryTimeoutMs;

    /// <summary>
    /// Use this constructor to create a producer
    /// </summary>
    internal Producer(
        IKafkaCluster kafkaCluster,
        string name,
        ProducerConfig config,
        ILoggerFactory loggerFactory)
        : this(kafkaCluster, name, config, null, null, null, null, loggerFactory)
    {
    }

    /// <summary>
    ///  Test-only constructor
    /// </summary>
    internal Producer(
        IKafkaCluster kafkaCluster,
        string name,
        ProducerConfig config,
        ITransactionManager? transactionManager,
        IRecordAccumulator? recordAccumulator,
        IMessagesSender? messagesSender,
        IProducerMetrics? producerMetrics,
        ILoggerFactory loggerFactory)
        : base(kafkaCluster, config, loggerFactory)
    {
        _name = name;
        _logger = loggerFactory.CreateLogger(name);

        var clientId = config.ClientId;
        var transactionId = config.TransactionalId;

        LoggerScope = _logger.Begin("producer", clientId, transactionId);

        _logger.StartProducerTrace(_name);

        _producerMetrics = producerMetrics ?? new DefaultProducerMetrics();
        _maxRequestSize = config.MaxRequestSize;
        _totalMemorySize = config.BufferMemory;
        _senderTask = Task.CompletedTask; //initialize in order not to make it nullable

        try
        {
            _partitioner = InitPartitioner(config.PartitionerConfig);
            _deliveryTimeoutMs = ConfigureDeliveryTimeout();
            _transactionManager = transactionManager ?? new TransactionManager(config, loggerFactory);
            _accumulator = recordAccumulator
                           ?? new RecordAccumulator(config, _transactionManager, _deliveryTimeoutMs, _producerMetrics, loggerFactory);
            _messagesSender = messagesSender ?? new MessagesSender(config, _accumulator, KafkaCluster, _producerMetrics, loggerFactory);
            _senderTask = _messagesSender.StartAsync(_tokenSource.Token);

            _logger.StartedProducer(_name);
        }
        catch (Exception exc)
        {
            //perhaps something has already managed to be created, so we are trying to clean everything up after ourselves.
            Close(TimeSpan.Zero, true);

            throw new ProducerException(EM.Producer_CreateError, exc);
        }
    }

    string IProducer.Name => _name;

    /// <inheritdoc/>
    public void Produce(TopicPartition topicPartition,
        Message message,
        Action<MessageDeliveryResult, Exception?> callback,
        CancellationToken cancellationToken)
    {
        var tp = topicPartition;
        var m = message;

        _ = InternalProduceAsync(topicPartition, message, true, cancellationToken)
            .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.WriteLine($"The message {m} was sent successfully");
                        callback(task.Result, null);

                        return;
                    }

                    if (task.IsFaulted)
                    {
                        callback(task.Result, task.Exception);

                        return;
                    }

                    if (task.IsCanceled)
                    {
                        callback(task.Result, new OperationCanceledException("The message was canceled"));

                        return;
                    }

                    _logger.ProduceMessageError(task.Exception!, tp);

                },
                cancellationToken);
    }

    /// <inheritdoc/>
    public async Task FlushAsync(CancellationToken token)
    {
        _logger.FlushingRecordsTrace();

        var timestamp = Stopwatch.StartNew();

        try
        {
            await _accumulator.FlushAllAsync(token);
        }
        finally
        {
            timestamp.Stop();
            _producerMetrics.Flush(timestamp.ElapsedMilliseconds);
        }
    }

    /// <inheritdoc/>
    public Task<MessageDeliveryResult> ProduceAsync(
        TopicPartition topicPartition,
        Message message,
        CancellationToken token)
    {
        return InternalProduceAsync(topicPartition, message, false, token);
    }

    /// <inheritdoc/>
    public ValueTask CloseAsync(CancellationToken cancellationToken)
    {
        _closed = true;

        return ValueTask.CompletedTask;
    }

    private void Close(TimeSpan timeSpan, bool swallowException)
    {
        _tokenSource.Cancel(!swallowException);

        if (_senderTask.IsCompleted)
        {
            _senderTask.Dispose();
        }

    }

    private static int LingerMs(ProducerConfig config)
    {
        return (int)Math.Min(config.LingerMs, int.MaxValue);
    }

    private int ConfigureDeliveryTimeout()
    {
        var deliveryTimeoutMs = Config.DeliveryTimeoutMs;
        var lingerMs = LingerMs(Config);
        var requestTimeoutMs = Config.RequestTimeoutMs;
        var lingerAndRequestTimeoutMs = (int)Math.Min((long)lingerMs + requestTimeoutMs, int.MaxValue);

        if (deliveryTimeoutMs < lingerAndRequestTimeoutMs)
        {
            deliveryTimeoutMs = lingerAndRequestTimeoutMs;
        }

        return deliveryTimeoutMs;
    }

    private static IPartitioner InitPartitioner(PartitionerConfig partitionerConfig)
    {
        switch (partitionerConfig.Partitioner)
        {
            case Partitioner.Custom:
                {
                    object? partitionerClass;

                    try
                    {
                        partitionerClass = Activator.CreateInstance(partitionerConfig.CustomPartitionerClass);
                    }
                    catch (Exception exc)
                    {
                        throw new ArgumentException(EM.PartitionerCreateError, exc);
                    }

                    if (partitionerClass is not IPartitioner partitioner)
                    {
                        throw new ArgumentException(EM.PartitionerCreateError);
                    }

                    return partitioner;
                }
            case Partitioner.Default:
                return new DefaultPartitioner();
            case Partitioner.RoundRobinPartitioner:
                return new RoundRobinPartitioner();
            default:
                throw new ArgumentOutOfRangeException(nameof(partitionerConfig), EM.PartitionerNotFound);
        }
    }

    private void ThrowIfProducerClosed()
    {
        if (_closed)
        {
            throw new ProducerException(EM.Producer_WasClosed);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="message"></param>
    /// <param name="isFireAndForget"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ProducerException"></exception>
    private async Task<MessageDeliveryResult> InternalProduceAsync(
        TopicPartition topicPartition,
        Message message,
        bool isFireAndForget,
        CancellationToken cancellationToken)
    {
        ThrowIfProducerClosed();

        var actualTopicPartition = topicPartition;

        _logger.ProduceMessage(actualTopicPartition);

        using var activity = KafkaDiagnosticsSource.ProduceMessage(actualTopicPartition, message, isFireAndForget);

        var serializedKeySize = message.Key.Length;
        var serializedValueSize = message.Value.Length;

        try
        {
            // We request data on topic partitions, for the case when the user has disabled the full update of metadata.  
            var partitions = await KafkaCluster.GetPartitions(actualTopicPartition.Topic, cancellationToken);

            if (partitions.Count == 0)
            {
                throw new ProducerException($"No partitions found in cluster for topic {actualTopicPartition.Topic}");
            }

            var headers = message.Headers;

            var serializedSize = RecordBatch.EstimateSizeInBytesUpperBound(message.Key, message.Value, headers);
            EnsureValidRecordSize(serializedSize);

            // Trying to get a partition if it is not set  
            if (topicPartition.Partition.IsSpecial)
            {
                var computedPartition = await _partitioner.Partition(
                    topicPartition.Topic,
                    message.Key,
                    message.Value,
                    KafkaCluster,
                    cancellationToken);

                actualTopicPartition = actualTopicPartition with
                {
                    Partition = computedPartition
                };
            }

            var appendResult = _accumulator.Append(
                actualTopicPartition,
                message.Timestamp.UnixTimestampMs,
                message.Key,
                message.Value,
                headers);

            if (_transactionManager.IsTransactional)
            {
                _transactionManager.TryAddPartition(actualTopicPartition);
            }

            if (appendResult.BatchIsFull || appendResult.NewBatchCreated)
            {
                _messagesSender.Wakeup();
            }

            _producerMetrics.AppendBytes(actualTopicPartition, appendResult.AppendedBytes);

            if (!isFireAndForget)
            {
                var sendResult = await appendResult
                    .SendResult!
                    .Task.WaitAsync(TimeSpan.FromMilliseconds(_deliveryTimeoutMs), cancellationToken);

                var topicPartitionOffset = new TopicPartitionOffset(actualTopicPartition, sendResult.Offset);

                return new MessageDeliveryResult(PersistenceStatus.Persisted,
                    topicPartitionOffset.TopicPartition,
                    message.Timestamp.UnixTimestampMs,
                    topicPartitionOffset.Offset,
                    serializedKeySize,
                    serializedValueSize,
                    message);
            }
            else
            {
                var topicPartitionOffset = new TopicPartitionOffset(actualTopicPartition, Offset.Unset);

                return new MessageDeliveryResult(PersistenceStatus.PossiblyPersisted,
                    topicPartitionOffset.TopicPartition,
                    message.Timestamp.UnixTimestampMs,
                    topicPartitionOffset.Offset,
                    serializedKeySize,
                    serializedValueSize,
                    message);
            }

        }
        catch (TimeoutException)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Timeout exception");

            var topicPartitionOffset = new TopicPartitionOffset(topicPartition, Offset.Unset);

            return new MessageDeliveryResult(PersistenceStatus.NotPersisted,
                topicPartitionOffset.TopicPartition,
                message.Timestamp.UnixTimestampMs,
                topicPartitionOffset.Offset,
                serializedKeySize,
                serializedValueSize,
                message);
        }
        catch (Exception exc)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exc.Message);

            throw;
        }
    }

    /// <summary>
    /// Make sure the record size is valid.
    /// </summary>
    private void EnsureValidRecordSize(int size)
    {
        if (size > _maxRequestSize)
        {
            var message = string.Format(EM.Producer_SizeVeryLarge, nameof(Config.MaxRequestSize), _maxRequestSize);

            throw new ProducerException(message);
        }

        // ReSharper disable once InvertIf
        if (size > _totalMemorySize)
        {
            var message = string.Format(EM.Producer_SizeVeryLarge, nameof(Config.BufferMemory), _totalMemorySize);

            throw new ProducerException(message);
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public override void Dispose()
    {
        _tokenSource.Dispose();
        base.Dispose();
    }
}