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

using System.Buffers;
using System.Diagnostics;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

using NKafka.Config;
using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Serialization;

namespace NKafka.Clients.Consumer.Internal;

internal class Fetcher<TKey, TValue>: IFetcher<TKey, TValue>
    where TValue : notnull
    where TKey : notnull
{
    private readonly ArrayPool<ConsumerRecord<TKey, TValue>> _arrayPool = ArrayPool<ConsumerRecord<TKey, TValue>>.Shared;
    private readonly bool _checkCrc;
    private readonly IsolationLevel _isolationLevel;
    private readonly IKafkaCluster _kafkaCluster;
    private readonly IAsyncDeserializer<TKey> _keyDeserializer;
    private readonly ILogger<Fetcher<TKey, TValue>> _logger;
    private readonly int _maxBytes;
    private readonly int _maxWaitTimeMs;
    private readonly int _minBytes;
    private readonly IAsyncDeserializer<TValue> _valueDeserializer;

    private CancellationTokenSource _cts = new();
    private Task _currentFetcherTask = Task.CompletedTask;

    public Fetcher(IKafkaCluster kafkaCluster,
        IAsyncDeserializer<TKey> keyDeserializer,
        IAsyncDeserializer<TValue> valueDeserializer,
        int minBytes,
        int maxBytes,
        int maxWaitTimeMs,
        bool checkCrc,
        IsolationLevel isolationLevel,
        ILoggerFactory loggerFactory)
    {
        _kafkaCluster = kafkaCluster;
        _keyDeserializer = keyDeserializer;
        _valueDeserializer = valueDeserializer;
        _minBytes = minBytes;
        _maxBytes = maxBytes;
        _maxWaitTimeMs = maxWaitTimeMs;
        _checkCrc = checkCrc;
        _isolationLevel = isolationLevel;
        _logger = loggerFactory.CreateLogger<Fetcher<TKey, TValue>>();
    }

    /// <inheritdoc />
    public async Task StartAsync(Subscription subscription, ChannelWriter<ConsumerRecord<TKey, TValue>> channelWriter, CancellationToken token)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        await UpdateAllOffsetsForFetcherAsync(subscription, _cts.Token);
        _currentFetcherTask = FetchProcessAsync(subscription, channelWriter, _cts.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel(false);
        await _currentFetcherTask;
    }

    private async Task UpdateAllOffsetsForFetcherAsync(Subscription subscription, CancellationToken ctsToken)
    {
        foreach (var topicPartition in subscription.AssignedTopicPartitions)
        {
            if (subscription.OffsetManager.GetFetchOffset(topicPartition) != -1)
            {
                continue;
            }

            var request = new ListOffsetsRequestMessage
            {
                IsolationLevel = (sbyte)_isolationLevel,
                Topics = new List<ListOffsetsRequestMessage.ListOffsetsTopicMessage>
                {
                    new()
                    {
                        Name = topicPartition.Topic,
                        Partitions = new List<ListOffsetsRequestMessage.ListOffsetsPartitionMessage>
                        {
                            new()
                            {
                                Timestamp = subscription.OffsetManager.AutoOffsetReset == AutoOffsetReset.Earliest
                                    ? ListOffsetsRequestMessage.EARLIEST_TIMESTAMP
                                    : ListOffsetsRequestMessage.LATEST_TIMESTAMP,
                                CurrentLeaderEpoch = -1,
                                PartitionIndex = topicPartition.Partition.Value
                            }
                        }
                    }
                }
            };

            var leader = _kafkaCluster.LeaderFor(topicPartition);
            var response = await _kafkaCluster.SendAsync<ListOffsetsRequestMessage, ListOffsetsResponseMessage>(request, leader.Id, ctsToken);

            var offset = response.Topics.First().Partitions.First().Offset;
            subscription.OffsetManager.UpdateOffsetForTopicPartition(topicPartition, new Offset(offset));
        }
    }

    private async Task FetchProcessAsync(Subscription subscription, ChannelWriter<ConsumerRecord<TKey, TValue>> writer, CancellationToken token)
    {
        await Task.Yield();

        /*
         * Процесс извлечения данных выглядит просто
         * Мы постоянно запрашиваем данные из разных партиций и топиков и отправляем их в канал
         * Весь процесс длится до тех пор, пока не остановится извлечение данных
         *
         */
        while (token.IsCancellationRequested is not true)
        {
            var messages = Array.Empty<ConsumerRecord<TKey, TValue>>();

            try
            {
                // Это бесконечный цикл, который постоянно жрет время CPU на бесполезных
                // обращениях к серверам при отсутствующих данных. Нужно создать механизм,
                // при котором после первого пустого ответа от сервера - следующий запрос данных по пустой парциции будет ждать некоторое
                // время.
                // Возможны как постоянный промежуток ожидания, так и экспоненциальный. Это нужно будет вынести в настройки.

                messages = await FetchOnceAsync(subscription, token);

                if (messages.Length == 0)
                {
                    await Task.Delay(100, token); // совсем нет сообщений, не будем нагружать сеть, подождем немного
                }

                foreach (var message in messages)
                {
                    if (message is null) // надо избавиться от пула тут
                    {
                        continue;
                    }

                    if (await writer.WaitToWriteAsync(token))
                    {
                        await writer.WriteAsync(message, token);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }
            finally
            {
                if (messages != Array.Empty<ConsumerRecord<TKey, TValue>>())
                {
                    _arrayPool.Return(messages);
                }
            }
        }
    }

    private async Task<ConsumerRecord<TKey, TValue>[]> FetchOnceAsync(Subscription subscription, CancellationToken token)
    {
        var requests = BuildFetchRequests(subscription);

        if (requests.Count == 0)
        {
            return Array.Empty<ConsumerRecord<TKey, TValue>>();
        }

        //todo запрос fetch должен всегда считываться динамически - нельзя его парсить сразу
        var responseTasks = requests
            .Select(r => _kafkaCluster.SendAsync<FetchRequestMessage, FetchResponseMessage>(r.Value, r.Key, token))
            .ToArray();

        await Task.WhenAll(responseTasks);

        var listResponses = new List<FetchResponseMessage.FetchableTopicResponseMessage>(responseTasks.Length);

        var maxResponsesSize = 0;

        foreach (var responseTask in responseTasks)
        {
            var response = await responseTask;

            if (!response.Code.IsSuccessCode())
            {
                _logger.LogWarning("Запрос Fetch вернул некорретный код {Code}", response.Code);

                continue;
            }
            maxResponsesSize += response.IncomingBufferLength;
            listResponses.AddRange(response.Responses);
        }

        return GetConsumerRecords(subscription, listResponses, maxResponsesSize);
    }

    private ConsumerRecord<TKey, TValue>[] GetConsumerRecords(Subscription subscription,
        IEnumerable<FetchResponseMessage.FetchableTopicResponseMessage> listResponses,
        int maxResponsesSize)
    {
        var response = _arrayPool.Rent(maxResponsesSize);
        var index = 0;

        foreach (var responses in listResponses)
        {
            var topicName = GetTopicName(responses.Topic, responses.TopicId);

            foreach (var partition in responses.Partitions)
            {
                if (!CheckSubscriptionAssigned(subscription, topicName, partition))
                {
                    _logger.LogDebug("Извлеченные данные более не нужны. Текущая подписка лишилась привязки к {Topic} {Partiton}",
                        topicName,
                        partition.PartitionIndex);

                    continue;
                }

                if (partition.Code != ErrorCodes.None)
                {
                    _logger.LogError("В ответе для {Topic} {Partiton} код ошибки не совпадает с успешным {Code}",
                        topicName,
                        partition.PartitionIndex,
                        partition.Code);

                    continue;
                }

                if (partition.Records is null)
                {
                    _logger.LogDebug("В ответе для {Topic} {Partiton} отсутствуют какие либо записи", topicName, partition.PartitionIndex);

                    continue;
                }
                ConvertToConsumerRecords(partition, topicName, response, ref index);
            }
        }

        if (response[0] is not null) //Первый элемент всегда должен быть
        {
            return response;
        }
        _arrayPool.Return(response);

        return Array.Empty<ConsumerRecord<TKey, TValue>>();

    }

    private static bool CheckSubscriptionAssigned(Subscription subscription, string topicName, FetchResponseMessage.PartitionDataMessage partition)
    {
        return subscription.IsAssignedTopicPartitions(topicName, partition.PartitionIndex);
    }

    private string GetTopicName(string name, Guid topicId)
    {
        var topicName = string.IsNullOrWhiteSpace(name) ? _kafkaCluster.TopicsById[topicId] : name;

        return topicName;
    }

    private void ConvertToConsumerRecords(FetchResponseMessage.PartitionDataMessage partitionDataMessage,
        string topic,
        ConsumerRecord<TKey, TValue>[] response,
        ref int index)
    {
        if (partitionDataMessage.Records is null)
        {
            return;
        }

        foreach (var batch in partitionDataMessage.Records.Batches)
        {
            foreach (var record in batch.Records)
            {
                try
                {
                    var key = _keyDeserializer.Deserialize(record.Key);
                    var value = _valueDeserializer.Deserialize(record.Value);
                    var message = new Message<TKey, TValue>(key, value);
                    var consumeRecord = new ConsumerRecord<TKey, TValue>(message)
                    {
                        Partition = partitionDataMessage.PartitionIndex,
                        Offset = batch.BaseOffset + record.OffsetDelta,
                        Topic = topic
                    };
                    response[index++] = consumeRecord;
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, "Не удалось считать запись по индексу {Index}", index);
                }
            }
        }
    }

    private Dictionary<int, FetchRequestMessage> BuildFetchRequests(Subscription subscription)
    {
        //todo при нормальной работе запросы вообще не меняются их можно кешировать и менять только текущий offset для разделов.
        // Инвалидировать кеш нужно в двух ситуация
        //1. Пришла ошибка на предущем запросе, что изменился лидер раздела
        //2. Изменились метаданные в кластере (не просто получены новые такие же, а именно что-то поменялось в кластере)

        var groupsAssignedTopicPartitionsByNode = subscription
            .AssignedTopicPartitions
            .GroupBy(tp => _kafkaCluster.LeaderFor(tp).Id);

        var result = new Dictionary<int, FetchRequestMessage>();

        foreach (var grouping in groupsAssignedTopicPartitionsByNode)
        {
            if (grouping.Key == -1)
            {
                continue;
            }

            var request = BuildFetchRequest(subscription, grouping.ToArray());

            if (request.IsValidRequest)
            {
                result.Add(grouping.Key, request);
            }
        }

        return result;
    }

    private FetchRequestMessage BuildFetchRequest(Subscription subscription, IReadOnlyCollection<TopicPartition> topicPartitions)
    {
        var request = new FetchRequestMessage
        {
            IsolationLevel = (sbyte)_isolationLevel,
            MaxBytes = _maxBytes,
            MinBytes = _minBytes,
            MaxWaitMs = _maxWaitTimeMs,
            ReplicaId = -1,
            SessionEpoch = -1,
            SessionId = 0
        };

        var topicPartitionsGroupByTopic = topicPartitions.GroupBy(x => x.Topic);

        var list = new List<FetchRequestMessage.FetchTopicMessage>();

        foreach (var grouping in topicPartitionsGroupByTopic)
        {
            var partitions = BuildFetchPartitionMessage(subscription, grouping.ToArray());

            if (partitions.Count > 0)
            {
                list.Add(new FetchRequestMessage.FetchTopicMessage
                {
                    Topic = grouping.Key,
                    TopicId = grouping.First().TopicId,
                    Partitions = partitions
                });
            }
        }

        if (list.Count > 0)
        {
            request.Topics.AddRange(list);
        }
        else
        {
            request.IsValidRequest = false;
        }

        return request;
    }

    private List<FetchRequestMessage.FetchPartitionMessage> BuildFetchPartitionMessage(Subscription subscription,
        IReadOnlyCollection<TopicPartition> partitions)
    {
        var result = new List<FetchRequestMessage.FetchPartitionMessage>(partitions.Count);

        var partitionsForTopic = _kafkaCluster.PartitionsForTopic(partitions.First().Topic);

        foreach (var topicPartition in partitions)
        {
            var offsetData = subscription.OffsetManager.GetFetchOffset(topicPartition) + 1; // Всегда считываем
            var fetchPartitionMessage = new FetchRequestMessage.FetchPartitionMessage
            {
                Partition = topicPartition.Partition.Value,
                FetchOffset = offsetData,
                PartitionMaxBytes = _maxBytes,
                LogStartOffset = -1,
                CurrentLeaderEpoch = partitionsForTopic.First(x => x.Partition.Value == topicPartition.Partition.Value).LeaderEpoch,
            };
            result.Add(fetchPartitionMessage);
        }

        return result;
    }
}