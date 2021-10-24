using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Protocol;
using Microlibs.Kafka.Protocol.Responses;

namespace Microlibs.Kafka
{
    internal class Producer : IProducer
    {
        private readonly string _name;
        private readonly IReadOnlyCollection<IConnection> _connection;
        private readonly Action<string> _disposeAction;
        private bool _disposing;

        private readonly object _lock = new();

        internal Producer(string name, IReadOnlyCollection<IConnection> connection, Action<string> disposeAction)
        {
            _name = name;
            _connection = connection;
            _disposeAction = disposeAction;
        }

        public void Produce<T>(string topicName, T Message)
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

            var kafkaResponse = await connection.SendAsync<ProduceResponseMessage>(new KafkaRequest(), token);

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