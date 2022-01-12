// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Confluent.Kafka;


var stopwatch = Stopwatch.StartNew();
var adminClientBuilder = new AdminClientBuilder(
    new AdminClientConfig
    {
        BootstrapServers = "localhost:29091",
    });

using var adminClient = adminClientBuilder.Build();

var tasks = Enumerable.Range(0, 10000)
    .Select(
        i =>
        {
            //Console.WriteLine($"Ok {i}");

            return Task.Run(
                () =>
                {
                    var meta = adminClient.GetMetadata("test", TimeSpan.FromSeconds(10));
                    //Console.WriteLine(meta.OriginatingBrokerId);
                });
        });

await Task.WhenAll(tasks);

Console.WriteLine($"All OK. {stopwatch.Elapsed}");
Console.ReadKey();