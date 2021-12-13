using System;
using System.IO;
using Microlibs.Kafka.Protocol.Extensions;

namespace Microlibs.Kafka.Protocol;

public class KafkaRequestMessage
{
    public readonly int RequestLength;

    public KafkaRequestMessage(KafkaRequestHeader header, KafkaContent content)
    {
        Header = header;
        Content = content;
        RequestLength = header.Length + content.Length;
    }

    /// <summary>
    /// </summary>
    public short ApiVersion { get; set; }

    public KafkaRequestHeader Header { get; set; }

    public KafkaContent Content { get; set; }

    public ReadOnlyMemory<byte> ToByteStream()
    {
        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);

        writer.WriteLength(RequestLength);
        writer.WriteHeader(Header);
        writer.WriteMessage(Content);

        return memoryStream.ToArray();
    }
}