using System;
using System.Linq;
using Microlibs.Kafka.Serialization;

namespace Microlibs.Kafka.Protocol.RequestsMessages;

public class MetadataRequestMessage : KafkaContent
{
    public MetadataRequestMessage(params string[] topics)
    {
        Topics = topics;
        ApiKey = ApiKeys.Metadata;
    }

    /// <summary>
    ///     The topics to fetch metadata for
    /// </summary>
    public string[] Topics { get; }

    /// <summary>
    ///     If this is true, the broker may auto-create topics that we requested which do not already exist, if it is
    ///     configured to do so
    /// </summary>
    public bool AllowAutoTopicCreation { get; set; }

    /// <summary>
    ///     Whether to include cluster authorized operations
    /// </summary>
    public bool IncludeClusterAuthorizedOperations { get; set; }

    /// <summary>
    ///     Whether to include topic authorized operations
    /// </summary>
    public bool IncludeTopicAuthorizedOperations { get; set; }

    public override ReadOnlySpan<byte> AsReadOnlySpan()
    {
        var serializer = new ListSerializer<string>(Serializers.String);
        var data = serializer.Serialize(Topics.ToList());

        return default;
    }
}