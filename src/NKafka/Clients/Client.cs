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

using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace NKafka.Clients;

internal abstract class Client<TConfig>(IKafkaCluster kafkaCluster, TConfig config, ILoggerFactory loggerFactory): IClient
{
    private IDisposable? _loggerScope;

    protected IDisposable? LoggerScope
    {
        get => _loggerScope;
        set
        {
            if (_loggerScope is null)
            {
                _loggerScope = value;
            }
            else
            {
                Debug.Assert(_loggerScope is not null, "LoggerScope is not null");
            }
        }
    }

    protected IKafkaCluster KafkaCluster { get; } = kafkaCluster;

    protected ILoggerFactory LoggerFactory { get; } = loggerFactory;

    protected TConfig Config { get; } = config;

    public virtual void Dispose()
    {
        LoggerScope?.Dispose();
        KafkaCluster.Dispose();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public virtual async ValueTask DisposeAsync()
    {
        await KafkaCluster.DisposeAsync();
    }
}