namespace Microlibs.Kafka.Clients;

public abstract class Client : IClient
{
    public virtual void Dispose()
    {
    }
}