using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Clients.Producer.Internals;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Exceptions;
using Microlibs.Kafka.Protocol;
using Microlibs.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microlibs.Kafka.Clients.Producer;

public sealed partial class Producer<TKey, TValue> : Client, IProducer<TKey, TValue>
{
    private const string _NETWORK_THREAD_PREFIX = "kafka-producer-thread";
    private const string _DEFAULT_PRODUCER_NAME = "__DefaultProducer";

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
    private readonly ApiVersions _apiVersions;
    private readonly ProducerConfig _config;

    private readonly Thread _idThread;
    private readonly IReadOnlyCollection<ProducerInterceptor<TKey, TValue>> _interceptors;

    private readonly KafkaCluster _kafkaCluster;
    private readonly ILogger _logger;
    private readonly ProducerMetadata _metadata;

    private readonly string _name;

    private readonly IPartitioner _partitioner;
    private readonly ISender _sender;
    private readonly TimeSpan _time;

    private readonly CancellationTokenSource _tokenSource = new();
    private readonly TransactionManager _transactionManager;
    private bool _enableDeliveryReports;
    private ISerializer<TKey> _keySerializer;
    private ISerializer<TValue> _valueSerializer;

    /// <summary>
    ///     A producer is instantiated by providing a set of key-value pairs as configuration.
    ///     Valid configuration strings are documented <see cref="http://must_be_link_to_docs" />
    /// </summary>
    /// <param name="config">The producer configs</param>
    /// <param name="kafkaCluster"></param>
    /// <param name="keySerializer">The serializer for key that implements <see cref="ISerializer{T}" /></param>
    /// <param name="valueSerializer">The serializer for value that implements <see cref="ISerializer{T}" /></param>
    /// ///
    /// <remarks> After creating a <see cref="Producer" /> you must always use Dispose() it to avoid resource leaks</remarks>
    public Producer(
        KafkaCluster kafkaCluster,
        ProducerConfig config,
        ISerializer<TKey> keySerializer = null!,
        ISerializer<TValue> valueSerializer = null!)
        : this(
            kafkaCluster,
            _DEFAULT_PRODUCER_NAME,
            config,
            keySerializer,
            valueSerializer,
            null,
            null!,
            DateTime.Today.TimeOfDay,
            NullLoggerFactory.Instance)
    {
    }

