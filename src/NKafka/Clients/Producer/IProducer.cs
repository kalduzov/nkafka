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
/// Base interface for the producer
/// </summary>
public interface IProducer: IAsyncDisposable
{
    /// <summary>
    /// Sends all pending accumulated messages and waits for a response to confirm their delivery
    /// </summary>
    /// <param name="token"></param>
    public Task FlushAsync(CancellationToken token);

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
    /// Sends a message without awaiting the result and invokes the callback after delivery or failure.
    /// </summary>
    /// <param name="topicPartition">The target topic and partition.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="callback">The callback receiving the delivery result or error.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public void Produce(
        TopicPartition topicPartition,
        Message message,
        Action<MessageDeliveryResult, Exception?> callback,
        CancellationToken cancellationToken);

    #endregion Produce

}
