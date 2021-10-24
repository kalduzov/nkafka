using System.Collections.Generic;

namespace Microlibs.Kafka.Protocol.Responses
{
    public record ApiVersionMessage : ResponseMessage
    {
        public IReadOnlyCollection<ApiVersion> ApiVersions { get; internal set;}

        public int ThrottleTimeMs { get; internal set; }
    }
}