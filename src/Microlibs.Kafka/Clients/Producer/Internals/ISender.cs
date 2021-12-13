using System.Threading;

namespace Microlibs.Kafka.Clients.Producer.Internals;

internal interface ISender
{
    void Wakeup();

    void Run(CancellationToken cancellationToken);
}