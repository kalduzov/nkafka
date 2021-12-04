using System.Text;

namespace Microlibs.Kafka.Serialization;

public sealed class StringSerializer : ISerializer<string>
{
    public byte[] Serialize(string data)
    {
        return Encoding.UTF8.GetBytes(data);
    }
}