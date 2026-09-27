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
using NKafka.Connection;
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
    internal enum SendCycleResult
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

    internal async Task<SendCycleResult> SendProducerDataAsync(CancellationToken token)
    {
        var batches = recordAccumulator.PullReadyBatches(config.MaxRequestSize).ToList();
        var ownedBatches = new HashSet<ProducerBatch>(batches);
        var batchesByNode = new Dictionary<int, (Node Node, List<ProducerBatch> Batches)>();
        var hasBatches = batches.Count != 0;
        var retryScheduled = false;

        try
        {
            foreach (var batch in batches)
            {
                try
                {
                    var node = await TryGetNodeAsync(batch.TopicPartition, token);
                    batch.MarkFinalized();

                    if (!batchesByNode.TryGetValue(node.Id, out var nodeBatches))
                    {
                        nodeBatches = (node, []);
                        batchesByNode.Add(node.Id, nodeBatches);
                    }

                    nodeBatches.Batches.Add(batch);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Не удалось подготовить пакет {TopicPartition}", batch.TopicPartition);
                    ownedBatches.Remove(batch);
                    retryScheduled |= FailBatchForSend(batch, exception);
                }
            }

            // Keep broker requests sequential; batches for partitions on the same broker share one frame.
            foreach (var nodeBatches in batchesByNode.Values)
            {
                var requestBatches = nodeBatches.Batches;

                try
                {
                    var topics = new ProduceRequestMessage.TopicProduceDataCollection();
                    foreach (var topicGroup in requestBatches.GroupBy(batch => batch.TopicPartition.Topic))
                    {
                        topics.Add(new ProduceRequestMessage.TopicProduceDataMessage
                        {
                            Name = topicGroup.Key,
                            PartitionData = topicGroup
                                .Select(batch => new ProduceRequestMessage.PartitionProduceDataMessage
                                {
                                    Index = batch.TopicPartition.Partition,
                                    Records = batch.GetAsRecords()
                                })
                                .ToList()
                        });
                    }

                    var produceRequestMessage = new ProduceRequestMessage
                    {
                        TimeoutMs = config.RequestTimeoutMs,
                        Acks = (short)config.Acks,
                        TopicData = topics
                    };

                    foreach (var batch in requestBatches)
                    {
                        batch.MarkSent();
                    }

                    if (config.Acks == Acks.None)
                    {
                        await kafkaCluster.SendAsync(produceRequestMessage, nodeBatches.Node.Id, token);

                        foreach (var batch in requestBatches)
                        {
                            batch.CompleteWithoutAcknowledgement();
                            ownedBatches.Remove(batch);
                        }

                        continue;
                    }

                    var result = await kafkaCluster.SendAsync<ProduceRequestMessage, ProduceResponseMessage>(
                        produceRequestMessage,
                        nodeBatches.Node.Id,
                        token);
                    var unresolvedBatches = requestBatches.ToDictionary(batch => batch.TopicPartition);

                    foreach (var response in result.Responses)
                    {
                        foreach (var partitionResponse in response.PartitionResponses)
                        {
                            var topicPartition = new TopicPartition(response.Name, partitionResponse.Index);

                            if (!unresolvedBatches.Remove(topicPartition, out var batch))
                            {
                                // Ignore unexpected or duplicate entries; missing expected entries below are retried.
                                continue;
                            }

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

                            ownedBatches.Remove(batch);
                        }
                    }

                    if (unresolvedBatches.Count != 0)
                    {
                        throw new ProtocolKafkaException(
                            ErrorCodes.NetworkException,
                            "The Produce response did not contain a result for every requested partition.");
                    }
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Ошибка отправки Produce-запроса узлу {NodeId}", nodeBatches.Node.Id);

                    foreach (var batch in requestBatches)
                    {
                        if (ownedBatches.Remove(batch))
                        {
                            retryScheduled |= FailBatchForSend(batch, exception);
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            foreach (var batch in ownedBatches)
            {
                batch.FailForClosing();
            }

            throw;
        }

        return retryScheduled
            ? SendCycleResult.RetryScheduled
            : hasBatches
                ? SendCycleResult.WorkCompleted
                : SendCycleResult.NoWork;
    }

    private bool FailBatchForSend(ProducerBatch batch, Exception exception)
    {
        // With acks=0, retrying an interrupted socket write could duplicate a record.
        if (config.Acks == Acks.None)
        {
            var status = exception is RequestWriteException
                ? PersistenceStatus.PossiblyPersisted
                : PersistenceStatus.NotPersisted;
            batch.FailForTransport(status);

            return false;
        }

        if (!batch.PrepareForRetry(config.DeliveryTimeoutMs))
        {
            batch.Fail(ErrorCodes.NetworkException);

            return false;
        }

        recordAccumulator.Requeue(batch);

        return true;
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
