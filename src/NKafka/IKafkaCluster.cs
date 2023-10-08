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
using NKafka.Connection;
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
    IDictionary<string, TopicMetadata> Topics { get; }

    /// <summary>
    /// 
    /// </summary>
    IDictionary<Guid, string> TopicsById { get; }

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
    /// <exception cref="ProtocolKafkaException">Throws if there is no such topic in kafka</exception>
    ValueTask<IReadOnlyCollection<Partition>> GetPartitionsAsync(string topic, CancellationToken token = default);

    /// <summary>
    /// Возвращает список партиций для указанных топиков
    /// </summary>
    /// <param name="topics">Список топиков, для которых нужно получить партиции</param>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<IReadOnlyCollection<TopicPartition>> GetTopicPartitionsAsync(IReadOnlyCollection<string> topics, CancellationToken token = default);

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
    /// Creates a consumer associated with the current cluster
    /// </summary>
    /// <param name="consumerConfig">Конфигурация консьюмера</param>
    /// <param name="keyDeserializer">The configured key deserializer</param>
    /// <param name="valueDeserializer">The configured value deserializer.</param>
    /// <remarks>The method always returns a new consumer associated with a specific group</remarks>
    IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(ConsumerConfig consumerConfig,
        IAsyncDeserializer<TKey> keyDeserializer,
        IAsyncDeserializer<TValue> valueDeserializer)
        where TKey : notnull
        where TValue : notnull;

    /// <summary>
    /// Updates metadata for the specified topics
    /// </summary>
    /// <param name="token"></param>
    /// <param name="topics">List of topics for which you need to get information from brokers</param>
    /// <remarks>If no topics are specified, information on all cluster topics will be returned</remarks>
    Task RefreshMetadataAsync(IEnumerable<string> topics, CancellationToken token = default);

    /// <summary>
    /// Opens a network connection to a kafka broker and initializes metadata for the entire kafka cluster
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task OpenAsync(CancellationToken token);

    /// <summary>
    /// Returns a list of available partitions for a topic.
    /// Available partitions are those that can now be accessed from the client
    /// In other words, the brokers on which these sections are located are online and you can make a request to them
    /// </summary>
    /// <param name="topic">Topic name</param>
    /// <remarks>
    /// This method returns the data that was received when calling the GetPartitionsAsync, RefreshMetadataAsync methods
    /// or background metadata update on the cluster 
    /// </remarks>
    /// <returns>
    /// List of available partitions or an empty collection if such sections are not yet available
    /// </returns>
    IReadOnlyList<Partition> GetAvailablePartitions(string topic);

    /// <summary>
    /// Returns the leader for the specified partition in the topic
    /// </summary>
    /// <returns>A valid node for the specified partition, or NoNode if the node was not found</returns>
    Node LeaderFor(TopicPartition topicPartition);

    /// <summary>
    /// Returns partitions metadata information for the specified topic
    /// </summary>
    IReadOnlyCollection<PartitionMetadata> PartitionsForTopic(string topic);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    TopicMetadata GetTopicMetadata(string name);

    /// <summary>
    /// Sends a request to the cluster
    /// </summary>
    /// <param name="message">Request to send to the broker</param>
    /// <param name="token"></param>
    /// <remarks>The broker for processing the request will be selected the least loaded. If the request requires a controller, it will be selected</remarks>
    internal Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(
        TRequestMessage message,
        CancellationToken token)
        where TResponseMessage : class, IResponseMessage
        where TRequestMessage : class, IRequestMessage;

    /// <summary>
    /// Sends a request to the specified broker
    /// </summary>
    /// <param name="message">Request to send to the broker</param>
    /// <param name="nodeId">Broker ID</param>
    /// <param name="token"></param>
    internal Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(
        TRequestMessage message,
        int nodeId,
        CancellationToken token)
        where TResponseMessage : class, IResponseMessage
        where TRequestMessage : class, IRequestMessage;

    /// <summary>
    /// Notifies the cluster that the consumer has been disposed
    /// </summary>
    internal void NotifyAboutDisposedConsumer(IConsumer consumer);

    /// <summary>
    /// Предоставляет выделенный коннектор к конкретному брокеру
    /// </summary>
    /// <returns>Выделенный коннектор - это отдельный физический канал к брокеру.
    /// В основном используется для работы нескольких консьюмеров одного кластера в едином адресном пространстве процесса</returns>
    internal IKafkaConnector ProvideDedicateConnector(int nodeId);

    /// <summary>
    /// Возвращает метаданные кластера
    /// </summary>
    internal ClusterMetadata GetClusterMetadata();
}