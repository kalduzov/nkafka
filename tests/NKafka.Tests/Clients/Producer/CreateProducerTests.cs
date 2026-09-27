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

using NKafka.Clients.Producer.Internals;
using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Metrics;

namespace NKafka.Tests.Clients.Producer;

public sealed class CreateProducerTests: ClientTests
{
    [Fact]
    public async Task DisposeAsync_WhenSenderExceedsTimeout_DefersResourceDisposalUntilSenderStops()
    {
        var senderTask = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var resourcesDisposed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var sender = Substitute.For<IMessagesSender>();
        sender.StartAsync(Arg.Any<CancellationToken>()).Returns(senderTask.Task);
        sender.DisposeAsync().Returns(_ =>
        {
            resourcesDisposed.TrySetResult();
            return ValueTask.CompletedTask;
        });
        var kafkaCluster = Substitute.For<IKafkaCluster>();

        var producer = new NKafka.Clients.Producer.Producer(
            kafkaCluster,
            "test_producer",
            new ProducerConfig { ClientDisposeTimeoutMs = 20 },
            Substitute.For<ITransactionManager>(),
            Substitute.For<IRecordAccumulator>(),
            sender,
            new NullProducerMetrics(),
            NullLoggerFactory.Instance);

        var firstDispose = producer.DisposeAsync().AsTask();
        var repeatedDispose = producer.DisposeAsync().AsTask();

        producer.Should().NotBeAssignableTo<IDisposable>();
        await firstDispose.WaitAsync(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        repeatedDispose.Should().BeSameAs(firstDispose);
        resourcesDisposed.Task.IsCompleted.Should().BeFalse();

        senderTask.SetResult();
        await resourcesDisposed.Task.WaitAsync(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        await sender.Received(1).DisposeAsync();
        kafkaCluster.DidNotReceive().Dispose();
        await kafkaCluster.DidNotReceive().DisposeAsync();
    }

    [Fact]
    public async Task BuildProducer_DefaultConfig_Successful()
    {
        await using var cluster = CreateKafkaClusterForTests();

        await cluster.OpenAsync(CancellationToken.None);

        await using var producer = cluster.BuildProducer();
        producer.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateProducer_ReturnsNewInstances()
    {
        await using var cluster = CreateKafkaClusterForTests();

        await cluster.OpenAsync(CancellationToken.None);

        await using var first = cluster.CreateProducer(new ProducerConfig());
        await using var second = cluster.CreateProducer(new ProducerConfig());

        first.Should().NotBeSameAs(second);
    }

    [Fact]
    public void CtorSimpleProducer_Successful()
    {
        var kafkaCluster = CreateKafkaClusterForTests();

        var action = () => new NKafka.Clients.Producer.Producer(kafkaCluster,
            "test_producer",
            new ProducerConfig(),
            NullLoggerFactory.Instance);

        action.Should().NotThrow();
    }

    [Fact]
    public void CtorProducer_Successful()
    {
        var kafkaCluster = CreateKafkaClusterForTests();

        var transactionManagerMock = Substitute.For<ITransactionManager>();
        var recordAccumulatorMock = Substitute.For<IRecordAccumulator>();
        var messageSenderMock = Substitute.For<IMessagesSender>();

        var action = () => new NKafka.Clients.Producer.Producer(kafkaCluster,
            "test_producer",
            new ProducerConfig(),
            transactionManagerMock,
            recordAccumulatorMock,
            messageSenderMock,
            new NullProducerMetrics(),
            NullLoggerFactory.Instance);

        action.Should().NotThrow();
    }

    [Fact]
    public void CtorProducer_WhenBadConfig_ShouldThrowException()
    {
        var kafkaCluster = CreateKafkaClusterForTests();

        var transactionManagerMock = Substitute.For<ITransactionManager>();
        var recordAccumulatorMock = Substitute.For<IRecordAccumulator>();
        var messageSenderMock = Substitute.For<IMessagesSender>();

        var action = () => new NKafka.Clients.Producer.Producer(kafkaCluster,
            "test_producer",
            new ProducerConfig
            {
                PartitionerConfig = new PartitionerConfig
                {
                    Partitioner = (Partitioner)4
                }
            },
            transactionManagerMock,
            recordAccumulatorMock,
            messageSenderMock,
            new NullProducerMetrics(),
            NullLoggerFactory.Instance);

        action.Should().Throw<KafkaException>().Which.Message.Should().Be(Resources.ExceptionMessages.Producer_CreateError);
    }
}
