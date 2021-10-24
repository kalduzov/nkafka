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
using NKafka.Protocol;
using NKafka.Resources;

namespace NKafka.Config;

/// <summary>
/// Common configs for all clients
/// </summary>
public abstract record CommonConfig
{
    /// <summary>
    /// Primary broker address list
    /// <p>
    /// default: <b>empty list</b>
    /// </p><br/>
    /// <p>
    /// <b>Mandatory indication required</b>
    /// </p>
    /// </summary>
    public IReadOnlyList<string> BootstrapServers { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Client application ID
    /// <p>
    /// default: <b>current host name or NKafka/{calling assembly version}</b>
    /// </p> 
    /// </summary>
    public string ClientId { get; set; } = GetHostName();

    /// <summary>
    /// The maximum number of attempts to resend any request
    /// <p>
    /// default: <b>2</b>
    /// </p> 
    /// </summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>
    /// Maximum Kafka protocol request message size. Due to differing framing overhead between protocol versions the
    /// producer is unable to reliably enforce a strict max message limit at produce time and may exceed the maximum size
    /// by one message in protocol ProduceRequests, the broker will enforce the the topic's `max.message.bytes` limit (see
    /// Apache Kafka documentation).
    /// <p>
    /// default: <b>1000000</b>
    /// </p>
    /// </summary>
    public int MessageMaxBytes { get; set; } = 1000 * 1000;

    /// <summary>
    /// Request broker's supported API versions to adjust functionality to available protocol features.
    /// If set to false, or the ApiVersionRequest fails, the fallback version `broker.version.fallback` will be used.
    /// **NOTE**:
    /// Depends on broker version >=0.10.0. If the request is not supported by (an older) broker the
    /// `broker.version.fallback` fallback is used.
    /// default: true
    /// importance: high
    /// </summary>
    public bool ApiVersionRequest { get; set; } = true;

    /// <summary>
    /// The base amount of time to wait before attempting to reconnect to a given host.
    /// This avoids repeatedly connecting to a host in a tight loop. This backoff applies to all connection attempts by the
    /// client to a broker.
    /// </summary>
    public int ReconnectBackoffMs { get; set; }

    /// <summary>
    /// The maximum amount of time in milliseconds to wait when reconnecting to a broker that has repeatedly failed to
    /// connect.
    /// If provided, the backoff per host will increase exponentially for each consecutive connection failure, up to this
    /// maximum.
    /// After calculating the backoff increase, 20% random jitter is added to avoid connection storms.
    /// </summary>
    public int ReconnectBackoffMaxMs { get; set; }

    /// <summary>
    /// Security protocol
    /// </summary>
    public SecurityProtocols SecurityProtocol { get; set; } = SecurityProtocols.PlainText;

    /// <summary>
    /// Ssl configuration section
    /// </summary>
    public SslSettings Ssl { get; set; } = SslSettings.None;

    /// <summary>
    /// Sasl configuration section
    /// </summary>
    public SaslSettings Sasl { get; set; } = SaslSettings.None;

    /// <summary>
    /// The amount of time the client will wait for the socket connection to be established.
    /// If the connection is not built before the timeout elapses, clients will close the socket channel.
    /// </summary>
    public int SocketConnectionSetupTimeoutMs { get; set; } = 10000;

    /// <summary>
    /// The maximum amount of time the client will wait for the socket connection to be established.
    /// The connection setup timeout will increase exponentially for each consecutive connection failure up to this
    /// maximum.
    /// To avoid connection storms, a randomization factor of 0.2 will be applied to the timeout resulting in a random
    /// range between 20% below and 20% above the computed value.
    /// </summary>
    public int SocketConnectionSetupTimeoutMaxMs { get; set; } = 30000;

    /// <summary>
    /// Close idle connections after the number of milliseconds specified by this config
    /// </summary>
    /// <remarks>
    ///  Default - 60 seconds
    /// </remarks>
    public int ConnectionsMaxIdleMs { get; set; } = 60 * 1000;

    /// <summary>
    /// Request execution timeout
    /// </summary>
    /// <remarks>
    /// Default - 30 seconds 
    /// </remarks>
    public int RequestTimeoutMs { get; set; } = 30 * 1000;

    /// <summary>
    ///     The amount of time to wait before attempting to retry a failed request to a given topic partition.
    ///     This avoids repeatedly sending requests in a tight loop under some failure scenarios.
    ///     retry.backoff.ms
    /// </summary>
    public long RetryBackoffMs { get; set; } = 100L;

    /// <summary>
    /// Timeout for closing the connection with the broker.
    /// </summary>
    /// <remarks>
    /// /// default - 5sec
    /// </remarks>
    public int CloseConnectionTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// The maximum number of requests per connection.
    /// </summary>
    /// <remarks>This field is still constant and cannot be changed</remarks>
    public int MaxInflightRequests => 100;

    /// <summary>
    /// Настройки проверки состояния соединений
    /// </summary>
    public HeartbeatSettings Heartbeat { get; set; } = new();

    /// <summary>
    /// The size of the TCP receive buffer (SO_RCVBUF) to use when reading data. If the value is -1, the OS default will be used.
    ///
    /// default - 32768
    /// </summary>
    public int ReceiveBufferBytes { get; set; } = 32768;

    /// <summary>
    /// Версия брокеров кафки в кластере
    /// </summary>
    /// <remarks>Клиент поддерживает минимально версию 1.0. Максимальная версия 3.0.
    /// Если задано данное свойство, то клиент не обращается к серверу за списком версий,
    /// а использует предустановленный набор для конкретной версии. Это помогает уменьшить время инициализации соединения
    /// с брокером за счет отстуствия одного запроса за списком поддерживаемого брокером API</remarks>
    public Version BrokerVersion { get; set; } = SupportVersionsExtensions.NotSetVersion;

    /// <summary>
    /// Конкретные значения конфигурации для каждого брокера
    /// </summary>
    /// <remarks>В кластере брокеры не всегда сконфигурированные одинаково.
    /// Для указания различий в конфигурации для брокеров используется данное свойство.
    /// Для этого требуется знать id каждого брокера, который отличается от общей конфигурации.
    /// Если конкретные настройки для брокера отсутствуют в данном словаре, то используются общие настройки</remarks>
    public Dictionary<int, BrokerConfig> PerBrokerConfigs { get; set; } = new(0);

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
    /// Validates the settings and throws an exception if the settings are invalid or missing required ones
    /// </summary>
    internal virtual void Validate()
    {
        BootstrapServersValidate();
        SecurityProtocolValidate();
        BrokerVersionValidate();

        if (ReceiveBufferBytes < -1)
        {
            throw new KafkaConfigException(nameof(ReceiveBufferBytes), ReceiveBufferBytes, "Размер буфера не может быть меньше -1");
        }

        // валидируем зависимые конфигурации
        foreach (var brokerConfig in PerBrokerConfigs)
        {
            brokerConfig.Value.Validate();
        }

        Sasl.Validate();
        Ssl.Validate();
    }

    private void BrokerVersionValidate()
    {
        if (BrokerVersion is null)
        {
            throw new KafkaConfigException(
                nameof(BrokerVersion),
                BrokerVersion!,
                ConfigExceptionMessages.NoBrokerVersion);
        }

        if (BrokerVersion == SupportVersionsExtensions.NotSetVersion)
        {
            return;
        }

        if (!SupportVersionsExtensions.IsSupportKafkaVersion(BrokerVersion, out var minMaxVersions))
        {
            throw new KafkaConfigException(
                nameof(BrokerVersion),
                BrokerVersion,
                string.Format(ConfigExceptionMessages.BrokerVersionInvalid, minMaxVersions.Min, minMaxVersions.Max));
        }
    }

    private void SecurityProtocolValidate()
    {
        switch (SecurityProtocol)
        {
            case SecurityProtocols.Ssl or SecurityProtocols.SaslSsl when Ssl == SslSettings.None:
                throw new KafkaConfigException(nameof(Ssl), Ssl, ConfigExceptionMessages.Ssl_no_configured);
            case SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl when Sasl == SaslSettings.None:
                throw new KafkaConfigException(nameof(Sasl), Sasl, ConfigExceptionMessages.Sasl_no_configured);
        }
    }

    private void BootstrapServersValidate()
    {
        if (BootstrapServers is null || BootstrapServers.Count == 0)
        {
            throw new KafkaConfigException(nameof(BootstrapServers), null!, ConfigExceptionMessages.No_bootstrap_servers);
        }
    }
}