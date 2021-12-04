namespace Microlibs.Kafka.Clients.Producer
{
    public class DeliveryResult<TKey, TValue>
    {
        /// <summary>
        ///     The topic associated with the message.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        ///     The partition associated with the message.
        /// </summary>
        public Partition Partition { get; set; }

        /// <summary>
        ///     The partition offset associated with the message.
        /// </summary>
        public Offset Offset { get; set; }

        /// <summary>
        ///     The TopicPartition associated with the message.
        /// </summary>
        public TopicPartition TopicPartition => new TopicPartition(Topic, Partition);

        /// <summary>
        ///     The TopicPartitionOffset associated with the message.
        /// </summary>
        public TopicPartitionOffset TopicPartitionOffset
        {
            get => new TopicPartitionOffset(Topic, Partition, Offset);
            set
            {
                Topic = value.Topic;
                Partition = value.Partition;
                Offset = value.Offset;
            }
        }

        /// <summary>
        ///     The persistence status of the message
        /// </summary>
        public PersistenceStatus Status { get; set; }

        /// <summary>
        ///     The Kafka message.
        /// </summary>
        public Message<TKey, TValue> Message { get; set; }

        public TKey Key => Message.Key;

        public TValue Value => Message.Value;

        public Timestamp Timestamp => Message.Timestamp;

        public Headers Headers => Message.Headers;
    }
}