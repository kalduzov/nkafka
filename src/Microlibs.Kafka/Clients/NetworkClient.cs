using System;
using Microlibs.Kafka.Clients.Producer.Internals;
using Microlibs.Kafka.Protocol;
using Microsoft.Extensions.Logging;

namespace Microlibs.Kafka.Clients;

internal class NetworkClient : IClient
{
    private readonly ProducerMetadata _metadata;
    private readonly string _clientId;
    private readonly int _maxInflightRequests;
    private readonly int _reconnectBackoffMs;
    private readonly int _reconnectBackoffMaxMs;
    private readonly int _socketSendBuffer;
    private readonly int _socketReceiveBuffer;
    private readonly int _defaultRequestTimeoutMs;
    private readonly int _connectionSetupTimeoutMs;
    private readonly int _connectionSetupTimeoutMaxMs;
    private readonly TimeSpan _time;
    private readonly bool _discoverBrokerVersions;
    private readonly ApiVersions _apiVersions;
    private readonly ILoggerFactory _loggerFactory;

    public NetworkClient(
        ProducerMetadata metadata,
        string clientId,
        int maxInflightRequests,
        int reconnectBackoffMs,
        int reconnectBackoffMaxMs,
        int socketSendBuffer,
        int socketReceiveBuffer,
        int defaultRequestTimeoutMs,
        int connectionSetupTimeoutMs,
        int connectionSetupTimeoutMaxMs,
        TimeSpan time,
        bool discoverBrokerVersions,
        ApiVersions apiVersions,
        ILoggerFactory loggerFactory)
    {
        _metadata = metadata;
        _clientId = clientId;
        _maxInflightRequests = maxInflightRequests;
        _reconnectBackoffMs = reconnectBackoffMs;
        _reconnectBackoffMaxMs = reconnectBackoffMaxMs;
        _socketSendBuffer = socketSendBuffer;
        _socketReceiveBuffer = socketReceiveBuffer;
        _defaultRequestTimeoutMs = defaultRequestTimeoutMs;
        _connectionSetupTimeoutMs = connectionSetupTimeoutMs;
        _connectionSetupTimeoutMaxMs = connectionSetupTimeoutMaxMs;
        _time = time;
        _discoverBrokerVersions = discoverBrokerVersions;
        _apiVersions = apiVersions;
        _loggerFactory = loggerFactory;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
    }
}