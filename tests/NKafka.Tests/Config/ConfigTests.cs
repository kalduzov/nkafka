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

using NKafka.Config;
using NKafka.Exceptions;

using Xunit;

namespace NKafka.Tests.Config;

public class ConfigTests
{
    private record TestCommonConfig: CommonConfig;

    [Fact]
    public void Validate_WhenBootstrapServersNoSet_MustByThrowException()
    {
        var config = new TestCommonConfig();

        void Validate()
            => config.Validate();

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.BootstrapServers));
    }

    [Fact]
    public void Validate_WhenBootstrapServersSet_Successful()
    {
        var config = new TestCommonConfig
        {
            BootstrapServers = new[]
            {
                "test"
            }
        };

        void Validate()
            => config.Validate();

        FluentActions.Invoking(Validate).Should().NotThrow();
    }

    [Fact]
    public void Validate_WhenBootstrapServersSetAsNull_MustByThrowException()
    {
        var config = new TestCommonConfig
        {
            BootstrapServers = null
        };

        void Validate()
            => config.Validate();

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.BootstrapServers));
        ;
    }
}