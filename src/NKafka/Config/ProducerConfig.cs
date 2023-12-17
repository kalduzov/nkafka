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

using NKafka.Metrics;

namespace NKafka.Config;

/// <summary>
/// Represents the configuration for a producer.
/// </summary>
public record ProducerConfig: CommonConfig
{
    /// <summary>
    /// Represents an empty producer configuration.
    /// </summary>
    public static readonly ProducerConfig EmptyProducerConfig = new();

    /// <summary>
    /// Specifies whether to enable notification of delivery reports. Typically
    /// you should set this parameter to true. Set it to false for "fire and
    /// forget" semantics and a small boost in performance.
    /// default: true
    /// </summary>
    /// <remarks>
    /// If the producer is used only to send "fire and forget" semantics, then it makes sense to set this field to false
    /// to avoid unnecessary processing of responses from the server.
    /// </remarks>
    public bool EnableDeliveryReports { get; set; } = true;

    /// <summary>
    /// Message delivery timeout
    /// </summary>
    /// <remarks>
    /// The producer will try to send messages before this timeout expires.
    /// </remarks>
    public int DeliveryTimeoutMs { get; set; } = 120 * 1000;

    /// <summary>
    /// Сonfiguration of the message distribution algorithm by sections
    /// </summary>
    /// <remarks>
    /// By default, the value for the Partitioner = Default property is set, and roundrobin will be used as the algorithm
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

    /// <summary>
    /// Record compression type
    /// </summary>
    public CompressionType CompressionType { get; set; } = CompressionType.None;

    /// <summary>
    /// Maximum size of one batch with records
    /// </summary>
    public int BatchSize { get; set; } = 65535;

    /// <summary>
    /// The total bytes of memory the producer can use to buffer records waiting to be sent to the server.
    /// </summary>
    public int BufferMemory { get; set; } = 32 * 1024 * 1024;

    /// <summary>
    /// Gives the producer access to the implementation of the metrics provider
    /// </summary>
    public IProducerMetrics Metrics { get; set; } = new NullProducerMetrics();

    /// <summary>
    /// Gets or sets the unique identifier for a transaction.
    /// </summary>
    /// <value>
    /// The transactional identifier.
    /// </value>
    public string TransactionalId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction timeout in milliseconds.
    /// </summary>
    public int TransactionTimeoutMs { get; set; } = 60000;

    /// <summary>
    /// Gets or sets a value indicating whether idempotence is enabled.
    /// </summary>
    public bool EnableIdempotence { get; set; } = false;

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