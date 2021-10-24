using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microlibs.Kafka.Protocol
{
    internal interface IKafkaBrokerConnection : IDisposable
    {
        void Send(KafkaRequest request);

        Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
            where TResponseMessage : ResponseMessage
            where TRequestMessage : RequestMessage;
    }
}