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

using NKafka.Clients.Producer;
using NKafka.Exceptions;

using CEM = NKafka.Resources.ConfigExceptionMessages;

namespace NKafka.Config;

/// <summary>
/// Represents the configuration for message partitioning.
/// </summary>
public class PartitionerConfig
{
    /// <summary>
    /// Algorithm type for distributing messages into partitions 
    /// </summary>
    public Partitioner Partitioner { get; set; } = Partitioner.Default;

    /// <summary>
    /// Class type for custom partitioning algorithm
    /// </summary>
    public Type CustomPartitionerClass { get; set; } = typeof(object);

    internal void Validate()
    {
        if (CustomPartitionerClass != typeof(object))
        {
            if (Partitioner != Partitioner.Custom)
            {
                throw new KafkaConfigException(nameof(Partitioner), Partitioner, CEM.PartitionerConfig_TypePartitionerInvalid);
            }

            if (!typeof(IPartitioner).IsAssignableFrom(CustomPartitionerClass))
            {
                throw new KafkaConfigException(nameof(CustomPartitionerClass), CustomPartitionerClass, CEM.PartitionerConfig_InterfaceInvalid);
            }
        }

        if (CustomPartitionerClass == typeof(object) && Partitioner == Partitioner.Custom)
        {
            throw new KafkaConfigException(nameof(CustomPartitionerClass), CustomPartitionerClass, CEM.PartitionerConfig_CustomClassNotFound);
        }
    }
}