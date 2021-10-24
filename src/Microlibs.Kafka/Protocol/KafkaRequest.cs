using System;
using System.IO;

namespace Microlibs.Kafka.Protocol
{
    internal readonly struct KafkaRequest
    {
        public readonly int RequestLength;

        public readonly RequestHeader Header;

        public readonly RequestMessage Message;

        public KafkaRequest(RequestHeader header, RequestMessage message)
        {
            Header = header;
            Message = message;
            RequestLength = header.Length + message.Length;
        }

        public ReadOnlyMemory<byte> ToByteStream()
        {
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);

            writer.WriteLength(RequestLength);
            writer.WriteHeader(Header);
            writer.WriteMessage(Message);

            return memoryStream.ToArray();
        }
    }
}