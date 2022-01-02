﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

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
using Microlibs.Kafka.Exceptions;

namespace Microlibs.Kafka.Config;

/// <summary>
///     Конфигурация кластера
/// </summary>
public record ClusterConfig : CommonConfig
{
    /// <summary>
    ///     Таймаут обновления данных по брокерам в ms
    /// </summary>
    /// <remarks>Default - 1000ms</remarks>
    public int BrokerUpdateTimeoutMs { get; set; } = 1000;

    /// <summary>
    ///     Сколько времени ждать инициализации кластера в ms
    /// </summary>
    /// <remarks>Default - 15 sec</remarks>
    public int ClusterInitTimeoutMs { get; set; } = 15000;

    /// <summary>
    ///     Валидирует настройки и кидает исключение, если настройки не верные или отсутствуют обязательные
    /// </summary>
    /// <exception cref="KafkaConfigException">Throw if configuration is not valid</exception>
    internal override void Validate()
    {
        base.Validate();

        if (BrokerUpdateTimeoutMs <= 0)
        {
            throw new ConfigException("BrokerUpdateTimeoutMs <= 0");
        }

        if (ClusterInitTimeoutMs <= 0)
        {
            throw new ConfigException("BrokerUpdateTimeoutMs <= 0");
        }
    }
}