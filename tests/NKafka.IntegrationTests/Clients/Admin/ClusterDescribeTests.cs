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

namespace NKafka.IntegrationTests.Clients.Admin;

public class ClusterDescribeTests
{
    [Fact]
    public async Task DescribeCluster_ShouldBe_Successful()
    {
        await using var kafkaCluster = await BuildKafkaCluster();
        var result = await kafkaCluster.AdminClient.DescribeClusterAsync(new DescribeClusterOptions());
        result.Controller.Should().NotBeNull();
        result.Nodes.Count.Should().Be(5);
        result.Nodes.Contains(result.Controller).Should().BeTrue();
        result.ClusterId.Should().NotBeNull();
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