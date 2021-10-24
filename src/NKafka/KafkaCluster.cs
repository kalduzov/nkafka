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
using System.Diagnostics;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using NKafka.Clients.Admin;
using NKafka.Clients.Consumer;
using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Connection;
using NKafka.Diagnostics;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Resources;
using NKafka.Serialization;

namespace NKafka;

/// <inheritdoc />
internal sealed class KafkaCluster: IKafkaCluster
{
    private readonly IKafkaConnectorPool _connectorPool;
    private readonly ConcurrentDictionary<string, IConsumer?> _consumers = new();
    private readonly ILogger<KafkaCluster> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Timer _metadataUpdaterTimer;

    //Минимально поддерживаемая версия кафки 
    private readonly Version _minSupportVersion = new(1, 0, 0, 0);
    private readonly ConcurrentDictionary<TopicPartition, PartitionMetadata> _partitionsMetadata = new();
    private readonly ConcurrentDictionary<string, SortedSet<PartitionMetadata>> _partitionsMetadatas = new();
    private readonly ConcurrentDictionary<string, IReadOnlyList<Partition>> _topicPartitions = new();
    private readonly ConcurrentDictionary<string, IProducer?> _producers = new();
    private IReadOnlyDictionary<int, Node> _nodes;

    private readonly HashSet<string> _topics;
    private IAdminClient? _adminClient;
    private volatile int _controllerId = Node.NoNode.Id;
    private volatile int _metadataUpdating;
    private volatile int _metadataUpdatingCounter;

    /// <summary>
    ///     Create a new kafka cluster
    /// </summary>
    internal KafkaCluster(ClusterConfig config, ILoggerFactory loggerFactory, IKafkaConnectorPool? kafkaConnectorPool = null)
    {
        Closed = true;
        Config = config;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<KafkaCluster>();
        _metadataUpdaterTimer = new Timer(UpdateMetadataCallback, null, Timeout.Infinite, Timeout.Infinite);
        _topics = new HashSet<string>();
        var seedBrokers = SeedBrokers(Config);
        _nodes = new Dictionary<int, Node>(seedBrokers.Count);

        _connectorPool = kafkaConnectorPool
                         ?? new KafkaConnectorPool(
                             seedBrokers,
                             config.Ssl,
                             config.Sasl,
                             config.MaxInflightRequests,
                             config.MessageMaxBytes,
                             config.CloseConnectionTimeoutMs,
                             config.ConnectionsMaxIdleMs,
                             config.RequestTimeoutMs,
                             config.ReceiveBufferBytes,
                             config.SecurityProtocol,
                             config.ClientId,
                             config.ApiVersionRequest,
                             loggerFactory);
    }

    /// <inheritdoc />
    public string? ClusterId { get; private set; }

