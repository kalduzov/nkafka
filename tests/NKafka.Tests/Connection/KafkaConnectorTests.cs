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
using Microsoft.IO;

using NKafka.Config;
using NKafka.Connection;
using NKafka.Tests.Connection.Fixtures;

namespace NKafka.Tests.Connection;

public class KafkaConnectorTests: IClassFixture<ConnectorFixture>
{
    private readonly ConnectorFixture _fixture;

    public KafkaConnectorTests(ConnectorFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreateConnector_Successful()
    {
        IKafkaConnector CreateConnector()
            => new KafkaConnector(
                _fixture.EndPoint,
                100,
                1000,
                1000,
                1000,
                1000,
                0,
                SecurityProtocols.PlainText,
                SaslSettings.None,
                SslSettings.None,
                "test",
                true,
                _fixture.SocketFactory,
                new RecyclableMemoryStreamManager(),
                NullLoggerFactory.Instance);

        FluentActions.Invoking(CreateConnector).Should().NotThrow();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ConnectorOpen_Successful(bool apiRequest)
    {
        var kafkaConnector = new KafkaConnector(
            _fixture.EndPoint,
            100,
            1000,
            1000,
            1000,
            1000,
            0,
            SecurityProtocols.PlainText,
            SaslSettings.None,
            SslSettings.None,
            "test",
            apiRequest,
            _fixture.SocketFactory,
            new RecyclableMemoryStreamManager(),
            NullLoggerFactory.Instance);

        await kafkaConnector.OpenAsync(CancellationToken.None);
        kafkaConnector.ConnectorState.Should().Be(KafkaConnector.State.Open);
    }
}