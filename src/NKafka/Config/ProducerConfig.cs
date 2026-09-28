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

using NKafka.Exceptions;

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
    /// Message delivery timeout
    /// </summary>
    /// <remarks>
    /// The producer will try to send messages before this timeout expires.
    /// </remarks>
    public int DeliveryTimeoutMs { get; set; } = 120 * 1000;

    /// <summary>
    /// Maximum time allowed for accepting a message into the producer.
    /// </summary>
    public int EnqueueTimeoutMs { get; set; } = 60_000;

    /// <summary>
    /// Maximum number of produce calls that may wait for metadata, partition selection, or buffer space.
    /// </summary>
    public int MaxPendingProduceRequests { get; set; } = 1_000;

    /// <summary>
    /// Maximum number of accepted messages that have not reached a final delivery result.
    /// </summary>
    public int MaxQueuedMessages { get; set; } = 100_000;

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
    /// Record compression config
    /// </summary>
    public CompressionConfig Compression { get; set; } = new();

    /// <summary>
    /// Maximum size of one batch with records
    /// </summary>
    public int BatchSize { get; set; } = 65535;

    /// <summary>
    /// The total bytes of memory the producer can use to buffer records waiting to be sent to the server.
    /// </summary>
    public int BufferMemory { get; set; } = 32 * 1024 * 1024;

    /// <summary>
    /// Gets or sets whether the producer assigns Kafka producer IDs and per-partition sequences
    /// so an internal retry of the same batch does not create duplicate records.
    /// </summary>
    /// <remarks>
    /// This does not deduplicate separate application calls, provide transactions, or make
    /// writes across partitions atomic. The current implementation permits one in-flight batch
    /// per partition when idempotence is enabled.
    /// </remarks>
    public bool EnableIdempotence { get; set; } = false;

    /// <summary>
    /// Creates a new configuration based on the current one
    /// </summary>
    public static ProducerConfig BaseFrom(CommonConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var result = new ProducerConfig
        {
            PartitionerConfig = new PartitionerConfig(),
            Compression = new CompressionConfig()
        };

        config.CopyCommonSettingsTo(result);
        return result;
    }

    /// <summary>
    /// Merges the main configuration with the current one
    /// </summary>
    /// <remarks>All parameters of the current configuration are overwritten by the parameters of the main</remarks>
    public ProducerConfig MergeFrom(CommonConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var result = this with
        {
            PartitionerConfig = new PartitionerConfig
            {
                Partitioner = PartitionerConfig.Partitioner,
                CustomPartitionerClass = PartitionerConfig.CustomPartitionerClass
            },
            Compression = Compression with { }
        };

        config.CopyCommonSettingsTo(result);
        return result;
    }

    /// <summary>
    /// Validates the settings and throws an exception if the settings are invalid or missing required ones
    /// </summary>
    internal override void Validate()
    {
        base.Validate();

        if (EnqueueTimeoutMs <= 0)
        {
            throw new KafkaConfigException(nameof(EnqueueTimeoutMs), EnqueueTimeoutMs, ConfigurationMessages.CommonConfig_ValueMustBePositive);
        }

        if (MaxPendingProduceRequests <= 0)
        {
            throw new KafkaConfigException(nameof(MaxPendingProduceRequests), MaxPendingProduceRequests, ConfigurationMessages.CommonConfig_ValueMustBePositive);
        }

        if (MaxQueuedMessages <= 0)
        {
            throw new KafkaConfigException(nameof(MaxQueuedMessages), MaxQueuedMessages, ConfigurationMessages.CommonConfig_ValueMustBePositive);
        }

        if (DeliveryTimeoutMs <= 0)
        {
            throw new KafkaConfigException(nameof(DeliveryTimeoutMs), DeliveryTimeoutMs, ConfigurationMessages.CommonConfig_ValueMustBePositive);
        }

        if (double.IsNaN(LingerMs) || double.IsInfinity(LingerMs) || LingerMs < 0 || LingerMs > int.MaxValue)
        {
            throw new KafkaConfigException(nameof(LingerMs), LingerMs, ConfigurationMessages.ProducerConfig_LingerInvalid);
        }

        if (DeliveryTimeoutMs < LingerMs + RequestTimeoutMs)
        {
            throw new KafkaConfigException(nameof(DeliveryTimeoutMs), DeliveryTimeoutMs, ConfigurationMessages.ProducerConfig_DeliveryTimeoutTooShort);
        }

        if (BufferMemory <= 0)
        {
            throw new KafkaConfigException(nameof(BufferMemory), BufferMemory, ConfigurationMessages.CommonConfig_ValueMustBePositive);
        }

        if (BatchSize <= 0)
        {
            throw new KafkaConfigException(nameof(BatchSize), BatchSize, ConfigurationMessages.CommonConfig_ValueMustBePositive);
        }

        if (MaxRequestSize <= 0)
        {
            throw new KafkaConfigException(nameof(MaxRequestSize), MaxRequestSize, ConfigurationMessages.CommonConfig_ValueMustBePositive);
        }

        if (!Enum.IsDefined(Acks))
        {
            throw new KafkaConfigException(nameof(Acks), Acks, ConfigurationMessages.ProducerConfig_AcksInvalid);
        }

        if (!Enum.IsDefined(Compression.CompressionType))
        {
            throw new KafkaConfigException(nameof(Compression), Compression.CompressionType, ConfigurationMessages.ProducerConfig_CompressionInvalid);
        }

        PartitionerConfig.Validate();
    }
}
