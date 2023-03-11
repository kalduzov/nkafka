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

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Clients.Producer;
using NKafka.Clients.Producer.Internals;
using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Serialization;

namespace NKafka.Tests.Clients.Producer;

public sealed class CreateProducerTests: ProducerTests
{
    [Fact]
    public async Task BuildProducer_DefaultConfig_Successful()
    {
        await using var cluster = CreateKafkaClusterForTests();

        await cluster.OpenAsync(CancellationToken.None);

        await using var producer = cluster.BuildProducer<int, string>();
        producer.Should().NotBeNull();
    }

    [Fact]
    public void CtorSimpleProducer_Successful()
    {
        var kafkaCluster = CreateKafkaClusterForTests();

        var action = () => new Producer<int, string>(kafkaCluster,
            "test_producer",
            new ProducerConfig(),
            NoneSerializer<int>.Instance,
            NoneSerializer<string>.Instance,
            NullLoggerFactory.Instance);

        action.Should().NotThrow();
    }

    [Fact]
    public void CtorProducer_Successful()
    {
        var kafkaCluster = CreateKafkaClusterForTests();

        var transactionManagerMock = new Mock<ITransactionManager>();
        var recordAccumulatorMock = new Mock<IRecordAccumulator>();
        var messageSenderMock = new Mock<IMessagesSender>();

        var action = () => new Producer<int, string>(kafkaCluster,
            "test_producer",
            new ProducerConfig(),
            NoneSerializer<int>.Instance,
            NoneSerializer<string>.Instance,
            transactionManagerMock.Object,
            recordAccumulatorMock.Object,
            messageSenderMock.Object,
            NullLoggerFactory.Instance);

        action.Should().NotThrow();
    }

    [Fact]
    public void CtorProducer_WhenBadConfig_ShouldThrowException()
    {
        var kafkaCluster = CreateKafkaClusterForTests();

        var transactionManagerMock = new Mock<ITransactionManager>();
        var recordAccumulatorMock = new Mock<IRecordAccumulator>();
        var messageSenderMock = new Mock<IMessagesSender>();

        var action = () => new Producer<int, string>(kafkaCluster,
            "test_producer",
            new ProducerConfig
            {
                PartitionerConfig = new PartitionerConfig
                {
                    Partitioner = (Partitioner)4
                }
            },
            NoneSerializer<int>.Instance,
            NoneSerializer<string>.Instance,
            transactionManagerMock.Object,
            recordAccumulatorMock.Object,
            messageSenderMock.Object,
            NullLoggerFactory.Instance);

        action.Should().Throw<KafkaException>().Which.Message.Should().Be(Resources.ExceptionMessages.Producer_CreateError);
    }
}