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

using NKafka.Config;
using NKafka.Messages;
using NKafka.Protocol;

namespace NKafka.Tests;

public partial class ClusterTests
{
    //https://cwiki.apache.org/confluence/display/KAFKA/KIP-35+-+Retrieving+protocol+version
    private static readonly Dictionary<ApiKeys, (ApiVersion MinVersion, ApiVersion MaxVersion)>[] _apiVersions =
    {
        new()
        {
            [ApiKeys.Produce] = (ApiVersion.Version0, ApiVersion.Version3),
            [ApiKeys.Fetch] = (ApiVersion.Version2, ApiVersion.Version3),
        },
        new()
        {
            [ApiKeys.Produce] = (ApiVersion.Version1, ApiVersion.Version2),
            [ApiKeys.Fetch] = (ApiVersion.Version0, ApiVersion.Version3),
            [ApiKeys.ListOffsets] = (ApiVersion.Version0, ApiVersion.Version0),
        }
    };

    [Fact(DisplayName = "Create new cluster and version check")]
    public async Task CreateNewClusterAndVersionCheckAsync_BrokerUpdated()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:29091"
            },
            ClusterInitTimeoutMs = 10000
        };

        await using var kafkaCluster = await clusterConfig.CreateClusterInternalAsync(
            NullLoggerFactory.Instance,
            true,
            _connectorPool,
            CancellationToken.None);
        var apiByVersion = kafkaCluster.GetClusterMetadata().AggregationApiByVersion;
        apiByVersion.Should().HaveCount(2);

        apiByVersion.Keys.Should().Contain(ApiKeys.Produce);
        apiByVersion.Keys.Should().Contain(ApiKeys.Fetch);
        apiByVersion.Keys.Should().NotContain(ApiKeys.ListOffsets);
    }
}