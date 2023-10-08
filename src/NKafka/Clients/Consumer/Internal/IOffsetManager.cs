//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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
using NKafka.Config;

namespace NKafka.Clients.Consumer.Internal;

/// <summary>
/// Storage of offsets by partitions for a specific consumer
/// </summary>
internal interface IOffsetManager
{
    /// <summary>
    /// 
    /// </summary>
    AutoOffsetReset AutoOffsetReset { get; }

    /// <summary>
    /// Updates the last read offset from the consumer channel 
    /// </summary>
    void UpdateLastReadingOffset(TopicPartitionOffset topicPartitionOffset);

    /// <summary>
    /// Обновляет offset партиции последнего записанного сообщения в канал 
    /// </summary>
    void UpdateLastWroteOffset(TopicPartitionOffset itemTopicPartitionOffset);

    /// <summary>
    /// Инициирует новое состояние смещениями для указанных разделов топиков
    /// </summary>
    void Init(Dictionary<string, TopicPartitionOffset[]> dictionary);

    /// <summary>
    /// Возврвщает последний записанный offset для указанного раздела
    /// </summary>
    long GetFetchOffset(TopicPartition topicPartition);

    /// <summary>
    /// 
    /// </summary>
    IReadOnlyCollection<TopicPartitionOffset> GetAllTopicPartitionOffset();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topicPartition"></param>
    /// <param name="offset"></param>
    void UpdateOffsetForTopicPartition(TopicPartition topicPartition, Offset offset);

    /// <summary>
    /// Сбрасывае offset в позицию указанную в AutoOffsetReset
    /// </summary>
    /// <param name="topicPartition">Партиция, для которой нужно сбросить offset</param>
    void ResetOffset(TopicPartition topicPartition);
}