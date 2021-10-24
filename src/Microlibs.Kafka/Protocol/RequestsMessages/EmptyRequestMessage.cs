using System;

namespace Microlibs.Kafka.Protocol.RequestsMessages
{
    internal class EmptyRequestMessage : RequestMessage
    {
        public EmptyRequestMessage()
        {
            Length = 0x00;
        }

        public override ReadOnlySpan<byte> AsReadOnlySpan()
        {
            return Array.Empty<byte>();
        }
    }
}