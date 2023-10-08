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

namespace NKafka.Tests.Clients.Consumer;

public class RoundRobinAssignorsTests
{
    [Fact]
    public void Assign_IfParamEmpty_MustByEmpty()
    {
        var assignor = new RoundRobinAssignor();
        var assignResult = assignor.Assign(Array.Empty<TopicPartition>(), new Dictionary<string, Subscription>());
        assignResult.Should().BeEmpty();
    }

    /// <summary>
    /// For example, there are two topics (t0, t1) and two consumer (m0, m1), and each topic has three partitions (p0, p1, p2):
    /// m0: [t0p0, t0p2, t1p1]
    /// m1: [t0p1, t1p0, t1p2]
    /// </summary>
    [Fact]
    public void Assign_WhenSequenceSorted_Successful()
    {
        var assignor = new RoundRobinAssignor();
        var topicPartitions = new List<TopicPartition>
        {
            new("t0", 0),
            new("t0", 1),
            new("t0", 2),
            new("t1", 0),
            new("t1", 1),
            new("t1", 2)
        };

        var m0 = new Subscription(new[]
            {
                "t0",
                "t1"
            },
            AutoOffsetReset.None,
            Array.Empty<IPartitionAssignor>());
        var m1 = new Subscription(new[]
            {
                "t0",
                "t1"
            },
            AutoOffsetReset.None,
            Array.Empty<IPartitionAssignor>());

        var assignResult = assignor.Assign(topicPartitions,
            new Dictionary<string, Subscription>
            {
                ["m0"] = m0,
                ["m1"] = m1
            });

        assignResult.Should().HaveCount(2);
        assignResult.Should().ContainKeys("m0", "m1");
        assignResult["m0"].Should().HaveCount(3);
        assignResult["m1"].Should().HaveCount(3);
        assignResult["m0"].Should().ContainInOrder(new TopicPartition("t0", 0), new TopicPartition("t0", 2), new TopicPartition("t1", 1));
        assignResult["m1"].Should().ContainInOrder(new TopicPartition("t0", 1), new TopicPartition("t1", 0), new TopicPartition("t1", 2));
    }

    /// <summary>
    /// For example, there are two topics (t0, t1) and two consumer (m0, m1), and each topic has three partitions (p0, p1, p2):
    /// m0: [t0p0, t0p2, t1p1]
    /// m1: [t0p1, t1p0, t1p2]
    /// </summary>
    [Fact]
    public void Assign_WhenSequenceNoSorted_Successful()
    {
        var assignor = new RoundRobinAssignor();
        var topicPartitions = new List<TopicPartition>
        {
            new("t0", 0),
            new("t1", 1),
            new("t1", 0),
            new("t0", 2),
            new("t0", 1),
            new("t1", 2)
        };

        var m0 = new Subscription(new[]
            {
                "t1",
                "t0"
            },
            AutoOffsetReset.None,
            Array.Empty<IPartitionAssignor>());
        var m1 = new Subscription(new[]
            {
                "t0",
                "t1"
            },
            AutoOffsetReset.None,
            Array.Empty<IPartitionAssignor>());

        var assignResult = assignor.Assign(topicPartitions,
            new Dictionary<string, Subscription>
            {
                ["m0"] = m0,
                ["m1"] = m1
            });

        assignResult.Should().HaveCount(2);
        assignResult.Should().ContainKeys("m0", "m1");
        assignResult["m0"].Should().HaveCount(3);
        assignResult["m1"].Should().HaveCount(3);
        assignResult["m0"].Should().ContainInOrder(new TopicPartition("t0", 0), new TopicPartition("t0", 2), new TopicPartition("t1", 1));
        assignResult["m1"].Should().ContainInOrder(new TopicPartition("t0", 1), new TopicPartition("t1", 0), new TopicPartition("t1", 2));
    }

    /// <summary>
    /// For example, there are two topics (t0, t1) and two consumer (m0, m1), and each topic has three partitions (p0, p1, p2). Consumer m0 subscribe only t0:
    /// m0: [t0p0, t0p2]
    /// m1: [t0p1, t1p0, t1p1, t1p2]
    /// </summary>
    [Fact]
    public void Assign_WhenTopicsDisbalanceByConsumer_Successful()
    {
        var assignor = new RoundRobinAssignor();
        var topicPartitions = new List<TopicPartition>
        {
            new("t0", 0),
            new("t0", 1),
            new("t0", 2),
            new("t1", 0),
            new("t1", 1),
            new("t1", 2)
        };

        var m0 = new Subscription(new[]
            {
                "t0"
            },
            AutoOffsetReset.None,
            Array.Empty<IPartitionAssignor>());
        var m1 = new Subscription(new[]
            {
                "t0",
                "t1"
            },
            AutoOffsetReset.None,
            Array.Empty<IPartitionAssignor>());

        var assignResult = assignor.Assign(topicPartitions,
            new Dictionary<string, Subscription>
            {
                ["m0"] = m0,
                ["m1"] = m1
            });

        assignResult.Should().HaveCount(2);
        assignResult.Should().ContainKeys("m0", "m1");
        assignResult["m0"].Should().HaveCount(2);
        assignResult["m1"].Should().HaveCount(4);
        assignResult["m0"].Should().ContainInOrder(new TopicPartition("t0", 0), new TopicPartition("t0", 2));
        assignResult["m1"].Should().ContainInOrder(new TopicPartition("t0", 1), new TopicPartition("t1", 0), new TopicPartition("t1", 1), new TopicPartition("t1", 2));
    }

    /// <summary>
    /// For example, there are three topics (t0, t1, t3) and two consumer (m0, m1), and each topic has three partitions (p0, p1, p2). Consumer m0 subscribe only t3:
    /// m0: [t3p0, t3p1, t3p2]
    /// m1: [t0p0, t0p1, t0p2, t1p0, t1p1, t1p2]
    /// </summary>
    [Fact]
    public void Assign_WhenTopicsDisbalanceByConsumer2_Successful()
    {
        var assignor = new RoundRobinAssignor();
        var topicPartitions = new List<TopicPartition>
        {
            new("t0", 0),
            new("t0", 1),
            new("t0", 2),
            new("t1", 0),
            new("t1", 1),
            new("t1", 2),
            new("t3", 0),
            new("t3", 1),
            new("t3", 2)
        };

        var m0 = new Subscription(new[]
            {
                "t3"
            },
            AutoOffsetReset.None,
            Array.Empty<IPartitionAssignor>());
        var m1 = new Subscription(new[]
            {
                "t0",
                "t1"
            },
            AutoOffsetReset.None,
            Array.Empty<IPartitionAssignor>());

        var assignResult = assignor.Assign(topicPartitions,
            new Dictionary<string, Subscription>
            {
                ["m0"] = m0,
                ["m1"] = m1
            });

        assignResult.Should().HaveCount(2);
        assignResult.Should().ContainKeys("m0", "m1");
        assignResult["m0"].Should().HaveCount(3);
        assignResult["m1"].Should().HaveCount(6);
        assignResult["m0"].Should().ContainInOrder(new TopicPartition("t3", 0), new TopicPartition("t3", 1), new TopicPartition("t3", 2));
        assignResult["m1"]
            .Should()
            .ContainInOrder(new TopicPartition("t0", 0),
                new TopicPartition("t0", 1),
                new TopicPartition("t0", 2),
                new TopicPartition("t1", 0),
                new TopicPartition("t1", 1),
                new TopicPartition("t1", 2));
    }

    //todo написать тест с автогенерацией данных
}