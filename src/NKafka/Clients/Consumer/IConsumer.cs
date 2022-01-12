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

using System.Threading.Channels;

namespace NKafka.Clients.Consumer;

public interface IConsumer<TKey, TValue>: IConsumer, IObservable<ConsumerRecord<TKey, TValue>>, IObserver<ConsumerRecord<TKey, TValue>>
{
    /// <summary>
    /// Подписывается на чтение топика и возвращает канал? в который будут прилетать сообщения
    /// </summary>
    /// <remarks>Поавторный вызов метода</remarks>
    ChannelReader<ConsumerRecord<TKey, TValue>> Subscribe(string topicName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topics"></param>
    IReadOnlyCollection<ChannelReader<ConsumerRecord<TKey, TValue>>> Subscribe(IEnumerable<string> topics)
    {
        return topics.Select(Subscribe).ToArray();
    }

    /// <summary>
    /// Отписывается от чтения всех топиков, на которые были подписки
    /// </summary>
    void Unsubscribe();

    /// <summary>
    /// Отписывается от чтения указанных топиков
    /// </summary>
    void Unsubscribe(IReadOnlyCollection<string> topics);
}

public interface IConsumer: IClient
{
}