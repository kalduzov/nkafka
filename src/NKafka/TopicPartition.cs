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

using System.Collections;

namespace NKafka;

/// <summary>
/// Information about kafka partition
/// </summary>
public class TopicPartition: IEqualityComparer<TopicPartition>
{
    /// <summary>
    /// Topic name
    /// </summary>
    public string Topic { get; init; }

    /// <summary>
    /// Partition in topic
    /// </summary>
    public Partition Partition { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="partition"></param>
    public TopicPartition(string topic, Partition partition)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            throw new ArgumentNullException(nameof(topic));
        }

        if (partition < Partition.Any) //не может быть значение раздела быть меньше -1 
        {
            throw new ArgumentNullException(nameof(partition));
        }

        Topic = topic;
        Partition = partition;
    }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Topic, Partition);
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(this, (TopicPartition)obj);
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"Topic = {Topic}, Partition = {Partition}";
    }

    public bool Equals(TopicPartition x, TopicPartition y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (ReferenceEquals(x, null))
        {
            return false;
        }

        if (ReferenceEquals(y, null))
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return x.Topic == y.Topic
               && x.Partition.Equals(y.Partition);
    }

    public int GetHashCode(TopicPartition obj)
    {
        return HashCode.Combine(obj.Topic, obj.Partition);
    }
}