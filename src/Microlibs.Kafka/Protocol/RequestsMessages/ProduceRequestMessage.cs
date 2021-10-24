using System;

namespace Microlibs.Kafka.Protocol.RequestsMessages
{
    internal class ProduceRequestMessage : RequestMessage
    {
        public override ReadOnlySpan<byte> AsReadOnlySpan()
        {
            return default;
        }
    }
}