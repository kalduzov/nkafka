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

// using BenchmarkDotNet.Attributes;
// using BenchmarkDotNet.Jobs;
// using NKafka.Config;
// using NKafka.Protocol.Responses;
// using Microsoft.Extensions.Logging.Abstractions;
// using ConfluentKafka = Confluent.Kafka;
//
// namespace NKafka.Benchmarks;
//
// [MemoryDiagnoser]
// //[SimpleJob(RuntimeMoniker.Net50)]
// [SimpleJob(RuntimeMoniker.Net60)]
// public class ProduceBenchmarks
// {
//     private ConfluentKafka.IAdminClient _adminClient;
//     private IKafkaCluster _cluster;
//
//     [GlobalSetup]
//     public void GlobalSetup()
//     {
//         var adminClientBuilder = new ConfluentKafka.AdminClientBuilder(
//             new ConfluentKafka.AdminClientConfig
//             {
//                 BootstrapServers = "localhost:29091",
//             });
//
//         _adminClient = adminClientBuilder.Build();
//
//         var clusterConfig = new ClusterConfig
//         {
//             BootstrapServers = new[]
//             {
//                 "localhost:29092"
//             }
//         };
//
//         _cluster = clusterConfig
//             .CreateClusterAsync(NullLoggerFactory.Instance)
//             .GetAwaiter()
//             .GetResult();
//     }
//
//     [Benchmark(Baseline = true)]
//     public ConfluentKafka.Metadata ConfluentKafkaGetMetadata()
//     {
//         return _adminClient.GetMetadata("test", TimeSpan.FromSeconds(10));
//     }
//
//     [Benchmark]
//     public Task<MetadataResponseMessage> NewKafkaGetMetadata()
//     {
//         return _cluster.RefreshMetadataAsync(default, "test");
//     }
// }