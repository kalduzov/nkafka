// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 *
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Diagnostics;
using System.Security.Authentication;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

using NKafka;
using NKafka.Clients.Consumer;
using NKafka.Config;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var stopwatch = Stopwatch.StartNew();

var clusterConfig = new ClusterConfig
{
    BootstrapServers = new[]
    {
        "localhost:29091"
    },

    // SecurityProtocol = SecurityProtocols.Ssl,
    // Ssl = new SslSettings
    // {
    //     Protocols = SslProtocols.Tls12,
    //     TrustServerCertificate = true
    // },
    ClusterInitTimeoutMs = 160000, // 160сек для отладки
    MetadataUpdateTimeoutMs = 60000, // 60 секунд на обновление данных по кластеру

    //MessageMaxBytes = 400000
    //MessageMaxBytes = 20,
};

// using var tracerProvider = Sdk.CreateTracerProviderBuilder()
//     .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("NKafka.Samples"))
//     .AddSource("NKafka.Internal")
//     .AddSource("NKafka")
//     .AddJaegerExporter(
//         options =>
//         {
//             options.AgentHost = "localhost";
//             options.AgentPort = 6831;
//             options.Protocol = JaegerExportProtocol.UdpCompactThrift;
//             options.ExportProcessorType = ExportProcessorType.Simple;
//         })
//     .AddConsoleExporter()
//     .Build();
//
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {SourceContext}  {EventId}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
    .MinimumLevel.Information()
    .CreateLogger();

var loggerFactory = LoggerFactory.Create(
    builder =>
    {
        builder.AddSerilog();
        builder.SetMinimumLevel(LogLevel.Information);
    });

var logger = loggerFactory.CreateLogger<Program>();

await using var kafkaCluster = await clusterConfig.CreateClusterAsync(loggerFactory);

// var result = await kafkaCluster.AdminClient.CreateTopicsAsync(
//     new[]
//     {
//         new Topic("test-protocol", 10, 2)
//     });

// for (var i = 1000; i < 1500; i++)
// {
//     var result = await kafkaCluster.AdminClient.CreateTopicsAsync(
//         new[]
//         {
//             new Topic("test-protocol" + i, 10, 2)
//         });
//
//     //await kafkaCluster.RefreshMetadataAsync();
// }

//await kafkaCluster.RefreshMetadataAsync(default, "test");

// await using var producer = kafkaCluster.BuildProducer<Null, string>(new ProducerConfig
// {
//     PartitionerConfig = new PartitionerConfig
//     {
//         Partitioner = Partitioner.RoundRobinPartitioner
//     },
//     BatchSize = 1000
// });
//
// foreach (var val in Enumerable.Range(0, 1000))
// {
//     var test = new Message<Null, string>(Null.Instance, "test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test" + val);
//     var result = await producer.ProduceAsync("test", test);
// }

var group = Guid.NewGuid().ToString();

await using var consumer1 = kafkaCluster.BuildConsumer<string, string>(new ConsumerConfig
{
    GroupId = "test_nkafka",
    PartitionAssignors = new IPartitionAssignor[]
    {
        new RoundRobinAssignor(),
        new RangeAssignor()
    },
    AutoOffsetReset = AutoOffsetReset.Earliest,
    FetchMaxBytes = 10000
});

// await using var consumer2 = kafkaCluster.BuildConsumer<Null, int>(new ConsumerConfig
// {
//     GroupId = group,
//     PartitionAssignors = new IPartitionAssignor[] { new RoundRobinAssignor() }
// });
//
// var tasks = new List<ValueTask<ChannelReader<ConsumerRecord<Null, int>>>> { consumer1.SubscribeAsync("test"), consumer2.SubscribeAsync("test") };

// var tasks = new List<ValueTask<ChannelReader<ConsumerRecord<Null, int>>>> { consumer1.SubscribeAsync("test") };
// await Task.Delay(5000);
// tasks.Add(consumer2.SubscribeAsync("test"));

var channel1 = await consumer1.SubscribeAsync("test");

// //var channel2 = await tasks[1];
//
while (await channel1.WaitToReadAsync())
{
    var record = await channel1.ReadAsync();
    Console.WriteLine(record.Message.Value);
    await consumer1.CommitOffsetAsync(); // Этот метод закоммитит все offsets, которые были считаны, (т.е. которые вернулись после ReadAsync) последними из канала выше
}

await consumer1.UnsubscribeAsync();
// //consumer2.UnsubscribeAsync();

//await channel2.Completion;

logger.LogInformation("All OK. {Elapsed}. Нажмите любую кнопку для завершения работы приложения...", stopwatch.Elapsed);

Console.ReadKey();