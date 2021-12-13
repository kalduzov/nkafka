using System;

namespace Microlibs.Kafka.Exceptions;

public class ClusterKafkaException : KafkaException
{
    public ClusterKafkaException(string message)
        : base(message)
    {
    }

    public ClusterKafkaException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}