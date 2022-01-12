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
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NKafka.Clients.Producer;

namespace NKafka.Clients.Consumer;

public class ConsumerRecord<TKey, TValue>
{
    /// <summary>
    ///     The topic associated with the message.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    ///     The partition associated with the message.
    /// </summary>
    public Partition Partition { get; set; }

    /// <summary>
    ///     The partition offset associated with the message.
    /// </summary>
    public Offset Offset { get; set; }

    /// <summary>
    ///     The TopicPartition associated with the message.
    /// </summary>
    public TopicPartition TopicPartition => new(Topic, Partition);

    /// <summary>
    ///     The TopicPartitionOffset associated with the message.
    /// </summary>
    public TopicPartitionOffset TopicPartitionOffset
    {
        get => new(Topic, Partition, Offset);
        set
        {
            Topic = value.Topic;
            Partition = value.Partition;
            Offset = value.Offset;
        }
    }

    /// <summary>
    ///     The Kafka message, or null if this ConsumeResult
    ///     instance represents an end of partition event.
    /// </summary>
    public Message<TKey, TValue> Message { get; set; }

    /// <summary>
    ///     True if this instance represents an end of partition
    ///     event, false if it represents a message in kafka.
    /// </summary>
    public bool IsPartitionEOF { get; set; }
}