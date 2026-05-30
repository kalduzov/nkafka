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
    /// Returns all opened shared connectors.
    /// </summary>
    internal IEnumerable<IKafkaConnector> GetOpenedSharedConnectors();

    /// <summary>
    /// Returns a shared connector for the specified broker.
    /// </summary>
    internal bool TryGetSharedConnector(int nodeId, out IKafkaConnector connector);

    /// <summary>
    /// Creates a dedicated connector for the specified broker.
    /// </summary>
    internal bool TryCreateDedicatedConnector(int nodeId, out IKafkaConnector connector);

    /// <summary>
    /// Returns any shared connector for a known broker.
    /// </summary>
    internal bool TryGetAnySharedBrokerConnector(out IKafkaConnector connector);

    /// <summary>
    /// Returns a bootstrap connector from the seed list.
    /// </summary>
    internal bool TryGetBootstrapConnector(out IKafkaConnector connector);

    /// <summary>
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal ValueTask AddOrUpdateConnectorsAsync(IEnumerable<Node> nodes, CancellationToken token);
}
