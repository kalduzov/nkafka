//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
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

namespace NKafka.Connection;

/// <summary>
///     A pool of connections to a kafka cluster
/// </summary>
internal interface IKafkaConnectorPool: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Возвращает все рабочие соединения
    /// </summary>
    IEnumerable<IKafkaConnector> GetAllOpenedConnectors();

    /// <summary>
    ///     Returns a connection to a specific broker
    /// </summary>
    /// <param name="nodeId">Broker id</param>
    /// <param name="isDedicate">Create a dedicated connection or use an existing one</param>
    /// <param name="connector">Created or existing connection</param>
    /// <returns>true if the connection was successfully obtained or false otherwise</returns>
    bool TryGetConnector(int nodeId, bool isDedicate, out IKafkaConnector connector);

    /// <summary>
    ///     Returns a connection
    /// </summary>
    /// <remarks>
    ///     The connection to return is selected based on the state of the pool.
    ///     If there is no information about the cluster brokers in the pool,
    ///     then each method call will return a connection to the seed broker using the Rundrobin algorithm.
    ///     If information about the broker is in the pool, then the least loaded connection to a randomly
    ///     selected broker will be returned.
    /// </remarks>
    IKafkaConnector GetConnector();

    /// <summary>
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask AddOrUpdateConnectorsAsync(IEnumerable<Node> nodes, CancellationToken token = default);
}