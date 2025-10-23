// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright © 2025 Aleksey Kalduzov. All rights reserved
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

namespace NKafka.Tests;

// Unit tests for the OpenAsync method
public partial class ClusterTests
{
    [Fact]
    public async Task OpenAsync_ShouldBe_Successful()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers =
            [
                "localhost:64000"
            ],
            MetadataUpdateTimeoutMs = 100,
            IsFullUpdateMetadata = false,
            ClusterInitTimeoutMs = 1 //из-за имитации сетевого вызова - этот код отработает корректно
        };

        await using var kafkaCluster = await clusterConfig.CreateClusterInternal(
            NullLoggerFactory.Instance,
            false,
            _connectorPool,
            CancellationToken.None);

        kafkaCluster.Brokers.Should().HaveCount(0);
        kafkaCluster.Controller.Should().Be(Node.NoNode);
        kafkaCluster.ClusterId.Should().BeNull();

        await kafkaCluster.OpenAsync(CancellationToken.None);

        await Task.Delay(1000, CancellationToken.None);
    }
}