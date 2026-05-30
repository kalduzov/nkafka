// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright © 2026 Aleksey Kalduzov. All rights reserved
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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NKafka.Clients.Consumer;
using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Serialization;

namespace NKafka.AspNetCore;

internal class KafkaClusterBuilder: IKafkaClusterBuilder
{
    private readonly IServiceCollection _services;
    private readonly ClusterConfig _clusterConfig;

    public string Name { get; }

    public KafkaClusterBuilder(IServiceCollection services, string name, ClusterConfig clusterConfig)
    {
        _services = services;
        _clusterConfig = clusterConfig;
        Name = name;

        _services.AddKeyedSingleton<IKafkaCluster>(name, (sp, _) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return clusterConfig.CreateCluster(loggerFactory, openImmediately: false).GetAwaiter().GetResult();
        });
    }

    public IKafkaClusterBuilder AddProducer<TProducer>(Action<ProducerConfig> options)
        where TProducer : class, IProducer
    {
        var config = new ProducerConfig();
        config.MergeFrom(_clusterConfig);
        options(config);

        var producerName = typeof(TProducer).Name;

        _services.AddKeyedSingleton(Name, config);
        _services.AddTransient<TProducer>(sp =>
        {
            var cluster = sp.GetRequiredKeyedService<IKafkaCluster>(Name);
            var producerConfig = sp.GetRequiredKeyedService<ProducerConfig>(Name);
            return (TProducer)cluster.BuildProducer(producerName, producerConfig);
        });

        return this;
    }

    public IKafkaClusterBuilder AddConsumer<TConsumer, TKey, TValue>(Action<ConsumerConfig> options)
        where TConsumer : class, IConsumer<TKey, TValue>
    {
        var config = new ConsumerConfig();
        config.MergeFrom(_clusterConfig);
        options(config);

        _services.AddKeyedSingleton(Name, config);
        _services.AddTransient<TConsumer>(sp =>
        {
            var cluster = sp.GetRequiredKeyedService<IKafkaCluster>(Name);
            var consumerConfig = sp.GetRequiredKeyedService<ConsumerConfig>(Name);
            var keyDeserializer = sp.GetRequiredService<IDeserializer<TKey>>();
            var valueDeserializer = sp.GetRequiredService<IDeserializer<TValue>>();
            return (TConsumer)cluster.BuildConsumer(consumerConfig, keyDeserializer, valueDeserializer);
        });

        return this;
    }
}