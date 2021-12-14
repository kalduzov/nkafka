using System;
using System.IO;
using System.Text;
using Microlibs.Kafka.Protocol.Extensions;

namespace Microlibs.Kafka.Protocol.RequestsMessages;

public class MetadataRequestMessage : KafkaContent
{
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

    public MetadataRequestMessage(params string[] topics)
    {
        Topics = topics ?? Array.Empty<string>();
        ApiKey = ApiKeys.Metadata;
        Length = CalculateLen();
    }

    private int CalculateLen()
    {
        var len = 4; //Длинна массива topics

        switch (Version)
        {
            case >= ApiVersions.Version0 and <= ApiVersions.Version3:
            {
                foreach (var topic in Topics)
                {
                    len += 2 + Encoding.UTF8.GetBytes(topic).Length;
                }

                break;
            }
        }

        return len;
    }

    public override void SerializeToStream(Stream stream)
    {
        if (Version == ApiVersions.Version0 || Topics.Length > 0)
        {
            stream.Write(Topics.Length.ToBigEndian());

            foreach (var topic in Topics)
            {
                stream.Write(topic.AsNullableString());
            }
        }
        else
        {
            stream.Write((-1).ToBigEndian());
        }

        if (Version > ApiVersions.Version3)
        {
            stream.WriteByte(AllowAutoTopicCreation.AsByte());
        }

        if (Version is > ApiVersions.Version7 and <= ApiVersions.Version11)
        {
            stream.WriteByte(IncludeClusterAuthorizedOperations.AsByte());
        }

        if (Version > ApiVersions.Version7)
        {
            stream.WriteByte(IncludeTopicAuthorizedOperations.AsByte());
        }
    }
}