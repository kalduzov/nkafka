using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Clients.Producer;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Exceptions;
using Microlibs.Kafka.Protocol;
using Microlibs.Kafka.Protocol.Connection;
using Microlibs.Kafka.Protocol.RequestsMessages;
using Microlibs.Kafka.Protocol.Responses;
using Microsoft.Extensions.Logging;

namespace Microlibs.Kafka;

/// <summary>
/// </summary>

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class KafkaCluster : IKafkaCluster
{
    private readonly BrokerConnectionPool _brokerConnectionPool;
    private readonly Dictionary<string, IConsumer?> _consumers = new();
    private readonly object _lockObject = new();
    private readonly ILogger<KafkaCluster> _logger;

    private readonly ILoggerFactory _loggerFactory;

    //Минимально поддерживаемая версия api кафки 
    private readonly Version _minSupportVersion = new(0, 10, 0, 0);
    private readonly Dictionary<string, IProducer?> _producers = new();
    private readonly Timer _metadataUpdaterTimer;
    private volatile int _metadataUpdating;
    private string[] _topics;

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
    public IBroker Controller => _brokerConnectionPool.GetController();

    /// <summary>
    ///     Список всех брокеров кластера
    /// </summary>
    public IReadOnlyCollection<IBroker> Brokers => _brokerConnectionPool.GetBrokers();

    /// <summary>
    /// Создает новый класс описывающий конкретный кластер
    /// </summary>
    internal KafkaCluster(ClusterConfig config, ILoggerFactory loggerFactory)
    {
        Closed = true;
        Config = config;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<KafkaCluster>();
        _brokerConnectionPool = new BrokerConnectionPool(config);
        _metadataUpdaterTimer = new Timer(UpdateMetadataCallback, null, Timeout.Infinite, Timeout.Infinite);
        _topics = Array.Empty<string>();
    }

    /// <summary>
    ///     Создает нового продюсера для работы с кластером или возвращает уже существующий
    /// </summary>
    /// <param name="name"></param>
    /// <param name="producerConfig">Конфигурация продюсера</param>
    /// <remarks>Библиотека </remarks>
    public IProducer<TKey, TValue> BuildProducer<TKey, TValue>(string name, ProducerConfig? producerConfig = null)
    {
        ThrowIfClusterClosed();

        lock (_lockObject)
        {
            if (_producers.TryGetValue(name, out var producer))
            {
                return (IProducer<TKey, TValue>)producer!;
            }

            producerConfig = producerConfig != null ? producerConfig.Merge(Config) : ProducerConfig.BaseFrom(Config);

            producer = new Producer<TKey, TValue>(
                this,
                name,
                producerConfig,
                null!,
                null,
                null!,
                DateTime.Today.TimeOfDay,
                _loggerFactory);

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
    public async Task RefreshMetadataAsync(CancellationToken token, params string[] topics)
    {
        ThrowIfClusterClosed();

        var broker = _brokerConnectionPool.GetLeastLoadedBroker();

        var request = new MetadataRequestMessage(ApiVersions.Version4, topics)
        {
            AllowAutoTopicCreation = true,
        };

        var response = await broker.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(request, token);
    }

    public void Dispose()
    {
        _metadataUpdaterTimer.Dispose();
        _brokerConnectionPool.Dispose();
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
    ///     asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _metadataUpdaterTimer.DisposeAsync();
        await _brokerConnectionPool.DisposeAsync();
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
        var broker = _brokerConnectionPool.GetLeastLoadedBroker();

        var request = new MetadataRequestMessage(ApiVersions.Version0, _topics)
        {
            AllowAutoTopicCreation = true,
        };

        var response = await broker.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(request, token);

        foreach (var (nodeId, host, port, rack) in response.Brokers)
        {
            var endpoint = Utils.BuildEndPoint(host, port);
            var newBroker = new Broker(endpoint, nodeId, rack);
            var isController = response.ControllerId == nodeId;

            var _ = await _brokerConnectionPool.TryAddBrokerAsync(newBroker, isController, false, token);
        }

        // foreach (var VARIABLE in response.)
        // {
        //     
        // }
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

        await UpdateMetadata(token);
    }

    private void ThrowIfClusterClosed()
    {
        if (Closed)
        {
            throw new ClusterKafkaException("kafka: tried to use a cluster that was closed");
        }
    }
}