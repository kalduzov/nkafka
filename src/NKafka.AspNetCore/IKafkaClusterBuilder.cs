// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright © 2026 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NKafka.Clients.Consumer;
using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Serialization;

namespace NKafka.AspNetCore;

/// <summary>
/// Provides a builder for configuring a Kafka cluster and its associated producers and consumers.
/// </summary>
public interface IKafkaClusterBuilder
{
    /// <summary>
    /// Gets the name of the Kafka cluster.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Adds a Kafka producer to the cluster.
    /// </summary>
    /// <typeparam name="TProducer">The type of the producer.</typeparam>
    /// <param name="options">The configuration action for the producer.</param>
    /// <returns>The builder instance.</returns>
    IKafkaClusterBuilder AddProducer<TProducer>(Action<ProducerConfig> options)
        where TProducer : class, IProducer;

    /// <summary>
    /// Adds a Kafka consumer to the cluster.
    /// </summary>
    /// <typeparam name="TConsumer">The type of the consumer.</typeparam>
    /// <typeparam name="TKey">The type of the message key.</typeparam>
    /// <typeparam name="TValue">The type of the message value.</typeparam>
    /// <param name="options">The configuration action for the consumer.</param>
    /// <returns>The builder instance.</returns>
    IKafkaClusterBuilder AddConsumer<TConsumer, TKey, TValue>(Action<ConsumerConfig> options)
        where TConsumer : class, IConsumer<TKey, TValue>;
}