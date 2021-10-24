using System;
using Microlibs.Kafka.Protocol.RequestsMessages;

namespace Microlibs.Kafka.Protocol
{
    internal abstract class RequestMessage
    {
        internal static readonly RequestMessage Empty = new EmptyRequestMessage();

        public int Length { get; protected init; }

        public ApiKeys ApiKey { get; protected init; }

        public abstract ReadOnlySpan<byte> AsReadOnlySpan();
    }
}