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

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Clients.Admin;
using NKafka.Config;
using NKafka.Protocol;

using TopicDetail = NKafka.Clients.Admin.TopicDetail;

namespace NKafka.IntegrationTests.Clients.Admin;

public class TopicsManipulationTests
{
    [Theory]
    [InlineData("topic_from_integration_tests_1", 5, 2)]
    [InlineData("topic_from_integration_tests_2", 11, 3)]
    public async Task SimpleCreateAndDeleteTopics_ShouldBe_Successful(string topicName, int partitions, short replicaFactor)
    {
        await using var kafkaCluster = await BuildKafkaCluster();

        var topics = new List<TopicDetail>
        {
            new(topicName, partitions, replicaFactor, new Dictionary<int, int>(0), new Dictionary<string, string>(0))
        };
        var createTopicResults = await kafkaCluster.AdminClient.CreateTopicsAsync(topics, new CreateTopicsOptions());
        var firstTopic = createTopicResults.First().Value;
        firstTopic.ErrorCode.Should().Be(ErrorCodes.None);
        firstTopic.NumPartitions.Should().Be(partitions);
        firstTopic.ReplicationFactor.Should().Be(replicaFactor);

        var deleteTopicsResult = await kafkaCluster.AdminClient.DeleteTopicsAsync(new[]
            {
                topicName
            },
            new DeleteTopicsOptions());

        deleteTopicsResult.First().Value.IsError.Should().BeFalse();
    }

    private static async Task<IKafkaCluster> BuildKafkaCluster()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:29091"
            }
        };

        var loggerFactory = NullLoggerFactory.Instance;
        var kafkaCluster = await clusterConfig.CreateClusterAsync(loggerFactory);

        return kafkaCluster;
    }
}