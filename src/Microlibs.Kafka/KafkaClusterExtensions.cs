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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microlibs.Kafka;

public static class KafkaClusterExtensions
{
    /// <summary>
    ///     Инициализирует подсистему для нового кафка кластера.
    /// </summary>
    /// <param name="config">Конфигурация</param>
    /// <param name="loggerFactory">Экземпляр фабрики логирования, может быть null</param>
    /// <exception cref="ClusterKafkaException">Не удалось инициализировать кластер</exception>
    public static async Task<IKafkaCluster> CreateNewClusterAsync(this ClusterConfig config, ILoggerFactory? loggerFactory = null)
    {
        /*
        * Создание нового кластера состоит из нескольких шагов
        * 1. Валидируем конфигурацию. Нам важно работать с валидной конфигураций.
        * 2. Создаем новый структуру кластера c уже валидной конфигурацией
        * 3. Когда структура создана и инициализирована - "запускаем" кластер
        */

        //Если фабрики нет - ничего никуда не пишем
        var factory = loggerFactory ?? NullLoggerFactory.Instance;
        var logger = factory.CreateLogger<KafkaCluster>();

        logger.LogDebug(Utils._LOGGER_PREFIX + "Creating new cluster");

        config.Validate();

        var cluster = new KafkaCluster(config, factory);

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

            await cluster.DisposeAsync(); //Не удалось инициализировать кластер - очишаем ресурсы, которые, возможно, уже были частично инициалированы

            throw new ClusterKafkaException(
                Utils._LOGGER_PREFIX + $"Не удалось инциализировать кластер за выделенное время {config.ClusterInitTimeoutMs}ms",
                exc);
        }
        finally
        {
            cts.Dispose();
        }
    }
}