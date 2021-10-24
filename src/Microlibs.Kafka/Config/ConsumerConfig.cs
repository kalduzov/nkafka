namespace Microlibs.Kafka.Config
{
    public record ConsumerConfig : CommonConfig
    {
        /// <summary>
        /// group.id
        /// </summary>
        public string GroupId { get; set; } = null!;
    }
}