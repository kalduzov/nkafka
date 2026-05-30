// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright © 2026 Aleksey Kalduzov. All rights reserved
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

using Microsoft.Extensions.DependencyInjection;

using NKafka.Config;

namespace NKafka.AspNetCore;

/// <summary>
/// Provides extension methods for registering Kafka services in the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a Kafka cluster to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The name of the Kafka cluster.</param>
    /// <param name="options">The configuration action for the cluster.</param>
    /// <returns>A <see cref="IKafkaClusterBuilder"/> to further configure the cluster.</returns>
    public static IKafkaClusterBuilder AddKafkaCluster(this IServiceCollection services, string name, Action<ClusterConfig> options)
    {
        var clusterConfig = new ClusterConfig();
        options.Invoke(clusterConfig);

        return new KafkaClusterBuilder(services, name, clusterConfig);
    }
}