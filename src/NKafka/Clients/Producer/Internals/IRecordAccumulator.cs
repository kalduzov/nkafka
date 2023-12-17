//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Provides an interface for a record accumulator
/// </summary>
internal interface IRecordAccumulator
{
    /// <summary>
    /// Flushes all pending changes asynchronously.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous flush operation.
    /// </returns>
    Task FlushAllAsync(CancellationToken token);

    /// <summary>
    /// Flushes all data with a given timeout.
    /// </summary>
    /// <param name="timeout">The timeout for flushing all data.</param>
    void FlushAll(TimeSpan timeout);

    /// <summary>
    /// Retrieves a set of batches ready to be sent, with a total size not exceeding the maximum request size.
    /// </summary>
    /// <param name="kafkaCluster">The Kafka cluster from which to pull the batches.</param>
    /// <param name="maxRequestSize">The maximum size allowed for a single request.</param>
    /// <returns>
    /// An enumerable collection of ProducerBatch objects that are ready to be sent.
    /// </returns>
    IEnumerable<ProducerBatch> PullReadyBatches(IKafkaCluster kafkaCluster, int maxRequestSize);

    /// <summary>
    /// Adds a new record to the accumulator
    /// </summary>
    /// <param name="topicPartition">Topic partition for which the entry is added</param>
    /// <param name="timestampUnixTimestampMs">Timestamp for adding an entry</param>
    /// <param name="serializedKey">Serialized uncompressed representation of the key</param>
    /// <param name="serializedValue">Serialized uncompressed representation of the value</param>
    /// <param name="headers">Record headers</param>
    /// <returns>The status of the add record operation</returns>
    RecordAppendResult Append(TopicPartition topicPartition,
        long timestampUnixTimestampMs,
        byte[] serializedKey,
        byte[] serializedValue,
        Headers headers);
}