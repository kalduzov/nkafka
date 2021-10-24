using System;

namespace Microlibs.Kafka.Protocol
{
    internal abstract class RequestMessage
    {
        internal static readonly RequestMessage Empty = new EmptyRequestMessage();

        public int Length { get; set; } = 0;

        public abstract ReadOnlySpan<byte> AsReadOnlySpan();
    }
}