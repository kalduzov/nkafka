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
internal sealed class Producer<TKey, TValue>: Client<ProducerConfig>, IProducer<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Dictionary<Type, object> _defaultSerializers = new()
    {
        [typeof(Null)] = Serializers.Null,
        [typeof(int)] = Serializers.Int,
        [typeof(long)] = Serializers.Long,
        [typeof(string)] = Serializers.String,
        [typeof(float)] = Serializers.Float,
        [typeof(double)] = Serializers.Double,
        [typeof(byte[])] = Serializers.ByteArray,
        [typeof(short)] = Serializers.Short,
        [typeof(Guid)] = Serializers.Guid
    };

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
    private readonly IAsyncSerializer<TKey> _keySerializer;
    private readonly IAsyncSerializer<TValue> _valueSerializer;
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
        IAsyncSerializer<TKey> keySerializer,
        IAsyncSerializer<TValue> valueSerializer,
        ILoggerFactory loggerFactory)
        : this(kafkaCluster, name, config, keySerializer, valueSerializer, null, null, null, loggerFactory)
    {
    }

    /// <summary>
    ///  Test-only constructor
    /// </summary>
    internal Producer(
        IKafkaCluster kafkaCluster,
        string name,
        ProducerConfig config,
        IAsyncSerializer<TKey> keySerializer,
        IAsyncSerializer<TValue> valueSerializer,
        ITransactionManager? transactionManager,
        IRecordAccumulator? recordAccumulator,
        IMessagesSender? messagesSender,
        ILoggerFactory loggerFactory)
        : base(kafkaCluster, config, loggerFactory)
    {
        _name = name;
        _logger = loggerFactory.CreateLogger(name);

        _logger.StartProducerTrace(_name);

        _producerMetrics = config.Metrics;
        _maxRequestSize = config.MaxRequestSize;
        _totalMemorySize = config.BufferMemory;
        _senderTask = Task.CompletedTask; //initialize in order not to make it nullable

        try
        {
            _partitioner = InitPartitionerClass(config.PartitionerConfig);
            _keySerializer = InitializeSerializer(keySerializer);
            _valueSerializer = InitializeSerializer(valueSerializer);
            _deliveryTimeoutMs = ConfigureDeliveryTimeout();
            _transactionManager = transactionManager ?? new TransactionManager(config, loggerFactory);
            _accumulator = recordAccumulator ?? new RecordAccumulator(config, _transactionManager, _deliveryTimeoutMs, loggerFactory);
            _messagesSender = messagesSender ?? new MessagesSender(config, _accumulator, KafkaCluster, loggerFactory);
            _senderTask = _messagesSender.StartAsync(_tokenSource.Token);
            _logger.StartedProducer(_name);
        }
        catch (Exception exc)
        {
            Close(TimeSpan.Zero,
                true); //perhaps something has already managed to be created, so we are trying to clean everything up after ourselves. 

            throw new ProducerException(EM.Producer_CreateError, exc);
        }
    }

    string IProducer.Name => _name;

    /// <inheritdoc/>
    public void Produce(TopicPartition topicPartition, Message<TKey, TValue> message)
    {
        var tp = topicPartition;

        _ = InternalProduceAsync(topicPartition, message, true, CancellationToken.None)
            .ContinueWith(
                task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.WriteLine($"The message {task.Result.Message} was sent successfully");

                        return;
                    }

                    if (!task.IsFaulted)
                    {
                        return;
                    }

                    _logger.ProduceMessageError(task.Exception!, tp);

                });
    }

    /// <inheritdoc/>
    public Task FlushAsync(CancellationToken token)
    {
        return _accumulator.FlushAllAsync(token);
    }

    /// <inheritdoc/>
    public void Flush(TimeSpan timeout)
    {
        _accumulator.FlushAll(timeout);
    }

    /// <inheritdoc/>
    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken token = default)
    {
        return InternalProduceAsync(topicPartition, message, false, token);
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<PartitionMetadata> PartitionsFor(string topic)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Close(TimeSpan timeout)
    {
        _closed = true;
    }

    /// <inheritdoc/>
    public ValueTask CloseAsync(CancellationToken token)
    {
        _closed = true;

        return ValueTask.CompletedTask;
    }

    private void Close(TimeSpan timeSpan, bool swallowException)
    {
        _tokenSource.Cancel(!swallowException);

        if (_senderTask.IsCompleted || _senderTask.IsFaulted || _senderTask.IsCanceled)
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

    private static IPartitioner InitPartitionerClass(PartitionerConfig partitionerConfig)
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

    private static IAsyncSerializer<T> InitializeSerializer<T>(IAsyncSerializer<T> serializer)
    {
        if (serializer != NoneSerializer<T>.Instance)
        {
            return serializer;
        }

        if (_defaultSerializers.TryGetValue(typeof(T), out var ser))
        {
            return (IAsyncSerializer<T>)ser;
        }

        var errorMessage = string.Format(EM.Producer_SerializerError, typeof(T).Name);

        throw new ArgumentNullException(errorMessage);

    }

    private void ThrowIfProducerClosed()
    {
        if (_closed)
        {
            throw new ProducerException(EM.Producer_WasClosed);
        }
    }

    private async Task<DeliveryResult<TKey, TValue>> InternalProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        bool isFireAndForget,
        CancellationToken token = default)
    {
        _logger.ProduceMessageTrace(topicPartition);

        ThrowIfProducerClosed();

        using var activity = KafkaDiagnosticsSource.ProduceMessage(topicPartition, message, isFireAndForget);

        try
        {
            // We request data on topic partitions, for the case when the user has disabled the full update of metadata.  
            _ = await KafkaCluster.GetPartitionsAsync(topicPartition.Topic, token);

            var headers = message.Headers;
            var serializedKey = await SerializeAsync(_keySerializer, message.Key);
            var serializedValue = await SerializeAsync(_valueSerializer, message.Value);

            var serializedSize = RecordsBatch.EstimateSizeInBytesUpperBound(serializedKey, serializedValue, headers);
            EnsureValidRecordSize(serializedSize);

            // Trying to get a partition if it is not set  
            if (topicPartition.Partition.IsSpecial)
            {
                var computedPartition = await _partitioner.PartitionAsync(
                    topicPartition.Topic,
                    typeof(TKey),
                    serializedKey,
                    typeof(TValue),
                    serializedValue,
                    KafkaCluster,
                    token);

                topicPartition.Partition = computedPartition;
            }

            var appendResult = _accumulator.Append(
                topicPartition,
                message.Timestamp.UnixTimestampMs,
                serializedKey,
                serializedValue,
                headers);

            if (appendResult.BatchIsFull || appendResult.NewBatchCreated)
            {
                _messagesSender.Wakeup();
            }

            _producerMetrics.AppendBytes(topicPartition, appendResult.AppendedBytes);

            if (!isFireAndForget)
            {
                var sendResult = await appendResult
                    .SendResult!
                    .Task.WaitAsync(TimeSpan.FromMilliseconds(_deliveryTimeoutMs), token);

                var topicPartitionOffset = new TopicPartitionOffset(topicPartition, sendResult.Offset);

                return new DeliveryResult<TKey, TValue>(message, PersistenceStatus.Persisted, topicPartitionOffset);
            }
            else
            {
                var topicPartitionOffset = new TopicPartitionOffset(topicPartition, Offset.Unset);

                return new DeliveryResult<TKey, TValue>(message, PersistenceStatus.PossiblyPersisted, topicPartitionOffset);
            }

        }
        catch (TimeoutException)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Timeout exception");

            var topicPartitionOffset = new TopicPartitionOffset(topicPartition, Offset.Unset);

            return new DeliveryResult<TKey, TValue>(message, PersistenceStatus.NotPersisted, topicPartitionOffset);
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

    private static Task<byte[]> SerializeAsync<T>(IAsyncSerializer<T> serializer, T value)
    {
        try
        {
            return serializer.PreferAsync
                ? serializer.SerializeAsync(value)
                : Task.FromResult(serializer.Serialize(value));
        }
        catch (Exception exc)
        {
            throw new ProduceException(exc);
        }
    }
}