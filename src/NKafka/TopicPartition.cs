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
public class TopicPartition
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
}