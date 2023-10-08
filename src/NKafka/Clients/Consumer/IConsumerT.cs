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

using System.Threading.Channels;

namespace NKafka.Clients.Consumer;

/// <summary>
///  Defines a high-level Apache Kafka consumer (with key and value deserialization).
/// </summary>
public interface IConsumer<TKey, TValue>: IConsumer
    where TKey : notnull
    where TValue : notnull
{
    /// <summary>
    /// List of partitions to which the consumer is currently attached
    /// </summary>
    IReadOnlyList<TopicPartition> Assignment { get; }

    /// <summary>
    /// Subscribes to read a topic and returns the channel to which messages will arrive
    /// </summary>
    /// <remarks>Calling the method again will return the same channel</remarks>
    ValueTask<ChannelReader<ConsumerRecord<TKey, TValue>>> SubscribeAsync(string topicName, CancellationToken token = default);

    /// <summary>
    /// Subscribes to read a group of topics and returns a channel to which messages from all subscribed topics will arrive
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="token"></param>
    /// <remarks>Calling the method again will return the same channel</remarks>
    ValueTask<ChannelReader<ConsumerRecord<TKey, TValue>>> SubscribeAsync(IReadOnlyCollection<string> topics, CancellationToken token = default);

    /// <summary>
    /// Unsubscribes from reading all topics to which there were subscriptions
    /// </summary>
    ValueTask UnsubscribeAsync(CancellationToken token = default);

    /// <summary>
    /// Commit offsets for all subscriptions to topics and partitions to the last read from the channel
    /// </summary>
    /// <param name="token">Cancellation token to complete the operation</param>
    Task CommitOffsetAsync(CancellationToken token = default);
}