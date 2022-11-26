# NKafka

It is a high performance fully managed client for Kafka.

## Quick start

```dotnet-nuget install Nkafka```

### Simple producing message

```csharp

using NKafka;
using NKafka.Config;

var clusterConfig = new ClusterConfig
{
    BootstrapServers = new[]
    {
        "127.0.0.1:29091"
    },
};

await using var kafkaCluster = await clusterConfig.CreateClusterAsync();

await using var producer = kafkaCluster.BuildProducer<Null, int>();

var testMessage = new Message<Null, int>
{
    Value = 1,
    Partition = new Partition(1)
};

//send message with awaiting
await producer.ProduceAsync("test_topic", testMessage);

//send message as fire and forget
producer.Produce("test_topic", testMessage);

Console.ReadKey();

```

## Начало работы с репозитарием

```
1. run git clone https://github.com/kalduzov/nkafka

2. run update_repo.ps1 or update_repo.sh 
```