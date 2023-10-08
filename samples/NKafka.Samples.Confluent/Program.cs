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

using System.Diagnostics;

using Confluent.Kafka;

var stopwatch = Stopwatch.StartNew();
// var producerBuilder = new ProducerBuilder<Null, string>(
//     new AdminClientConfig
//     {
//         BootstrapServers = "localhost:29091",
//     });
//
// using var producer = producerBuilder.Build();
//
// for (var i = 0; i < 10; i++)
// {
//     producer.Produce("test",
//         new Message<Null, string>
//         {
//             Value = "test"
//         });
//}

var consumerBuilder = new ConsumerBuilder<byte[], string>(new ConsumerConfig
{
    BootstrapServers = "devkafkabroker1.s.o3.ru:9092",
    //BootstrapServers = "localhost:29091",
    GroupId = "test-k",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    FetchMaxBytes = 60000,
    MessageMaxBytes = 50000
});

using var consumer1 = consumerBuilder.Build();
//using var consumer2 = consumerBuilder.Build();

consumer1.Subscribe("avail_service_stock_queue_stocks");
//consumer2.Subscribe("test");

var record = consumer1.Consume();

Console.WriteLine(record.Message.Value);

Console.WriteLine($"All OK. {stopwatch.Elapsed}");
Console.ReadKey();