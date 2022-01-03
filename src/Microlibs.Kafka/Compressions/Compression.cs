namespace Microlibs.Kafka.Compressions;

public interface ICompression
{
    byte[] Decode(byte[] data);

    byte[] Encode(byte[] data);
}