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

using NKafka.Clients.Consumer;
using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Serialization;

namespace NKafka;

/// <summary>
/// Provides a starting point for working with a Kafka cluster
/// </summary>
public interface IKafkaCluster: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Cluster identifier 
    /// </summary>
    string? ClusterId { get; }

    /// <summary>
    /// Cluster configuration
    /// </summary>
    ClusterConfig Config { get; }

    /// <summary>
    /// List of topics in the cluster
    /// </summary>
    /// <remarks>Depending on the situation, all topics that were requested or exist in the cluster are returned</remarks>
    IReadOnlyCollection<string> Topics { get; }

    /// <summary>
    /// Закрыт кластер или открыт
    /// </summary>
    /// <remarks>Из закрытого или уничтоженного кластера невозможно получить никакую информацию</remarks>
    bool Closed { get; }

    /// <summary>
    /// Information about the broker that is the controller
    /// </summary>
    IBroker? Controller { get; }

    /// <summary>
    /// List of all cluster brokers
    /// </summary>
    IReadOnlyCollection<IBroker> Brokers { get; }

    /// <summary>
    /// Returns a list of partitions for a topic
    /// </summary>
    /// <remarks>If the data is not in the internal cache, then a request is made to the broker for this information.</remarks>
    /// <exception cref="ProtocolKafkaException">Бросается в случае, если такого топика нет в кафке</exception>
    ValueTask<IReadOnlyCollection<Partition>> GetPartitionsAsync(string topic, CancellationToken token = default);

    /// <summary>
    /// Returns the current partition offset in the topic
    /// </summary>
    /// <remarks>If the data is not in the internal cache, then a request is made to the broker for this information</remarks>
    ValueTask<Offset> GetOffsetAsync(string topic, Partition partition, CancellationToken token = default);

    /// <summary>
    /// Creates a producer associated with the current cluster
    /// </summary>
    /// <param name="name">Producer name</param>
    /// <param name="producerConfig">Producer сonfiguration</param>
    /// <param name="keySerializer">Key serializer implementation</param>
    /// <param name="valueSerializer">Value serializer implementation</param>
    /// <remarks>
    /// <p>
    /// If a producer with the same name already exists, an instance of it will be returned. If not, a new producer will be created.
    /// </p>
    /// <p>
    /// If no producer name is given, then the producer that was created with the default name will be returned.
    /// </p>
    /// <p>
    /// The producer configuration will override some of the values that were set to create the cluster.
    /// </p>
    /// </remarks>
    IProducer<TKey, TValue> BuildProducer<TKey, TValue>(
        string name = KafkaCluster.DEFAULT_PRODUCER_NAME,
        ProducerConfig? producerConfig = null,
        IAsyncSerializer<TKey>? keySerializer = null,
        IAsyncSerializer<TValue>? valueSerializer = null);

    /// <summary>
    /// Создает консьюмера связанного с текущим кластером
    /// </summary>
    /// <param name="consumeGroupName">Название группы консьюмера</param>
    /// <param name="consumerConfig">Конфигурация консьюмера</param>
    /// <param name="keySerializer"></param>
    /// <param name="valueSerializer"></param>
    /// <remarks>Метод всегда возвращает новый консьюмер, привязанный к конкретной группе</remarks>
    IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(
        string consumeGroupName,
        ConsumerConfig? consumerConfig = null,
        IAsyncSerializer<TKey>? keySerializer = null,
        IAsyncSerializer<TValue>? valueSerializer = null);

    /// <summary>
    /// Обновляет метаданные по указанным топикам
    /// </summary>
    /// <param name="token"></param>
    /// <param name="topics">Список топиков, по которым необходимо получить информацию из брокеров</param>
    /// <remarks>Если топики не указаны, будет возвращена информация по всем топикам кластера</remarks>
    Task<MetadataResponseMessage> RefreshMetadataAsync(CancellationToken token = default, params string[] topics);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task OpenAsync(CancellationToken token);
}