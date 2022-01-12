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
    /// Initializes the subsystem for a new kafka cluster
    /// </summary>
    /// <param name="config">Configuration</param>
    /// <param name="loggerFactory">Logging factory instance, can be null</param>
    /// <exception cref="ClusterKafkaException">Failed to initialize cluster</exception>
    public static Task<IKafkaCluster> CreateClusterAsync(this ClusterConfig config, ILoggerFactory? loggerFactory = null)
    {
        return CreateClusterInternalAsync(config, loggerFactory);
    }

    /// <summary>
    /// use for tests
    /// </summary>
    internal static async Task<IKafkaCluster> CreateClusterInternalAsync(
        this ClusterConfig config,
        ILoggerFactory? loggerFactory = null,
        List<IBroker>? seedBrokers = null)
    {
        /*
      * Создание нового кластера состоит из нескольких шагов
      * 1. Валидируем конфигурацию. Нам важно работать с валидной конфигураций.
      * 2. Создаем новую структуру кластера c уже валидной конфигурацией
      * 3. Когда структура создана и инициализирована - "запускаем" кластер
      */

        config.Validate();

        var factory = loggerFactory ?? NullLoggerFactory.Instance;
        var logger = factory.CreateLogger<KafkaCluster>();

        logger.CreateCluster();

        var cluster = seedBrokers is null ? new KafkaCluster(config, factory) : new KafkaCluster(config, factory, seedBrokers);

        var cts = new CancellationTokenSource(config.ClusterInitTimeoutMs);

        try
        {
            await cluster.InitializationAsync(cts.Token);

            return cluster;
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
            cts.Dispose();
        }
    }
}