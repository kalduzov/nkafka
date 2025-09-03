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

using NKafka.Clients.Consumer;
using NKafka.Config;

namespace NKafka.Clients.Producer;

/// <summary>
/// Base interface for the producer
/// </summary>
public interface IProducer: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Producer internal name
    /// </summary>
    internal string Name { get; }

    /// <summary>
    /// Sends all pending accumulated messages and waits for a response to confirm their delivery
    /// </summary>
    /// <param name="token"></param>
    public Task Flush(CancellationToken token);

    /// <summary>
    /// Closes the asynchronous operation.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public ValueTask Close(CancellationToken token);

    #region Produce

    /// <summary>
    /// Asynchronously produces a message to the specified topic and partition.
    /// </summary>
    /// <param name="topicPartition">The topic and partition to produce the message to.</param>
    /// <param name="message">The message to produce.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the delivery result.</returns>
    public Task<MessageDeliveryResult> ProduceAsync(
        TopicPartition topicPartition,
        Message message,
        CancellationToken token);

    /// <summary>
    ///  
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <param name="callback"></param>
    public void Produce(TopicPartition topicPartition,
        Message message,
        CancellationToken token,
        Action<MessageDeliveryResult, Exception?> callback);

    #endregion Produce

    #region Transaction

    /// <summary>
    /// Needs to be called before any other methods when the <see cref="ProducerConfig.TransactionalId"/> is set in the configuration.
    /// </summary>
    /// <param name="token"></param>
    Task InitTransactions(CancellationToken token);

    /// <summary>
    /// Should be called before the start of each new transaction.
    /// </summary>
    void BeginTransaction();

    /// <summary>
    /// Commits the ongoing transaction
    /// </summary>
    Task CommitTransaction(CancellationToken token);

    /// <summary>
    /// Aborts the ongoing transaction
    /// </summary>
    Task AbortTransaction(CancellationToken token);

    /// <summary>
    /// Sends a list of specified offsets to the consumer group coordinator, and also marks those offsets as part of the current transaction.
    /// </summary>
    /// <param name="offsets"></param>
    /// <param name="groupMetadata"></param>
    /// <param name="token"></param>
    Task SendOffsetsToTransaction(IReadOnlyCollection<TopicPartitionOffset> offsets, ConsumerGroupMetadata groupMetadata, CancellationToken token);

    #endregion
}