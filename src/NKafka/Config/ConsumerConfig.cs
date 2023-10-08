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

using NKafka.Clients.Consumer;
using NKafka.Exceptions;
using NKafka.Metrics;

namespace NKafka.Config;

/// <summary>
/// 
/// </summary>
public record ConsumerConfig: CommonConfig
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly ConsumerConfig EmptyConsumerConfig = new();

    /// <summary>
    /// Поддерживаемые алгоритмы балансировки 
    /// </summary>
    /// <remarks>Порядок укзаания важен, т.к. выбирается по умолчанию первый указанный протокол, если консьюмер единственный в группе.
    /// В остальных случаях будет выбран протокол, который поддерживается всеми констюмерами в группе.
    /// Однако, если ведомый консьюмер не поддерживает ни один протокол лидера - он не сможет присоединиться к группе</remarks>
    public IReadOnlyCollection<IPartitionAssignor> PartitionAssignors { get; set; } = new[] { new RoundRobinAssignor() };

    /// <summary>
    ///     group.id
    /// </summary>
    public string GroupId { get; set; } = null!;

    /// <summary>
    ///  UserData для протокола ребалансировки
    /// </summary>
    public byte[]? GroupUserData { get; set; }

    /// <summary>
    /// Размер канала для принятых сообщений
    /// </summary>
    /// <remarks>После достижения данного лимита сообщений - дальнейшее поведение зависит от параметра <see cref="DropOldRecordsFromChannel"/></remarks>
    public int ChannelSize { get; set; } = 10000;

    /// <summary>
    /// Флаг указывает нужно ли удалять старые сообщения из канала, если он достиг лимита 
    /// </summary>
    /// <remarks>По умалчанию false. В этом случае добавление сообщений в канал остановится, как только он достигнет лимита.
    /// Если выставить значение в true - канал будет отбрасывать самые старые сообщения</remarks>
    public bool DropOldRecordsFromChannel { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public bool EnableAutoCommit { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public int AutoCommitIntervalMs { get; set; } = 5000;

    /// <summary>
    /// Настройки проверки состояния соединений
    /// </summary>
    public HeartbeatSettings Heartbeat { get; set; } = new();

    /// <summary>
    /// Минимальное количество данных, которые нужно извлеч из раздела за один запрос
    /// </summary>
    public int FetchMinBytes { get; set; } = 1;

    /// <summary>
    /// Максимальное количество данных, которое нужно извлечь за один запрос 
    /// </summary>
    public int FetchMaxBytes { get; set; } = 1024 * 1024;

    /// <summary>
    ///  Максимальное время выборки данных из раздела
    /// </summary>
    public int FetchMaxWaitMs { get; set; } = 500;

    /// <summary>
    /// Требуется ли расчет и сверка контрольной суммы после извлечения данных 
    /// </summary>
    public bool CheckCrc32 { get; set; } = false;

    /// <summary>
    /// Уровень изоляции при чтении данных
    /// </summary>
    public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadUncommitted;

    /// <summary>
    /// С какой позиции начать чтение данные из раздела  
    /// </summary>
    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Latest;

    /// <summary>
    /// Список слушателей для различных событий консьюмера
    /// </summary>
    public IReadOnlyCollection<IConsumerEventListener> EventListeners { get; set; } = Array.Empty<IConsumerEventListener>();

    /// <summary>
    /// 
    /// </summary>
    public int SessionTimeoutMs { get; set; } = 45000;

    /// <summary>
    /// 
    /// </summary>
    public int RebalanceTimeoutMs { get; set; } = 300000;

    /// <summary>
    /// Gives the consumer access to the implementation of the metrics provider
    /// </summary>
    public IConsumerMetrics Metrics { get; set; } = new NullConsumerMetrics();

    /// <summary>
    /// Creates a new configuration based on the current one
    /// </summary>
    public static ConsumerConfig BaseFrom(CommonConfig config)
    {
        return new ConsumerConfig
        {
            ClientId = config.ClientId,
            BootstrapServers = config.BootstrapServers,
            ApiVersionRequest = config.ApiVersionRequest,
            MaxRetries = config.MaxRetries
        };
    }

    /// <summary>
    /// Merges the main configuration with the current one
    /// </summary>
    /// <remarks>All parameters of the current configuration are overwritten by the parameters of the main</remarks>
    public ConsumerConfig MergeFrom(CommonConfig config)
    {
        return this with
        {
            BootstrapServers = config.BootstrapServers,
            ApiVersionRequest = config.ApiVersionRequest,
            MaxRetries = config.MaxRetries,
        };
    }

    /// <summary>
    /// Validates the settings and throws an exception if the settings are invalid or missing required ones
    /// </summary>
    internal override void Validate()
    {
        base.Validate();

        if (ChannelSize <= 0)
        {
            throw new KafkaConfigException(nameof(ChannelSize), ChannelSize, "Размер канал для приема сообщения не может быть меньше 1");
        }

        if (string.IsNullOrWhiteSpace(GroupId))
        {
            throw new KafkaConfigException(nameof(GroupId), GroupId, "Группа для консьюмера обязательно должна быть задана");
        }

        if (EnableAutoCommit && AutoCommitIntervalMs <= 0)
        {
            throw new KafkaConfigException(nameof(AutoCommitIntervalMs), AutoCommitIntervalMs, "В настройках включен автокомит, но настроенный интервал меньше 1 мс");
        }

        if (PartitionAssignors.Count == 0)
        {
            throw new KafkaConfigException(nameof(PartitionAssignors), string.Empty, "Не задан ни один тип балансировки. Укажите хотя бы один тип");
        }

        var uniqueName = new HashSet<string>(PartitionAssignors.Count);

        foreach (var assignor in PartitionAssignors)
        {
            if (uniqueName.Add(assignor.Name) is false)
            {
                throw new KafkaConfigException(nameof(PartitionAssignors), string.Empty, "В коллекции типов балансировки содержатся не уникальные значения");
            }
        }

        if (Heartbeat.IntervalMs >= SessionTimeoutMs)
        {
            throw new KafkaConfigException(nameof(Heartbeat.IntervalMs), Heartbeat.IntervalMs, "Нельзя установить Heartbeat интервал больше, чем таймут сессии");

        }
    }
}