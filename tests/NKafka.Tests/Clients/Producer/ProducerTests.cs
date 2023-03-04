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
using NKafka.Connection;
using NKafka.Messages;
using NKafka.Serialization;

namespace NKafka.Tests.Clients.Producer;

public abstract class ProducerTests
{
    protected static IKafkaCluster CreateKafkaClusterForTests()
    {
        var clusterConfig = new ClusterConfig();

        var connectionPool = CreateKafkaConnectorPool();

        return new KafkaCluster(clusterConfig, NullLoggerFactory.Instance, connectionPool);
    }

    protected static IProducer<TKey, TValue> CreateProducerForTests<TKey, TValue>(ProducerConfig config)
        where TKey : notnull
        where TValue : notnull
    {
        var kafkaClusterMock = new Mock<IKafkaCluster>();
        kafkaClusterMock.SetupGet(c => c.Closed).Returns(true);

        var transactionManagerMock = new Mock<ITransactionManager>();
        var recordAccumulatorMock = new Mock<IRecordAccumulator>();
        recordAccumulatorMock
            .Setup(x => x.Append(It.Is<TopicPartition>(tp => tp.Partition == 1 && tp.Topic == "test_topic"), It.IsAny<long>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<Headers>()))
            .Returns(new RecordAppendResult(new SendResultTask(new TaskCompletionSource(), 1, 0, 0, 0), false, true, 10));

        var messageSenderMock = new Mock<IMessagesSender>();

        return new Producer<TKey, TValue>(kafkaClusterMock.Object,
            "test_producer",
            config,
            NoneSerializer<TKey>.Instance,
            NoneSerializer<TValue>.Instance,
            transactionManagerMock.Object,
            recordAccumulatorMock.Object,
            messageSenderMock.Object,
            NullLoggerFactory.Instance);
    }

    private static IKafkaConnectorPool CreateKafkaConnectorPool()
    {
        var mockConnectionPool = new Mock<IKafkaConnectorPool>();
        var mockKafkaConnector1 = new Mock<IKafkaConnector>();

        mockKafkaConnector1
            .Setup(
                c => c.SendAsync<MetadataResponseMessage, MetadataRequestMessage>(
                    It.IsAny<MetadataRequestMessage>(),
                    true,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MetadataResponseMessage());

        mockConnectionPool.Setup(c => c.GetRandomConnector())
            .Returns(mockKafkaConnector1.Object);

        return mockConnectionPool.Object;
    }
}