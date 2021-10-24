using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Protocol;
using Microsoft.Extensions.Logging;

namespace Microlibs.Kafka
{
    public sealed class KafkaCluster : IKafkaCluster
    {
        private readonly IRequestBuilder _requestBuilder;
        private readonly ILogger _logger;

        private KafkaCluster(IRequestBuilder requestBuilder, ILogger logger)
        {
            _logger = logger;
            _requestBuilder = requestBuilder;
        }

        public static IProducerBuilder CreateProducerBuilder(CommonConfig config)
        {
            if (config is ProducerConfig producerConfig)
            {
                return new ProducerBuilder(producerConfig);
            }

            throw new ArgumentException("Параметр должен быть типом ProducerConfig", nameof(config));
        }

        public Task<IEnumerable<string>> CreateTopics(IReadOnlyCollection<string> names, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}