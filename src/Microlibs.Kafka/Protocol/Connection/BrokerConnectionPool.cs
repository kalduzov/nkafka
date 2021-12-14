using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;

namespace Microlibs.Kafka.Protocol.Connection;

internal sealed class BrokerConnectionPool : IDisposable, IAsyncDisposable
{
    private readonly Task _brokersUpdater;
    private readonly CommonConfig _commonConfig;
    private readonly List<IBroker> _connections;
    private readonly CancellationTokenSource _tokenSource = new();

    public BrokerConnectionPool(CommonConfig commonConfig)
    {
        _commonConfig = commonConfig;
        _connections = new List<IBroker>(commonConfig.BootstrapServers.Count);

        SeedBrokers(commonConfig);

        //_brokersUpdater = Task.Factory.StartNew(() => BrokerUpdaterTask(_tokenSource.Token), TaskCreationOptions.LongRunning);
    }

    public ValueTask DisposeAsync()
    {
        //todo аккуратно закрыть все соединения и задачи обновления данных по брокерам
        _tokenSource.Cancel();

        return default;
    }

    public void Dispose()
    {
        _brokersUpdater.Dispose();
    }

    // private async Task BrokerUpdaterTask(CancellationToken token)
    // {
    //     while (!token.IsCancellationRequested)
    //     {
    //         var connection = _connections.First();
    //
    //         var message = new MetadataRequestMessage
    //         {
    //             IncludeClusterAuthorizedOperations = true
    //         };
    //
    //         try
    //         {
    //             var describe = await connection.SendAsync<DescribeResponseMessage, DescribeClusterContent>(message, token);
    //         }
    //         catch (Exception exc)
    //         {
    //             Console.WriteLine(exc.Message);
    //         }
    //
    //         //await Task.Delay(_commonConfig.BrokerUpdateTimeout, token);
    //     }
    // }

    private void SeedBrokers(CommonConfig commonConfig)
    {
        foreach (var bootstrapServer in commonConfig.BootstrapServers)
        {
            var endpoint = Utils.BuildBrokerEndPoint(bootstrapServer);
            var connection = new Broker(endpoint);

            _connections.Add(connection);
        }
    }

    public IReadOnlyCollection<IBroker> GetConnections()
    {
        return _connections;
    }

    public IBroker GetController()
    {
        return null;
    }

    public IReadOnlyCollection<IBroker> GetBrokers()
    {
        return null;
    }

    /// <summary>
    /// Возвращает наименее нагруженный брокер
    /// </summary>
    public IBroker GetLeastLoadedBroker()
    {
        return _connections.First();
    }

    public Task<bool> TryAddBrokerAsync(Broker newBroker, bool isController, bool b, CancellationToken token)
    {
        return Task.FromResult(true);
    }
}