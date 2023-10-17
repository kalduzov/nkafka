//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NKafka.Exceptions;
using NKafka.Protocol;

namespace NKafka;

/// <summary>
/// Full current information about the cluster
/// </summary>
internal sealed record ClusterMetadata
{
    /// <summary>
    /// Aggregated set of minimum and maximum API versions in a cluster
    /// </summary>
    internal Dictionary<ApiKeys, ApiMetadata> AggregationApiByVersion { get; } = new(64);

    /// <summary>
    /// Returns the current max aggregated version of the Api key
    /// </summary>
    /// <param name="apiKey">API key for which you need to get information</param>
    /// <exception cref="UnsupportedVersionException">If the passed API is not supported by the cluster</exception>
    /// <returns>Maximum supported API version</returns>
    internal ApiVersion GetMaxCurrentApiVersion(ApiKeys apiKey)
    {
        if (AggregationApiByVersion.TryGetValue(apiKey, out var version))
        {
            return version.MaxVersion;
        }

        throw new UnsupportedVersionException($"In this cluster configuration, the passed {apiKey} is not supported");
    }
}