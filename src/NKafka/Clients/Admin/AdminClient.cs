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
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol;

namespace NKafka.Clients.Admin;

internal class AdminClient: IAdminClient
{
    private readonly IKafkaCluster _kafkaCluster;
    private readonly ILogger<AdminClient> _logger;
    private readonly int _defaultTimeout;

    public AdminClient(IKafkaCluster kafkaCluster, ILogger<AdminClient> logger)
    {
        _kafkaCluster = kafkaCluster;
        _logger = logger;
        _defaultTimeout = kafkaCluster.Config.RequestTimeoutMs;
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
    public async Task<Dictionary<string, CreateTopicResult>> CreateTopicsAsync(
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
                if (string.IsNullOrWhiteSpace(topic.Name))
                {
                    throw new InvalidTopicException($"Некорректное имя топика '{topic.Name}'");
                }
                topicCollection.Add(
                    new CreateTopicsRequestMessage.CreatableTopicMessage
                    {
                        Name = topic.Name,
                        NumPartitions = topic.Partitions,
                        ReplicationFactor = topic.ReplicationFactor,
                        Configs = new CreateTopicsRequestMessage.CreateableTopicConfigCollection()
                    });
            }

            if (topics.Count == 0)
            {
                return new Dictionary<string, CreateTopicResult>(0);
            }

            var timeout = GetTimeout(options.TimeoutMs);
            var request = CreateTopicsRequestMessage.Build(timeout);
            request.validateOnly = options.ValidateOnly;
            request.Topics = topicCollection;

            var result = await _kafkaCluster.SendAsync<CreateTopicsRequestMessage, CreateTopicsResponseMessage>(request, token);

            var results = result.Topics.ToDictionary(
                x => x.Name,
                y => new CreateTopicResult(y.TopicId, y.NumPartitions, y.ReplicationFactor, y.Code));

            return results;
        }
        catch (Exception exc)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exc.Message);

            throw;
        }
    }

    /// <summary>
    /// Delete a batch of topics
    /// </summary>
    /// <param name="topicsName">The topic names to delete</param>
    /// <param name="options">The options to use when deleting the topics</param>
    /// <param name="token"></param>
    public async Task<Dictionary<string, DeleteTopicsResult>> DeleteTopicsAsync(IReadOnlyCollection<string> topicsName,
        DeleteTopicsOptions options,
        CancellationToken token = default)
    {
        var maxApiVersion = _kafkaCluster
            .GetClusterMetadata()
            .GetMaxCurrentApiVersion(ApiKeys.DeleteTopics);

        var timeout = GetTimeout(options.TimeoutMs);
        var deleteTopicsRequest = DeleteTopicsRequestMessage.Build(maxApiVersion, topicsName, timeout);
        var deleteTopicsResponse = await _kafkaCluster.SendAsync<DeleteTopicsRequestMessage, DeleteTopicsResponseMessage>(deleteTopicsRequest, token);

        var result = new Dictionary<string, DeleteTopicsResult>(deleteTopicsResponse.Responses.Count);

        foreach (var response in deleteTopicsResponse.Responses)
        {
            var exception = response.Code == ErrorCodes.None ? null : new ProtocolKafkaException(response.Code);
            result.Add(response.Name, new DeleteTopicsResult(response.Code != ErrorCodes.None, exception));
        }

        return result;
    }

    /// <summary>
    /// List  the topics available in the cluster
    /// </summary>
    /// <param name="options">The options to use when listing the topics</param>
    /// <param name="token"></param>
    public async Task<IReadOnlyCollection<TopicMetadata>> ListTopicsAsync(ListTopicsOptions options, CancellationToken token = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var timeout = GetTimeout(options.TimeoutMs);
        cts.CancelAfter(timeout);
        await _kafkaCluster.RefreshMetadataAsync(null!, cts.Token);

        return _kafkaCluster.Topics.Values
            .Where(t => t.IsInternal is false || t.IsInternal == options.IncludeInternal)
            .ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    public async Task<Dictionary<string, TopicDescription>> DescribeTopicsAsync(HashSet<string> topics,
        DescribeTopicsOptions options,
        CancellationToken token = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var timeout = GetTimeout(options.TimeoutMs);
        cts.CancelAfter(timeout);
        await _kafkaCluster.RefreshMetadataAsync(topics, cts.Token);

        var result = new Dictionary<string, TopicDescription>(topics.Count);

        foreach (var topic in _kafkaCluster.Topics.Where(t => topics.Contains(t.Key)))
        {
            var topicPartitions = _kafkaCluster.PartitionsForTopic(topic.Key);
            var topicDescription = new TopicDescription(topic.Key, topic.Value.TopicId, topic.Value.IsInternal, topicPartitions);
            result.Add(topic.Key, topicDescription);
        }

        return result;
    }

    /// <summary>
    /// Describe the cluster information.
    /// </summary>
    /// <param name="options">Describe the cluster information.</param>
    /// <param name="token"></param>
    public Task<DescribeClusterResult> DescribeClusterAsync(DescribeClusterOptions options, CancellationToken token = default)
    {
        return Task.FromResult(new DescribeClusterResult());
    }

    /// <summary>
    ///  Finds ACL bindings using a filter
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    public Task<DescribeAclsResult> DescribeAclsAsync(AclBindingFilter filter, DescribeAclsOptions options, CancellationToken token = default)
    {
        return Task.FromResult(new DescribeAclsResult());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aclBindings"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<CreateAclsResult> CreateAclsAsync(IReadOnlyCollection<AclBinding> aclBindings,
        CreateAclsOptions options,
        CancellationToken token = default)
    {
        return Task.FromResult(new CreateAclsResult());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aclBindingFilters"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    public Task<DeleteAclsResult> DeleteAclsAsync(IReadOnlyCollection<AclBindingFilter> aclBindingFilters,
        DeleteAclsOptions options,
        CancellationToken token = default)
    {
        return Task.FromResult(new DeleteAclsResult());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resources"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    public Task<DescribeConfigsResult> DescribeConfigsAsync(IReadOnlyCollection<ConfigResource> resources,
        DescribeConfigsOptions options,
        CancellationToken token = default)
    {
        return Task.FromResult(new DescribeConfigsResult());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configs"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <remarks>Since 2.3.0 use <see cref="IAdminClient.IncrementalAlterConfigsAsync"/></remarks>
    public Task<AlterConfigsResult> AlterConfigsAsync(Dictionary<ConfigResource, List<ConfigEntry>> configs,
        AlterConfigsOptions options,
        CancellationToken token = default)
    {
        return Task.FromResult(new AlterConfigsResult());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configs"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <remarks>This operation is supported by brokers with version 2.3.0 or higher</remarks>
    public Task<IncrementalAlterConfigsResult> IncrementalAlterConfigsAsync(Dictionary<ConfigResource, List<ConfigEntry>> configs,
        IncrementalAlterConfigsOptions options,
        CancellationToken token = default)
    {
        return Task.FromResult(new IncrementalAlterConfigsResult());
    }

    private int GetTimeout(int optionsTimeoutMs)
    {
        return optionsTimeoutMs == -1 ? _defaultTimeout : optionsTimeoutMs;
    }
}