    /// <inheritdoc />
    public ClusterConfig Config { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Topics => _topics;

    /// <inheritdoc />
    public bool Closed { get; private set; }

    /// <inheritdoc />
    public Node Controller => _controllerId == Node.NoNode.Id ? Node.NoNode : _nodes[_controllerId];

    /// <inheritdoc />
    public IReadOnlyCollection<Node> Brokers { get; private set; } = Array.Empty<Node>();

    public IAdminClient AdminClient
    {
        get
        {
            ThrowExceptionIfClusterClosed();

            return _adminClient ??= new AdminClient(this, _loggerFactory.CreateLogger<AdminClient>());
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyCollection<Partition>> GetPartitionsAsync(string topic, CancellationToken token = default)
    {
        ThrowExceptionIfClusterClosed();

        if (_topicPartitions.TryGetValue(topic, out var partitions) && partitions.Count != 0)
        {
            return partitions;
        }

        await InternalRefreshMetadataAsync(
                new[]
                {
                    topic
                },
                token: token)
            .ConfigureAwait(false);

        if (_topicPartitions.TryGetValue(topic, out partitions) && partitions.Count != 0)
        {
            return partitions;
        }

        return Array.Empty<Partition>();
    }

    /// <inheritdoc />
    public ValueTask<Offset> GetOffsetAsync(string topic, Partition partition, CancellationToken token = default)
    {
        return ValueTask.FromResult(Offset.Unset);
    }

    /// <inheritdoc />
    public IProducer<TKey, TValue> BuildProducer<TKey, TValue>(
        string name,
        ProducerConfig producerConfig,
        IAsyncSerializer<TKey> keySerializer,
        IAsyncSerializer<TValue> valueSerializer)
        where TKey : notnull
        where TValue : notnull
    {
        ThrowExceptionIfClusterClosed();

        if (_producers.TryGetValue(name, out var producer))
        {
            Debug.Assert(producer is not null);

            return (IProducer<TKey, TValue>)producer;
        }

        producerConfig = producerConfig != ProducerConfig.EmptyProducerConfig ? producerConfig.MergeFrom(Config) : ProducerConfig.BaseFrom(Config);

        producer = new Producer<TKey, TValue>(
            this,
            name,
            producerConfig,
            keySerializer,
            valueSerializer,
            _loggerFactory);

        return (IProducer<TKey, TValue>)_producers.GetOrAdd(name, producer)!;
    }

    /// <inheritdoc />
    public IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(
        string consumeGroupName,
        ConsumerConfig? consumerConfig = null,
        IAsyncSerializer<TKey>? keySerializer = null,
        IAsyncSerializer<TValue>? valueSerializer = null)
        where TKey : notnull
        where TValue : notnull
    {
        ThrowExceptionIfClusterClosed();

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task RefreshMetadataAsync(CancellationToken token = default, IEnumerable<string>? topics = null)
    {
        return InternalRefreshMetadataAsync(topics, token: token);
    }

    /// <summary>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task OpenAsync(CancellationToken token)
    {
        return OpenInternalAsync(token);
    }

    /// <inheritdoc />
    async Task<TResponseMessage> IKafkaCluster.SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
    {
        var messageIsRequiredController = message.OnlyController;
        var connector = GetConnectorForServiceRequests(messageIsRequiredController);

        return await connector.SendAsync<TResponseMessage, TRequestMessage>(message, false, token);
    }

    /// <inheritdoc />
    Task<TResponseMessage> IKafkaCluster.SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, int nodeId, CancellationToken token)
    {
        if (_connectorPool.TryGetConnector(nodeId, out var connector))
        {
            return connector.SendAsync<TResponseMessage, TRequestMessage>(message, false, token);
        }

        throw new ConnectorNotFoundException($"Коннектор для брокера {nodeId} не найден");
    }

    /// <summary>
    ///     Возвращает список доступных разделов для топика
    ///     Доступные разделов - это те, к которым сейчас можно обратиться из клиента.
    ///     Т.е. брокеры, на которых находятся данные разделы, в сети и к ним можно сделать запрос.
    /// </summary>
    /// <param name="topic">Имя топика</param>
    /// <remarks>
    ///     Данный метод возвращает данные, который были получены при вызове методов GetPartitionsAsync, RefreshMetadataAsync
    ///     или фоновым обновление данных по кластеру
    /// </remarks>
    /// <returns>
    ///     Список доступных разделов или пустую коллекцию, если такие разделы пока не доступны
    /// </returns>
    public IReadOnlyList<Partition> GetAvailablePartitions(string topic)
    {
        return _topicPartitions.TryGetValue(topic, out var partitions) ? partitions : Array.Empty<Partition>();

    }

    /// <summary>
    /// Возвращает лидера для указннаго раздела в топике
    /// </summary>
    public Node? LeaderFor(TopicPartition topicPartition)
    {
        return !_partitionsMetadata.TryGetValue(topicPartition, out var partitionMetadata)
            ? null
            : _nodes[partitionMetadata.Leader];

    }

    /// <summary>
    /// Возвращает информацию о разделах для указанного топика
    /// </summary>
    public IReadOnlyCollection<PartitionMetadata> PartitionsForTopic(string topic)
    {
        if (_partitionsMetadatas.TryGetValue(topic, out var partitionMetadatas))
        {
            return partitionMetadatas;
        }

        return Array.Empty<PartitionMetadata>();
    }

    public void Dispose()
    {
        _metadataUpdaterTimer.Dispose();
        _connectorPool.Dispose();
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
    ///     asynchronously.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous dispose operation.
    /// </returns>
    public async ValueTask DisposeAsync()
    {
        await _metadataUpdaterTimer.DisposeAsync();
        await _connectorPool.DisposeAsync();
    }

    private async Task InternalRefreshMetadataAsync(
        IEnumerable<string>? topics = null,
        bool skipException = false,
        bool isFirstRequest = false,
        CancellationToken token = default)
    {
        if (!skipException)
        {
            ThrowExceptionIfClusterClosed();
        }

        token.ThrowIfCancellationRequested();

        var kafkaConnector = GetConnectorForServiceRequests(); //Обновляем метаданные из брокера, который является контроллером

        await kafkaConnector.OpenAsync(token);

        var request = new MetadataRequestMessage();

        if (topics is not null)
        {
            foreach (var topic in topics)
            {
                token.ThrowIfCancellationRequested();

                request.Topics.Add(
                    new MetadataRequestMessage.MetadataRequestTopicMessage
                    {
                        Name = topic
                    });
            }
        }
        else
        {
            request.Topics = null!; //для версии 1+ это указывает на то, что нужно все топики получить
        }

        var responseMessage = await kafkaConnector.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(request, true, token);

        Debug.WriteLine($"Message {responseMessage.ClusterId ?? "none"}");

        ClusterId = responseMessage.ClusterId;
        var nodes = responseMessage.Brokers.ConvertToNodes();

        Debug.WriteLine($"Got information about {nodes.Count} nodes");

        await UpdateBrokersAsync(nodes, responseMessage.ControllerId, token);

        Debug.WriteLine($"Got information about {responseMessage.Topics.Count} topics");

        UpdateTopicPartitions(responseMessage.Topics, token);
    }

    /// <summary>
    ///     Формирует список посевных брокеров
    /// </summary>
    private List<Node> SeedBrokers(CommonConfig commonConfig)
    {
        var brokers = new List<Node>(Config.BootstrapServers.Count);

        foreach (var bootstrapServer in commonConfig.BootstrapServers)
        {
            var (host, port) = Utils.GetHostAndPort(bootstrapServer);
            var broker = new Node(-1, host, port);
            brokers.Add(broker);
        }

        return brokers;
    }

    private void UpdateTopicPartitions(IEnumerable<MetadataResponseMessage.MetadataResponseTopicMessage> messageTopics, CancellationToken token)
    {
        foreach (var messageTopic in messageTopics)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            _topics.Add(messageTopic.Name);

            SortedSet<PartitionMetadata> partitionMetadatas = new();
            var partitions = new List<Partition>(messageTopic.Partitions.Count);

            foreach (var topicPartition in messageTopic.Partitions)
            {
                var partitionMetadata = new PartitionMetadata(topicPartition.PartitionIndex,
                    topicPartition.LeaderId,
                    topicPartition.LeaderEpoch,
                    topicPartition.ReplicaNodes,
                    topicPartition.IsrNodes,
                    topicPartition.OfflineReplicas);

                partitionMetadatas.Add(partitionMetadata);
                partitions.Add(topicPartition.PartitionIndex);

                var tp = new TopicPartition(messageTopic.Name, topicPartition.PartitionIndex);

                _partitionsMetadata.AddOrUpdate(tp, _ => partitionMetadata, (_, _) => partitionMetadata);
            }

            _topicPartitions.AddOrUpdate(messageTopic.Name, _ => partitions, (_, _) => partitions);
            _partitionsMetadatas.AddOrUpdate(messageTopic.Name, _ => partitionMetadatas, (_, _) => partitionMetadatas);

        }
    }

    private async ValueTask UpdateBrokersAsync(
        IReadOnlyDictionary<int, Node> nodes,
        int? controllerId,
        CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        lock (_nodes) //Т.к. обновления могут идти из разных мест, то требуется блокировка
        {
            _nodes = nodes;
            Brokers = _nodes.Values.ToArray();

            foreach (var node in nodes)
            {
                token.ThrowIfCancellationRequested();

                if (node.Key == controllerId)
                {
                    _controllerId = controllerId.Value;
                }
            }
        }

        await _connectorPool.AddOrUpdateConnectorsAsync(Brokers, token).ConfigureAwait(false);
    }

    /// <summary>
    ///     Возвращает брокера для сервисных запросов
    /// </summary>
    /// <remarks>
    ///     Сервисные запросы обычно делаются на контроллер, либо, если он отсутствует, на произвольный брокер кластера.
    ///     Кроме того, часть запросов должна обязательно делаться на контроллере. В случае, если для запроса требуется контроллер,
    ///     а был выбран отличный от него брокер, то такой запрос упадет с ошибкой
    /// </remarks>
    private IKafkaConnector GetConnectorForServiceRequests(bool throwExceptionIfNoController = false)
    {
        if (_controllerId == -1 && throwExceptionIfNoController)
        {
            throw new ClusterKafkaException(ExceptionMessages.NoController);
        }

        if (_connectorPool.TryGetConnector(_controllerId, out var controllerConnector))
        {
            return controllerConnector;
        }

        if (throwExceptionIfNoController)
        {
            throw new ClusterKafkaException(ExceptionMessages.NoConnectionToController);
        }

        return _connectorPool.GetRandomConnector();
    }

    /// <summary>
    ///     Периодически обновляет метаданные по сохраненным топикам
    /// </summary>
    private async void UpdateMetadataCallback(object? state)
    {
        ThrowExceptionIfClusterClosed();

        using var activity = KafkaDiagnosticsSource.UpdateMetadataActivity();

        var counter = Interlocked.Increment(ref _metadataUpdatingCounter);

        var metadataUpdating = Interlocked.CompareExchange(ref _metadataUpdating, 1, 0);

        if (metadataUpdating == _metadataUpdating)
        {
            _logger.WarningMetadataMaxAge(Config.MetadataUpdateTimeoutMs);

            return;
        }

        var stopWatch = Stopwatch.StartNew();

        _logger.UpdateMetadataStart(counter);

        using var tokenSource = new CancellationTokenSource();

        try
        {
            tokenSource.CancelAfter(Config.RequestTimeoutMs);
            await InternalRefreshMetadataAsync(_topics, token: tokenSource.Token);

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                activity?.AddEvent(
                    new ActivityEvent(
                        "Metadata updated",
                        DateTimeOffset.UtcNow,
                        new ActivityTagsCollection
                        {
                            {
                                "brokers", JsonSerializer.Serialize(_nodes.Values)
                            },
                            {
                                "topics_partitions", JsonSerializer.Serialize(_partitionsMetadata)
                            }
                        }));
            }
        }
        catch (Exception exc)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exc.Message);
            _logger.UpdateMetadataError(exc, counter);
        }
        finally
        {
            _logger.UpdateMetadataEnd(counter, stopWatch.Elapsed);
            Interlocked.CompareExchange(ref _metadataUpdating, 0, 1);
        }
    }

    /// <summary>
    ///     Инициализируем кластер
    /// </summary>
    private async Task OpenInternalAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (!Closed)
        {
            return;
        }

        if (Config.IsFullUpdateMetadata)
        {
            await InternalRefreshMetadataAsync(skipException: true, isFirstRequest: true, token: token);
        }

        _metadataUpdaterTimer.Change(Config.MetadataUpdateTimeoutMs, Config.MetadataUpdateTimeoutMs);

        Closed = false;
    }

    private void ThrowExceptionIfClusterClosed()
    {
        if (Closed)
        {
            throw new ClusterKafkaException("Tried to use a cluster that was closed");
        }
    }
}