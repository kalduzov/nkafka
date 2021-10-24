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

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;

namespace NKafka.Tests.Connection;

public class KafkaConnectorPoolTests
{
    [Fact]
    public void CreateConnectorPool_Successful()
    {
        var connectorPool = CreateConnectorPoolForTests();
        connectorPool.Should().NotBeNull();
    }

    [Fact]
    public void GetConnectorFromPool_WhenNoConnectors_Successful()
    {
        var connectorPool = CreateConnectorPoolForTests();

        IKafkaConnector GetConnector()
        {
            return connectorPool.GetRandomConnector();
        }

        FluentActions.Invoking(GetConnector)
            .Should()
            .Throw<ConnectorNotFoundException>();
    }

    private static KafkaConnectorPool CreateConnectorPoolForTests()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            Array.Empty<Node>(),
            config.Ssl,
            config.Sasl,
            config.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        return connectorPool;
    }
}