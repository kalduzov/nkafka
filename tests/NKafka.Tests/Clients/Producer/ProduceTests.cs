//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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
using NKafka.Resources;

namespace NKafka.Tests.Clients.Producer;

public sealed class ProduceTests: ProducerTests
{
    private readonly ProducerConfig _producer;

    private class TestPartitioner: IPartitioner
    {
        public ValueTask<int> PartitionAsync<TKey, TValue>(string topic,
            TKey key,
            byte[] keyBytes,
            TValue value,
            byte[] valueBytes,
            IKafkaCluster cluster,
            CancellationToken token = default)
            where TKey : notnull
            where TValue : notnull
        {
            return topic switch
            {
                "test_topic" => new ValueTask<int>(1),
                "test_topic_without_partitions" => throw new ProduceException(ExceptionMessages.Producer_NoAvailablePartitions),
                _ => throw new ProduceException("No topics")
            };

        }
    }

    public ProduceTests()
    {
        _producer = new ProducerConfig
        {
            PartitionerConfig = new PartitionerConfig
            {
                Partitioner = Partitioner.Custom,
                CustomPartitionerClass = typeof(TestPartitioner)
            },
            DeliveryTimeoutMs = 1000,
            RequestTimeoutMs = 1000
        };
    }

    [Theory]
    [InlineData(1, "test")]
    public void Produce(int key, string value)
    {
        var producer = CreateProducerForTests<int, string>(_producer);
        var action = () => producer.Produce("test_topic", new Message<int, string>(key, value));
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(1, "test")]
    public async Task ProduceAsync(int key, string value)
    {
        var producer = CreateProducerForTests<int, string>(_producer);
        var action = async () => await producer.ProduceAsync("test_topic", new Message<int, string>(key, value));
        var result = await action.Should().NotThrowAsync();
        result.Subject.Status.Should().Be(PersistenceStatus.NotPersisted);
    }
}