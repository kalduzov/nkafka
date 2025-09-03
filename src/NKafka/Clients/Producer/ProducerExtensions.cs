// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2024 Aleksey Kalduzov. All rights reserved
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

using System.Runtime.CompilerServices;

namespace NKafka.Clients.Producer;

/// <summary>
/// Extension methods for the producer
/// </summary>
public static class ProducerExtensions
{
    /// <summary>
    /// Produces a message asynchronously to the specified topic.
    /// </summary>
    /// <param name="producer"></param>
    /// <param name="topicName">The name of the topic to which the message will be produced.</param>
    /// <param name="message">The message to be produced.</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the delivery result of the produced message.</returns>
    public static Task<MessageDeliveryResult> ProduceAsync(this IProducer producer,
        string topicName,
        Message message,
        CancellationToken token)
    {
        var topicPartition = new TopicPartition(topicName, Partition.Any);

        return producer.ProduceAsync(topicPartition, message, token);
    }

    /// <summary>
    /// Produces a batch of messages to the specified topic asynchronously.
    /// </summary>
    /// <param name="producer"></param>
    /// <param name="topicName">The name of the topic.</param>
    /// <param name="messages">The messages to be produced.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>Asynchronous enumerable that represents the delivery results for each produced message.</returns>
    public static IAsyncEnumerable<MessageDeliveryResult> ProduceAsync(this IProducer producer,
        string topicName,
        IReadOnlyCollection<Message> messages,
        CancellationToken token)
    {
        var topicPartition = new TopicPartition(topicName, Partition.Any);

        return producer.ProduceAsync(topicPartition, messages, token);
    }

    /// <summary>
    /// Asynchronously produces a batch of messages to the specified topic and partition.
    /// </summary>
    /// <param name="producer"></param>
    /// <param name="topicPartition">The topic and partition to produce the messages to.</param>
    /// <param name="messages">The messages to be produced.</param>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>A sequence of delivery results for each produced message.</returns>
    public static async IAsyncEnumerable<MessageDeliveryResult> ProduceAsync(this IProducer producer,
        TopicPartition topicPartition,
        IReadOnlyCollection<Message> messages,
        [EnumeratorCancellation] CancellationToken token)
    {
        var results = new List<Task<MessageDeliveryResult>>(messages.Count);

        foreach (var message in messages)
        {
            results.Add(producer.ProduceAsync(topicPartition, message, token));
        }

        foreach (var result in results)
        {
            token.ThrowIfCancellationRequested();

            yield return await result;
        }
    }

    /// <summary>
    /// Fire and forget producing a message
    /// </summary>
    /// <param name="producer"></param>
    /// <param name="topicName"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    public static void Produce(this IProducer producer,
        string topicName,
        Message message,
        CancellationToken cancellationToken)
    {
        var topicPartition = new TopicPartition(topicName, Partition.Any);
        producer.Produce(topicPartition, message, cancellationToken, (_, _) => { });
    }

    /// <summary>
    ///     Fire and forget producing messages
    /// </summary>
    /// <param name="producer"></param>
    /// <param name="topicName"></param>
    /// <param name="messages"></param>
    /// <param name="cancellationToken"></param>
    public static void Produce(this IProducer producer,
        string topicName,
        IReadOnlyCollection<Message> messages,
        CancellationToken cancellationToken)
    {
        var topicPartition = new TopicPartition(topicName, Partition.Any);
        producer.Produce(topicPartition, messages, cancellationToken);
    }

    /// <summary>
    ///     Fire and forget producing messages
    /// </summary>
    /// <param name="producer"></param>
    /// <param name="topicPartition"></param>
    /// <param name="messages"></param>
    /// <param name="cancellationToken"></param>
    public static void Produce(this IProducer producer,
        TopicPartition topicPartition,
        IEnumerable<Message> messages,
        CancellationToken cancellationToken)
    {
        foreach (var message in messages)
        {
            producer.Produce(topicPartition, message, cancellationToken, (_, _) => { });
        }
    }
}