using System;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka
{
    public class KafkaException : Exception
    {
        internal KafkaException(ErrorCodes code, string message, Exception innerException = null!)
            : base(message, innerException)
        {
            InternalError = code;
        }

        public ErrorCodes InternalError { get; }
    }
}