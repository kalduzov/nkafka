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

using System.Diagnostics;

using Microsoft.Extensions.Logging;

using NKafka.Diagnostics;
using NKafka.Messages;

namespace NKafka.Clients.Admin;

internal class AdminClient: IAdminClient
{
    private readonly IKafkaCluster _kafkaCluster;
    private readonly ILogger<AdminClient> _logger;

    public AdminClient(IKafkaCluster kafkaCluster, ILogger<AdminClient> logger)
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

    /// <inheritdoc/>
    public async Task<CreateTopicsResult> CreateTopicsAsync(IReadOnlyCollection<TopicDetail> topics, CancellationToken token = default)
    {
        _logger.CallMethodTrace();
        using var activity = KafkaDiagnosticsSource.CreateTopics(topics);

        try
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

            var result = await _kafkaCluster.SendAsync<CreateTopicsResponseMessage, CreateTopicsRequestMessage>(request, token);
            var results = result.Topics.ToDictionary(
                x => x.Name,
                y => new TopicMetadataAndConfig(y.TopicId, y.NumPartitions, y.ReplicationFactor, y.Code));

            return new CreateTopicsResult(results);
        }
        catch (Exception exc)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exc.Message);

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<CreateTopicsResult> CreateTopicsAsync(
        IReadOnlyCollection<TopicDetail> topics,
        CreateTopicsOptions options,
        CancellationToken token = default)
    {
        _logger.CallMethodTrace();
        using var activity = KafkaDiagnosticsSource.CreateTopics(topics);

        try
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
                Topics = topicCollection,
                timeoutMs = options.TimeoutMs,
                validateOnly = options.ValidateOnly
            };

            var result = await _kafkaCluster.SendAsync<CreateTopicsResponseMessage, CreateTopicsRequestMessage>(request, token);
            var results = result.Topics.ToDictionary(
                x => x.Name,
                y => new TopicMetadataAndConfig(y.TopicId, y.NumPartitions, y.ReplicationFactor, y.Code));

            return new CreateTopicsResult(results);
        }
        catch (Exception exc)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exc.Message);

            throw;
        }
    }
}