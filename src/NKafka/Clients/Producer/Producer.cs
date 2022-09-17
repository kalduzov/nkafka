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

using NKafka.Clients.Producer.Internals;
using NKafka.Config;
using NKafka.Diagnostics;
using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Serialization;

namespace NKafka.Clients.Producer;

/// <summary>
/// A Kafka client that publishes messages to the Kafka cluster
/// </summary>
internal sealed partial class Producer<TKey, TValue>: Client<ProducerConfig>, IProducer<TKey, TValue>
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
    private readonly ApiVersion _apiVersion;

    private readonly IReadOnlyCollection<ProducerInterceptor<TKey, TValue>> _interceptors;

    private readonly string _name;

    private readonly IPartitioner _partitioner;
    private readonly TimeSpan _time;

    private readonly CancellationTokenSource _tokenSource = new();
    private bool _enableDeliveryReports;
    private IAsyncSerializer<TKey> _keySerializer;
    private IAsyncSerializer<TValue> _valueSerializer;

    string IProducer.Name => _name;

    internal Producer(
        IKafkaCluster kafkaCluster,
        string name,
        ProducerConfig config,
        IAsyncSerializer<TKey>? keySerializer,
        IAsyncSerializer<TValue>? valueSerializer,
        IReadOnlyCollection<ProducerInterceptor<TKey, TValue>> interceptors,
        ILogger logger)
        : base(kafkaCluster, config, logger)
    {
        _name = name;

        var transactionalId = config.TransactionalId;
        var clientId = config.ClientId;

        var logFormat = transactionalId is null
            ? "[Producer clientId={ClientId}] "
            : "[Producer clientId={ClientId}, transactionalId={TransactionalId}] ";

        Logger.LogTrace(logFormat + "Starting the Kafka producer", clientId, transactionalId);

        //todo: иницилизировать механиз метрик

        try
        {
            _partitioner = InitPartitionerClass();
            InitializeSerializers(keySerializer, valueSerializer);
            _interceptors = InitInterceptors(interceptors);

            _apiVersion = new ApiVersion();

            var deliveryTimeoutMs = ConfigureDeliveryTimeout();
            _accumulator = new RecordAccumulator(config, deliveryTimeoutMs);

            //create new long thread for IO operations
            Logger.LogDebug(logFormat + "Kafka producer started", clientId, transactionalId);
        }
        catch (Exception exc)
        {
            Close(TimeSpan.Zero, true); //возможно уже что-то успело создатся, поэтому пробуем подчистить все за собой 

            throw new ProtocolKafkaException(ErrorCodes.UnknownServerError, "Failed to construct kafka producer", exc);
        }
    }

    public Task AbortTransaction(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void SendOffsetsToTransaction(Dictionary<TopicPartition, OffsetAndMetadata> offsets, string consumerGroupId)
    {
    }

    public void SendOffsetsToTransaction(Dictionary<TopicPartition, OffsetAndMetadata> offsets, ConsumerGroupMetadata groupMetadata)
    {
    }

    // /// <summary>
    // /// </summary>
    // /// <param name="topicPartition"></param>
    // /// <param name="messages"></param>
    // /// <param name="token"></param>
    // /// <returns></returns>
    // public async Task<IEnumerable<DeliveryResult<TKey, TValue>>> ProduceAsync(
    //     TopicPartition topicPartition,
    //     IEnumerable<Message<TKey, TValue>> messages,
    //     CancellationToken token = default)
    // {
    //     var enumerable = messages as Message<TKey, TValue>[] ?? messages.ToArray();
    //
    //     var list = new List<Task<DeliveryResult<TKey, TValue>>>(enumerable.Length);
    //     list.AddRange(enumerable.Select(message => ProduceAsync(topicPartition, message, token)));
    //
    //     await Task.WhenAll(list);
    //
    //     return list.Select(x => x.GetAwaiter().GetResult());
    // }

    /// <summary>
    ///     Fire and forget produce message
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="message"></param>
    public void Produce(TopicPartition topicPartition, Message<TKey, TValue> message)
    {
        using var _ = KafkaDiagnosticsSource.ProduceMessage(topicPartition, message, true);

        //todo call interceptors
        ThrowIfProducerClosed();
    }

    /// <summary>
    ///     Fire and forget produce message
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="messages"></param>
    public void Produce(TopicPartition topicPartition, IEnumerable<Message<TKey, TValue>> messages)
    {
        foreach (var message in messages)
        {
            Produce(topicPartition, message);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="message"></param>
    /// <param name="deliveryCallback"></param>
    public void Produce(TopicPartition topicPartition, Message<TKey, TValue> message, Action<DeliveryResult<TKey, TValue>> deliveryCallback)
    {
    }

    /// <summary>
    /// </summary>
    public Task FlushAsync(CancellationToken cancellationToken)
    {
        return null;
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
    public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken token = default)
    {
        using var _ = KafkaDiagnosticsSource.ProduceMessage(topicPartition, message, false);

        return await InternalProduceAsync(topicPartition, message, token);
    }

    public IReadOnlyCollection<PartitionInfo> PartitionsFor(string topic)
    {
        return null;
    }

    public void Close(TimeSpan timeout)
    {
    }

    public Task CloseAsync(CancellationToken token)
    {
        return null;
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }

    private void Close(TimeSpan timeSpan, bool swallowException)
    {
        _tokenSource.Cancel(!swallowException);
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

    private IReadOnlyCollection<ProducerInterceptor<TKey, TValue>> InitInterceptors(
        IReadOnlyCollection<ProducerInterceptor<TKey, TValue>> interceptors)
    {
        return interceptors;
    }

    private IPartitioner InitPartitionerClass()
    {
        return null;
    }

    private void InitializeSerializers(
        IAsyncSerializer<TKey>? keySerializer,
        IAsyncSerializer<TValue>? valueSerializer)
    {
        if (keySerializer is null)
        {
            if (!_defaultSerializers.TryGetValue(typeof(TKey), out var serializer))
            {
                throw new ArgumentNullException(
                    $"Key serializer not specified and there is no default serializer defined for type {typeof(TKey).Name}.");
            }

            _keySerializer = (IAsyncSerializer<TKey>)serializer;
        }
        else
        {
            _keySerializer = keySerializer;
        }

        if (valueSerializer is null)
        {
            if (!_defaultSerializers.TryGetValue(typeof(TValue), out var serializer))
            {
                throw new ArgumentNullException(
                    $"Value serializer not specified and there is no default serializer defined for type {typeof(TValue).Name}.");
            }

            _valueSerializer = (IAsyncSerializer<TValue>)serializer;
        }
        else
        {
            _valueSerializer = valueSerializer;
        }
    }

    private void ThrowIfProducerClosed()
    {
    }

    /// <summary>
    /// </summary>
    public Task FlushAsync()
    {
        return null;
    }

    public void Flush()
    {
    }

    private async ValueTask<DeliveryResult<TKey, TValue>> InternalProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken token = default)
    {
        var headers = message.Headers;
        var keyByte = await SerializeKeyAsync(message.Key);
        var valueByte = await SerializeValueAsync(message.Value);

        var appendResult = await _accumulator.AppendAsync(topicPartition, message.Timestamp, keyByte, valueByte, headers, token);

        if (Config.EnableDeliveryReports)
        {
            var offset = await appendResult.TaskCompletionSource.Task;

            return new DeliveryResult<TKey, TValue>
            {
                TopicPartitionOffset = new TopicPartitionOffset(topicPartition, offset),
                Message = message,
                Status = PersistenceStatus.Persisted
            };
        }

        return new DeliveryResult<TKey, TValue>
        {
            TopicPartitionOffset = new TopicPartitionOffset(topicPartition, Offset.Unset),
            Message = message,
            Status = PersistenceStatus.NotPersisted
        };
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