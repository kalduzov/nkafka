//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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
using NKafka.Clients.Consumer.Internal;
using NKafka.Config;
using NKafka.Serialization;

namespace NKafka.Tests.Clients.Consumer;

public partial class ConsumerTests
{
    [Fact]
    public async Task Subscribe_Success()
    {
        await using var consumer = BuildConsumer<int, string>();
        var channel = await consumer.SubscribeAsync("test");

        channel.Should().NotBeNull();
    }

    private IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>()
        where TKey : notnull
        where TValue : notnull
    {
        var config = new ConsumerConfig();
        var fetcher = Substitute.For<IFetcher<TKey, TValue>>();
        var coordinator = Substitute.For<ICoordinator>();

        coordinator.NewSessionAsync(Arg.Any<Subscription>(), Arg.Any<CancellationToken>()).Returns(true);

        var consumer = new Consumer<TKey, TValue>(_kafkaCluster,
            config,
            NoneDeserializer<TKey>.Instance,
            NoneDeserializer<TValue>.Instance,
            fetcher,
            coordinator,
            NullLoggerFactory.Instance);

        return consumer;
    }
}