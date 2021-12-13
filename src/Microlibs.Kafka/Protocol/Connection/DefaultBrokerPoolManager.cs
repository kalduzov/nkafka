using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;

namespace Microlibs.Kafka.Protocol.Connection;

/// <summary>
///     Содержит всю информацию о брокерах
/// </summary>
internal sealed class DefaultBrokerPoolManager : IBrokerPoolManager
{
    private readonly CommonConfig _commonConfig;
    private readonly object _syncObj = new();

    //private 

    public DefaultBrokerPoolManager(CommonConfig commonConfig)
    {
        _commonConfig = commonConfig;
    }

    public void Dispose()
    {
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

    /// <summary>
    ///     Возвращает брокера по roundrobin
    /// </summary>
    public IBroker NextBroker()
    {
        return null;
    }

    /// <summary>
    ///     Пытается добавить брокер в пулл брокеров
    /// </summary>
    /// <exception cref="ClusterKafkaException">Возникает, если брокер не удалось добавить в пул</exception>
    public async Task<bool> TryAddBrokerAsync(IBroker broker, bool isController, bool throwExceptionIfNoAdded, CancellationToken token)
    {
        return true;
    }

    /// <summary>
    ///     Возвращает наименее нагруженный запросами брокер
    /// </summary>
    /// <remarks>Такой брокер используется для высокоприоритетных запросов</remarks>
    public IBroker GetLeastLoadedBroker()
    {
        return null;
    }

    public IReadOnlyCollection<IBroker> GetBrokers()
    {
        return null;
    }

    public IBroker GetController()
    {
        return null;
    }

    ~DefaultBrokerPoolManager()
    {
    }

    public bool TryAddBroker(IBroker broker)
    {
        return false;
    }

    internal readonly struct ClusterConnectionKey : IEquatable<ClusterConnectionKey>
    {
        public readonly string ClusterId;

        public ClusterConnectionKey(string clusterId)
        {
            ClusterId = clusterId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClusterId);
        }

        public override bool Equals(object? obj)
        {
            return obj is ClusterConnectionKey cck && Equals(cck);
        }

        public bool Equals(ClusterConnectionKey other)
        {
            return ClusterId.Equals(other.ClusterId, StringComparison.Ordinal);
        }
    }
}