using System;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka.Exceptions;

public class ProtocolKafkaException : KafkaException
{
    internal ProtocolKafkaException(StatusCodes code, string message, Exception innerException = null!)
        : base(message, innerException)
    {
        InternalStatus = code;
    }

    public StatusCodes InternalStatus { get; }
}