    internal Producer(
        KafkaCluster kafkaCluster,
        string name,
        ProducerConfig config,
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer,
        ProducerMetadata? metadata,
        IReadOnlyCollection<ProducerInterceptor<TKey, TValue>> interceptors,
        TimeSpan time,
        ILoggerFactory loggerFactory)
    {
        _kafkaCluster = kafkaCluster;
        _time = time;
        _logger = loggerFactory.CreateLogger(name);
        _name = name;
        _config = config;

        var transactionalId = config.TransactionalId;
        var clientId = config.ClientId;

        var logFormat = transactionalId is null
            ? "[Producer clientId={ClientId}] "
            : "[Producer clientId={ClientId}, transactionalId={TransactionalId}] ";

        _logger.LogTrace(logFormat + "Starting the Kafka producer", clientId, transactionalId);

        //todo: иницилизировать механиз метрик

        try
        {
            _partitioner = InitPartitionerClass();
            InitializeSerializers(keySerializer, valueSerializer);
            _interceptors = InitInterceptors(interceptors);

            _apiVersions = new ApiVersions();

            _transactionManager = ConfigureTransactionState(loggerFactory);

            var deliveryTimeoutMs = ConfigureDeliveryTimeout();
            var bufferPool = new BufferPool(config.BufferMemory, config.BatchSize, time);
            _accumulator = new RecordAccumulator(config, loggerFactory, deliveryTimeoutMs, time, _apiVersions, _transactionManager, bufferPool);

            var addresses = ParseAndValidateAddresses(config.BootstrapServers, config.ClientDnsLookup);

            var clusterResourceListeners = ConfigureClusterResourceListeners(keySerializer, valueSerializer, interceptors);
            _metadata = metadata
                        ?? new ProducerMetadata(
                            config.RetryBackoffMs,
                            config.MetadataMaxAgeConfig,
                            config.MetadataMaxIdleConfig,
                            loggerFactory,
                            clusterResourceListeners,
                            DateTime.Today.TimeOfDay);
            _metadata.Bootstrap(addresses);

            _sender = BuildSender(loggerFactory, _metadata);

            var ioThreadName = _NETWORK_THREAD_PREFIX + " | " + _name + " | " + clientId;

            //create new long thread for IO operations
            _idThread = new Thread(ctx => _sender.Run((ctx as CancellationTokenSource)!.Token))
            {
                Name = ioThreadName,
                IsBackground = true
            };

            _idThread.Start(_tokenSource);

            _logger.LogDebug(logFormat + "Kafka producer started", clientId, transactionalId);
        }
        catch (Exception exc)
        {
            Close(TimeSpan.Zero, true); //возможно уже что-то успело создатся, поэтому пробуем подчистить все за собой 

            throw new ProtocolKafkaException(StatusCodes.UnknownServerError, "Failed to construct kafka producer", exc);
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

    /// <summary>
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="messages"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IEnumerable<DeliveryResult<TKey, TValue>>> ProduceAsync(
        TopicPartition topicPartition,
        IEnumerable<Message<TKey, TValue>> messages,
        CancellationToken token = default)
    {
        var enumerable = messages as Message<TKey, TValue>[] ?? messages.ToArray();

        var list = new List<Task<DeliveryResult<TKey, TValue>>>(enumerable.Length);
        list.AddRange(enumerable.Select(message => ProduceAsync(topicPartition, message, token)));

        await Task.WhenAll(list);

        return list.Select(x => x.GetAwaiter().GetResult());
    }

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

    public Task InitTransaction(CancellationToken cancellationToken)
    {
        return null;
    }

    public Task BeginTransaction(CancellationToken cancellationToken)
    {
        return null;
    }

    public Task CommitTransaction(CancellationToken cancellationToken)
    {
        return null;
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
        await Task.Yield();

        return new DeliveryResult<TKey, TValue>();
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

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
    ///     asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        return default;
    }

    private ISender BuildSender(ILoggerFactory loggerFactory, ProducerMetadata metadata)
    {
        var maxInflightRequests = ConfigureInflightRequests(_config);
        var channelBuilder = ClientUtils.CreateChannelBuilder(_config, _time, loggerFactory);
        var client = new NetworkClient(
            metadata,
            _config.ClientId,
            maxInflightRequests,
            _config.ReconnectBackoffMs,
            _config.ReconnectBackoffMaxMs,
            _config.SendBufferConfig,
            _config.ReceiveBufferConfig,
            _config.RequestTimeoutMs,
            _config.SocketConnectionSetupTimeoutMs,
            _config.SocketConnectionSetupTimeoutMaxMs,
            _time,
            true,
            _apiVersions,
            loggerFactory);

        var acks = ConfigureAcks(_config, _logger);

        return new Sender(
            loggerFactory,
            client,
            metadata,
            _accumulator,
            maxInflightRequests == 1,
            _config.MaxRequestSize,
            acks,
            _config.Retries,
            _time,
            _config.RequestTimeoutMs,
            _config.RetryBackoffMs,
            _transactionManager,
            _apiVersions);
    }

    private void Close(TimeSpan timeSpan, bool swallowException)
    {
        _tokenSource.Cancel(!swallowException);
    }

    private short ConfigureAcks(ProducerConfig config, ILogger logger)
    {
        var acks = (short)_config.Acks;

        if (!config.IdempotenceEnabled)
        {
            return acks;
        }

        if (_config.Acks == Acks.NoSet)
        {
            _logger.LogInformation("Overriding the default {Acks} to all since idempotence is enabled", acks);
        }
        else if (acks != -1)
        {
            throw new ConfigException("Must set {0} to all in order to use the idempotent");
        }

        return acks;
    }

    private static int ConfigureInflightRequests(ProducerConfig config)
    {
        if (config.IdempotenceEnabled && config.MaxInFlightPerRequest < 5)
        {
            throw new ConfigException("Must set  MaxInFlight to at most 5 to use the idempotent producer.");
        }

        return config.MaxInFlightPerRequest;
    }

    private ClusterResourceListeners ConfigureClusterResourceListeners(
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer,
        IReadOnlyCollection<ProducerInterceptor<TKey, TValue>> interceptors)
    {
        return null;
    }

    private IReadOnlyCollection<IPEndPoint> ParseAndValidateAddresses(IReadOnlyList<string> configBootstrapServers, string configClientDnsLookup)
    {
        return Array.Empty<IPEndPoint>();
    }

    private TransactionManager ConfigureTransactionState(ILoggerFactory loggerFactory)
    {
        TransactionManager transactionManager = null;

        // var userConfiguredIdempotence = _config.IdempotenceEnabled;
        // var userConfiguredTransactions = _config.TransactionalId;

        // if (userConfiguredTransactions && !userConfiguredIdempotence)
        // {
        //     _logger.LogInformation("Overriding the default {ENABLE_IDEMPOTENCE_CONFIG} to true since {TRANSACTIONAL_ID_CONFIG} is specified.", ProducerConfig.ENABLE_IDEMPOTENCE_CONFIG,
        //         ProducerConfig.TRANSACTIONAL_ID_CONFIG);
        // }

        if (_config.IdempotenceEnabled)
        {
            // var transactionTimeoutMs = _config.TransactionTimeoutMs;
            // var retryBackoffMs = _config.RetryBackoffMs;

            transactionManager = new TransactionManager(
                loggerFactory,
                _config.TransactionalId,
                _config.TransactionTimeoutMs,
                _config.RetryBackoffMs,
                _apiVersions);

            if (transactionManager.IsTransactional)
            {
                _logger.LogInformation("Instantiated a transactional producer");
            }
            else
            {
                _logger.LogInformation("Instantiated an idempotent producer");
            }
        }

        return transactionManager;
    }

    private static int LingerMs(ProducerConfig config)
    {
        return (int)Math.Min(config.LingerMs, int.MaxValue);
    }

    private int ConfigureDeliveryTimeout()
    {
        var deliveryTimeoutMs = _config.DeliveryTimeoutMs;
        var lingerMs = LingerMs(_config);
        var requestTimeoutMs = _config.RequestTimeoutMs;
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
        ISerializer<TKey>? keySerializer,
        ISerializer<TValue>? valueSerializer)
    {
        if (keySerializer is null)
        {
            if (!_defaultSerializers.TryGetValue(typeof(TKey), out var serializer))
            {
                throw new ArgumentNullException(
                    $"Key serializer not specified and there is no default serializer defined for type {typeof(TKey).Name}.");
            }

            _keySerializer = (ISerializer<TKey>)serializer;
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

            _valueSerializer = (ISerializer<TValue>)serializer;
        }
        else
        {
            _valueSerializer = valueSerializer;
        }
    }

    public void InitTransaction()
    {
        ThrowIfNoTransactionManager();
        ThrowIfProducerClosed();
        var stopWatch = Stopwatch.StartNew();
        var result = _transactionManager.InitTransaction();
        _sender.Wakeup();

        //result.Await(_config.MaxBlockTimeMs);
        stopWatch.Stop();
    }

    private void ThrowIfProducerClosed()
    {
    }

    private void ThrowIfNoTransactionManager()
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

    private async Task<DeliveryResult<TKey, TValue>> InternalProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken token = default)
    {
        var headers = message.Headers;
        var keyByte = SerializeKey(message.Key);
        var valueByte = SerializeValue(message.Value);

        var tcs = await _accumulator.AppendAsync(topicPartition, message.Timestamp, keyByte, valueByte, headers, token);

        _sender.Wakeup();

        if (_config.EnableDeliveryReports)
        {
            var result = await tcs.Task;
        }

        return new DeliveryResult<TKey, TValue>
        {
            TopicPartitionOffset = new TopicPartitionOffset(topicPartition, Offset.Unset),
            Message = message,
            Status = PersistenceStatus.NotPersisted
        };
    }

    private byte[] SerializeKey(TKey key)
    {
        try
        {
            return _keySerializer.Serialize(key);
        }
        catch (Exception exc)
        {
            throw new ProduceException(exc);
        }
    }

    private byte[] SerializeValue(TValue value)
    {
        try
        {
            return _valueSerializer.Serialize(value);
        }
        catch (Exception exc)
        {
            throw new ProduceException(exc);
        }
    }
}

internal class ClusterResourceListeners
{
}