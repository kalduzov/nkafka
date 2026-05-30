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
using NKafka.Protocol;
using NKafka.Resources;
using NKafka.Serialization;

namespace NKafka;

/// <inheritdoc />
internal sealed class KafkaCluster: IKafkaCluster
{
    private readonly IKafkaConnectorPool _connectorPool;
    private readonly ConcurrentDictionary<ulong, IConsumer> _consumers = new();
    private readonly ILogger<KafkaCluster> _logger;
    private readonly ILoggerFactory _loggerFactory;

    private Task _metadataUpdaterTask;

    private readonly ClusterMetadata _clusterMetadata;

    // Минимально поддерживаемая версия кафки 
    private readonly Version _minSupportVersion = new(2, 0, 0, 0);
    private readonly ConcurrentDictionary<TopicPartition, PartitionMetadata> _partitionsMetadata = new();
    private readonly ConcurrentDictionary<string, SortedSet<PartitionMetadata>> _partitionsMetadatas = new();
    private readonly ConcurrentDictionary<string, IReadOnlyList<Partition>> _topicPartitions = new();
    private readonly ConcurrentDictionary<string, IProducer?> _producers = new();
    private IReadOnlyDictionary<int, Node> _nodes;

    private readonly ConcurrentDictionary<string, TopicMetadata> _topics;
    private IAdminClient? _adminClient;
    private volatile int _controllerId = Node.NoNode.Id;
    private volatile int _metadataUpdating;
    private volatile int _metadataUpdatingCounter;

    // Самое большое количество разделов на топик 
    private int _maxPartitionsByTopic = 1;
    private readonly ConcurrentDictionary<Guid, string> _topicsById;

    private readonly SemaphoreSlim _syncMetadataRequest = new(1, 1);
    private readonly ConcurrentDictionary<string, TaskCompletionSource> _inflightTopics = new();

#if NET9_0_OR_GREATER
    private readonly Lock _lockObject = new();
#else
    private readonly object _lockObject = new();
#endif

    private readonly CancellationTokenSource _closeClusterTokenSource = new();

