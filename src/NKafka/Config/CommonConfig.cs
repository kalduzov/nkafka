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
using System.Reflection;

using NKafka.Exceptions;

namespace NKafka.Config;

/// <summary>
///     Общие параметры конфигурации для всех клиентов
/// </summary>
public abstract record CommonConfig
{
    /// <summary>
    ///     Первичный список брокеров 
    /// </summary>
    public IReadOnlyList<string> BootstrapServers { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     Идентификатор клиентского приложения
    /// </summary>
    public string ClientId { get; set; } = GetHostName();

    /// <summary>
    ///     Максимальное количество попыток переотправки любого запроса
    /// </summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>
    ///     Maximum Kafka protocol request message size. Due to differing framing overhead between protocol versions the
    ///     producer is unable to reliably enforce a strict max message limit at produce time and may exceed the maximum size
    ///     by one message in protocol ProduceRequests, the broker will enforce the the topic's `max.message.bytes` limit (see
    ///     Apache Kafka documentation).
    ///     default: 1000000
    ///     importance: medium
    /// </summary>
    public int MessageMaxBytes { get; set; } = 1000000;

    /// <summary>
    ///     Request broker's supported API versions to adjust functionality to available protocol features.
    ///     If set to false, or the ApiVersionRequest fails, the fallback version `broker.version.fallback` will be used.
    ///     **NOTE**:
    ///     Depends on broker version >=0.10.0. If the request is not supported by (an older) broker the
    ///     `broker.version.fallback` fallback is used.
    ///     default: true
    ///     importance: high
    /// </summary>
    public bool ApiVersionRequest { get; set; } = true;

    /// <summary>
    ///     Timeout for broker API version requests.
    /// </summary>
    public int ApiVersionRequestTimeoutMs { get; set; } = 10000;

    /// <summary>
    ///     The period of time in milliseconds after which we force a refresh of metadata even if we haven't seen any partition
    ///     leadership changes to proactively discover any new brokers or partitions.
    ///
    ///     default: 5 minutes
    /// </summary>
    public int MetadataMaxAge { get; set; } = 300000;

    /// <summary>
    ///     The base amount of time to wait before attempting to reconnect to a given host.
    ///     This avoids repeatedly connecting to a host in a tight loop. This backoff applies to all connection attempts by the
    ///     client to a broker.
    /// </summary>
    public int ReconnectBackoffMs { get; set; }

    /// <summary>
    ///     The maximum amount of time in milliseconds to wait when reconnecting to a broker that has repeatedly failed to
    ///     connect.
    ///     If provided, the backoff per host will increase exponentially for each consecutive connection failure, up to this
    ///     maximum.
    ///     After calculating the backoff increase, 20% random jitter is added to avoid connection storms.
    /// </summary>
    public int ReconnectBackoffMaxMs { get; set; }

    /// <summary>
    /// </summary>
    public SecurityProtocols SecurityProtocol { get; set; } = SecurityProtocols.PlainText;

    /// <summary>
    ///     The amount of time the client will wait for the socket connection to be established.
    ///     If the connection is not built before the timeout elapses, clients will close the socket channel.
    /// </summary>
    public int SocketConnectionSetupTimeoutMs { get; set; } = 10000;

    /// <summary>
    ///     The maximum amount of time the client will wait for the socket connection to be established.
    ///     The connection setup timeout will increase exponentially for each consecutive connection failure up to this
    ///     maximum.
    ///     To avoid connection storms, a randomization factor of 0.2 will be applied to the timeout resulting in a random
    ///     range between 20% below and 20% above the computed value.
    /// </summary>
    public int SocketConnectionSetupTimeoutMaxMs { get; set; } = 30000;

    /// <summary>
    ///     Close idle connections after the number of milliseconds specified by this config
    ///     default: 60 seconds
    /// </summary>
    public int ConnectionsMaxIdleMs { get; set; } = 60000;

    /// <summary>
    /// </summary>
    public int RequestTimeoutMs { get; set; } = 30 * 1000;

    /// <summary>
    ///     The amount of time to wait before attempting to retry a failed request to a given topic partition.
    ///     This avoids repeatedly sending requests in a tight loop under some failure scenarios.
    ///     retry.backoff.ms
    /// </summary>
    public long RetryBackoffMs { get; set; } = 100L;

    /// <summary>
    /// Таймаут закрытия соединения с брокером
    /// default - 5sec
    /// </summary>
    public int CloseConnectionTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// Максимальное количество запросов для одного соединения
    /// </summary>
    /// <remarks>Это поле пока константа и не может быть измененно</remarks>
    public int MaxInflightRequests => 100;

    private static string GetHostName()
    {
        try
        {
            return Dns.GetHostName();
        }
        catch
        {
            return $"NKafka/{Assembly.GetCallingAssembly().GetName().Version}";
        }
    }

    /// <summary>
    ///     Валидирует настройки и кидает исключение, если настройки не верные или отсутствуют обязательные
    /// </summary>
    internal virtual void Validate()
    {
        if ((BootstrapServers?.Count ?? 0) == 0)
        {
            throw new KafkaConfigException(nameof(BootstrapServers), 0, "BootstrapServers not set");
        }
    }
}