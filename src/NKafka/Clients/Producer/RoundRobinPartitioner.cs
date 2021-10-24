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

using System.Collections.Concurrent;

namespace NKafka.Clients.Producer;

/// <summary>
/// The "Round-Robin" partitioner
/// </summary>
internal class RoundRobinPartitioner: IPartitioner
{
    private readonly ConcurrentDictionary<string, ThreadSafeCounter> _topicCounterMap = new();

    ///<inheritdoc /> 
    public async ValueTask<int> PartitionAsync<TKey, TValue>(
        string topic,
        TKey key,
        byte[] keyBytes,
        TValue value,
        byte[] valueBytes,
        IKafkaCluster cluster,
        CancellationToken token = default)
        where TKey : notnull
        where TValue : notnull
    {
        var partitions = await cluster.GetPartitionsAsync(topic, token);
        var numPartitions = partitions.Count;
        var nextValue = NextValue(topic);

        var availablePartitions = cluster.GetAvailablePartitions(topic);

        if (availablePartitions.Count == 0)
        {
            return (int)(nextValue % numPartitions);
        }

        var part = nextValue % availablePartitions.Count;

        return availablePartitions[(int)part].Value;
    }

    private uint NextValue(string topic)
    {
        var counter = _topicCounterMap.GetOrAdd(topic, new ThreadSafeCounter());

        return counter.GetAndIncrement();
    }
}