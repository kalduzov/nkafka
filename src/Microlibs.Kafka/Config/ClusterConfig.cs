namespace Microlibs.Kafka.Config
{
    public record ClusterConfig : CommonConfig
    {
        /// <summary>
        /// Таймаут обновления данных по брокерам в ms
        /// </summary>
        /// <remarks>Default - 1000ms</remarks>
        public int BrokerUpdateTimeout { get; set; } = 1000;
    }
}