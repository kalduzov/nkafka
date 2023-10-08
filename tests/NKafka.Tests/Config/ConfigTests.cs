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

using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka.Tests.Config;

public sealed class ConfigTests
{
    [Fact]
    public void Validate_WhenBootstrapServersNoSet_MustByThrowException()
    {
        var config = new TestCommonConfig();

        FluentActions.Invoking(() => config.Validate())
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

        FluentActions.Invoking(() => config.Validate()).Should().NotThrow();
    }

    [Fact]
    public void Validate_WhenBootstrapServersSetAsNull_MustByThrowException()
    {
        var config = new TestCommonConfig
        {
            BootstrapServers = null!
        };

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.BootstrapServers));
    }

    [Fact]
    public void Validate_WhenBrokerVersionSetAsNull_MustByThrowException()
    {
        var config = new TestCommonConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            FallbackBrokerVersion = null!
        };

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.FallbackBrokerVersion));
    }

    [Fact]
    public void Validate_WhenBrokerVersionSetAsIncorrect_ShouldByThrowException()
    {
        var config = new TestCommonConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            FallbackBrokerVersion = new Version(4, 0)
        };

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.FallbackBrokerVersion));
    }

    [Theory]
    [InlineData("0.0")]
    [InlineData("2.0")]
    [InlineData("2.1")]
    [InlineData("2.2")]
    [InlineData("2.3")]
    [InlineData("2.4")]
    [InlineData("2.5")]
    [InlineData("2.6")]
    [InlineData("2.7")]
    [InlineData("2.8")]
    [InlineData("3.0")]
    [InlineData("3.1")]
    [InlineData("3.2")]
    [InlineData("3.3")]
    [InlineData("3.4")]
    public void Validate_SupportBrokerVersion_Successful(string version)
    {
        var config = new TestCommonConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            FallbackBrokerVersion = Version.Parse(version)
        };

        config.Validate();
    }

    private record TestCommonConfig: CommonConfig;
}