    /// <summary>
    ///     Create a new kafka cluster
    /// </summary>
    internal KafkaCluster(ClusterConfig config,
        ILoggerFactory loggerFactory,
        IKafkaConnectorPool? kafkaConnectorPool = null,
        ClusterMetadata? clusterMetadata = null)
    {
        Closed = true;
        Config = config;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<KafkaCluster>();
        _clusterMetadata = clusterMetadata ?? new ClusterMetadata();
        _metadataUpdaterTask = Task.CompletedTask;
        _topics = new ConcurrentDictionary<string, TopicMetadata>();
        _topicsById = new ConcurrentDictionary<Guid, string>();
        var seedBrokers = SeedBrokers(Config);
        _nodes = new Dictionary<int, Node>(seedBrokers.Count);

        _connectorPool = kafkaConnectorPool
                         ?? new KafkaConnectorPool(
                             seedBrokers,
                             config.Ssl,
                             config.Sasl,
                             CommonConfig.MaxInflightRequests,
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
    public IReadOnlyDictionary<string, TopicMetadata> Topics => _topics;

    /// <inheritdoc />
    public IReadOnlyDictionary<Guid, string> TopicsById => _topicsById;

    /// <inheritdoc />
    public bool Closed { get; private set; }

    /// <inheritdoc />
    public Node Controller => _controllerId == Node.NO_ID ? Node.NoNode : _nodes[_controllerId];

    /// <inheritdoc />
    public IReadOnlyCollection<Node> Brokers { get; private set; } = [];

    private bool HasUsableBrokerTopology => _nodes.Count != 0 && Brokers.Count != 0;

    private bool HasKnownController => _controllerId != Node.NoNode.Id;

    /// <inheritdoc />
    public IAdminClient AdminClient
    {
        get
        {
            ThrowExceptionIfClusterClosed();

            return _adminClient ??= new AdminClient(this, _loggerFactory.CreateLogger<AdminClient>());
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyCollection<Partition>> GetPartitions(string topic, CancellationToken token)
    {
        ThrowExceptionIfClusterClosed();

        TaskCompletionSource? tcs = null;

        try
        {
            if (_topicPartitions.TryGetValue(topic, out var partitions) && partitions.Count != 0)
            {
                return partitions;
            }

            if (_inflightTopics.TryGetValue(topic, out var tcsOld))
            {
                await tcsOld.Task;
            }
            else
            {
                tcs = new TaskCompletionSource();

                _inflightTopics.TryAdd(topic, tcs);

                string[] topics =
                [
                    topic
                ];

                await InternalRefreshMetadataAsync(topics, false, token);
            }

            if (_topicPartitions.TryGetValue(topic, out partitions) && partitions.Count != 0)
            {
                return partitions;
            }

            return [];
        }
        finally
        {
            _inflightTopics.TryRemove(topic, out _);
            tcs?.SetResult();
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyCollection<TopicPartition>> GetTopicPartitions(IReadOnlyCollection<string> topics,
        CancellationToken token)
    {
        // We always fetch the latest data about topics from the cluster 

        await InternalRefreshMetadataAsync(
            topics,
            false,
            token: token);

        var result = new List<TopicPartition>(topics.Count * _maxPartitionsByTopic);

        foreach (var topic in topics)
        {
            if (_topicPartitions.TryGetValue(topic, out var partitions))
            {
                result.AddRange(partitions.Select(p => new TopicPartition(topic, p)));
            }
        }

        return result;
    }

    /// <inheritdoc />
    public ValueTask<Offset> GetOffset(string topic, Partition partition, CancellationToken token)
    {
        return ValueTask.FromResult(Offset.Unset);
    }

    /// <inheritdoc />
    public IProducer BuildProducer(
        string name,
        ProducerConfig producerConfig)
    {
        ThrowExceptionIfClusterClosed();

        if (_producers.TryGetValue(name, out var producer))
        {
            Debug.Assert(producer is not null);

            return producer;
        }

        producerConfig = producerConfig != ProducerConfig.EmptyProducerConfig ? producerConfig.MergeFrom(Config) : ProducerConfig.BaseFrom(Config);

        producer = new Producer(
            this,
            name,
            producerConfig,
            _loggerFactory);

        return _producers.GetOrAdd(name, producer)!;
    }

    /// <inheritdoc />
    public IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(ConsumerConfig consumerConfig,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer)
        where TKey : notnull
        where TValue : notnull
    {
        ThrowExceptionIfClusterClosed();

        consumerConfig = consumerConfig != ConsumerConfig.EmptyConsumerConfig ? consumerConfig.MergeFrom(Config) : ConsumerConfig.BaseFrom(Config);

        var consumer = new Consumer<TKey, TValue>(this,
            consumerConfig,
            keyDeserializer,
            valueDeserializer,
            _loggerFactory);

        return (IConsumer<TKey, TValue>)_consumers.GetOrAdd(consumer.ConsumerInstanceId, consumer);
    }

    /// <inheritdoc />
    public Task RefreshMetadataAsync(IReadOnlyCollection<string> topics, CancellationToken token)
    {
        return InternalRefreshMetadataAsync(topics, false, token);
    }

    /// <inheritdoc />
    public Task OpenAsync(CancellationToken token)
    {
        return OpenInternalAsync(token);
    }

    /// <inheritdoc />
    async Task<TResponseMessage> IKafkaCluster.SendAsync<TRequestMessage, TResponseMessage>(TRequestMessage message, CancellationToken token)
    {
        var retryAttempt = Config.MaxRetries;

        TResponseMessage response;

        do
        {
            response = await SendRequestAsync(message, token);

            switch (response)
            {
                case IResponseMessage { IsSuccessStatusCode: true }:
                    return response;
                case IResponseMessage { IsRetriableCode: true }:
                    continue;
                case IResponseMessage { IsProcessingRequiredClient: true }:
                    return response;
                default:
                    throw new ProtocolKafkaException(response.Code);
            }
        } while (--retryAttempt != 0);

        return response;

        async Task<TResponseMessage> SendRequestAsync(TRequestMessage messageLocal, CancellationToken tokenLocal)
        {
            var messageIsRequiredController = messageLocal.OnlyController;
            var connector = GetConnectorForServiceRequests(messageIsRequiredController);

            return await connector.SendAsync<TRequestMessage, TResponseMessage>(messageLocal, false, tokenLocal);
        }
    }

    /// <inheritdoc />
    Task<TResponseMessage> IKafkaCluster.SendAsync<TRequestMessage, TResponseMessage>(TRequestMessage message, int nodeId, CancellationToken token)
    {
        return GetConnectorForKnownBroker(nodeId)
            .SendAsync<TRequestMessage, TResponseMessage>(message, false, token);
    }

    /// <summary>
    /// Notifies when a consumer is disposed.
    /// </summary>
    /// <param name="consumer">The consumer instance to be disposed.</param>
    public void NotifyAboutDisposedConsumer(IConsumer consumer)
    {
        _consumers.TryRemove(consumer.ConsumerInstanceId, out _);
    }

    /// <summary>
    /// Provides a dedicated Kafka connector for the given node ID.
    /// </summary>
    /// <param name="nodeId">The ID of the Kafka node.</param>
    /// <returns>The dedicated Kafka connector for the specified node ID.</returns>
    /// <exception cref="ClusterKafkaException">Thrown when unable to create a dedicated connection.</exception>
    public IKafkaConnector ProvideDedicatedConnector(int nodeId)
    {
        if (_connectorPool.TryCreateDedicatedConnector(nodeId, out var connector))
        {
            return connector;
        }

        throw new ClusterKafkaException("Невозможно создать выделенное соединение");
    }

    /// <inheritdoc />
    public ClusterMetadata GetClusterMetadata()
    {
        return _clusterMetadata;
    }

    /// <inheritdoc />
    public TopicMetadata GetTopicMetadata(string name)
    {
        return _topics[name];
    }

    /// <summary>
    /// Returns a list of available partitions for a given topic.
    /// Available partitions are those that can currently be accessed from the client.
    /// This means that brokers hosting these partitions are online and can be queried.
    /// </summary>
    /// <param name="topic">The name of the topic.</param>
    /// <remarks>
    /// This method returns the data that was obtained when calling the GetPartitionsAsync, RefreshMetadataAsync methods,
    /// or through background updates of cluster data.
    /// </remarks>
    /// <returns>
    /// A list of available partitions or an empty collection if no such partitions are currently available.
    /// </returns>
    public IReadOnlyList<Partition> GetAvailablePartitions(string topic)
    {
        return _topicPartitions.TryGetValue(topic, out var partitions) ? partitions : Array.Empty<Partition>();

    }

    /// <inheritdoc />
    public Node LeaderFor(TopicPartition topicPartition)
    {
        return !_partitionsMetadata.TryGetValue(topicPartition, out var partitionMetadata)
            ? Node.NoNode
            : _nodes[partitionMetadata.Leader];

    }

    /// <inheritdoc />
    public IReadOnlyCollection<PartitionMetadata> PartitionsForTopic(string topic)
    {
        if (_partitionsMetadatas.TryGetValue(topic, out var partitionMetadatas))
        {
            return partitionMetadatas;
        }

        return [];
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _closeClusterTokenSource.Cancel();
        _metadataUpdaterTask.Dispose();
        _connectorPool.Dispose();
        _closeClusterTokenSource.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _closeClusterTokenSource.CancelAsync();
        await _metadataUpdaterTask;
        await _connectorPool.DisposeAsync();
        _closeClusterTokenSource.Dispose();
    }

    private async Task InternalRefreshMetadataAsync(
        IEnumerable<string>? topics,
        bool skipException,
        CancellationToken token)
    {
        await _syncMetadataRequest.WaitAsync(token);

        var localTopics = topics?.ToArray();

        using var activity = KafkaDiagnosticsSource.RefreshMetadata(localTopics);

        try
        {
            if (!skipException)
            {
                ThrowExceptionIfClusterClosed();
            }

            token.ThrowIfCancellationRequested();

            var kafkaConnector = GetConnectorForServiceRequests();
            await kafkaConnector.OpenAsync(token);
            var request = MetadataRequestMessage.Build(Config.AllowAutoTopicCreation, localTopics);
            var response = await kafkaConnector.SendAsync<MetadataRequestMessage, MetadataResponseMessage>(request, true, token);
            await ProcessMetadataResponse(response, token);
        }
        catch (Exception exc)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exc.Message);

            throw;
        }
        finally
        {
            _syncMetadataRequest.Release();
        }
    }

    private async Task ProcessMetadataResponse(MetadataResponseMessage responseMessage, CancellationToken token)
    {
        _logger.LogTrace("Message {ResponseMessageClusterId}", responseMessage.ClusterId ?? "none");

        ClusterId = responseMessage.ClusterId;
        var nodes = responseMessage.Brokers.ConvertToNodes();

        _logger.LogDebug("Got information about {NodesCount} nodes", nodes.Count);

        await UpdateBrokersAsync(nodes, responseMessage.ControllerId, token);

        _logger.LogDebug("Got information about {TopicsCount} topics", responseMessage.Topics.Count);

        UpdateTopicPartitions(responseMessage.Topics, token);
    }

    /// <summary>
    /// Forms a list of seed brokers.
    /// </summary>
    /// <param name="commonConfig">The common configuration.</param>
    /// <returns>The list of broker nodes.</returns>
    private List<Node> SeedBrokers(CommonConfig commonConfig)
    {
        var brokers = new List<Node>(Config.BootstrapServers.Count);

        foreach (var bootstrapServer in commonConfig.BootstrapServers)
        {
            var (host, port) = Utils.GetHostAndPort(bootstrapServer);
            var broker = new Node(Node.UNKNOWN_ID, host, port);
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

            if (messageTopic.Code != ErrorCodes.None)
            {
                throw new ProtocolKafkaException(messageTopic.Code);
            }

            _topics.TryAdd(messageTopic.Name, new TopicMetadata(messageTopic.Name, messageTopic.TopicId, messageTopic.IsInternal));

            if (messageTopic.TopicId != Guid.Empty)
            {
                _topicsById.AddOrUpdate(messageTopic.TopicId, _ => messageTopic.Name, (_, _) => messageTopic.Name);
            }

            SortedSet<PartitionMetadata> partitionMetadatas = new();

            if (messageTopic.Partitions.Count > _maxPartitionsByTopic)
            {
                _maxPartitionsByTopic = messageTopic.Partitions.Count;
            }

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

                var tp = new TopicPartition(messageTopic.Name, topicPartition.PartitionIndex, messageTopic.TopicId);

                _partitionsMetadata.AddOrUpdate(tp, _ => partitionMetadata, (_, _) => partitionMetadata);
            }

            _topicPartitions.AddOrUpdate(messageTopic.Name, _ => partitions, (_, _) => partitions);
            _partitionsMetadatas.AddOrUpdate(messageTopic.Name, _ => partitionMetadatas, (_, _) => partitionMetadatas);

        }
    }

    private ValueTask UpdateBrokersAsync(
        IReadOnlyDictionary<int, Node> nodes,
        int? controllerId,
        CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        lock (_lockObject) //Т.к. обновления могут идти из разных мест, то требуется блокировка
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

        return _connectorPool.AddOrUpdateConnectorsAsync(Brokers, token);
    }

    /// <summary>
    /// Returns a connector for service requests.
    /// </summary>
    /// <param name="throwExceptionIfNoController">Specifies whether to throw an exception if no controller is available. Default value is false.</param>
    /// <returns>The connector for service requests.</returns>
    /// <remarks>
    /// Service requests are usually made to a controller, or if it is not available, to an arbitrary broker in the cluster.
    /// Additionally, some requests must be made to the controller. If a controller is required for a request, but a different
    /// broker is chosen, the request will fail with an error.
    /// </remarks>
    private IKafkaConnector GetConnectorForServiceRequests(bool throwExceptionIfNoController = false)
    {
        if (throwExceptionIfNoController && !HasKnownController)
        {
            throw new ClusterKafkaException(ExceptionMessages.NoController);
        }

        if (HasKnownController && _connectorPool.TryGetSharedConnector(_controllerId, out var controllerConnector))
        {
            return controllerConnector;
        }

        if (throwExceptionIfNoController)
        {
            throw new ClusterKafkaException(ExceptionMessages.NoConnectionToController);
        }

        return GetConnectorForBootstrapOrAnyBroker();
    }

    private IKafkaConnector GetConnectorForKnownBroker(int nodeId)
    {
        if (_connectorPool.TryGetSharedConnector(nodeId, out var connector))
        {
            return connector;
        }

        throw new ConnectorNotFoundException($"Коннектор для брокера {nodeId} не найден");
    }

    private IKafkaConnector GetConnectorForBootstrapOrAnyBroker()
    {
        if (HasUsableBrokerTopology && _connectorPool.TryGetAnySharedBrokerConnector(out var brokerConnector))
        {
            return brokerConnector;
        }

        if (_connectorPool.TryGetBootstrapConnector(out var bootstrapConnector))
        {
            return bootstrapConnector;
        }

        if (_connectorPool.TryGetAnySharedBrokerConnector(out var fallbackBrokerConnector))
        {
            return fallbackBrokerConnector;
        }

        throw new ConnectorNotFoundException(ExceptionMessages.ConnectorPool_NoAvailableConnections);
    }

    /// <summary>
    /// Periodically updates metadata for the topics that are currently being worked on.
    /// </summary>
    /// <param name="metadataUpdateTimeoutMs"></param>
    private async Task UpdateMetadataTask(int metadataUpdateTimeoutMs)
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(metadataUpdateTimeoutMs));

        try
        {
            while (await periodicTimer.WaitForNextTickAsync(_closeClusterTokenSource.Token))
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
                    await InternalRefreshMetadataAsync(_topics.Keys, false, tokenSource.Token);

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
        }
        catch (OperationCanceledException exc)
        {
            _logger.LogTrace(exc, "UpdateMetadataTask has terminated because cluster operations were completed");
        }
    }

    /// <summary>
    /// Initializes the cluster.
    /// </summary>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task OpenInternalAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (!Closed)
        {
            return;
        }

        if (Config.IsFullUpdateMetadata) // Fetch full cluster details (brokers + topics) immediately
        {
            await InternalRefreshMetadataAsync(topics: null, skipException: true, token: token);
        }
        else // else, we only require broker information
        {
            await InternalRefreshMetadataAsync(topics: _topics.Keys, skipException: true, token: token);
        }

        _metadataUpdaterTask = UpdateMetadataTask(Config.MetadataUpdateTimeoutMs);

        MergeAllVersions();

        Closed = false;
    }

