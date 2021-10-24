using System;

namespace Microlibs.Kafka.Protocol
{
    internal class EmptyRequestMessage : RequestMessage
    {
        public override ReadOnlySpan<byte> AsReadOnlySpan()
        {
            return Array.Empty<byte>();
        }
    }
}