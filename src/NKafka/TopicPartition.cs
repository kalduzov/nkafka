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

namespace NKafka;

/// <summary>
/// Information about kafka partition
/// </summary>
public class TopicPartition: IComparable<TopicPartition>
{
    /// <summary>
    /// Topic name
    /// </summary>
    public string Topic { get; }

    /// <summary>
    /// 
    /// </summary>
    public Guid TopicId { get; }

    /// <summary>
    /// Partition in topic
    /// </summary>
    public Partition Partition { get; set; }

    /// <summary>
    /// Information about kafka partition
    /// </summary>
    public TopicPartition(string topic, Partition partition, Guid topicId = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            throw new ArgumentNullException(nameof(topic));
        }

        if (partition < Partition.Any) // не может быть значение раздела быть меньше -1 
        {
            throw new ArgumentNullException(nameof(partition));
        }

        Topic = topic;
        TopicId = topicId;
        Partition = partition;
    }

    /// <inheritdoc />
    public override int GetHashCode()
        => Partition.GetHashCode() * 251 + Topic.GetHashCode();

    /// <inheritdoc />
    public int CompareTo(TopicPartition? other)
    {
        if (other is null)
        {
            return 1;
        }
        var topicComparison = string.Compare(Topic, other.Topic, StringComparison.Ordinal);

        return topicComparison != 0 ? topicComparison : Partition.CompareTo(other.Partition);
    }

    /// <inheritdoc />
    public override bool Equals(object? other)
    {
        if (other is not TopicPartition topicPartition)
        {
            return false;
        }

        return Topic.Equals(topicPartition.Topic) && Partition == topicPartition.Partition;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(TopicPartition a, TopicPartition b)
    {
        if (a is null)
        {
            return (b is null);
        }

        return a.Equals(b);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(TopicPartition a, TopicPartition b)
        => !(a == b);

    /// <inheritdoc />
    public override string ToString()
    {
        return "TopicPartition("
               + $"Topic={Topic}"
               + $", Partition={Partition}"
               + $", TopicId={TopicId}"
               + ")";
    }
}