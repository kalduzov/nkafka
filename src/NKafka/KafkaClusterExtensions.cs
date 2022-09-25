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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;
using NKafka.Resources;

namespace NKafka;

public static class KafkaClusterExtensions
{
    /// <summary>
    /// Initializes the subsystem for a new kafka cluster and open it
    /// </summary>
    /// <param name="config">Configuration</param>
    /// <param name="loggerFactory">Logging factory instance, can be null</param>
    /// <param name="token"></param>
    /// <param name="openImmediately">The connection to the cluster will be established immediately. Otherwise, you must call the <see cref="KafkaCluster.OpenAsync"/>OpenAsync method.</param>
    /// <exception cref="ClusterKafkaException">Failed to initialize cluster</exception>
    public static Task<IKafkaCluster> CreateClusterAsync(
        this ClusterConfig config,
        ILoggerFactory? loggerFactory = null,
        CancellationToken token = default,
        bool openImmediately = true)
    {
        return CreateClusterInternalAsync(config, loggerFactory, token: token, openImmediately: openImmediately);
    }

    /// <summary>
    /// Initializes the subsystem for a new kafka cluster and open it
    /// </summary>
    /// <param name="clusterConfigFactory">Configuration factory for cluster</param>
    /// <param name="loggerFactory">Logging factory instance, can be null</param>
    /// <param name="token"></param>
    /// <param name="openImmediately">The connection to the cluster will be established immediately. Otherwise, you must call the <see cref="KafkaCluster.OpenAsync"/>OpenAsync method.</param>
    /// <exception cref="ClusterKafkaException">Failed to initialize cluster</exception>
    public static Task<IKafkaCluster> CreateClusterAsync(
        this IClusterConfigFactory clusterConfigFactory,
        ILoggerFactory? loggerFactory = null,
        CancellationToken token = default,
        bool openImmediately = true)
    {
        return CreateClusterInternalAsync(clusterConfigFactory.Build(), loggerFactory, token: token, openImmediately: openImmediately);
    }

    /// <summary>
    /// use for tests
    /// </summary>
    internal static async Task<IKafkaCluster> CreateClusterInternalAsync(
        this ClusterConfig config,
        ILoggerFactory? loggerFactory = null,
        List<IBroker>? seedBrokers = null,
        CancellationToken token = default,
        bool openImmediately = true)
    {
        config.Validate();

        var factory = loggerFactory ?? NullLoggerFactory.Instance;
        var logger = factory.CreateLogger<KafkaCluster>();

        logger.CreateCluster();

        var cluster = seedBrokers is null ? new KafkaCluster(config, factory) : new KafkaCluster(config, factory, seedBrokers);

        var cts = new CancellationTokenSource(config.ClusterInitTimeoutMs);

        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, token);

        if (!openImmediately)
        {
            return cluster;
        }

        try
        {
            await cluster.OpenAsync(linkedTokenSource.Token);
        }
        catch (OperationCanceledException exc)
        {
            if (!cts.IsCancellationRequested)
            {
                throw;
            }

            await cluster.DisposeAsync();

            throw new ClusterKafkaException(string.Format(ExceptionMessages.ClusterInitFailed, config.ClusterInitTimeoutMs), exc);
        }
        finally
        {
            linkedTokenSource.Dispose();
            cts.Dispose();
        }

        return cluster;
    }
}