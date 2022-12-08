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

using FluentAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using NKafka.Clients.Consumer;
using NKafka.Config;
using NKafka.Connection;
using NKafka.Serialization;

using Xunit;

namespace NKafka.Tests.Clients.Consumer;

public class ConsumerTests
{
    private IKafkaCluster _kafkaCluster;

    public ConsumerTests()
    {
        _kafkaCluster = BuildMockForKafkaCluster();
    }

    [Fact]
    public void Ctor_WithSerializersIsNull_Success()
    {
        var config = new ConsumerConfig();

        void Ctor()
        {
            var _ = new Consumer<Null, Null>(
                _kafkaCluster,
                config,
                null,
                null,
                NullLoggerFactory.Instance.CreateLogger(""));
        }

        FluentActions.Invoking(Ctor).Should().NotThrow();
    }

    [Fact]
    public void Ctor_WithSerializersIsNotNull_Success()
    {
        var config = new ConsumerConfig();

        var stringSerializer = new StringSerializer();
        void Ctor()
        {
            var _ = new Consumer<string, string>(
                _kafkaCluster,
                config,
                stringSerializer,
                stringSerializer,
                NullLoggerFactory.Instance.CreateLogger(""));
        }

        FluentActions.Invoking(Ctor).Should().NotThrow();
    }

    [Fact]
    public async Task Subscribe_Success()
    {
        var config = new ConsumerConfig();

        await using var consumer = _kafkaCluster.BuildConsumer<Null, Null>("testConsumer");
        var channel = consumer.Subscribe("test");

        await foreach (var record in channel.ReadAllAsync())
        {
        }
    }

    private static IKafkaCluster BuildMockForKafkaCluster()
    {
        var mockCluster = new Mock<IKafkaCluster>();

        var brokers = BuildBrokersMock();
        mockCluster.Setup(x => x.Brokers).Returns(brokers);

        return mockCluster.Object;
    }

    private static IReadOnlyCollection<IBroker> BuildBrokersMock()
    {
        var list = new List<IBroker>(5);

        return list;
    }
}