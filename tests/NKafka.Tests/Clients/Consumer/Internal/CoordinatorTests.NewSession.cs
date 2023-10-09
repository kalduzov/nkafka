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

using NKafka.Clients.Consumer;
using NKafka.Config;

namespace NKafka.Tests.Clients.Consumer.Internal;

public partial class CoordinatorTests
{
    [Fact]
    public async Task NewSession_StartAsLeader_Successful()
    {
        var kafkaCluster = CreateKafkaClusterForTests();
        await kafkaCluster.OpenAsync(CancellationToken.None);
        var consumerConfig = ConsumerConfig.BaseFrom(kafkaCluster.Config);
        consumerConfig.GroupId = "good_test";

        var coordinator = BuildCorrectCoordinator(kafkaCluster, consumerConfig);
        var subscription = new Subscription(new[]
            {
                "test"
            },
            AutoOffsetReset.Latest,
            new[]
            {
                new RoundRobinAssignor()
            });

        var result = await coordinator.NewSessionAsync(subscription, CancellationToken.None);

        result.Should().BeTrue("Новая сессия должна быть создана успешно");
        coordinator.IsLeader.Should().Be(true);
    }
}