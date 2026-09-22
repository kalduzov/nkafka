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

public sealed class ProducerConfigTests
{
    [Fact]
    public void Validate_DefaultValues_Successful()
    {
        var config = new ProducerConfig
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

    [Theory]
    [InlineData(nameof(ProducerConfig.EnqueueTimeoutMs), 0)]
    [InlineData(nameof(ProducerConfig.DeliveryTimeoutMs), 0)]
    [InlineData(nameof(ProducerConfig.BatchSize), 0)]
    [InlineData(nameof(ProducerConfig.BufferMemory), 0)]
    [InlineData(nameof(ProducerConfig.MaxRequestSize), 0)]
    public void Validate_WhenPositiveProducerSettingIsNotPositive_MustThrowException(string optionName, int value)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = ["test"]
        };
        typeof(ProducerConfig).GetProperty(optionName)!.SetValue(config, value);

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(optionName);
    }

    [Fact]
    public void Validate_WhenDeliveryTimeoutDoesNotCoverLingerAndRequest_MustThrowException()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = ["test"],
            DeliveryTimeoutMs = 100,
            LingerMs = 50,
            RequestTimeoutMs = 51
        };

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.DeliveryTimeoutMs));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(-1)]
    public void Validate_WhenLingerIsInvalid_MustThrowException(double lingerMs)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = ["test"],
            LingerMs = lingerMs
        };

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.LingerMs));
    }

    [Fact]
    public void Validate_WhenAcksIsUnknown_MustThrowException()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = ["test"],
            Acks = (Acks)42
        };

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.Acks));
    }

    [Fact]
    public void Validate_WhenCompressionIsUnknown_MustThrowException()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = ["test"],
            Compression = new CompressionConfig((CompressionType)42)
        };

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.Compression));
    }

    [Fact]
    public void Validate_WhenMaxPendingProduceRequestsIsNotPositive_MustThrowException()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = ["test"],
            MaxPendingProduceRequests = 0
        };

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.MaxPendingProduceRequests));
    }

    [Fact]
    public void Validate_WhenMaxQueuedMessagesIsNotPositive_MustThrowException()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = ["test"],
            MaxQueuedMessages = 0
        };

        FluentActions.Invoking(() => config.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(config.MaxQueuedMessages));
    }

    [Fact]
    public void BaseFrom_CopiesCommonSettingsWithoutSharingMutableValues()
    {
        var common = new TestCommonConfig
        {
            BootstrapServers = ["broker:9092"],
            ClientId = "client",
            MaxRetries = 7,
            ClientDisposeTimeoutMs = 1234,
            Ssl = new SslSettings { TrustServerCertificate = true },
            PerBrokerConfigs = new Dictionary<int, BrokerConfig>
            {
                [1] = new BrokerConfig { BrokerVersion = new Version(3, 8) }
            }
        };

        var producer = ProducerConfig.BaseFrom(common);

        producer.ClientId.Should().Be("client");
        producer.MaxRetries.Should().Be(7);
        producer.ClientDisposeTimeoutMs.Should().Be(1234);
        producer.BootstrapServers.Should().BeEquivalentTo("broker:9092");
        producer.Ssl.Should().NotBeSameAs(common.Ssl);
        producer.PerBrokerConfigs.Should().NotBeSameAs(common.PerBrokerConfigs);
        producer.PerBrokerConfigs[1].Should().NotBeSameAs(common.PerBrokerConfigs[1]);

        common.BootstrapServers = ["changed:9092"];
        common.PerBrokerConfigs[1].BrokerVersion = new Version(3, 9);

        producer.BootstrapServers.Should().BeEquivalentTo("broker:9092");
        producer.PerBrokerConfigs[1].BrokerVersion.Should().Be(new Version(3, 8));
    }

    [Fact]
    public void MergeFrom_PreservesProducerAndTransactionalSettings()
    {
        var config = new TransactionalProducerConfig
        {
            TransactionalId = "tx-1",
            TransactionTimeoutMs = 12_000,
            EnqueueTimeoutMs = 2_000,
            BatchSize = 42,
            Compression = new CompressionConfig(CompressionType.Gzip)
        };
        var common = new TestCommonConfig
        {
            BootstrapServers = ["broker:9092"],
            ClientDisposeTimeoutMs = 9_000
        };

        var merged = config.MergeFrom(common);

        merged.Should().BeOfType<TransactionalProducerConfig>();
        var transactional = (TransactionalProducerConfig)merged;
        transactional.TransactionalId.Should().Be("tx-1");
        transactional.TransactionTimeoutMs.Should().Be(12_000);
        transactional.EnqueueTimeoutMs.Should().Be(2_000);
        transactional.BatchSize.Should().Be(42);
        transactional.Compression.Should().NotBeSameAs(config.Compression);
        transactional.ClientDisposeTimeoutMs.Should().Be(9_000);
    }

    private sealed record TestCommonConfig : CommonConfig;
}
