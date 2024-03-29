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

using NKafka.Exceptions;

using EM = NKafka.Resources.ExceptionMessages;

namespace NKafka.Clients.Producer;

/// <summary>
///  Simple random partitioner
/// </summary> 
internal class DefaultPartitioner: IPartitioner
{
    private readonly Random _random = new();

    /// <inheritdoc/> 
    public ValueTask<int> PartitionAsync<TKey, TValue>(
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
        var availablePartitions = cluster.GetAvailablePartitions(topic);

        if (availablePartitions.Count == 0)
        {
            var message = string.Format(EM.Producer_NoAvailablePartitions, topic);

            throw new ProduceException(message);
        }

        var maxPartitionIndex = availablePartitions.Max();

        return new ValueTask<int>(_random.Next(maxPartitionIndex + 1));
    }
}