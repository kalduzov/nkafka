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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;

namespace Microlibs.Kafka.Protocol.Connection;

internal sealed class BrokerConnectionPool : IDisposable, IAsyncDisposable
{
    private readonly CommonConfig _commonConfig;
    private readonly List<IBroker> _connections;
    private readonly CancellationTokenSource _tokenSource = new();

    public BrokerConnectionPool(CommonConfig commonConfig)
    {
        _commonConfig = commonConfig;
        _connections = new List<IBroker>(commonConfig.BootstrapServers.Count);

        SeedBrokers(commonConfig);
    }

    public ValueTask DisposeAsync()
    {
        //todo аккуратно закрыть все соединения и задачи обновления данных по брокерам
        _tokenSource.Cancel();

        return default;
    }

    public void Dispose()
    {
        //_brokersUpdater.Dispose();
    }

    // private async Task BrokerUpdaterTask(CancellationToken token)
    // {
    //     while (!token.IsCancellationRequested)
    //     {
    //         var connection = _connections.First();
    //
    //         var message = new MetadataRequestMessage
    //         {
    //             IncludeClusterAuthorizedOperations = true
    //         };
    //
    //         try
    //         {
    //             var describe = await connection.SendAsync<DescribeResponseMessage, DescribeClusterContent>(message, token);
    //         }
    //         catch (Exception exc)
    //         {
    //             Console.WriteLine(exc.Message);
    //         }
    //
    //         //await Task.Delay(_commonConfig.BrokerUpdateTimeout, token);
    //     }
    // }

    private void SeedBrokers(CommonConfig commonConfig)
    {
        foreach (var bootstrapServer in commonConfig.BootstrapServers)
        {
            var endpoint = Utils.BuildBrokerEndPoint(bootstrapServer);
            var connection = new Broker(endpoint);

            _connections.Add(connection);
        }
    }

    public IReadOnlyCollection<IBroker> GetConnections()
    {
        return _connections;
    }

    public IBroker GetController()
    {
        return null;
    }

    public IReadOnlyCollection<IBroker> GetBrokers()
    {
        return _connections;
    }

    /// <summary>
    /// Возвращает наименее нагруженный брокер
    /// </summary>
    public IBroker GetLeastLoadedBroker()
    {
        return _connections.First();
    }

    public Task<bool> TryAddBrokerAsync(Broker broker, bool isController, bool b, CancellationToken token)
    {
        return Task.FromResult(true);
    }
}