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

using System.Net;

using NKafka.Protocol;

namespace NKafka.Connection;

/// <summary>
/// Интерфейс брокера
/// </summary>
public interface IBroker : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Список топиков обслуживающихся на брокере
    /// </summary>
    IReadOnlySet<string> Topics { get; }

    /// <summary>
    /// Флаг указывающий, что данный брокер является контроллером
    /// </summary>
    bool IsController { get; }

    /// <summary>
    /// The broker ID
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Точка подключения к брокеру
    /// </summary>
    public EndPoint EndPoint { get; set; }

    /// <summary>
    /// The rack of the broker, or null if it has not been assigned to a rack.
    /// </summary>
    string? Rack { get; }

    /// <summary>
    /// Список партиций топиков
    /// </summary>
    IReadOnlyDictionary<string, TopicPartition> TopicPartitions { get; }
    
    /// <summary>
    /// The current number of inflight requests
    /// </summary>
    public int CurrentNumberInflightRequests { get; }

    public Task OpenAsync(CancellationToken token);
    
    /// <summary>
    /// Закрывает соединение с брокером
    /// </summary>
    public Task CloseAsync(CancellationToken token);

    /// <summary>
    /// Отправка сообщений брокеру по типу fire and forget
    /// </summary>
    internal void Send<TRequestMessage>(TRequestMessage message)
        where TRequestMessage : RequestMessage;

    /// <summary>
    /// Отправка сообщения брокеру и ожидание получения результата этого сообщения
    /// </summary>
    internal Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
        where TResponseMessage : ResponseMessage
        where TRequestMessage : RequestMessage;

    /// <summary>
    /// Обновляет информацию о брокере
    /// </summary>
    internal void UpdateInfo(EndPoint endpoint, string? rack, bool isController);

    /// <summary>
    /// Обновляет информацию по топикам и партициям которые обслуживаются данным брокером
    /// </summary>
    internal void UpdateTopicsAndPartitions(string messageTopicName, Partition partition);
}