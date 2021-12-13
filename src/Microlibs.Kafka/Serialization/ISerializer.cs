namespace Microlibs.Kafka.Serialization;

public interface ISerializer<in T>
{
    byte[] Serialize(T data);
}