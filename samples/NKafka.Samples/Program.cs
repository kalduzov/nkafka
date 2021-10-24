using System.Diagnostics;

using Microsoft.Extensions.Logging;

using NKafka;
using NKafka.Config;

using Serilog;

var stopwatch = Stopwatch.StartNew();

var clusterConfig = new ClusterConfig
{
    BootstrapServers = new[]
    {
        "127.0.0.1:29091"
    },

    //SecurityProtocol = SecurityProtocols.Ssl,
    //Ssl = new SslSettings(),
    ClusterInitTimeoutMs = 160000, //160сек для отладки
    MetadataUpdateTimeoutMs = 10000, //10 секунд на обновление данных по кластеру
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
    .WriteTo.Console()
    .MinimumLevel.Verbose()
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

await using var producer = kafkaCluster.BuildProducer<Null, string>(new ProducerConfig
{
    PartitionerConfig = new PartitionerConfig
    {
        Partitioner = Partitioner.RoundRobinPartitioner
    }
});

foreach (var val in Enumerable.Range(0, 100))
{
    var test = new Message<Null, string>(Null.Instance, "test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test test" + val);
    producer.Produce("test", test);
}

logger.LogInformation("All OK. {Elapsed}. Нажмите любую кнопку для завершения работы приложения...", stopwatch.Elapsed);

Console.ReadKey();