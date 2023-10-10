//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Collections.Immutable;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

using NKafka.Clients.Consumer.Internal;
using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Serialization;

using EM = NKafka.Resources.ExceptionMessages;

namespace NKafka.Clients.Consumer;

internal class Consumer<TKey, TValue>: Client<ConsumerConfig>, IConsumer<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Dictionary<Type, object> _defaultDeserializers = new()
    {
        [typeof(Null)] = Deserializers.Null,
        [typeof(int)] = Deserializers.Int,
        [typeof(long)] = Deserializers.Long,
        [typeof(string)] = Deserializers.String,
        [typeof(float)] = Deserializers.Float,
        [typeof(double)] = Deserializers.Double,
        [typeof(byte[])] = Deserializers.ByteArray,
        [typeof(short)] = Deserializers.Short,
        [typeof(Guid)] = Deserializers.Guid
    };

    // ReSharper disable once StaticMemberInGenericType
    private static ulong _consumerInstanceId;

    private Channel<ConsumerRecord<TKey, TValue>>? _currentChannel;
    private readonly IFetcher<TKey, TValue> _fetcher;
    private readonly ICoordinator _coordinator;
    private readonly ILogger _logger;
    private Subscription? _currentSubscription;
    private readonly SemaphoreSlim _subscribeSyncBlock;

    public string GroupId { get; }

    public ulong ConsumerInstanceId { get; }

    /// <inheritdoc />
    public IReadOnlyList<TopicPartition> Assignment { get; } = Array.Empty<TopicPartition>();

    internal Consumer(IKafkaCluster kafkaCluster,
        ConsumerConfig config,
        IAsyncDeserializer<TKey> keyDeserializer,
        IAsyncDeserializer<TValue> valuedDeserializer,
        ILoggerFactory loggerFactory)
        :
        this(kafkaCluster, config, keyDeserializer, valuedDeserializer, null, null, loggerFactory)
    {
    }

    internal Consumer(
        IKafkaCluster kafkaCluster,
        ConsumerConfig config,
        IAsyncDeserializer<TKey> keyDeserializer,
        IAsyncDeserializer<TValue> valueDeserializer,
        IFetcher<TKey, TValue>? fetcher,
        ICoordinator? coordinator,
        ILoggerFactory loggerFactory)
        : base(kafkaCluster, config, loggerFactory)
    {
        ConsumerInstanceId = Interlocked.Increment(ref _consumerInstanceId);
        _subscribeSyncBlock = new SemaphoreSlim(1, 1);
        GroupId = config.GroupId;
        config.EventListeners.ToImmutableList();
        _logger = LoggerFactory.CreateLogger(GetType());

        _coordinator = coordinator
                       ?? new Coordinator(kafkaCluster,
                           config.GroupId,
                           config.Heartbeat,
                           config.PartitionAssignors.ToDictionary(x => x.Name),
                           config.EnableAutoCommit,
                           config.AutoCommitIntervalMs,
                           config.RebalanceTimeoutMs,
                           config.SessionTimeoutMs,
                           config.MaxRetries,
                           config.RetryBackoffMs,
                           config.Metrics,
                           LoggerFactory);

        _fetcher = fetcher
                   ?? new Fetcher<TKey, TValue>(kafkaCluster,
                       InitializeDeserializer(keyDeserializer),
                       InitializeDeserializer(valueDeserializer),
                       config.FetchMinBytes,
                       config.FetchMaxBytes,
                       config.FetchMaxWaitMs,
                       config.CheckCrc32,
                       config.IsolationLevel,
                       LoggerFactory);

    }

    /// <inheritdoc/>
    public ValueTask<ChannelReader<ConsumerRecord<TKey, TValue>>> SubscribeAsync(string topicName, CancellationToken token = default)
    {
        return SubscribeAsync(new[]
            {
                topicName
            },
            token);
    }

    /// <inheritdoc />
    public async ValueTask<ChannelReader<ConsumerRecord<TKey, TValue>>> SubscribeAsync(IReadOnlyCollection<string> topics,
        CancellationToken token = default)
    {
        try
        {
            if (_subscribeSyncBlock.CurrentCount == 0)
            {
                throw new ConsumerException("Уже идет операция подписки, нельзя подписываться параллельно");
            }

            await _subscribeSyncBlock.WaitAsync(token);

            CheckTopics(topics);

            await UnsubscribeAsync(token); //Всегда пересоздаем подписку. Консьюмер может быть подписан только на один набор топиков за раз

            _currentSubscription = new Subscription(topics, Config.AutoOffsetReset, Config.PartitionAssignors);

            if (!await _coordinator.NewSessionAsync(_currentSubscription, token))
            {
                throw new ConsumerException("Не удалось создать новую сессию");
            }

            var channel = BuildChannel();

            _currentChannel = channel;

            await _fetcher.StartAsync(_currentSubscription, channel.Writer, token);

            _logger.ConsumerNewSubscriptionTrace(ConsumerInstanceId, _currentSubscription);

            return _currentChannel.Reader;

        }
        finally
        {
            _subscribeSyncBlock.Release();
        }
    }

    private static IAsyncDeserializer<T> InitializeDeserializer<T>(IAsyncDeserializer<T> deserializer)
    {
        if (deserializer != NoneDeserializer<T>.Instance)
        {
            return deserializer;
        }

        if (_defaultDeserializers.TryGetValue(typeof(T), out var ser))
        {
            return (IAsyncDeserializer<T>)ser;
        }

        var errorMessage = string.Format(EM.Producer_SerializerError, typeof(T).Name);

        throw new ArgumentNullException(errorMessage);

    }

    public async ValueTask UnsubscribeAsync(CancellationToken token)
    {
        _currentSubscription = null;
        await _fetcher.StopAsync(token); //Сначала останавливаем все считывания из кафки
        _currentChannel?.Writer.Complete();
        await _coordinator.StopSessionAsync(token);
    }

    public async Task CommitOffsetAsync(CancellationToken token = default)
    {
        if (_currentSubscription is null)
        {
            _logger.LogWarning("Консьюмер не подписан ни на один топик");

            return;
        }

        await _coordinator.CommitAsync(_currentSubscription.OffsetManager, token);
    }

    private static void CheckTopics(IReadOnlyCollection<string> topics)
    {
        ArgumentNullException.ThrowIfNull(topics);

        if (topics.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(topics), "Колекция должна содержать хотя бы один элемент");
        }

        foreach (var topic in topics)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentNullException(nameof(topics), "Коллекция не может содержать не заданных или пустых имен топиков");
            }
        }
    }

    private Channel<ConsumerRecord<TKey, TValue>> BuildChannel()
    {
        if (_currentSubscription is null)
        {
            throw new ConsumerException("Консьюмер не подписан ни на один топик.");
        }

        var logger = LoggerFactory.CreateLogger<ConsumerChannel<TKey, TValue>>();
        var channel = new ConsumerChannel<TKey, TValue>(Config.ChannelSize,
            Config.DropOldRecordsFromChannel,
            _currentSubscription,
            logger);

        return channel;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public override void Dispose()
    {
        base.Dispose();
        _subscribeSyncBlock.Dispose();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public override ValueTask DisposeAsync()
    {
        var result = base.DisposeAsync();

        _subscribeSyncBlock.Dispose();

        return result;

    }
}