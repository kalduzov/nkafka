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

namespace NKafka.Connection;

/// <summary>
/// </summary>
internal sealed class Broker: IBroker, IEquatable<Broker>
{
    private volatile IReadOnlyDictionary<string, TopicPartition> _topicPartitions;
    private volatile IReadOnlySet<string> _topics;

    /// <summary>
    ///     Флаг указывающий, что данный брокер является контроллером
    /// </summary>
    public bool IsController { get; private set; }

    /// <summary>
    ///     Идентикификатор брокера
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 
    /// </summary>
    public string? Rack { get; private set; }

    /// <summary>
    ///     Точка подключения к брокеру
    /// </summary>
    public EndPoint EndPoint { get; set; }

    /// <summary>
    ///     Список партиций топиков связанных с этим брокером
    /// </summary>
    public IReadOnlyDictionary<string, TopicPartition> TopicPartitions => _topicPartitions;

    /// <summary>
    /// Топики
    /// </summary>
    public IReadOnlySet<string> Topics => _topics;

    public Broker(
        EndPoint endpoint,
        int id = -1,
        string? rack = null,
        bool isController = false)
    {
        Id = id;
        Rack = rack;
        EndPoint = endpoint;
        IsController = isController;
    }

    /// <summary>
    /// Обновляет информацию о брокере
    /// </summary>
    void IBroker.UpdateInfo(EndPoint endpoint, string? rack, bool isController)
    {
        if (EndPoint != endpoint)
        {
            EndPoint = endpoint;
        }

        if (Rack != rack)
        {
            Rack = rack;
        }

        IsController = isController;
    }

    void IBroker.UpdateTopicsAndPartitions(string messageTopicName, Partition partition)
    {
    }

    public bool Equals(Broker? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EndPoint.Equals(other.EndPoint);
    }

    public override int GetHashCode()
    {
        return Id;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is Broker other && Equals(other));
    }
}