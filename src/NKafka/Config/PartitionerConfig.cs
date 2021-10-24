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

namespace NKafka.Config;

public class PartitionerConfig
{
    /// <summary>
    /// Тип алгоритма распределения сообщений по разделам 
    /// </summary>
    public Partitioner Partitioner { get; set; } = Partitioner.Default;

    /// <summary>
    /// Тип класса для пользовательского алгоритма распределения по разделам. 
    /// </summary>
    public Type CustomPartitionerClass { get; set; } = typeof(object);

    internal void Validate()
    {
        if (CustomPartitionerClass != typeof(object))
        {
            if (Partitioner != Partitioner.Custom)
            {
                throw new KafkaConfigException(
                    nameof(Partitioner),
                    Partitioner,
                    "В настройках указан пользовательский класс алгоритма распределения по разделам, но тип распределения указан отличный от Custom");
            }

            if (!typeof(IPartitioner).IsAssignableFrom(CustomPartitionerClass))
            {
                throw new KafkaConfigException(
                    nameof(CustomPartitionerClass),
                    CustomPartitionerClass,
                    "Указанный пользовательский тип не наследует интерфейс 'IPartitioner'");
            }
        }

        if (CustomPartitionerClass == typeof(object) && Partitioner == Partitioner.Custom)
        {
            throw new KafkaConfigException(
                nameof(CustomPartitionerClass),
                CustomPartitionerClass,
                "В настройках указан выбран тип распределения по разделам Custom, но не указан пользовательский класс");
        }
    }
}