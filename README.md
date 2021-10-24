

![Hex.pm](https://img.shields.io/hexpm/l/apa)

[![Build](https://github.com/kalduzov/nkafka/actions/workflows/dotnet.yml/badge.svg)](https://github.com/kalduzov/nkafka/actions/workflows/dotnet.yml)

# NKafka

It is a high performance fully managed client for Kafka.

# Disclaimer

This project is not a port of some existing library. However, some of the solutions used in it have been adapted from other sources. The following libraries acted as such sources:

* https://github.com/apache/kafka
* https://github.com/Shopify/sarama
* https://github.com/segmentio/kafka-go
* https://github.com/confluentinc/confluent-kafka-dotnet

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

## Getting started with the repository

```
1. run git clone https://github.com/kalduzov/nkafka

2. run update_repo.ps1 or update_repo.sh 
```

