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

using NKafka.Clients.Admin;
using NKafka.Clients.Consumer;
using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Protocol;
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
    Node Controller { get; }

    /// <summary>
    /// List of all cluster brokers
    /// </summary>
    IReadOnlyCollection<Node> Brokers { get; }

    /// <summary>
    /// 
    /// </summary>
    IAdminClient AdminClient { get; }

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
        string name,
        ProducerConfig producerConfig,
        IAsyncSerializer<TKey> keySerializer,
        IAsyncSerializer<TValue> valueSerializer)
        where TKey : notnull
        where TValue : notnull;

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
        IAsyncSerializer<TValue>? valueSerializer = null)
        where TKey : notnull
        where TValue : notnull;

    /// <summary>
    /// Обновляет метаданные по указанным топикам
    /// </summary>
    /// <param name="token"></param>
    /// <param name="topics">Список топиков, по которым необходимо получить информацию из брокеров</param>
    /// <remarks>Если топики не указаны, будет возвращена информация по всем топикам кластера</remarks>
    Task RefreshMetadataAsync(CancellationToken token = default, IEnumerable<string>? topics = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task OpenAsync(CancellationToken token);

    /// <summary>
    /// Отправляет запрос в произвольный брокер
    /// </summary>
    internal Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(
        TRequestMessage message,
        CancellationToken token)
        where TResponseMessage : class, IResponseMessage
        where TRequestMessage : class, IRequestMessage;

    /// <summary>
    /// Отправляет запрос в указанный брокер
    /// </summary>
    internal Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(
        TRequestMessage message,
        int nodeId,
        CancellationToken token)
        where TResponseMessage : class, IResponseMessage
        where TRequestMessage : class, IRequestMessage;

    /// <summary>
    /// Возвращает список доступных разделов для топика
    /// Доступные разделов - это те, к которым сейчас можно обратиться из клиента.
    /// Т.е. брокеры, на которых находятся данные разделы, в сети и к ним можно сделать запрос.
    /// </summary>
    /// <param name="topic">Имя топика</param>
    /// <remarks>
    /// Данный метод возвращает данные, который были получены при вызове методов GetPartitionsAsync, RefreshMetadataAsync
    /// или фоновым обновление данных по кластеру 
    /// </remarks>
    /// <returns>
    /// Список доступных разделов или пустую коллекцию, если такие разделы пока не доступны
    /// </returns>
    IReadOnlyList<Partition> GetAvailablePartitions(string topic);

    /// <summary>
    /// Возвращает лидера для указннаго раздела в топике
    /// </summary>
    Node? LeaderFor(TopicPartition topicPartition);

    /// <summary>
    /// Возвращает информацию о разделах для указанного топика
    /// </summary>
    IReadOnlyCollection<PartitionMetadata> PartitionsForTopic(string topic);
}