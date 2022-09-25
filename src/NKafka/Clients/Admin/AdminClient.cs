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

using Microsoft.Extensions.Logging;

using NKafka.Messages;

namespace NKafka.Clients.Admin;

internal class AdminClient: IAdminClient
{
    private readonly KafkaCluster _kafkaCluster;
    private readonly ILogger<AdminClient> _logger;

    public AdminClient(KafkaCluster kafkaCluster, ILogger<AdminClient> logger)
    {
        _kafkaCluster = kafkaCluster;
        _logger = logger;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public async Task<CreateTopicsResult> CreateTopicsAsync(IReadOnlyCollection<Topic> topics, CancellationToken token = default)
    {
        var topicCollection = new CreateTopicsRequestMessage.CreatableTopicCollection(topics.Count);

        foreach (var topic in topics)
        {
            topicCollection.Add(
                new CreateTopicsRequestMessage.CreatableTopicMessage
                {
                    Name = topic.Name,
                    NumPartitions = topic.Partitions,
                    ReplicationFactor = topic.ReplicationFactor
                });
        }

        var request = new CreateTopicsRequestMessage
        {
            Topics = topicCollection
        };

        var broker = _kafkaCluster.Controller ?? _kafkaCluster.Brokers.First();

        var result = await broker.SendAsync<CreateTopicsResponseMessage, CreateTopicsRequestMessage>(request, token);

        foreach (var topicResult in result.Topics)
        {
        }

        return new CreateTopicsResult(new Dictionary<string, TopicMetadataAndConfig>());
    }
}