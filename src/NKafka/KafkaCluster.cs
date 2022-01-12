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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using NKafka.Clients.Consumer;
using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Connection;
using NKafka.Diagnostics;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Serialization;

namespace NKafka;

/// <inheritdoc />
public sealed class KafkaCluster: IKafkaCluster
{
    internal const string DEFAULT_PRODUCER_NAME = "__DefaultProducer__";

    private readonly ConcurrentDictionary<string, IConsumer?> _consumers = new();
    private readonly ConcurrentDictionary<string, IProducer?> _producers = new();

    private readonly object _lockObject = new();
    private readonly ILogger<KafkaCluster> _logger;
    private readonly ILoggerFactory _loggerFactory;

    //Минимально поддерживаемая версия кафки 
    private readonly Version _minSupportVersion = new(1, 0, 0, 0);

    #region Metadata update

    private readonly Timer _metadataUpdaterTimer;
    private volatile int _metadataUpdating;
    private volatile int _metadataUpdatingCounter;

    #endregion

    private readonly HashSet<string> _topics;

    //private volatile int _brokerIndex = 0;
    private readonly List<IBroker> _seedBrokers;
    private readonly Dictionary<int, IBroker> _brokers = new();
    private int _controllerId = -1;

    private readonly Dictionary<string, SortedSet<Partition>> _partitions = new();

    /// <inheritdoc />
    public string? ClusterId { get; private set; }

    /// <inheritdoc />
    public ClusterConfig Config { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Topics => _topics;

    /// <inheritdoc />
    public bool Closed { get; private set; }

    /// <inheritdoc />
    public IBroker? Controller => _controllerId == -1 ? null : _brokers[_controllerId];

    /// <inheritdoc />
    public IReadOnlyCollection<IBroker> Brokers => _brokers.Values;

    /// <summary>
    /// Создает новый класс кластера кафки
    /// </summary>
    internal KafkaCluster(ClusterConfig config, ILoggerFactory loggerFactory)
    {
        Closed = true;
        Config = config;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<KafkaCluster>();
        _metadataUpdaterTimer = new Timer(UpdateMetadataCallback, null, Timeout.Infinite, Timeout.Infinite);
        _topics = new HashSet<string>();
        _seedBrokers = SeedBrokers(Config);
    }

    /// <summary>
    /// ctor only for testing
    /// </summary>
    /// <remarks>ignored brokers from Config.BootstrapServers</remarks>
    internal KafkaCluster(ClusterConfig config, ILoggerFactory loggerFactory, List<IBroker> brokers)
        : this(config, loggerFactory)
    {
        _seedBrokers = brokers;
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyCollection<Partition>> GetPartitionsAsync(string topic, CancellationToken token = default)
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

        return Array.Empty<Partition>();
    }

    /// <inheritdoc />
    public ValueTask<Offset> GetOffsetAsync(string topic, Partition partition, CancellationToken token = default)
    {
        return ValueTask.FromResult(Offset.Unset);
    }

    /// <inheritdoc />
    public IProducer<TKey, TValue> BuildProducer<TKey, TValue>(
        string name = DEFAULT_PRODUCER_NAME,
        ProducerConfig? producerConfig = null,
        IAsyncSerializer<TKey>? keySerializer = null,
        IAsyncSerializer<TValue>? valueSerializer = null)
    {
        ThrowIfClusterClosed();

        if (_producers.TryGetValue(name, out var producer))
        {
            Debug.Assert(producer is not null);

            return (IProducer<TKey, TValue>)producer;
        }

        producerConfig = producerConfig != null ? producerConfig.MergeFrom(Config) : ProducerConfig.BaseFrom(Config);

        producer = new Producer<TKey, TValue>(
            kafkaCluster: this,
            name: name,
            config: producerConfig,
            keySerializer: keySerializer,
            valueSerializer: valueSerializer,
            interceptors: null!,
            _loggerFactory.CreateLogger(name));

        return (IProducer<TKey, TValue>)_producers.GetOrAdd(name, producer)!;
    }

    /// <inheritdoc />
    public IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(
        string consumeGroupName,
        ConsumerConfig? consumerConfig = null,
        IAsyncSerializer<TKey>? keySerializer = null,
        IAsyncSerializer<TValue>? valueSerializer = null)
    {
        ThrowIfClusterClosed();

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<MetadataResponseMessage> RefreshMetadataAsync(CancellationToken token = default, params string[] topics)
    {
        ThrowIfClusterClosed();

        var broker = GetControllerBroker();

        var request = new MetadataRequestMessage
        {
            AllowAutoTopicCreation = true,
        };

        var message = await broker.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(request, token);

        Debug.WriteLine($"Message {message.ClusterId ?? "none"}");

        _controllerId = message.ControllerId ?? _controllerId;

        await UpdateBrokersAndReturnController(message.Brokers, message.ControllerId, token);
        UpdateTopicPartitions(message.Topics, token);

        return message;
    }

    public void Dispose()
    {
        _metadataUpdaterTimer.Dispose();
        Controller?.Dispose();
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
    ///     asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _metadataUpdaterTimer.DisposeAsync();

        if (Controller is not null)
        {
            await Controller.DisposeAsync();
        }
    }

    private List<IBroker> SeedBrokers(CommonConfig commonConfig)
    {
        var brokers = new List<IBroker>(Config.BootstrapServers.Count);

        foreach (var bootstrapServer in commonConfig.BootstrapServers)
        {
            var endpoint = Utils.BuildBrokerEndPoint(bootstrapServer);
            var broker = new Broker(commonConfig, _loggerFactory, endpoint);
            brokers.Add(broker);
        }

        return brokers;
    }

    private void UpdateTopicPartitions(IEnumerable<MetadataResponseMessage.MetadataResponseTopicMessage> messageTopics, CancellationToken token)
    {
        var topicsByBrokers = new Dictionary<int, IReadOnlyDictionary<string, TopicPartition>>();

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
                var partition = new Partition(topicPartition.PartitionIndex);
                partitions.Add(partition);

                if (_brokers.TryGetValue(topicPartition.LeaderId, out var broker))
                {
                    broker.UpdateTopicsAndPartitions(messageTopic.Name, partition);
                }
            }
        }
    }

