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
/// 
/// </summary>
public static class ProducerExtensions
{
    private const string _DEFAULT_PRODUCER_NAME_FORMAT = "__DefaultProducer<{0},{1}>";

    /// <summary>
    ///     Создает нового продюсера с указанными типами ключа и сообщения
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <remarks>Если такой продюсер уже существует и не уничтожен - то возвращается он</remarks>
    public static IProducer<TKey, TValue> BuildProducer<TKey, TValue>(this IKafkaCluster kafkaCluster)
        where TKey : notnull
        where TValue : notnull
    {
        return BuildProducer<TKey, TValue>(kafkaCluster, ProducerConfig.EmptyProducerConfig);
    }

    /// <summary>
    ///     Создает нового продюсера с указанными типами ключа и сообщения
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <param name="kafkaCluster"></param>
    /// <param name="producerConfig">Producer specific configuration</param>
    /// <remarks>Если такой продюсер уже существует и не уничтожен - то возвращается он</remarks>
    public static IProducer<TKey, TValue> BuildProducer<TKey, TValue>(this IKafkaCluster kafkaCluster, ProducerConfig producerConfig)
        where TKey : notnull
        where TValue : notnull
    {
        var name = string.Format(_DEFAULT_PRODUCER_NAME_FORMAT, typeof(TKey).Name, typeof(TValue).Name);

        return kafkaCluster.BuildProducer<TKey, TValue>(name, producerConfig);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="kafkaCluster"></param>
    /// <param name="name"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IProducer<TKey, TValue> BuildProducer<TKey, TValue>(this IKafkaCluster kafkaCluster, string name)
        where TKey : notnull
        where TValue : notnull
    {
        return kafkaCluster.BuildProducer<TKey, TValue>(name, ProducerConfig.EmptyProducerConfig);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="kafkaCluster"></param>
    /// <param name="name"></param>
    /// <param name="producerConfig"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IProducer<TKey, TValue> BuildProducer<TKey, TValue>(this IKafkaCluster kafkaCluster, string name, ProducerConfig producerConfig)
        where TKey : notnull
        where TValue : notnull
    {
        return kafkaCluster.BuildProducer(
            name,
            producerConfig,
            NoneSerializer<TKey>.Instance,
            NoneSerializer<TValue>.Instance);
    }
}