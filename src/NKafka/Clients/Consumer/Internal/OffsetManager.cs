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

using System.Collections.Concurrent;

using NKafka.Clients.Producer;
using NKafka.Config;

namespace NKafka.Clients.Consumer.Internal;

/// <inheritdoc />
internal class OffsetManager: IOffsetManager
{
    public AutoOffsetReset AutoOffsetReset { get; }

    private readonly ConcurrentDictionary<TopicPartition, Offset> _readingOffsets = new();
    private volatile ConcurrentDictionary<TopicPartition, Offset> _currentState = new();

    public OffsetManager(AutoOffsetReset autoOffsetReset)
    {
        AutoOffsetReset = autoOffsetReset;
    }

    /// <inheritdoc />
    public void UpdateLastReadingOffset(TopicPartitionOffset topicPartitionOffset)
    {
        _readingOffsets.AddOrUpdate(topicPartitionOffset.TopicPartition, _ => topicPartitionOffset.Offset, (_, _) => topicPartitionOffset.Offset);
    }

    public void UpdateLastWroteOffset(TopicPartitionOffset topicPartitionOffset)
    {
        _currentState
            .AddOrUpdate(topicPartitionOffset.TopicPartition, _ => topicPartitionOffset.Offset, (_, _) => topicPartitionOffset.Offset);
    }

    public void Init(Dictionary<string, TopicPartitionOffset[]> dictionary)
    {
        var initState = new ConcurrentDictionary<TopicPartition, Offset>();

        foreach (var element in dictionary.SelectMany(x => x.Value))
        {
            initState.TryAdd(element.TopicPartition, element.Offset);
            _readingOffsets.TryAdd(element.TopicPartition, element.Offset);
        }

        _currentState = initState;
    }

    /// <summary>
    /// Возвращает текущий offset для выборки из партиции 
    /// </summary>
    public long GetFetchOffset(TopicPartition topicPartition)
    {
        if (_readingOffsets.TryGetValue(topicPartition, out var offset))
        {
            return offset.Value;
        }

        return -1; // Нет офсета
    }

    public IReadOnlyCollection<TopicPartitionOffset> GetAllTopicPartitionOffset()
    {
        // Возвращаем по сути копию текущего стейта по всем смещениям
        return _currentState.Select(x => new TopicPartitionOffset(x.Key, x.Value)).ToArray();
    }

    public void UpdateOffsetForTopicPartition(TopicPartition topicPartition, Offset offset)
    {
        _currentState.AddOrUpdate(topicPartition, _ => offset, (_, _) => offset);
    }

    /// <summary>
    /// Сбрасывае offset в позицию указанную в AutoOffsetReset
    /// </summary>
    /// <param name="topicPartition">Партиция, для которой нужно сбросить offset</param>
    public void ResetOffset(TopicPartition topicPartition)
    {
    }
}