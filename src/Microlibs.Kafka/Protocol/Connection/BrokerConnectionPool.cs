using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Protocol.RequestsMessages;
using Microlibs.Kafka.Protocol.Responses;

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

        InitFirstConnections(commonConfig);

        _brokersUpdater = Task.Factory.StartNew(() => BrokerUpdaterTask(_tokenSource.Token), TaskCreationOptions.LongRunning);
    }

    public ValueTask DisposeAsync()
    {
        //todo аккуратно закрыть все соединения и задачи обновления данных по брокерам
        return default;
    }

    public void Dispose()
    {
        _brokersUpdater.Dispose();
    }

    private async Task BrokerUpdaterTask(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var connection = _connections.First();

            var message = new DescribeClusterContent
            {
                IncludeClusterAuthorizedOperations = true
            };

            try
            {
                var describe = await connection.SendAsync<DescribeResponseMessage, DescribeClusterContent>(message, token);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            //await Task.Delay(_commonConfig.BrokerUpdateTimeout, token);
        }
    }

    private void InitFirstConnections(CommonConfig commonConfig)
    {
        foreach (var server in commonConfig.BootstrapServers)
        {
            var (host, port) = GetHostAndPort(server);

            var endpoint = new BrokerEndpoint(host, port);
            var connection = new Broker(endpoint);

            _connections.Add(connection);
        }
    }

    private static (string, int) GetHostAndPort(string server)
    {
        var hostsAndPorts = server.Split(":", StringSplitOptions.RemoveEmptyEntries);

        var host = hostsAndPorts[0];
        ValidateHost(host);

        if (int.TryParse(hostsAndPorts[1], out var port))
        {
            ValidatePort(port);
        }
        else
        {
            throw new ArgumentException($"Port {port} is not integer");
        }

        return (host, port);
    }

    private static void ValidatePort(int port)
    {
        if (port <= 1024)
        {
            throw new ArgumentException($"Port {port} is incorrect");
        }
    }

    private static void ValidateHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentException($"Host {host} is incorrect");
        }
    }

    public IReadOnlyCollection<IBroker> GetConnections()
    {
        return _connections;
    }
}