    private void MergeAllVersions()
    {
        var isFirst = true;

        foreach (var connector in _connectorPool.GetOpenedSharedConnectors())
        {
            var added = new HashSet<ApiKeys>();

            foreach (var supportVersion in connector.SupportVersions)
            {
                added.Add(supportVersion.Key);

                if (_clusterMetadata.AggregationApiByVersion.TryGetValue(supportVersion.Key, out var curVersion))
                {
                    var minVersion = Math.Max((short)curVersion.MinVersion, (short)supportVersion.Value.MinVersion);
                    var maxVersion = Math.Min((short)curVersion.MaxVersion, (short)supportVersion.Value.MaxVersion);
                    _clusterMetadata.AggregationApiByVersion[supportVersion.Key] = new ApiMetadata
                    {
                        MinVersion = (ApiVersion)minVersion,
                        MaxVersion = (ApiVersion)maxVersion
                    };
                }
                else
                {
                    if (isFirst)
                    {
                        _clusterMetadata.AggregationApiByVersion[supportVersion.Key] = new ApiMetadata
                        {
                            MinVersion = supportVersion.Value.MinVersion,
                            MaxVersion = supportVersion.Value.MaxVersion
                        };
                    }
                }
            }

            if (!isFirst)
            {
                continue;
            }

            //удаляем ключи, которые не были в поддерживаемых для соединения
            foreach (var key in _clusterMetadata.AggregationApiByVersion.Keys.ToArray())
            {
                if (!added.Contains(key))
                {
                    _clusterMetadata.AggregationApiByVersion.Remove(key);
                }
            }
            isFirst = false;
        }
    }

    private void ThrowExceptionIfClusterClosed()
    {
        if (Closed)
        {
            throw new ClusterKafkaException("Tried to use a cluster that was closed");
        }
    }
}
