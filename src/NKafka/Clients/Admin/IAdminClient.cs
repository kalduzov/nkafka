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

namespace NKafka.Clients.Admin;

/// <summary>
/// 
/// </summary>
public interface IAdminClient: IClient
{
    /// <summary>
    /// Create a batch of new topics
    /// </summary>
    /// <param name="topics">The new topics to create</param>
    /// <param name="options">The options to use when creating the new topics</param>
    /// <param name="token"></param>
    Task<Dictionary<string, CreateTopicResult>> CreateTopicsAsync(IReadOnlyCollection<TopicDetail> topics,
        CreateTopicsOptions options,
        CancellationToken token = default);

    /// <summary>
    /// Delete a batch of topics
    /// </summary>
    /// <param name="topicsName">The topic names to delete</param>
    /// <param name="options">The options to use when deleting the topics</param>
    /// <param name="token"></param>
    Task<Dictionary<string, DeleteTopicsResult>> DeleteTopicsAsync(IReadOnlyCollection<string> topicsName,
        DeleteTopicsOptions options,
        CancellationToken token = default);

    /// <summary>
    /// List the topics available in the cluster
    /// </summary>
    /// <param name="options">The options to use when listing the topics</param>
    /// <param name="token"></param>
    Task<IReadOnlyCollection<TopicMetadata>> ListTopicsAsync(ListTopicsOptions options, CancellationToken token = default);

    /// <summary>
    /// Describe some topics in the cluster
    /// </summary>
    /// <param name="topics">The names of the topics to describe</param>
    /// <param name="options"> The options to use when describing the topic</param>
    /// <param name="token"></param>
    Task<Dictionary<string, TopicDescription>> DescribeTopicsAsync(HashSet<string> topics,
        DescribeTopicsOptions options,
        CancellationToken token = default);

    /// <summary>
    /// Describe the cluster information.
    /// </summary>
    /// <param name="options">Describe the cluster information.</param>
    /// <param name="token"></param>
    Task<DescribeClusterResult> DescribeClusterAsync(DescribeClusterOptions options, CancellationToken token = default);

    /// <summary>
    ///  Finds ACL bindings using a filter
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    Task<DescribeAclsResult> DescribeAclsAsync(AclBindingFilter filter, DescribeAclsOptions options, CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aclBindings"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<CreateAclsResult> CreateAclsAsync(IReadOnlyCollection<AclBinding> aclBindings, CreateAclsOptions options, CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aclBindingFilters"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    Task<DeleteAclsResult> DeleteAclsAsync(IReadOnlyCollection<AclBindingFilter> aclBindingFilters,
        DeleteAclsOptions options,
        CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resources"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    Task<DescribeConfigsResult> DescribeConfigsAsync(IReadOnlyCollection<ConfigResource> resources,
        DescribeConfigsOptions options,
        CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configs"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <remarks>Since 2.3.0 use <see cref="IncrementalAlterConfigsAsync"/></remarks>
    Task<AlterConfigsResult> AlterConfigsAsync(Dictionary<ConfigResource, List<ConfigEntry>> configs,
        AlterConfigsOptions options,
        CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configs"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <remarks>This operation is supported by brokers with version 2.3.0 or higher</remarks>
    Task<IncrementalAlterConfigsResult> IncrementalAlterConfigsAsync(Dictionary<ConfigResource, List<ConfigEntry>> configs,
        IncrementalAlterConfigsOptions options,
        CancellationToken token = default);
}