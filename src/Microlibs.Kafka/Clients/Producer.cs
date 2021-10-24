using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Protocol;
using Microlibs.Kafka.Protocol.Connection;
using Microlibs.Kafka.Protocol.RequestsMessages;
using Microlibs.Kafka.Protocol.Responses;

namespace Microlibs.Kafka.Clients
{
    internal class Producer : IProducer
    {
        private readonly string _name;
        private readonly IReadOnlyCollection<IKafkaBrokerConnection> _connection;
        private readonly Action<string> _disposeAction;
        private bool _disposing;

        private readonly object _lock = new();

        internal Producer(string name, IReadOnlyCollection<IKafkaBrokerConnection> connection, Action<string> disposeAction)
        {
            _name = name;
            _connection = connection;
            _disposeAction = disposeAction;
        }

        public void Produce<T>(string topicName, T message)
        {
            if (_disposing)
            {
                throw new ObjectDisposedException(nameof(Producer), $"Producer with name {_name} has been disposed");
            }
        }

        public async Task<ProduceResult> ProduceAsync<T>(IReadOnlyList<string> topics, IReadOnlyList<T> messages, CancellationToken token = default)
        {
            if (_disposing)
            {
                throw new ObjectDisposedException(nameof(Producer), $"Producer with name {_name} has been disposed");
            }

            var connection = _connection.First();

            //var kafkaResponse = await connection.SendAsync<ProduceResponseMessage, ProduceRequestMessage>(new ProduceRequestMessage(), token);

            return new ProduceResult();
        }

        public void Dispose()
        {
            if (_disposing)
            {
                return;
            }

            lock (_lock)
            {
                if (_disposing)
                {
                    return;
                }

                _disposing = true;
                _disposeAction(_name);
            }
        }
    }
}