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
using NKafka.Exceptions;
using NKafka.Tests.Clients.Consumer;

namespace NKafka.Tests.Config;

public class ConsumerConfigTests
{
    [Fact]
    public void Validate_DefaultValues_Successful()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            GroupId = "test_group"
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate).Should().NotThrow();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-200)]
    [InlineData(int.MinValue)]
    public void Validate_ChannelSize_WhenValueLessThenZero_MustBeThrowException(int channelSize)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            GroupId = "test_group",
            ChannelSize = channelSize
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.ChannelSize));
        ;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Validate_GroupId_WhenValueNullOrEmpty_MustBeThrowException(string? groupId)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            GroupId = groupId!,
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.GroupId));
        ;
    }

    [Fact]
    public void Validate_AutoCommitIntervalMs_WhenEnableAutoCommitIsSet_And_ValueLessThenZero_MustBeThrowException()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            GroupId = "test_group",
            EnableAutoCommit = true,
            AutoCommitIntervalMs = 0
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.AutoCommitIntervalMs));
    }

    [Fact]
    public void Validate_PartitionAssignors_WhenEmpty_MustBeThrowException()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            GroupId = "test_group",
            PartitionAssignors = Array.Empty<IPartitionAssignor>()
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.PartitionAssignors));
    }

    [Fact]
    public void Validate_PartitionAssignors_WhenValuesNotUnique_MustBeThrowException()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            GroupId = "test_group",
            PartitionAssignors = new[]
            {
                new RoundRobinAssignor(),
                new RoundRobinAssignor()
            }
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.PartitionAssignors));
    }

    [Fact]
    public void Validate_PartitionAssignors_WhenValuesUnique_Successful()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            GroupId = "test_group",
            PartitionAssignors = new IPartitionAssignor[]
            {
                new RoundRobinAssignor(),
                new RangeAssignor()
            }
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate).Should().NotThrow();
    }
}