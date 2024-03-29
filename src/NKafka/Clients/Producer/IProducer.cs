﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

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
    /// 
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    IReadOnlyCollection<PartitionMetadata> PartitionsFor(string topic);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeout"></param>
    void Close(TimeSpan timeout);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask CloseAsync(CancellationToken token);

    #region Produce

    /// <summary>
    /// </summary>
    /// <param name="topicName"></param>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topicName, Message<TKey, TValue> message, CancellationToken token = default)
    {
        return ProduceAsync(new TopicPartition(topicName, Partition.Any), message, token);
    }

    /// <summary>
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken token = default);

    /// <summary>
    /// </summary>
    /// <param name="topicName"></param>
    /// <param name="messages"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    IAsyncEnumerable<DeliveryResult<TKey, TValue>> ProduceAsync(
        string topicName,
        IEnumerable<Message<TKey, TValue>> messages,
        CancellationToken token = default)
    {
        return ProduceAsync(new TopicPartition(topicName, Partition.Any), messages, token);
    }

    /// <summary>
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="messages"></param>
    /// <param name="token"></param>
    /// <returns></returns>
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