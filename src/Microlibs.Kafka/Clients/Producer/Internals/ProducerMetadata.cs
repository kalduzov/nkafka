using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Microlibs.Kafka.Clients.Producer.Internals;

internal class ProducerMetadata : Metadata
{
    public ProducerMetadata(
        long configRetryBackoffMs,
        long configMetadataMaxAgeConfig,
        long configMetadataMaxIdleConfig,
        ILoggerFactory loggerFactory,
        ClusterResourceListeners clusterResourceListeners,
        TimeSpan time)
    {
    }
}