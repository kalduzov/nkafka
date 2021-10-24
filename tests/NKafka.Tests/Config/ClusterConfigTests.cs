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

public sealed class ClusterConfigTests
{
    [Fact]
    public void Validate_DefaultValues_Successful()
    {
        var config = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "test"
            }
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate).Should().NotThrow();
    }

    [Fact]
    public void Validate_WhenMetadataUpdateTimeoutMsIncorrect_MustByThrowException()
    {
        var config = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            MetadataUpdateTimeoutMs = 0
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(ClusterConfig.MetadataUpdateTimeoutMs));
    }

    [Fact]
    public void Validate_WhenClusterInitTimeoutMsIncorrect_MustByThrowException()
    {
        var config = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            ClusterInitTimeoutMs = 0
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(ClusterConfig.ClusterInitTimeoutMs));
    }

    [Theory]
    [InlineData(SecurityProtocols.Ssl)]
    [InlineData(SecurityProtocols.SaslSsl)]
    public void Validate_WhenSecurityProtocolSetAsSslButNoSslSettings_MustByThrowException(SecurityProtocols securityProtocol)
    {
        var config = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            SecurityProtocol = securityProtocol
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(ClusterConfig.Ssl));
    }

    [Theory]
    [InlineData(SecurityProtocols.SaslPlaintext)]
    [InlineData(SecurityProtocols.SaslSsl)]
    public void Validate_WhenSecurityProtocolSetAsSaslButNoSaslSettings_MustByThrowException(SecurityProtocols securityProtocol)
    {
        var config = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "test"
            },
            SecurityProtocol = securityProtocol,

            //Исключаем ошибку при значении SaslSsl, в остальном все должно остаться как было
            Ssl = securityProtocol == SecurityProtocols.SaslSsl ? new SslSettings() : SslSettings.None
        };

        void Validate()
        {
            config.Validate();
        }

        FluentActions.Invoking(Validate)
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(ClusterConfig.Sasl));
    }
}