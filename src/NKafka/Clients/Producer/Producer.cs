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
using NKafka.Protocol;
using NKafka.Protocol.Records;
using NKafka.Resources;
using NKafka.Serialization;

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

    private readonly RecordAccumulator _accumulator;
    private readonly CompressionType _compressionType;
    private readonly ILogger _logger;
    private readonly int _maxRequestSize;
    private readonly string _name;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IPartitioner _partitioner;
    private readonly TimeSpan _time;
    private readonly CancellationTokenSource _tokenSource = new();
    private readonly int _totalMemorySize;
    private readonly ITransactionManager _transactionManager;
    private bool _closed;
    private readonly IAsyncSerializer<TKey> _keySerializer;
    private readonly IAsyncSerializer<TValue> _valueSerializer;
    private readonly Task _senderTask;
    private readonly ISender _sender;

    internal Producer(
        IKafkaCluster kafkaCluster,
        string name,
        ProducerConfig config,
        IAsyncSerializer<TKey> keySerializer,
        IAsyncSerializer<TValue> valueSerializer,
        ILoggerFactory loggerFactory)
        : base(kafkaCluster, config, loggerFactory)
    {
        _name = name;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger(name);

        _logger.StartProducer(_name);

        _transactionManager = new TransactionManager();
        _maxRequestSize = config.MaxRequestSize;
        _totalMemorySize = config.BufferMemory;

        try
        {
            _compressionType = config.CompressionType;
            _partitioner = InitPartitionerClass(config.PartitionerConfig);
            _keySerializer = InitializeSerializer(keySerializer);
            _valueSerializer = InitializeSerializer(valueSerializer);
            var deliveryTimeoutMs = ConfigureDeliveryTimeout();
            _accumulator = new RecordAccumulator(config, _transactionManager, deliveryTimeoutMs, loggerFactory);
            _sender = BuildSender();
            _senderTask = Task.Factory.StartNew(_sender.RunAsync, _tokenSource, TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously);
            _logger.StartedProducer(_name);
        }
        catch (Exception exc)
        {
            Close(TimeSpan.Zero, true); //возможно уже что-то успело создаться, поэтому пробуем подчистить все за собой 

            throw new ProtocolKafkaException(ErrorCodes.UnknownServerError, "Failed to construct kafka producer", exc);
        }
    }

    private ISender BuildSender()
    {
        return new Sender(Config, _accumulator, KafkaCluster, _loggerFactory);
    }

    string IProducer.Name => _name;

    /// <summary>
    ///     Fire and forget produce message
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="message"></param>
    public void Produce(TopicPartition topicPartition, Message<TKey, TValue> message)
    {
        //using var activity = KafkaDiagnosticsSource.ProduceMessage(topicPartition, message, true);

        InternalProduceAsync(topicPartition, message, true, CancellationToken.None)
            .ContinueWith(
                task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.WriteLine($"Отправка сообщения {message} прошла успешно");
                    }
                });
    }

    /// <summary>
    /// </summary>
    public Task FlushAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public void Flush(TimeSpan timeout)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken token = default)
    {
        return InternalProduceAsync(topicPartition, message, false, token);
    }

    public IReadOnlyCollection<PartitionMetadata> PartitionsFor(string topic)
    {
        throw new NotImplementedException();
    }

    public void Close(TimeSpan timeout)
    {
        _closed = true;
    }

    public ValueTask CloseAsync(CancellationToken token)
    {
        _closed = true;

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }

    public Task AbortTransaction(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public void SendOffsetsToTransaction(Dictionary<TopicPartition, OffsetAndMetadata> offsets, string consumerGroupId)
    {
    }

    public void SendOffsetsToTransaction(Dictionary<TopicPartition, OffsetAndMetadata> offsets, ConsumerGroupMetadata groupMetadata)
    {
    }

    private void Close(TimeSpan timeSpan, bool swallowException)
    {
        _tokenSource.Cancel(!swallowException);

        if (_senderTask.IsCompleted || _senderTask.IsFaulted || _senderTask.IsCanceled)
        {
            _senderTask.Dispose();
        }
    }

    // private short ConfigureAcks(ProducerConfig config, ILogger logger)
    // {
    //     var acks = (short)Config.Acks;
    //
    //     if (!config.IdempotenceEnabled)
    //     {
    //         return acks;
    //     }
    //
    //     if (Config.Acks == Acks.NoSet)
    //     {
    //         Logger.OverrideDefaultAcks(acks);
    //     }
    //     else if (acks != -1)
    //     {
    //         throw new KafkaConfigException("Must set {0} to all in order to use the idempotent");
    //     }
    //
    //     return acks;
    // }

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
                    try
                    {
                        var partitionerClass = Activator.CreateInstance(partitionerConfig.CustomPartitionerClass);

                        if (partitionerClass is null)
                        {
                            throw new ArgumentException(
                                "Класс, выбранный в качестве пользовательского алгоритма распределения по разделам, не может быть создан.");
                        }

                        return (IPartitioner)partitionerClass;
                    }
                    catch (Exception exc)
                    {
                        throw new ArgumentException(
                            "Класс, выбранный в качестве пользовательского алгоритма распределения по разделам, не может быть создан.",
                            exc);
                    }
                }
            case Partitioner.Default:
                return new DefaultPartitioner();
            case Partitioner.RoundRobinPartitioner:
                return new RoundRobinPartitioner();
            default:
                throw new ArgumentOutOfRangeException(nameof(partitionerConfig), ExceptionMessages.PartitionerNotFound);
        }
    }

    private static IAsyncSerializer<T> InitializeSerializer<T>(IAsyncSerializer<T> serializer)
    {
        if (serializer != NoneSerializer<T>.Instance)
        {
            return serializer;
        }

        if (!_defaultSerializers.TryGetValue(typeof(T), out var ser))
        {
            throw new ArgumentNullException(
                $"Serializer not specified and there is no default serializer defined for type {typeof(T).Name}.");
        }

        return (IAsyncSerializer<T>)ser;

    }

    private void ThrowIfProducerClosed()
    {
        if (_closed)
        {
            throw new KafkaException("Невозможно выполнить операцию, т.к. продюсер был закрыт");
        }
    }

    /// <summary>
    /// </summary>
    public Task FlushAsync()
    {
        return _accumulator.FlushAllAsync();
    }

    public void Flush()
    {
        _accumulator.FlushAll();
    }

    private async Task<DeliveryResult<TKey, TValue>> InternalProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        bool isFireAndForget,
        CancellationToken token = default)
    {
        _logger.ProduceMessage(topicPartition);

        ThrowIfProducerClosed();

        using var activity = KafkaDiagnosticsSource.ProduceMessage(topicPartition, message, isFireAndForget);

        try
        {
            // Получаем данные по разделам топика, для случаев, если пользователь отключил полное обновление метаданных  
            await KafkaCluster.GetPartitionsAsync(topicPartition.Topic, token);

            var headers = message.Headers;
            var serializedKey = await SerializeKeyAsync(message.Key);
            var serializedValue = await SerializeValueAsync(message.Value);

            // Пробуем вычислить раздел  
            if (topicPartition.Partition.IsSpecial)
            {
                topicPartition.Partition = await _partitioner.PartitionAsync(
                    topicPartition.Topic,
                    typeof(TKey),
                    serializedKey,
                    typeof(TValue),
                    serializedValue,
                    KafkaCluster,
                    token);
            }

            headers.SetReadOnly(); //Запрещаем изменение заголовков, пока не будут отправлены данные

            var serializedSize = Records.EstimateSizeInBytesUpperBound(serializedKey, serializedValue, headers);
            EnsureValidRecordSize(serializedSize);

            var appendResult = _accumulator.Append(
                topicPartition,
                message.Timestamp.UnixTimestampMs,
                serializedKey,
                serializedValue,
                headers);

            if (appendResult.BatchIsFull || appendResult.NewBatchCreated)
            {
                _sender.Wakeup();
            }

            if (!isFireAndForget)
            {
                var sendResult = await appendResult.SendResult!.Task;

                var topicPartitionOffset = new TopicPartitionOffset(topicPartition, sendResult.Offset);

                return new DeliveryResult<TKey, TValue>(message, PersistenceStatus.Persisted, topicPartitionOffset);
            }

            {
                var topicPartitionOffset = new TopicPartitionOffset(topicPartition, Offset.Unset);

                return new DeliveryResult<TKey, TValue>(message, PersistenceStatus.PossiblyPersisted, topicPartitionOffset);
            }
        }
        catch (Exception exc)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exc.Message);

            throw;
        }
    }

    private void EnsureValidRecordSize(int size)
    {
        if (size > _maxRequestSize)
        {
            throw new KafkaException($"Размер записи слишком большой и превышает параметр {_maxRequestSize}");
        }

        if (size > _totalMemorySize)
        {
            throw new KafkaException($"Размер записи слишком большой и превышает параметр {_totalMemorySize}");
        }
    }

    private async Task<byte[]> SerializeKeyAsync(TKey key)
    {
        try
        {
            return await _keySerializer.SerializeAsync(key);
        }
        catch (Exception exc)
        {
            throw new ProduceException(exc);
        }
    }

    private async Task<byte[]> SerializeValueAsync(TValue value)
    {
        try
        {
            return await _valueSerializer.SerializeAsync(value);
        }
        catch (Exception exc)
        {
            throw new ProduceException(exc);
        }
    }
}