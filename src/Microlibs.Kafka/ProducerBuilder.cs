// using System;
// using System.Collections.Concurrent;
// using Microlibs.Kafka.Clients;
// using Microlibs.Kafka.Config;
// using Microlibs.Kafka.Protocol.Connection;
//
// namespace Microlibs.Kafka
// {
//     internal class ProducerBuilder : IProducerBuilder
//     {
//         private readonly ProducerConfig _producerConfig;
//         private readonly ConcurrentDictionary<string, IProducer> _producers = new();
//         private readonly BrokerConnectionPool _connectionPool;
//         private readonly Action<string> _disposeAction;
//
//         public ProducerBuilder(ProducerConfig producerConfig)
//         {
//             _producerConfig = producerConfig;
//             _connectionPool = new BrokerConnectionPool(producerConfig);
//             _disposeAction = DisposeAction;
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <param name="name"></param>
//         /// <returns></returns>
//         public IProducer Build(string name)
//         {
//             if (string.IsNullOrWhiteSpace(name))
//             {
//                 throw new ArgumentNullException(nameof(name), "Должно быть задано имя продюсера");
//             }
//
//             if (_producers.TryGetValue(name, out var existsProducer))
//             {
//                 return existsProducer;
//             }
//
//             var producer = CreateProducerInternal(name);
//
//             existsProducer = _producers.GetOrAdd(name, producer);
//
//             if (existsProducer != producer)
//             {
//                 producer.Dispose();
//             }
//
//             return existsProducer;
//         }
//
//         private IProducer CreateProducerInternal(string name)
//         {
//             var connections = _connectionPool.GetConnections();
//
//             return new Producer(name, connections, _disposeAction);
//         }
//
//         private void DisposeAction(string name)
//         {
//             _producers.TryRemove(name, out _);
//         }
//     }
// }

