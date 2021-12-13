using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Exceptions;

namespace Microlibs.Kafka.Protocol.Connection;

internal interface IBrokerPoolManager : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     Пытается добавить брокер в пулл брокеров
    /// </summary>
    /// <exception cref="ClusterKafkaException">Возникает, если брокер не удалось добавить в пул</exception>
    Task<bool> TryAddBrokerAsync(IBroker broker, bool isController, bool throwExceptionIfNoAdded, CancellationToken token);

    IBroker GetLeastLoadedBroker();

    IReadOnlyCollection<IBroker> GetBrokers();

    IBroker GetController();

    IBroker NextBroker();
}