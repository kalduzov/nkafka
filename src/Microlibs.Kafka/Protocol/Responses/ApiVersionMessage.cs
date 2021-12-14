using System.Buffers;
using System.Collections.Generic;
using System.IO;
using Microsoft.IO;

namespace Microlibs.Kafka.Protocol.Responses;

public class ApiVersionMessage : KafkaResponseMessage
{
    public IReadOnlyCollection<ApiVersion> ApiVersions { get; internal set; }

    public int ThrottleTimeMs { get; internal set; }

    public override void DeserializeFromStream(Stream stream)
    {
    }
}