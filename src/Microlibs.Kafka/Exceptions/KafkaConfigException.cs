using System;

namespace Microlibs.Kafka.Exceptions;

public class KafkaConfigException : KafkaException
{
    public KafkaConfigException(string message)
        : base(message)
    {
    }

    public KafkaConfigException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}