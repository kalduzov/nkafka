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

using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka.Tests.Config;

public sealed class PartitionerConfigTests
{
    [Fact]
    public void Validate_DefaultValues_Successful()
    {
        var config = new PartitionerConfig();

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate).Should().NotThrow();
    }

    [Theory]
    [InlineData(Partitioner.RoundRobinPartitioner)]
    [InlineData(Partitioner.Default)]
    public void Validate_Partitioner_Successful(Partitioner partitioner)
    {
        var config = new PartitionerConfig
        {
            Partitioner = partitioner
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate).Should().NotThrow();
    }

    [Fact]
    public void Validate_CustomPartitioner_Successful()
    {
        var config = new PartitionerConfig
        {
            Partitioner = Partitioner.Custom,
            CustomPartitionerClass = typeof(TestPartitioner)
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate).Should().NotThrow();
    }

    [Fact]
    public void Validate_CustomPartitioner_ButClassInvalid_Successful()
    {
        var config = new PartitionerConfig
        {
            Partitioner = Partitioner.Custom,
            CustomPartitionerClass = typeof(PartitionerConfig)
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.CustomPartitionerClass));
    }

    [Fact]
    public void Validate_CustomClass_ButPartitionerInvalid_Successful()
    {
        var config = new PartitionerConfig
        {
            Partitioner = Partitioner.Default,
            CustomPartitionerClass = typeof(PartitionerConfig)
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.Partitioner));
    }

    [Fact]
    public void Validate_CustomPartitioner_ButNoCustomClass_Successful()
    {
        var config = new PartitionerConfig
        {
            Partitioner = Partitioner.Custom
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.CustomPartitionerClass));
    }

    private class TestPartitioner: IPartitioner
    {
        public ValueTask<int> PartitionAsync<TKey, TValue>(
            string topic,
            TKey key,
            byte[] keyBytes,
            TValue value,
            byte[] valueBytes,
            IKafkaCluster cluster,
            CancellationToken token = default)
            where TKey : notnull
            where TValue : notnull
        {
            return default;
        }
    }
}