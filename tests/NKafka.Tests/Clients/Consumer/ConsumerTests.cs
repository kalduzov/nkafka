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

using NKafka.Clients.Consumer;
using NKafka.Config;
using NKafka.Serialization;

namespace NKafka.Tests.Clients.Consumer;

public partial class ConsumerTests: IDisposable
{
    private readonly IKafkaCluster _kafkaCluster = BuildMockForKafkaCluster();

    [Fact]
    public void Ctor_WithSerializersIsNull_Success()
    {
        var config = new ConsumerConfig();

        FluentActions.Invoking(Ctor).Should().NotThrow();

        return;

        void Ctor()
        {
            _ = new Consumer<Null, Null>(
                kafkaCluster: _kafkaCluster,
                config: config,
                keyDeserializer: null,
                valueDeserializer: null,
                fetcher: null,
                coordinator: null,
                loggerFactory: NullLoggerFactory.Instance);
        }
    }

    [Fact]
    public void Ctor_WithSerializersIsNotNull_Success()
    {
        var config = new ConsumerConfig();

        var stringDeserializer = new StringDeserializer();

        void Ctor()
        {
            var _ = new Consumer<string, string>(
                _kafkaCluster,
                config,
                stringDeserializer,
                stringDeserializer,
                null!,
                null!,
                NullLoggerFactory.Instance);
        }

        FluentActions.Invoking(Ctor).Should().NotThrow();
    }

    private static IKafkaCluster BuildMockForKafkaCluster()
    {
        var mockCluster = Substitute.For<IKafkaCluster>();

        var consumer = Substitute.For<IConsumer<int, string>>();

        mockCluster.BuildConsumer(Arg.Any<ConsumerConfig>(),
                Arg.Any<IAsyncDeserializer<int>>(),
                Arg.Any<IAsyncDeserializer<string>>())
            .Returns(consumer);

        var brokers = BuildBrokersMock();
        mockCluster.Brokers.Returns(brokers);

        return mockCluster;
    }

    private static IReadOnlyCollection<Node> BuildBrokersMock()
    {
        var list = new List<Node>(5);

        var coordinator = BuildBrokerCoordinator();

        list.Add(coordinator);

        return list;
    }

    private static Node BuildBrokerCoordinator()
    {
        return new Node(1, "localhost", 9092);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    void IDisposable.Dispose()
    {
        _kafkaCluster.Dispose();
        GC.SuppressFinalize(this);
    }
}