using System;
using System.Collections.Generic;
using System.Threading;
using Microlibs.Kafka.Protocol;
using Microsoft.Extensions.Logging;

namespace Microlibs.Kafka.Clients.Producer.Internals;

internal class Sender : ISender
{
    private readonly ProducerMetadata _metadata;
    private readonly RecordAccumulator _accumulator;
    private readonly bool _guaranteeMessageOrder;
    private readonly int _maxRequestSize;
    private readonly short _asks;
    private readonly int _retries;
    private readonly Timestamp _time;
    private readonly int _requestTimeoutMs;
    private readonly long _retryBackoffMs;
    private readonly TransactionManager _transactionManager;
    private readonly ApiVersions _apiVersions;

    private bool _running;
    private readonly ILogger<Sender> _logger;
    private readonly Dictionary<TopicPartition, List<ProducerBatch>> _inFlightBatches;

    public Sender(
        ILoggerFactory loggerFactory,
        ProducerMetadata metadata,
        RecordAccumulator accumulator,
        bool guaranteeMessageOrder,
        int maxRequestSize,
        short asks,
        int retries,
        Timestamp time,
        int requestTimeoutMs,
        long retryBackoffMs,
        TransactionManager transactionManager,
        ApiVersions apiVersions)
    {
        _logger = loggerFactory.CreateLogger<Sender>();
        _metadata = metadata;
        _accumulator = accumulator;
        _guaranteeMessageOrder = guaranteeMessageOrder;
        _maxRequestSize = maxRequestSize;
        _asks = asks;
        _retries = retries;
        _time = time;
        _requestTimeoutMs = requestTimeoutMs;
        _retryBackoffMs = retryBackoffMs;
        _transactionManager = transactionManager;
        _apiVersions = apiVersions;

        _running = true;

        _inFlightBatches = new Dictionary<TopicPartition, List<ProducerBatch>>();
    }

    public Sender(
        ILoggerFactory loggerFactory,
        NetworkClient metadata,
        ProducerMetadata accumulator,
        RecordAccumulator guaranteeMessageOrder,
        bool maxRequestSize,
        int configMaxRequestSize,
        short retries,
        int configRetries,
        TimeSpan requestTimeoutMs,
        int? configRequestTimeoutMs,
        long configRetryBackoffMs,
        TransactionManager transactionManager,
        ApiVersions apiVersions)
    {
    }

    public void Wakeup()
    {
    }

    public void Run(CancellationToken tokenSourceToken)
    {
        while (_running)
        {
            if (tokenSourceToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                RunOnce();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Uncaught error in kafka producer I/O thread");
            }
        }

        _logger.LogDebug("Beginning shutdown of Kafka producer I/O thread, sending remaining records");
    }

    private void RunOnce()
    {
    }
}