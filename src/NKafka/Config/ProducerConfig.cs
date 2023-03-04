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

namespace NKafka.Config;

public record ProducerConfig: CommonConfig
{
    public static readonly ProducerConfig EmptyProducerConfig = new();

    /// <summary>
    ///     Specifies whether to enable notification of delivery reports. Typically
    ///     you should set this parameter to true. Set it to false for "fire and
    ///     forget" semantics and a small boost in performance.
    ///     default: true
    /// </summary>
    public bool EnableDeliveryReports { get; set; } = true;

    /// <summary>
    /// Таймаут на доставку сообщения
    /// </summary>
    /// <remarks>
    /// Продюсер будет пытаться отправить сообщения до истечения данного таймаута
    /// </remarks>
    public int DeliveryTimeoutMs { get; set; } = 120 * 1000;

    /// <summary>
    /// Конфигурация алгоритма распределения сообщений по разделам
    /// </summary>
    /// <remarks>По умалчанию выставлено значение для свойства Partitioner = Default, и в качестве алгоритма будет использоваться адаптивное распределение.
    ///
    /// Подробнее можно прочитать в https://cwiki.apache.org/confluence/display/KAFKA/KIP-794%3A+Strictly+Uniform+Sticky+Partitioner
    /// </remarks>
    public PartitionerConfig PartitionerConfig { get; set; } = new();

    /// <summary>
    /// Waiting time for filling a batch of records to be sent
    /// <p>
    /// <br/>
    /// Not earlier than through <b>lingerMs</b> the batch, even if it is not completely filled yet, will be sent to the broker.
    /// If set to <b>0</b>, then such a batch will be sent in the next message sending cycle
    /// </p>
    /// </summary> 
    public double LingerMs { get; set; } = 0;

    /// <summary>
    /// Type of message delivery confirmation
    /// </summary>
    public Acks Acks { get; set; } = Acks.All;

    /// <summary>
    /// Maximum request size
    /// </summary>
    public int MaxRequestSize { get; set; } = 1024 * 1024;

    public long TotalMemorySize { get; set; }

    /// <summary>
    /// Record compression type
    /// </summary>
    public CompressionType CompressionType { get; set; } = CompressionType.None;

    public long MaxBlockTimeMs { get; set; }

    public string? TransactionalId { get; set; }

    /// <summary>
    /// Maximum size of one batch with records
    /// </summary>
    public int BatchSize { get; set; } = 16384;

    /// <summary>
    /// The total bytes of memory the producer can use to buffer records waiting to be sent to the server.
    /// </summary>
    public int BufferMemory { get; set; } = 32 * 1024 * 1024;

    public long MetadataMaxAgeConfig { get; set; }

    public long MetadataMaxIdleConfig { get; set; }

    /// <summary>
    /// The maximum number of unacknowledged requests the client will send on a single connection before blocking.
    /// Note that if this config is set to be greater than 1 and <see cref="IdempotenceEnabled" /> is set to false, there
    /// is a risk of message re-ordering after a failed send due to retries (i.e., if retries are enabled)
    /// default: 5
    /// importance: low
    /// </summary>
    public int MaxInFlightPerRequest { get; set; } = 5;

    public bool IdempotenceEnabled => false;

    public int TransactionTimeoutMs { get; set; } = 6000;

    /// <summary>
    /// Creates a new configuration based on the current one
    /// </summary>
    public static ProducerConfig BaseFrom(CommonConfig config)
    {
        return new ProducerConfig
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
    public ProducerConfig MergeFrom(CommonConfig config)
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
        PartitionerConfig.Validate();
    }
}