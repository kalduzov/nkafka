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
/// Partitioner interface
/// </summary>
public interface IPartitioner
{
    /// <summary>
    /// Compute the partition for the given record.
    /// </summary>
    /// <param name="topic">The topic name</param>
    /// <param name="key">The key to partition on or Null if no key</param>
    /// <param name="keyBytes">The serialized key to partition on or empty buffer if no key</param>
    /// <param name="value">The value to partition on or Null</param>
    /// <param name="valueBytes">The serialized value to partition on empty buffer</param>
    /// <param name="cluster"></param>
    /// <param name="token"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    ValueTask<int> PartitionAsync<TKey, TValue>(
        string topic,
        TKey key,
        byte[] keyBytes,
        TValue value,
        byte[] valueBytes,
        IKafkaCluster cluster,
        CancellationToken token = default)
        where TKey : notnull
        where TValue : notnull;
}