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
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Serialization;

namespace NKafka;

/// <summary>
/// A set of auxiliary functions for creating a producer in the Kafka cluster.
/// </summary>
public static class ProducerExtensions
{
    private const string _PRODUCER_NAME_FORMAT = "__Producer_{0}";

    /// <summary>
    /// Creates a new producer with the specified key and message types
    /// </summary>
    /// <param name="kafkaCluster"></param>
    /// <remarks>If such a producer already exists and has not been destroyed, then he returns</remarks>
    public static IProducer BuildProducer(this IKafkaCluster kafkaCluster)
        => BuildProducer(kafkaCluster, ProducerConfig.EmptyProducerConfig);

    /// <summary>
    /// Creates a new producer with the specified key and message types
    /// </summary>
    /// <param name="kafkaCluster"></param>
    /// <param name="producerConfig">Producer specific configuration</param>
    /// <remarks>If such a producer already exists and has not been destroyed, then he returns</remarks>
    public static IProducer BuildProducer(this IKafkaCluster kafkaCluster, ProducerConfig producerConfig)
    {
        var name = string.Format(_PRODUCER_NAME_FORMAT, Guid.NewGuid());

        return kafkaCluster.BuildProducer(name, producerConfig);
    }

    /// <summary>
    /// Creates a new producer with the specified key and message types and unique name
    /// </summary>
    /// <param name="kafkaCluster"></param>
    /// <param name="name"></param>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>If a producer with the same name has already been registered, then the existing one is returned</returns>
    public static IProducer BuildProducer(this IKafkaCluster kafkaCluster, string name)
        => kafkaCluster.BuildProducer(name, ProducerConfig.EmptyProducerConfig);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="kafkaCluster"></param>
    /// <param name="name"></param>
    /// <param name="producerConfig"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IProducer BuildProducer(this IKafkaCluster kafkaCluster, string name, ProducerConfig producerConfig)
        => kafkaCluster.BuildProducer(name, producerConfig);
}