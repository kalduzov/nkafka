using System;

namespace Microlibs.Kafka.Exceptions;

public abstract class KafkaException : Exception
{
    protected KafkaException(string message)
        : base(message)
    {
    }

    protected KafkaException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}