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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka.Connection;

/// <summary>
///     Интерфейсы брокера
/// </summary>
public interface IBroker : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     Список топиков обслуживающихся на брокере
    /// </summary>
    IReadOnlyCollection<string> Topics { get; }

    /// <summary>
    ///     Флаг указывающий, что данный брокер является контроллером
    /// </summary>
    bool IsController { get; }

    /// <summary>
    ///     Идентикификатор брокера
    /// </summary>
    int Id { get; }

    /// <summary>
    ///     Точка подключения к брокеру
    /// </summary>
    public EndPoint EndPoint { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? Rack { get; }

    /// <summary>
    ///     Список партиций топиков
    /// </summary>
    IReadOnlyCollection<TopicPartition> TopicPartitions { get; }

    /// <summary>
    /// Открывает соединение с брокером
    /// </summary>
    Task OpenAsync(CommonConfig commonConfig, CancellationToken token);

    /// <summary>
    /// Закрывает соединение с токеном
    /// </summary>
    void Close();

    /// <summary>
    /// Отправка сообщений брокеру по типу fire and forget
    /// </summary>
    void Send<TRequestMessage>(TRequestMessage message)
        where TRequestMessage : RequestBody;

    /// <summary>
    ///     Отправка сообщения брокеру и ожидание получения результата этого сообщения
    /// </summary>
    Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
        where TResponseMessage : KafkaResponseMessage, new()
        where TRequestMessage : RequestBody;
}