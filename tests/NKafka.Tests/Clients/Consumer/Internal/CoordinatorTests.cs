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

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Clients.Consumer.Internal;
using NKafka.Config;
using NKafka.Metrics;

namespace NKafka.Tests.Clients.Consumer.Internal;

public partial class CoordinatorTests: ClientTests
{
    [Fact]
    public void Coordinator_Ctor_Successful()
    {
        var kafkaCluster = CreateKafkaClusterForTests();
        var consumerConfig = ConsumerConfig.BaseFrom(kafkaCluster.Config);
        consumerConfig.GroupId = "test";

        var action = () => BuildCorrectCoordinator(kafkaCluster, consumerConfig);

        action.Should().NotThrow();
    }

    [Fact]
    public async Task Coordinator_TryFindCoordinatorForGroupAsync_Successful()
    {
        var kafkaCluster = CreateKafkaClusterForTests();
        var consumerConfig = ConsumerConfig.BaseFrom(kafkaCluster.Config);
        consumerConfig.GroupId = "good_test";

        var coordinator = BuildCorrectCoordinator(kafkaCluster, consumerConfig);

        var action = () => coordinator.TryFindCoordinatorForGroupAsync(CancellationToken.None);
        await action.Should().NotThrowAsync();
    }

    private static Coordinator BuildCorrectCoordinator(IKafkaCluster kafkaCluster, ConsumerConfig consumerConfig)
    {
        return new Coordinator(kafkaCluster,
            consumerConfig.GroupId,
            consumerConfig.Heartbeat,
            consumerConfig.PartitionAssignors.ToDictionary(x => x.Name),
            consumerConfig.EnableAutoCommit,
            consumerConfig.AutoCommitIntervalMs,
            consumerConfig.RebalanceTimeoutMs,
            consumerConfig.SessionTimeoutMs,
            consumerConfig.MaxRetries,
            consumerConfig.RetryBackoffMs,
            new NullConsumerMetrics(),
            new NullLoggerFactory()
        );
    }
}