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

using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace NKafka.Clients.Consumer;

/// <summary>
/// 
/// </summary>
public class RangeAssignor: IPartitionAssignor
{
    /// <summary>
    /// The name partition assignor 
    /// </summary>
    public string Name => "range";

    /// <summary>
    /// Returns a list of valid protocols
    /// </summary>
    public IReadOnlySet<RebalanceProtocol> RebalanceProtocols { get; } = new HashSet<RebalanceProtocol>
    {
        RebalanceProtocol.Eager
    };

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IDictionary<string, List<TopicPartition>> Assign(IReadOnlyCollection<TopicPartition> topicPartitions, Dictionary<string, Subscription> subscriptions)
    {
        var result = new Dictionary<string, List<TopicPartition>>(subscriptions.Count);

        foreach (var subscription in subscriptions)
        {
            result.Add(subscription.Key, new List<TopicPartition>());
        }

        var sortedPartitionsPerTopic = topicPartitions
            .ToImmutableSortedSet()
            .GroupBy(x => x.Topic);

        foreach (var topic in sortedPartitionsPerTopic)
        {
            var p = topic.ToArray();
            var subscriptionContainTopic = subscriptions.Where(x => x.Value.Topics.Contains(topic.Key)).ToArray();
            var maxCountInGroup = CalculateMaxCountInGroup(p.Length, subscriptionContainTopic.Length);

            var index = 0;

            foreach (var subscription in subscriptionContainTopic)
            {
                var chunk = GetChunk(p, index, maxCountInGroup);
                result[subscription.Key].AddRange(chunk);
                index += maxCountInGroup;
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<TopicPartition> GetChunk(TopicPartition[] elements, int startIndex, int maxCountInGroup)
    {
        var endIndex = Math.Min(maxCountInGroup + startIndex, elements.Length);

        return elements[startIndex..endIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateMaxCountInGroup(int countElements, int countGroups)
    {
        return countElements / countGroups + countElements % countGroups;
    }
}