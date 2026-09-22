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

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Implementation of a manager interface for sending messages in a kafka cluster
/// </summary>
internal sealed class MessagesSender(
    ProducerConfig config,
    IRecordAccumulator recordAccumulator,
    ITransactionManager transactionManager,
    IKafkaCluster kafkaCluster,
    IProducerMetrics metrics,
    ILoggerFactory loggerFactory)
    : IMessagesSender
{
    private enum SendCycleResult
    {
        NoWork,
        WorkCompleted,
        RetryScheduled
    }

    private readonly ILogger<MessagesSender> _logger = loggerFactory.CreateLogger<MessagesSender>();
    private CancellationTokenSource _tokenSource = new();
    private readonly IProducerMetrics _metrics = metrics;
    private readonly ManualResetEventSlim _resetEvent = new(true);

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken stoppingToken)
    {
        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        return RunAsync();
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

    private async Task RunAsync()
    {
        var delay = TimeSpan.FromMilliseconds(config.RetryBackoffMs);

        try
        {
            _logger.StartMessageSenderTrace();

            var token = _tokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                _resetEvent.Wait(token);
                // Consume the current wakeup only after observing it. A wakeup raised during processing
                // remains set and is therefore preserved for the next cycle.
                _resetEvent.Reset();
                var result = await RunOnceAsync(token);

                if (result is not SendCycleResult.WorkCompleted)
                {
                    _resetEvent.Wait(delay, token);
                }
            }
        }
        catch (OperationCanceledException exc)
        {
            _logger.LogTrace(exc, "Operation cancelled");
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "");
        }

    }

    private async Task<SendCycleResult> RunOnceAsync(CancellationToken cancellationToken)
    {

        if (transactionManager.IsTransactional)
        {
            await transactionManager.BumpIdempotentEpochAndResetIdIfNeededAsync(cancellationToken);
        }
        return await SendProducerDataAsync(cancellationToken);
    }

    private async Task<SendCycleResult> SendProducerDataAsync(CancellationToken token)
    {
        var batches = recordAccumulator.PullReadyBatches(config.MaxRequestSize);
        var hasBatches = false;
        var retryScheduled = false;

        foreach (var batch in batches)
        {
            hasBatches = true;

            try
            {
                var node = await TryGetNodeAsync(batch.TopicPartition, token);
                batch.MarkFinalized();

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

                batch.MarkSent();
                if (config.Acks == Acks.None)
                {
                    await kafkaCluster.SendAsync(produceRequestMessage, node.Id, token);
                    batch.CompleteWithoutAcknowledgement();
                    continue;
                }

                var result = await kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(produceRequestMessage, node.Id, token);

                foreach (var response in result.Responses)
                {
                    foreach (var partitionResponse in response.PartitionResponses)
                    {
                        if (partitionResponse.Code == ErrorCodes.None)
                        {
                            batch.Complete(partitionResponse.BaseOffset, partitionResponse.LogAppendTimeMs);
                        }
                        else if (IsRetriableProduceError(partitionResponse.Code))
                        {
                            _logger.Error(partitionResponse.Code);

                            // A leader-related response can make the cached node stale.
                            // Refresh metadata before requeueing so the next attempt can choose a new leader.
                            if (partitionResponse.Code is ErrorCodes.LeaderNotAvailable or
                                ErrorCodes.NotLeaderOrFollower or
                                ErrorCodes.ReplicaNotAvailable)
                            {
                                await kafkaCluster.RefreshMetadataAsync([batch.TopicPartition.Topic], token);
                            }

                            // Requeue the same immutable batch to preserve its bytes, record order, and delivery deadline.
                            if (!batch.PrepareForRetry(config.DeliveryTimeoutMs))
                            {
                                batch.Fail(partitionResponse.Code);
                            }
                            else
                            {
                                recordAccumulator.Requeue(batch);
                                retryScheduled = true;
                            }
                        }
                        else
                        {
                            _logger.Error(partitionResponse.Code);
                            batch.Fail(partitionResponse.Code);
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                batch.FailForClosing();
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Ошибка отправки пакета {TopicPartition}", batch.TopicPartition);

                if (!batch.PrepareForRetry(config.DeliveryTimeoutMs))
                {
                    batch.Fail(ErrorCodes.NetworkException);
                }
                else
                {
                    recordAccumulator.Requeue(batch);
                    retryScheduled = true;
                }
            }
        }

        return retryScheduled
            ? SendCycleResult.RetryScheduled
            : hasBatches
                ? SendCycleResult.WorkCompleted
                : SendCycleResult.NoWork;
    }

    internal static bool IsRetriableProduceError(ErrorCodes errorCode)
        // Permanent broker errors must be reported to the records instead of being retried indefinitely.
        => errorCode is ErrorCodes.LeaderNotAvailable
            or ErrorCodes.NotLeaderOrFollower
            or ErrorCodes.RequestTimedOut
            or ErrorCodes.BrokerNotAvailable
            or ErrorCodes.ReplicaNotAvailable
            or ErrorCodes.NetworkException;

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

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or
    /// resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await CastAndDispose(_tokenSource);
        await CastAndDispose(_resetEvent);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
            {
                await resourceAsyncDisposable.DisposeAsync();
            }
            else
            {
                resource.Dispose();
            }
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _tokenSource.Dispose();
        _resetEvent.Dispose();
    }
}
