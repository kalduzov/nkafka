using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microlibs.Kafka.Protocol.Connection
{
    internal interface IKafkaBrokerConnection : IDisposable
    {
        void Send(KafkaRequestMessage requestMessage);

        Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
            where TResponseMessage : KafkaResponseMessage
            where TRequestMessage : KafkaContent;
    }
}