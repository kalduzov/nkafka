using System;
using System.Collections.Generic;
using System.IO;

namespace Microlibs.Kafka.Protocol.Responses;

public class ApiVersionMessage : KafkaResponseMessage
{
    public IReadOnlyCollection<ApiVersion> ApiVersions { get; internal set; }

    public int ThrottleTimeMs { get; internal set; }

    public override void DeserializeFromStream(ReadOnlySpan<byte> span)
    {
    }
}