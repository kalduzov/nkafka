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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Clients.Producer;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Connection;
using Microlibs.Kafka.Protocol.Responses;

namespace Microlibs.Kafka;

/// <summary>
///     Описывает API работы с kakfa кластером
/// </summary>
public interface IKafkaCluster : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Идентификатор кластера
    /// </summary>
    string? ClusterId { get; }

    /// <summary>
    ///     Конфигурация кластера
    /// </summary>
    ClusterConfig Config { get; }

    /// <summary>
    ///     Список топиков в кластере
    /// </summary>
    /// <remarks>Возвращаются все топики, которые были запрошены или существуют в кластере</remarks>
    IReadOnlyCollection<string> Topics { get; }

    /// <summary>
    ///     Закрыт кластер или открыт
    /// </summary>
    /// <remarks>Из закрытого или уничтоженного кластера невозможно получить никакую информацию</remarks>
    bool Closed { get; }

    /// <summary>
    ///     Информация о брокере, который является контроллером
    /// </summary>
    IBroker Controller { get; }

    /// <summary>
    ///     Список всех брокеров кластера
    /// </summary>
    IReadOnlyCollection<IBroker> Brokers { get; }

    /// <summary>
    /// Возвращает список партиций для топика
    /// </summary>
    /// <remarks>Если топик не найден во внутреннем кеше, то происходит запрос к брокеру за информацией о топике. В случае, если такого топика нет в кафке - вовзращается null</remarks>
    Task<IReadOnlyCollection<Partition>> GetPartitionsAsync(string topic, CancellationToken token = default);

    /// <summary>
    /// Возвращает текущее смещение партиции в топике
    /// </summary>
    Task<Offset> GetOffsetAsync(string topic, Partition partition, CancellationToken token = default);

    /// <summary>
    ///     Создает продюсера связанного с текущим кластером
    /// </summary>
    /// <param name="name">Имя продюсера</param>
    /// <param name="producerConfig">Конфигурация продюсера</param>
    /// <remarks>
    ///     Если продюсер с таким именем уже сушествует, то вернется его экземпляр. Если нет, то будет создан новый
    ///     продюсер
    /// </remarks>
    IProducer<TKey, TValue> BuildProducer<TKey, TValue>(string? name = null, ProducerConfig? producerConfig = null);

    /// <summary>
    ///     Создает консьюмера связанного с текущим кластером
    /// </summary>
    /// <param name="consumeGroupName">Название группы консьюмера</param>
    /// <param name="consumerConfig">Конфигурация консьюмера</param>
    /// <remarks>Метод всегда возвращает новый консьюмер, привязанный к конкретной группе</remarks>
    IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(string consumeGroupName, ConsumerConfig? consumerConfig = null);

    /// <summary>
    ///     Обновляет метаданные по указанным топикам
    /// </summary>
    /// <param name="token"></param>
    /// <param name="topics">Список топиков, по которым необходимо получить информацию из брокеров</param>
    Task<MetadataResponseMessage> RefreshMetadataAsync(CancellationToken token, params string[] topics);
}