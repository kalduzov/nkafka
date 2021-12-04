using System;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka
{
    public class KafkaException : Exception
    {
        internal KafkaException(StatusCodes code, string message, Exception innerException = null!)
            : base(message, innerException)
        {
            InternalStatus = code;
        }

        public StatusCodes InternalStatus { get; }
    }
}