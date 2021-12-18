using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;

namespace Microlibs.Kafka.Protocol.Responses;

public record ApiVersionMessage : KafkaResponseMessage
{
    public IReadOnlyCollection<ApiVersion> ApiVersions { get; internal set; }

    public int ThrottleTimeMs { get; internal set; }

}