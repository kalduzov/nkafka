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

namespace NKafka.Clients.Producer;

/// <summary>
/// Represents the result of delivering a Kafka message.
/// </summary>
/// <typeparam name="TKey">The type of the message key.</typeparam>
/// <typeparam name="TValue">The type of the message value.</typeparam>
public class DeliveryResult<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private TopicPartition? _topicPartition;
    private TopicPartitionOffset? _topicPartitionOffset;

    /// <summary>
    ///     The topic associated with the message.
    /// </summary>
    public string Topic { get; private set; } = string.Empty;

    /// <summary>
    ///     The partition associated with the message.
    /// </summary>
    public Partition Partition { get; private set; }

    /// <summary>
    ///     The partition offset associated with the message.
    /// </summary>
    public Offset Offset { get; private set; }

    /// <summary>
    ///     The TopicPartition associated with the message.
    /// </summary>
    public TopicPartition TopicPartition
    {
        get { return _topicPartition ??= new TopicPartition(Topic, Partition); }
    }

    /// <summary>
    ///     The TopicPartitionOffset associated with the message.
    /// </summary>
    public TopicPartitionOffset TopicPartitionOffset
    {
        get => _topicPartitionOffset ??= new(Topic, Partition, Offset);
        set
        {
            Topic = value.Topic;
            Partition = value.Partition;
            Offset = value.Offset;

            _topicPartitionOffset = new(Topic, Partition, Offset);
        }
    }

    /// <summary>
    ///     The persistence status of the message
    /// </summary>
    public PersistenceStatus Status { get; private set; }

    /// <summary>
    ///     The Kafka message.
    /// </summary>
    public Message<TKey, TValue> Message { get; private set; }

    /// <summary>
    /// Gets the key associated with the property.
    /// </summary>
    public TKey Key => Message.Key;

    /// <summary>
    /// Gets the value of the property.
    /// </summary>
    /// <remarks>
    /// This property returns the value of the property stored in the Message.Value property.
    /// </remarks>
    public TValue Value => Message.Value;

    /// <summary>
    /// Gets the timestamp of the message.
    /// </summary>
    /// <remarks>
    /// This property returns the timestamp of the message, indicating when the message was generated or received.
    /// </remarks>
    public Timestamp Timestamp => Message.Timestamp;

    /// <summary>
    /// 
    /// </summary>
    public Headers Headers => Message.Headers;

    /// <summary>
    /// Represents the result of delivering a message.
    /// </summary>
    /// <typeparam name="TKey">The type of the message key.</typeparam>
    /// <typeparam name="TValue">The type of the message value.</typeparam>
    public DeliveryResult(Message<TKey, TValue> message, PersistenceStatus persisted, TopicPartitionOffset topicPartitionOffset)
    {
        Message = message;
        Status = persisted;
        TopicPartitionOffset = topicPartitionOffset;
    }
}