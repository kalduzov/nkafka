namespace Microlibs.Kafka.Config
{
    public record ProducerConfig : CommonConfig
    {
        /// <summary>
        ///  Specifies whether to enable notification of delivery reports. Typically
        ///  you should set this parameter to true. Set it to false for "fire and
        ///  forget" semantics and a small boost in performance.
        ///
        ///  default: true
        /// </summary>
        public bool EnableDeliveryReports { get; set; } = true;

        public int DeliveryTimeoutMs { get; set; } = 120 * 1000;

        public Partitioner Partitioner { get; set; } = Partitioner.ConsistentRandom;

        public int QueueBufferingMaxMessages { get; set; } = 10000;

        public int QueueBufferingMaxKbytes { get; set; } = 1048576;

        public double LingerMs { get; set; } = 5;

        public Acks Acks { get; set; } = Acks.All;

        public int MaxRequestSize { get; set; }

        public long TotalMemorySize { get; set; }

        public CompressionType CompressionType { get; set; }

        public long MaxBlockTimeMs { get; set; }

        public string? TransactionalId { get; set; }

        public int BatchSize { get; set; }

        public long BufferMemory { get; set; }

        public string ClientDnsLookup { get; set; }

        public long MetadataMaxAgeConfig { get; set; }

        public long MetadataMaxIdleConfig { get; set; }

        /// <summary>
        /// The maximum number of unacknowledged requests the client will send on a single connection before blocking.
        /// Note that if this config is set to be greater than 1 and <see cref="IdempotenceEnabled"/> is set to false, there is a risk of
        /// message re-ordering after a failed send due to retries (i.e., if retries are enabled)
        ///     default: 5
        ///     importance: low
        /// </summary>
        public int MaxInFlightPerRequest { get; set; } = 5;

        public bool IdempotenceEnabled
        {
            get { return false; }
        }

        public int SendBufferConfig { get; set; } = 128 * 1024;

        public int ReceiveBufferConfig { get; set; } = 32 * 1024;

        public int Retries { get; set; } = int.MaxValue;

        public int TransactionTimeoutMs { get; set; } = 6000;

        public static ProducerConfig BaseFrom(CommonConfig config)
        {
            return new ProducerConfig
            {
                ClientId = config.ClientId,
                BootstrapServers = config.BootstrapServers,
                ApiVersionRequest = config.ApiVersionRequest
            };
        }
    }
}