using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Exceptions;

namespace Microlibs.Kafka.Protocol.Connection;

/// <summary>
///     Содержит всю информацию о брокерах
/// </summary>
internal sealed class DefaultBrokerPoolManager : IBrokerPoolManager
{
    private readonly object _syncObj = new();

    private readonly ConcurrentDictionary<int, IBroker> _brokers;
    private IBroker _controller = null!;

    public DefaultBrokerPoolManager(CommonConfig commonConfig)
    {
        _brokers = new ConcurrentDictionary<int, IBroker>(Environment.ProcessorCount, commonConfig.BootstrapServers.Count);
        
    }

    public void Dispose()
    {
        foreach (var broker in _brokers.Values)
        {
            broker.Dispose();
        }

        _brokers.Clear();
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
    ///     asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        foreach (var broker in _brokers.Values)
        {
            await broker.DisposeAsync();
        }

        _brokers.Clear();
    }

    /// <summary>
    ///     Возвращает брокера по roundrobin
    /// </summary>
    public IBroker NextBroker()
    {
        return _brokers.Values.First();
    }

    /// <summary>
    ///     Пытается добавить брокер в пулл брокеров
    /// </summary>
    /// <exception cref="ClusterKafkaException">Возникает, если брокер не удалось добавить в пул</exception>
    public Task<bool> TryAddBrokerAsync(IBroker broker, bool isController, bool throwExceptionIfNoAdded, CancellationToken token)
    {
        if (!_brokers.TryAdd(broker.GetHashCode(), broker) && throwExceptionIfNoAdded)
        {
            throw new ClusterKafkaException($"Can't added broker {broker.EndPoint} to cluster");
        }

        if (isController)
        {
            _controller = broker;
        }

        return Task.FromResult(true);
    }

    /// <summary>
    ///     Возвращает наименее нагруженный запросами брокер
    /// </summary>
    /// <remarks>Такой брокер используется для высокоприоритетных запросов</remarks>
    public IBroker GetLeastLoadedBroker()
    {
        return _brokers.Values.First();
    }

    public IReadOnlyCollection<IBroker> GetBrokers()
    {
        return _brokers.Values.ToArray();
    }

    public IBroker GetController()
    {
        return _controller;
    }

    ~DefaultBrokerPoolManager()
    {
    }
}