namespace Microlibs.Kafka.Clients.Producer;

public interface IPartitioner
{
    int Partition();
}