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
/// Cluster configuration
/// </summary>
public record ClusterConfig: CommonConfig
{
    /// <summary>
    /// Metadata update timeout
    /// </summary>
    /// <remarks>Default - 5 minutes</remarks>
    public int MetadataUpdateTimeoutMs { get; set; } = 5 * 60 * 1000;

    /// <summary>
    ///     Cluster initialization timeout 
    /// </summary>
    /// <remarks>Default - 15 seconds</remarks>
    public int ClusterInitTimeoutMs { get; set; } = 15 * 1000;

    /// <summary>
    /// Fully update metadata
    /// </summary>
    /// <remarks>
    /// A full update of the metadata can greatly inflate the memory if there are a lot of topics and partitions in the cluster.
    /// If the metadata is not completely updated, then data is requested only for those topics that are used by the cluster client
    /// </remarks>
    public bool IsFullUpdateMetadata { get; set; } = true;

    /// <summary>
    /// Validates the settings and throws an exception if the settings are invalid or missing required ones
    /// </summary>
    /// <exception cref="KafkaConfigException">Throw if configuration is not valid</exception>
    internal override void Validate()
    {
        base.Validate();

        if (MetadataUpdateTimeoutMs <= 0)
        {
            throw new KafkaConfigException(nameof(MetadataUpdateTimeoutMs), MetadataUpdateTimeoutMs, "MetadataUpdateTimeoutMs <= 0");
        }

        if (ClusterInitTimeoutMs <= 0)
        {
            throw new KafkaConfigException(nameof(ClusterInitTimeoutMs), ClusterInitTimeoutMs, "ClusterInitTimeoutMs <= 0");
        }
    }
}