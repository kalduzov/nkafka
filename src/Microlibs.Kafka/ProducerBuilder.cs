// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 * 
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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

