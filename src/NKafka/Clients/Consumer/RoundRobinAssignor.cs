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

using System.Collections.Immutable;

namespace NKafka.Clients.Consumer;

/// <summary>
///  Протокол ребалансировки round robin
/// </summary>
public sealed class RoundRobinAssignor: IPartitionAssignor
{
    /// <inheritdoc />
    public string Name => "roundrobin";

    /// <inheritdoc />
    public IReadOnlySet<RebalanceProtocol> RebalanceProtocols { get; } = new HashSet<RebalanceProtocol>
    {
        RebalanceProtocol.Eager
    };

    /// <inheritdoc />
    public IDictionary<string, List<TopicPartition>> Assign(IReadOnlyCollection<TopicPartition> topicPartitions, Dictionary<string, Subscription> subscriptions)
    {
        var result = new Dictionary<string, List<TopicPartition>>(subscriptions.Count);

        foreach (var subscription in subscriptions)
        {
            result.Add(subscription.Key, new List<TopicPartition>());
        }

        var value = 0;
        var sortedPartitionsPerTopic = topicPartitions.ToImmutableSortedSet();
        var members = subscriptions.Keys.ToArray();

        foreach (var topicPartition in sortedPartitionsPerTopic)
        {
            string member;
            while (true)
            {
                var index = value++ % members.Length;
                member = members[index];
                if (subscriptions[member].Topics.Contains(topicPartition.Topic))
                {
                    break;
                }
            }

            result[member].Add(topicPartition);
        }

        return result;
    }
}