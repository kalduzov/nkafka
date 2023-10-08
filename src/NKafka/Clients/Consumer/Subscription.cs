//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
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

using NKafka.Clients.Consumer.Internal;
using NKafka.Config;
using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;
using NKafka.Serialization;

namespace NKafka.Clients.Consumer;

/// <summary>
/// Содержит полную информацию о текущей подписке консьюмера 
/// </summary>
public class Subscription
{
    private readonly SubscriptionSerializer _serializer;
    private static readonly SubscriptionDeserializer _deserializer = new();

    private readonly object _lockObject = new();
    private volatile int _generationId;

    private HashSet<TopicPartition> _assignedTopicPartitions = new();

    /// <summary>
    /// Номер поколения подписки 
    /// </summary>
    public int GenerationId
    {
        get => _generationId;
        set
        {
            lock (_lockObject)
            {
                _generationId = value;
            }
        }
    }

    /// <summary>
    /// Список топиков для этой подписки 
    /// </summary>
    public IReadOnlyCollection<string> Topics { get; }

    /// <summary>
    /// Какие протоколы балансировки поддерживает данная подписка 
    /// </summary>
    public IReadOnlyCollection<IPartitionAssignor> SupportPartitionAssignors { get; }

    /// <summary>
    /// Хранилище offsets для каждой партиции каждого топика, который назначен данной подписке
    /// </summary>
    internal IOffsetManager OffsetManager { get; }

    /// <summary>
    /// Текущие партции по топикам, которые связаны с данной подпиской 
    /// </summary>
    public IReadOnlyDictionary<string, Partition[]> AssignedPartitionsByTopic { get; private set; }

    /// <summary>
    ///  Список топиков-партиций связыннах с данной подпиской после ребалансировки 
    /// </summary>
    public IReadOnlyCollection<TopicPartition> AssignedTopicPartitions => _assignedTopicPartitions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topics">Список топиков подписки</param>
    /// <param name="autoOffsetReset">С какого offsets считывать данные из партиции</param>
    /// <param name="supportPartitionAssignors">Какие протоколы ребалансировки могут использоваться в данной подписке</param>
    /// <param name="generationId">Исходное поколение группы</param>
    /// <param name="apiVersion">Версия апи для сериализации метаданных</param>
    internal Subscription(IReadOnlyCollection<string> topics,
        AutoOffsetReset autoOffsetReset,
        IReadOnlyCollection<IPartitionAssignor> supportPartitionAssignors,
        int generationId = -1,
        ApiVersion apiVersion = ApiVersion.Version1)
    {
        GenerationId = generationId;
        Topics = topics;
        SupportPartitionAssignors = supportPartitionAssignors;
        OffsetManager = new OffsetManager(autoOffsetReset);
        AssignedPartitionsByTopic = new Dictionary<string, Partition[]>();
        _serializer = new SubscriptionSerializer(apiVersion);

    }

    /// <summary>
    /// Сериализует текущую подписку как метаданные для присоединения к группе 
    /// </summary>
    public byte[] SerializeAsMetadata()
    {
        return _serializer.Serialize(this);
    }

    /// <summary>
    /// Десериализует данные из ответа JoinGroup в подписку 
    /// </summary>
    public static Subscription Deserialize(byte[] data)
    {
        return _deserializer.Deserialize(data);
    }

    /// <summary>
    /// Связывает с данной подпиской список TopicPartitions, которые она должна обрабатывтаь после ребалансировки
    /// </summary>
    public void Assign(IReadOnlyCollection<TopicPartition> topicPartitions)
    {
        _assignedTopicPartitions = topicPartitions.ToHashSet();
        AssignedPartitionsByTopic = topicPartitions.GroupBy(g => g.Topic).ToDictionary(x => x.Key, x => x.Select(y => y.Partition).ToArray());
    }

    /// <summary>
    /// Проверяет что в данной подписке сейчас еще есть свзянный раздел с топиком
    /// </summary>
    public bool IsAssignedTopicPartitions(string topic, Partition partition)
    {
        var topicPartitions = new TopicPartition(topic, partition);

        return _assignedTopicPartitions.Contains(topicPartitions);
    }

    private class SubscriptionSerializer: IAsyncSerializer<Subscription>
    {
        private readonly ApiVersion _apiVersion;

        public SubscriptionSerializer(ApiVersion apiVersion)
        {
            _apiVersion = apiVersion;

        }

        public bool PreferAsync => false;

        /// <inheritdoc />
        public Task<byte[]> SerializeAsync(Subscription data)
        {
            return Task.FromResult(Serialize(data));
        }

        /// <inheritdoc />
        public byte[] Serialize(Subscription data)
        {
            var cps = new ConsumerProtocolSubscription
            {
                Topics = new List<string>(data.Topics),
                GenerationId = data.GenerationId,
            };

            using var ms = new MemoryStream();
            var writer = new BufferWriter(ms, 0);
            writer.WriteShort((short)_apiVersion);
            cps.Write(writer, _apiVersion);
            var result = writer.WrittenSpan.ToArray();

            return result;
        }
    }

    private class SubscriptionDeserializer: IAsyncDeserializer<Subscription>
    {
        /// <inheritdoc />
        public Task<Subscription> DeserializeAsync(ReadOnlySpan<byte> data)
        {
            return Task.FromResult(Deserialize(data));
        }

        /// <inheritdoc />
        public Subscription Deserialize(ReadOnlySpan<byte> data)
        {
            var bufferReader = new BufferReader(data);
            var version = bufferReader.ReadShort();
            var cps = new ConsumerProtocolSubscription(ref bufferReader, (ApiVersion)version);

            return new Subscription(cps.Topics, AutoOffsetReset.None, Array.Empty<IPartitionAssignor>());
        }
    }
}