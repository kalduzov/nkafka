using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Clients.Producer;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Exceptions;
using Microlibs.Kafka.Protocol.Connection;
using Microlibs.Kafka.Protocol.RequestsMessages;
using Microlibs.Kafka.Protocol.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microlibs.Kafka;

/// <summary>
/// </summary>

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class KafkaCluster : IKafkaCluster
{
    private readonly IBrokerPoolManager _connectionPoolManager;
    private readonly Dictionary<string, IConsumer?> _consumers = new();
    private readonly object _lockObject = new();
    private readonly ILogger<KafkaCluster> _logger;

    private readonly ILoggerFactory _loggerFactory;

    //Минимально поддерживаемая версия api кафки 
    private readonly Version _minSupportVersion = new(0, 10, 0, 0);
    private readonly Dictionary<string, IProducer?> _producers = new();
    private readonly Timer _timer;
    private volatile int _metadataUpdating;
    private string[] _topics;

    private KafkaCluster(ClusterConfig config, ILoggerFactory loggerFactory)
    {
        Config = config;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<KafkaCluster>();
        _connectionPoolManager = new DefaultBrokerPoolManager(config);

        _timer = new Timer(UpdateMetadataCallback);
        _timer.Change(0, Timeout.Infinite); //Таймер создаем, но до открытия таймера ничего не делаем
    }

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
    public IBroker Controller => _connectionPoolManager.GetController();

    /// <summary>
    ///     Список всех брокеров кластера
    /// </summary>
    public IReadOnlyCollection<IBroker> Brokers => _connectionPoolManager.GetBrokers();

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

        return null;
    }

    /// <summary>
    ///     Обновляет метаданные по указанным топикам
    /// </summary>
    /// <param name="topics">Список топиков, по которым необходимо получить информацию из брокеров</param>
    public Task RefreshMetadataAsync(params string[] topics)
    {
        ThrowIfClusterClosed();

        return null;
    }

    public void Dispose()
    {
        _timer.Dispose();
        _connectionPoolManager.Dispose();
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
    ///     asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        return default;
    }

    private async void UpdateMetadataCallback(object state)
    {
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
        var broker = _connectionPoolManager.GetLeastLoadedBroker();
        var request = new MetadataRequestMessage(_topics);

        var response = await broker.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(request, token);

        foreach (var (nodeId, host, port, rack) in response.Brokers)
        {
            var endpoint = new BrokerEndpoint(host, port);
            var newBroker = new Broker(endpoint, nodeId, rack);
            var isController = response.ControllerId == nodeId;

            var _ = await _connectionPoolManager.TryAddBrokerAsync(newBroker, isController, false, token);
        }

        // foreach (var VARIABLE in response.)
        // {
        //     
        // }
    }

    /// <summary>
    ///     Инициализирует подсистему для нового кафка кластера.
    /// </summary>
    /// <param name="config">Конфигурация</param>
    /// <param name="loggerFactory">Экземпляр фабрики логирования</param>
    /// <exception cref="ProtocolKafkaException">Не удалось инициализировать кластер</exception>
    /// <exception cref="ClusterKafkaException"></exception>
    public static IKafkaCluster Create(ClusterConfig config, ILoggerFactory? loggerFactory = null)
    {
        return CreateAsync(config, loggerFactory).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Инициализирует подсистему для нового кафка кластера.
    /// </summary>
    /// <param name="config">Конфигурация</param>
    /// <param name="loggerFactory">Экземпляр фабрики логирования, может быть null</param>
    /// <exception cref="ClusterKafkaException">Не удалось инициализировать кластер</exception>
    public static async Task<IKafkaCluster> CreateAsync(ClusterConfig config, ILoggerFactory? loggerFactory = null)
    {
        config.Validate();

        var cluster = new KafkaCluster(config, loggerFactory ?? NullLoggerFactory.Instance);

        var cts = new CancellationTokenSource(config.ClusterInitTimeoutMs);

        try
        {
            await cluster.OpenAsync(cts.Token);

            return cluster;
        }
        catch (OperationCanceledException exc)
        {
            if (cts.IsCancellationRequested)
            {
                throw new ClusterKafkaException($"Не удалось инциализировать кластер за выделенное время {config.ClusterInitTimeoutMs}", exc);
            }

            throw;
        }
        finally
        {
            await cluster.DisposeAsync();
            cts.Dispose();
        }
    }

    private async Task OpenAsync(CancellationToken token)
    {
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