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

using Microsoft.Extensions.Logging;

using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Metrics;
using NKafka.Protocol;
using NKafka.Resources;

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Implementation of a manager interface for sending messages in a kafka cluster
/// </summary>
internal class MessagesSender(ProducerConfig config, IRecordAccumulator recordAccumulator, IKafkaCluster kafkaCluster, ILoggerFactory loggerFactory)
    : IMessagesSender
{
    private readonly ILogger<MessagesSender> _logger = loggerFactory.CreateLogger<MessagesSender>();
    private CancellationTokenSource _tokenSource = new();
    private readonly IProducerMetrics _metrics = config.Metrics;
    private readonly ManualResetEventSlim _resetEvent = new(true);

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken stoppingToken)
    {
        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        //run in a dedicated thread
        return Task.Factory.StartNew(RunAsync, this, TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <inheritdoc/>
    public void Sleep()
    {
        _resetEvent.Reset();
    }

    /// <inheritdoc/>
    public void Wakeup()
    {
        _resetEvent.Set();
    }

    /// <inheritdoc/>
    public void Stop(TimeSpan timeout)
    {
    }

    private async void RunAsync(object? messageSender)
    {
        var oldThreadName = Thread.CurrentThread.Name;
        Thread.CurrentThread.Name = "Kafka producer I/O thread";

        _logger.StartMessageSenderTrace();

        try
        {
            if (messageSender is not MessagesSender sender)
            {
                throw new ArgumentException(ExceptionMessages.MessagesSenderInvalidType, nameof(messageSender));
            }
            var token = sender._tokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                _resetEvent.Wait(token);
                await RunOnceAsync(token);
                await Task.Delay(TimeSpan.FromMilliseconds(config.RetryBackoffMs), token);
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "");
        }
        finally
        {
            Thread.CurrentThread.Name = oldThreadName;
        }
    }

    private async Task RunOnceAsync(CancellationToken token)
    {
        await SendProducerDataAsync(token);
    }

    private async Task SendProducerDataAsync(CancellationToken token)
    {
        var batches = recordAccumulator.PullReadyBatches(config.MaxRequestSize);

        foreach (var batch in batches)
        {
            var node = await TryGetNodeAsync(batch.TopicPartition, token);

            var produceRequestMessage = new ProduceRequestMessage
            {
                TimeoutMs = config.RequestTimeoutMs,
                Acks = (short)config.Acks,
                TopicData =
                [
                    new ProduceRequestMessage.TopicProduceDataMessage
                    {
                        Name = batch.TopicPartition.Topic,
                        PartitionData =
                        [
                            new ProduceRequestMessage.PartitionProduceDataMessage
                            {
                                Index = batch.TopicPartition.Partition,
                                Records = batch.GetAsRecords()
                            }
                        ]
                    }
                ]
            };

            var result = await kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(produceRequestMessage, node.Id, token);

            foreach (var response in result.Responses)
            {
                foreach (var partitionResponse in response.PartitionResponses)
                {
                    if (partitionResponse.Code == ErrorCodes.None)
                    {
                        batch.Complete(partitionResponse.BaseOffset, partitionResponse.LogAppendTimeMs);
                    }
                    else
                    {
                        _logger.ErrorTrace(partitionResponse.Code);
                        batch.Fail(partitionResponse.Code);
                    }
                }
            }
        }

    }

    private async Task<Node> TryGetNodeAsync(TopicPartition topicPartition, CancellationToken token)
    {
        // Пробуем получить лидера для парцитии.
        // Если вернулась пустая нода, считаем что данных по лидеру нет в метаданных.
        // Просим кластер обновить метаданные для указанного топика, если по прежнему не удалось получить лидера - кидаем исключение
        var node = kafkaCluster.LeaderFor(topicPartition);

        if (node != Node.NoNode)
        {
            return node;
        }

        var topics = new[]
        {
            topicPartition.Topic
        };

        await kafkaCluster.RefreshMetadataAsync(topics, token);

        node = kafkaCluster.LeaderFor(topicPartition);

        if (node == Node.NoNode)
        {
            // todo данное исключение нужно обрабатывать для батча и перевыставлять батч на отправку позже
            throw new ProduceException("Отсутствует лидер для указанной парции");
        }

        return node;
    }
}