    private async ValueTask UpdateBrokersAndReturnController(
        IEnumerable<MetadataResponseMessage.MetadataResponseBrokerMessage> incomingBrokers,
        int? controllerId,
        CancellationToken token)
    {
        var actualBrokersIds = new HashSet<int>();

        lock (_brokers)
        {
            foreach (var responseBrokerMessage in incomingBrokers)
            {
                token.ThrowIfCancellationRequested();

                var nodeId = responseBrokerMessage.NodeId;
                var host = responseBrokerMessage.Host;
                var port = responseBrokerMessage.Port;
                var rack = responseBrokerMessage.Rack;

                var endpoint = Utils.BuildEndPoint(host, port);
                var isController = controllerId != -1 && controllerId == nodeId;

                if (_brokers.TryGetValue(nodeId, out var broker))
                {
                    broker.UpdateInfo(endpoint, rack, isController);
                }
                else
                {
                    var newBroker = new Broker(Config, _loggerFactory, endpoint, nodeId, rack, isController);
                    _brokers.Add(newBroker.Id, newBroker);
                }

                actualBrokersIds.Add(nodeId);
            }
        }

        foreach (var b in _brokers.Values.ToImmutableArray())
        {
            if (actualBrokersIds.Contains(b.Id))
            {
                continue;
            }

            _brokers.Remove(b.Id, out var oldBroker);
            await oldBroker!.DisposeAsync(); //Ждем уничтожения соединений со всеми "старыми" брокерами
        }
    }

    /// <summary>
    /// Возвращает наименее нагруженный запросами брокер
    /// </summary>
    private IBroker GetControllerBroker()
    {
        return Controller ?? _seedBrokers.Shuffle().First();
    }

    /// <summary>
    /// Периодически обновляет метаданные по сохраненным топикам 
    /// </summary>
    private async void UpdateMetadataCallback(object? state)
    {
        ThrowIfClusterClosed();

        using var activity = KafkaDiagnosticsSource.UpdateMetadata();

        var counter = Interlocked.Increment(ref _metadataUpdatingCounter);

        var metadataUpdating = Interlocked.CompareExchange(ref _metadataUpdating, 1, 0);

        if (metadataUpdating == _metadataUpdating)
        {
            _logger.WarningMetadataMaxAge(Config.MetadataMaxAge);

            return;
        }

        var stopWatch = Stopwatch.StartNew();

        _logger.UpdateMetadataStart(counter);

        using var tokenSource = new CancellationTokenSource();

        try
        {
            tokenSource.CancelAfter(Config.RequestTimeoutMs);
            await UpdateMetadataAsync(tokenSource.Token);

            activity?.AddBaggage("test", "test");

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                activity?.AddEvent(
                    new ActivityEvent(
                        "Metadata updated",
                        DateTimeOffset.UtcNow,
                        new ActivityTagsCollection
                        {
                            {
                                "brokers", JsonSerializer.Serialize(_brokers.Values)
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

    private async Task UpdateMetadataAsync(CancellationToken token)
    {
        var broker = GetControllerBroker();

        var request = new MetadataRequestMessage
        {
            Version = ApiVersions.Version2,
            Topics = _topics.Select(
                    t => new MetadataRequestMessage.MetadataRequestTopicMessage
                    {
                        Name = t
                    })
                .ToArray()
        };

        var response = await broker.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(request, token);

        _controllerId = response.ControllerId ?? _controllerId;

        await UpdateBrokersAndReturnController(response.Brokers, response.ControllerId, token);

        ClusterId = response.ClusterId;

        //UpdateTopicPartitions(response.Topics, token);
    }

    /// <summary>
    /// Инициализируем кластер
    /// </summary>
    internal async Task InitializationAsync(CancellationToken token)
    {
        if (!Closed)
        {
            return;
        }

        Closed = false;

        if (Config.FullUpdateMetadata)
        {
            await UpdateMetadataAsync(token);
        }

        _metadataUpdaterTimer.Change(Config.MetadataMaxAge, Config.MetadataMaxAge);
    }

    private void ThrowIfClusterClosed()
    {
        if (Closed)
        {
            throw new ClusterKafkaException("Tried to use a cluster that was closed");
        }
    }
}