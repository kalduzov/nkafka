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
{
    /// <summary>
    /// </summary>
    Task FlushAsync(CancellationToken cancellationToken);

    void Flush(TimeSpan timeout);

    IReadOnlyCollection<PartitionInfo> PartitionsFor(string topic);

    void Close(TimeSpan timeout);

    Task CloseAsync(CancellationToken token);

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
    void Produce(TopicPartition topicPartition, IEnumerable<Message<TKey, TValue>> messages);

    /// <summary>
    /// </summary>
    /// <param name="topicName"></param>
    /// <param name="message"></param>
    /// <param name="deliveryHandler"></param>
    void Produce(string topicName, Message<TKey, TValue> message, Action<DeliveryResult<TKey, TValue>> deliveryHandler)
    {
        Produce(new TopicPartition(topicName, Partition.Any), message, deliveryHandler);
    }

    /// <summary>
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="message"></param>
    /// <param name="deliveryHandler"></param>
    void Produce(TopicPartition topicPartition, Message<TKey, TValue> message, Action<DeliveryResult<TKey, TValue>> deliveryHandler);

    #endregion Produce

    #region Transaction

    void InitTransactions(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    void BeginTransaction()
    {
        throw new NotImplementedException();
    }

    void CommitTransaction(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    void CommitTransaction()
    {
        throw new NotImplementedException();
    }

    void AbortTransaction(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    void AbortTransaction()
    {
        throw new NotImplementedException();
    }

    void SendOffsetsToTransaction(IEnumerable<TopicPartitionOffset> offsets, IConsumerGroupMetadata groupMetadata)
    {
        throw new NotImplementedException();
    }

    #endregion
}

public interface IConsumerGroupMetadata
{
}

public class RecordMetadata
{
}

public class ConsumerGroupMetadata
{
}

public class OffsetAndMetadata
{
}