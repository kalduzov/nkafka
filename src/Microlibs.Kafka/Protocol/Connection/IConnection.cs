using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microlibs.Kafka.Protocol
{
    internal interface IConnection : IDisposable
    {
        void Send(KafkaRequest request);

        Task<TResponseMessage> SendAsync<TResponseMessage>(KafkaRequest request, CancellationToken token)
            where TResponseMessage : ResponseMessage;
    }
}