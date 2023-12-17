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

using System.Runtime.CompilerServices;

using NKafka.Protocol;

namespace NKafka.Clients.Producer;

/// <summary>
///     Маркерный интерфейс для продюсера
/// </summary>
public interface IProducer: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Producer internal name
    /// </summary>
    internal string Name { get; }
}

/// <summary>
///  Defines a high-level Apache Kafka producer client that provides key and value serialization <see cref="Producer{TKey,TValue}"/>
/// </summary>
/// <typeparam name="TKey">Key type</typeparam>
/// <typeparam name="TValue">Value type</typeparam>
public interface IProducer<TKey, TValue>: IProducer
    where TKey : notnull
    where TValue : notnull
{
    /// <summary>
    /// Sends all pending accumulated messages and waits for a response to the result of the send
    /// </summary>
    /// <param name="token"></param>
    Task FlushAsync(CancellationToken token);

    /// <summary>
    /// Sends all pending accumulated messages and waits for a response to the result of the send
    /// </summary>
    /// <param name="timeout">Send timeout</param>
    /// <remarks>The method blocks for the timeout</remarks>
    void Flush(TimeSpan timeout);

    /// <summary>
    /// Retrieves the partitions metadata for a given topic.
    /// </summary>
    /// <param name="topic">The name of the topic.</param>
    /// <returns>A read-only collection of PartitionMetadata objects representing the partitions for the topic.</returns>
    IReadOnlyCollection<PartitionMetadata> PartitionsFor(string topic);

    /// <summary>
    /// Closes the application with the specified timeout.
    /// </summary>
    /// <param name="timeout">The duration to wait before closing the application.</param>
    void Close(TimeSpan timeout);

    /// <summary>
    /// Closes the asynchronous operation.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask CloseAsync(CancellationToken token);

    #region Produce

    /// <summary>
    /// Produces a message asynchronously to the specified topic.
    /// </summary>
    /// <param name="topicName">The name of the topic to which the message will be produced.</param>
    /// <param name="message">The message to be produced.</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the delivery result of the produced message.</returns>
    Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topicName, Message<TKey, TValue> message, CancellationToken token = default)
    {
        return ProduceAsync(new TopicPartition(topicName, Partition.Any), message, token);
    }

    /// <summary>
    /// Asynchronously produces a message to the specified topic and partition.
    /// </summary>
    /// <param name="topicPartition">The topic and partition to produce the message to.</param>
    /// <param name="message">The message to produce.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the delivery result.</returns>
    Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken token = default);

    /// <summary>
    /// Produces a batch of messages to the specified topic asynchronously.
    /// </summary>
    /// <typeparam name="TKey">The type of the message key.</typeparam>
    /// <typeparam name="TValue">The type of the message value.</typeparam>
    /// <param name="topicName">The name of the topic.</param>
    /// <param name="messages">The messages to be produced.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable that represents the delivery results for each produced message.</returns>
    IAsyncEnumerable<DeliveryResult<TKey, TValue>> ProduceAsync(
        string topicName,
        IEnumerable<Message<TKey, TValue>> messages,
        CancellationToken token = default)
    {
        return ProduceAsync(new TopicPartition(topicName, Partition.Any), messages, token);
    }

    /// <summary>
    /// Asynchronously produces a batch of messages to the specified topic and partition.
    /// </summary>
    /// <param name="topicPartition">The topic and partition to produce the messages to.</param>
    /// <param name="messages">The messages to be produced.</param>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>A sequence of delivery results for each produced message.</returns>
    async IAsyncEnumerable<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        IEnumerable<Message<TKey, TValue>> messages,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var results = messages.Select(message => ProduceAsync(topicPartition, message, token));

        foreach (var result in results)
        {
            token.ThrowIfCancellationRequested();

            yield return await result;
        }
    }

    /// <summary>
    ///     Fire and forget produce message
    /// </summary>
    /// <param name="topicName"></param>
    /// <param name="message"></param>
    void Produce(string topicName, Message<TKey, TValue> message)
    {
        Produce(new TopicPartition(topicName, Partition.Any), message);
    }

    /// <summary>
    ///     Fire and forget produce message
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="message"></param>
    void Produce(TopicPartition topicPartition, Message<TKey, TValue> message);

    /// <summary>
    ///     Fire and forget produce message
    /// </summary>
    /// <param name="topicName"></param>
    /// <param name="messages"></param>
    void Produce(string topicName, IEnumerable<Message<TKey, TValue>> messages)
    {
        Produce(new TopicPartition(topicName, Partition.Any), messages);
    }

    /// <summary>
    ///     Fire and forget produce message
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="messages"></param>
    void Produce(TopicPartition topicPartition, IEnumerable<Message<TKey, TValue>> messages)
    {
        foreach (var message in messages)
        {
            Produce(topicPartition, message);
        }
    }

    #endregion Produce

    // #region Transaction
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="timeout"></param>
    // /// <exception cref="NotImplementedException"></exception>
    // void InitTransactions(TimeSpan timeout);
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // /// <exception cref="NotImplementedException"></exception>
    // void BeginTransaction()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="timeout"></param>
    // /// <exception cref="NotImplementedException"></exception>
    // void CommitTransaction(TimeSpan timeout)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // /// <exception cref="NotImplementedException"></exception>
    // void CommitTransaction()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="timeout"></param>
    // /// <exception cref="NotImplementedException"></exception>
    // void AbortTransaction(TimeSpan timeout)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // /// <exception cref="NotImplementedException"></exception>
    // void AbortTransaction()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="offsets"></param>
    // /// <param name="groupMetadata"></param>
    // /// <exception cref="NotImplementedException"></exception>
    // void SendOffsetsToTransaction(IEnumerable<TopicPartitionOffset> offsets, IConsumerGroupMetadata groupMetadata)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // #endregion
}