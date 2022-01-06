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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Clients.Producer;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Connection;
using Microlibs.Kafka.Exceptions;
using Microlibs.Kafka.Protocol;
using Microlibs.Kafka.Protocol.Requests;
using Microlibs.Kafka.Protocol.Responses;
using Microsoft.Extensions.Logging;

namespace Microlibs.Kafka;

/// <summary>
/// </summary>

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class KafkaCluster : IKafkaCluster
{
    private const string _DEFAULT_PRODUCER_NAME = "__DefaultProducer";

    private readonly Dictionary<string, IConsumer?> _consumers = new();
    private readonly object _lockObject = new();
    private readonly ILogger<KafkaCluster> _logger;
    private readonly ILoggerFactory _loggerFactory;

    //Минимально поддерживаемая версия кафки 
    private readonly Version _minSupportVersion = new(1, 0, 0, 0);
    private readonly Dictionary<string, IProducer?> _producers = new();
    private readonly Timer _metadataUpdaterTimer;
    private volatile int _metadataUpdating;
    private readonly HashSet<string> _topics;
    private List<IBroker> _brokers;

    private Dictionary<string, SortedSet<Partition>> _partitions = new();

    /// <summary>
    /// Идентификатор кластера
    /// </summary>
    public string? ClusterId { get; private set; }

    /// <summary>
    ///     Конфигурация кластера
    /// </summary>
    public ClusterConfig Config { get; }

    /// <summary>
    ///     Список топиков в кластере
    /// </summary>
    /// <remarks>Возвращаются все топики, которые были запрошены для кластера</remarks>
    public IReadOnlyCollection<string> Topics => _topics;

    /// <summary>
    ///     Закрыт кластер или открыт
    /// </summary>
    /// <remarks>Из закрытого или уничтоженного кластера невозможно получить никакую информацию</remarks>
    public bool Closed { get; private set; }

    /// <summary>
    ///     Информация о брокере, который является контроллером
    /// </summary>
    public IBroker Controller { get; private set; }

    /// <summary>
    ///     Список всех брокеров кластера
    /// </summary>
    public IReadOnlyCollection<IBroker> Brokers => _brokers;

    /// <summary>
    /// Создает новый класс описывающий конкретный кластер
    /// </summary>
    internal KafkaCluster(ClusterConfig config, ILoggerFactory loggerFactory)
    {
        Closed = true;
        Config = config;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<KafkaCluster>();
        _metadataUpdaterTimer = new Timer(UpdateMetadataCallback, null, Timeout.Infinite, Timeout.Infinite);
        _topics = new HashSet<string>();
        _brokers = SeedBrokers(Config);
    }

    /// <summary>
    /// Возвращает список партиций для топика
    /// </summary>
    public async Task<IReadOnlyCollection<Partition>> GetPartitionsAsync(string topic, CancellationToken token = default)
    {
        ThrowIfClusterClosed();

        if (_partitions.TryGetValue(topic, out var partitions) && partitions.Count != 0)
        {
            return partitions;
        }

        await RefreshMetadataAsync(token, topic).ConfigureAwait(false);

        if (_partitions.TryGetValue(topic, out partitions) && partitions.Count != 0)
        {
            return partitions;
        }

        return null;
    }

    /// <summary>
    /// Возвращает текущее смещение партиции в топике
    /// </summary>
    public Task<Offset> GetOffsetAsync(string topic, Partition partition, CancellationToken token = default)
    {
        return null;
    }

    /// <summary>
    ///     Создает нового продюсера для работы с кластером или возвращает уже существующий
    /// </summary>
    /// <param name="name"></param>
    /// <param name="producerConfig">Конфигурация продюсера</param>
    public IProducer<TKey, TValue> BuildProducer<TKey, TValue>(string? name = null, ProducerConfig? producerConfig = null)
    {
        ThrowIfClusterClosed();

        lock (_lockObject)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = _DEFAULT_PRODUCER_NAME;
            }

            if (_producers.TryGetValue(name, out var producer))
            {
                Debug.Assert(producer is not null);

                return (IProducer<TKey, TValue>)producer;
            }

            producerConfig = producerConfig != null ? producerConfig.Merge(Config) : ProducerConfig.BaseFrom(Config);

            producer = new Producer<TKey, TValue>(
                kafkaCluster: this,
                name: name,
                config: producerConfig,
                keySerializer: null!,
                valueSerializer: null,
                interceptors: null!,
                loggerFactory: _loggerFactory);

            _producers.Add(name, producer);

            return (IProducer<TKey, TValue>)producer;
        }
    }

    /// <summary>
    ///     Создает консьюмера связанного с текущим кластером
    /// </summary>
    /// <param name="consumeGroupName">Название группы консьюмера</param>
    /// <param name="consumerConfig">Конфигурация консьюмера</param>
    /// <remarks>Метод всегда возвращает новый консьюмер, привязанный к конкретной группе</remarks>
    public IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(string consumeGroupName, ConsumerConfig? consumerConfig = null)
    {
        ThrowIfClusterClosed();

        throw new NotImplementedException();
    }

    /// <summary>
    ///     Обновляет метаданные по указанным топикам
    /// </summary>
    /// <param name="topics">Список топиков, по которым необходимо получить информацию из брокеров</param>
    public async Task<MetadataResponseMessage> RefreshMetadataAsync(CancellationToken token, params string[] topics)
    {
        ThrowIfClusterClosed();

        var broker = GetLeastLoadedBroker();

        var request = new MetadataRequestMessage(ApiVersions.Version4, topics)
        {
            AllowAutoTopicCreation = true,
        };

        var message = await broker.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(request, token);

        Debug.WriteLine($"Message {message.ClusterId ?? "none"}");

        UpdateBrokers(message.Brokers, token);
        UpdateTopicPartitions(message.Topics, token);

        return message;
    }

    public void Dispose()
    {
        _metadataUpdaterTimer.Dispose();
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
    ///     asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _metadataUpdaterTimer.DisposeAsync();
    }

    private List<IBroker> SeedBrokers(CommonConfig commonConfig)
    {
        var brokers = new List<IBroker>(Config.BootstrapServers.Count);

        foreach (var bootstrapServer in commonConfig.BootstrapServers)
        {
            var endpoint = Utils.BuildBrokerEndPoint(bootstrapServer);
            var connection = new Broker(endpoint);
            brokers.Add(connection);
        }

        return brokers;
    }

    private void UpdateTopicPartitions(IReadOnlyCollection<TopicInfo> messageTopics, CancellationToken token)
    {
        foreach (var messageTopic in messageTopics)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            _topics.Add(messageTopic.Name);

            SortedSet<Partition> partitions;

            if (!_partitions.ContainsKey(messageTopic.Name))
            {
                partitions = new SortedSet<Partition>();
                _partitions.Add(messageTopic.Name, partitions);
            }

            partitions = _partitions[messageTopic.Name];

            foreach (var topicPartition in messageTopic.Partitions)
            {
                partitions.Add(new Partition(topicPartition.PartitionIndex));
            }
        }
    }

    private void UpdateBrokers(IReadOnlyCollection<BrokerInfo> messageBrokers, CancellationToken token)
    {
    }

    private IBroker GetLeastLoadedBroker()
    {
        return Brokers.First();
    }

    private async void UpdateMetadataCallback(object state)
    {
        ThrowIfClusterClosed();

        _logger.LogTrace("UpdateMetadata start");

        var metadataUpdating = Interlocked.CompareExchange(ref _metadataUpdating, 1, 0);

        var tokenSource = new CancellationTokenSource();

        try
        {
            tokenSource.CancelAfter(Config.RequestTimeoutMs);
            await UpdateMetadata(tokenSource.Token);
        }
        catch
        {
            _logger.LogTrace("UpdateMetadata end");
            tokenSource.Dispose();
        }
    }

    private async Task UpdateMetadata(CancellationToken token)
    {
        var broker = GetLeastLoadedBroker();

        var request = new MetadataRequestMessage(ApiVersions.Version2, _topics);

        var response = await broker.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(request, token);

        var brokers = new List<IBroker>(response.Brokers.Count);

        foreach (var (nodeId, host, port, rack) in response.Brokers)
        {
            var endpoint = Utils.BuildEndPoint(host, port);
            var newBroker = new Broker(endpoint, nodeId, rack);

            if (response.ControllerId != -1)
            {
                var isController = response.ControllerId == nodeId;

                if (isController)
                {
                    Controller = newBroker;
                }
            }

            await newBroker.OpenAsync(Config, token);
            brokers.Add(newBroker);
        }

        var oldBrokers = _brokers;
        _brokers = brokers;

        foreach (var oldBroker in oldBrokers)
        {
            oldBroker.Close();
        }

        ClusterId = response.ClusterId;

        UpdateTopicPartitions(response.Topics, token);
    }

    /// <summary>
    /// Инициализируем кластер первый раз
    /// </summary>
    internal async Task InitializationAsync(CancellationToken token)
    {
        if (!Closed)
        {
            return;
        }

        Closed = false;

        foreach (var broker in Brokers)
        {
            await broker.OpenAsync(Config, token);
        }

        if (Config.FullUpdateMetadata)
        {
            await UpdateMetadata(token);
        }
    }

    private void ThrowIfClusterClosed()
    {
        if (Closed)
        {
            throw new ClusterKafkaException("kafka: tried to use a cluster that was closed");
        }
    }
}