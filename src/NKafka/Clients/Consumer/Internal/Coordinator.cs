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

using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Metrics;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;

namespace NKafka.Clients.Consumer.Internal;

internal class Coordinator: ICoordinator
{
    /// <summary>
    /// For consumer use only "consumer" protocol type
    /// </summary>
    private const string _PROTOCOL_TYPE = "consumer";

    private readonly TaskCompletionSource _activeSessionAwaiter = new();
    private readonly int _autoCommitIntervalMs;
    private readonly IConsumerMetrics _consumerMetrics;
    private readonly bool _enableAutoCommit;
    private readonly string _groupId;
    private readonly string? _groupInstanceId = null;
    private readonly Task _heartbeatTask;

    private readonly IKafkaCluster _kafkaCluster;
    private readonly ILogger _logger;
    private readonly int _maxRetries;
    private readonly int _rebalanceTimeoutMs;
    private readonly long _retryBackoffMs;
    private readonly int _sessionTimeoutMs;
    private readonly IDictionary<string, IPartitionAssignor> _supportPartitionAssignors;
    private readonly PeriodicTimer _timer;

    private readonly CancellationTokenSource _tokenSource = new();
    private IKafkaConnector _coordinatorConnector = KafkaConnector.Null;
    private volatile bool _disposed;
    private volatile bool _disposing;
    private HeartbeatRequestMessage? _heartbeatRequest;
    private string? _protocolName;

    private IPartitionAssignor? _selectedPartitionAssignor;

    private CoordinatorState _state = CoordinatorState.Initialization;

    public Coordinator(IKafkaCluster kafkaCluster,
        string groupId,
        HeartbeatSettings heartbeatSettings,
        IDictionary<string, IPartitionAssignor> supportPartitionAssignors,
        bool enableAutoCommit,
        int autoCommitIntervalMs,
        int rebalanceTimeoutMs,
        int sessionTimeoutMs,
        int maxRetries,
        long retryBackoffMs,
        IConsumerMetrics consumerMetrics,
        ILoggerFactory loggerFactory)
    {
        _kafkaCluster = kafkaCluster;
        _groupId = groupId;
        _supportPartitionAssignors = supportPartitionAssignors;
        _enableAutoCommit = enableAutoCommit;
        _autoCommitIntervalMs = autoCommitIntervalMs;
        _rebalanceTimeoutMs = rebalanceTimeoutMs;
        _sessionTimeoutMs = sessionTimeoutMs;
        _maxRetries = maxRetries;
        _retryBackoffMs = retryBackoffMs;
        _consumerMetrics = consumerMetrics;
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(heartbeatSettings.IntervalMs));
        _logger = loggerFactory.CreateLogger($"{nameof(Coordinator)}.{_groupId}");
        _logger.StartingCoordinatorTrace(groupId);
        _heartbeatTask = HeartbeatLoop();
    }

    /// <inheritdoc />
    public int GenerationId { get; private set; }

    /// <inheritdoc />
    public string MemberId { get; private set; } = string.Empty;

    /// <inheritdoc />
    public bool IsLeader { get; private set; }

    /// <inheritdoc />
    public async ValueTask<bool> NewSessionAsync(Subscription subscription, CancellationToken token)
    {
        _logger.LogInformation("Start consumer new session with session timeout {SessionTimeout} ms", _sessionTimeoutMs);

        try
        {
            await TryFindCoordinatorForGroupAsync(token);
            await _kafkaCluster.RefreshMetadataAsync(subscription.Topics, token);
            await JoinToGroupAsync(subscription, token);

            _heartbeatRequest = null;
            _activeSessionAwaiter.SetResult();

            await FetchOffsetsAsync(subscription, token);
            await _kafkaCluster.RefreshMetadataAsync(subscription.Topics, token);

        }
        catch (ProtocolKafkaException exc)
        {
            if (exc.InternalError == ErrorCodes.UnknownTopicOrPartition)
            {
                _logger.LogError(exc, "Не удалось создать новую сессию работы координатора группы");

                throw;
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Не удалось создать новую сессию работы координатора группы");

            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public async ValueTask FetchOffsetsAsync(Subscription subscription, CancellationToken token)
    {
        var currentApiVersion = _kafkaCluster
            .GetClusterMetadata()
            .GetMaxCurrentApiVersion(ApiKeys.OffsetFetch);

        var request = OffsetFetchRequestMessage.Build(currentApiVersion, _groupId, true, subscription.AssignedPartitionsByTopic);

        var response = await _coordinatorConnector.SendAsync<OffsetFetchResponseMessage, OffsetFetchRequestMessage>(request, false, token);

        if (response.Code == ErrorCodes.None)
        {
            var dictionary = new Dictionary<string, TopicPartitionOffset[]>();

            if (response.Groups.Count == 1)
            {
                var groupInfo = response.Groups[0];

                if (groupInfo.groupId != _groupId)
                {
                    throw new KafkaException("В ответе на запрос offsets пришла неизвестная группа");
                }

                foreach (var topicsMessage in groupInfo.Topics)
                {
                    var tpo = topicsMessage.Partitions
                        .Select(x => new TopicPartitionOffset(topicsMessage.Name, x.PartitionIndex, x.CommittedOffset))
                        .ToArray();

                    dictionary.Add(topicsMessage.Name, tpo);
                }

            }
            else if (response.Topics.Count > 0)
            {
                foreach (var topicsMessage in response.Topics)
                {
                    var tpo = topicsMessage.Partitions
                        .Select(x => new TopicPartitionOffset(topicsMessage.Name, x.PartitionIndex, x.CommittedOffset))
                        .ToArray();

                    dictionary.Add(topicsMessage.Name, tpo);
                }
            }

            // Теперь надо обновить текущее состояние менеджера
            subscription.OffsetManager.Init(dictionary);
        }
    }

    /// <inheritdoc />
    public ValueTask StopSessionAsync(CancellationToken token)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offsetManager"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask CommitAsync(IOffsetManager offsetManager, CancellationToken token)
    {
        var request = new OffsetCommitRequestMessage
        {
            GroupId = _groupId,
            MemberId = MemberId,
            GenerationId = GenerationId,
            Topics = offsetManager.GetAllTopicPartitionOffset()
                .GroupBy(t => t.Topic)
                .Select(x => new OffsetCommitRequestMessage.OffsetCommitRequestTopicMessage
                {
                    Name = x.Key,
                    Partitions = x.Select(z => new OffsetCommitRequestMessage.OffsetCommitRequestPartitionMessage
                        {
                            PartitionIndex = z.Partition,
                            CommittedOffset = z.Offset
                        })
                        .ToList()
                })
                .ToList(),
        };

        var result = await _coordinatorConnector.SendAsync<OffsetCommitResponseMessage, OffsetCommitRequestMessage>(request, false, token);

        if (result.Topics[0].Partitions[0].Code == ErrorCodes.None)
        {
            return;
        }

        throw new ProtocolKafkaException(result.Topics[0].Partitions[0].Code);

    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposing && _disposed)
        {
            return;
        }
        _disposing = true;

        await LeaveGroupAsync(_tokenSource.Token);
        _tokenSource.CancelAfter(100); //todo настраиваемый параметр нужно сделать
        await _heartbeatTask;
        _tokenSource.Dispose();
        _timer.Dispose();
        _disposed = true;
    }

    private async Task<ConsumerProtocolAssignment> SyncGroupAsync(SyncGroupRequestMessage request, CancellationToken token)
    {
        var response = await _coordinatorConnector.SendAsync<SyncGroupResponseMessage, SyncGroupRequestMessage>(request, false, token);

        switch (response.Code)
        {
            case ErrorCodes.None:
                {
                    if (!response.ProtocolType?.Equals(_PROTOCOL_TYPE) ?? false)
                    {
                        throw new KafkaException(
                            $"SyncGroup failed due to inconsistent Protocol Type, received {response.ProtocolType} but expected {_PROTOCOL_TYPE}");
                    }

                    return GetConsumerProtocolAssignment(response);
                }
            case ErrorCodes.GroupAuthorizationFailed:
                break;
            case ErrorCodes.RebalanceInProgress:
                break;
            case ErrorCodes.FencedInstanceId:
                break;
            case ErrorCodes.UnknownMemberId:
            case ErrorCodes.IllegalGeneration:
                break;
            case ErrorCodes.CoordinatorNotAvailable:
            case ErrorCodes.NotCoordinator:
                break;
            default:
                throw new ProtocolKafkaException(response.Code, "Unexpected error from SyncGroup");
        }

        throw new NotSupportedException();
    }

    private static ConsumerProtocolAssignment GetConsumerProtocolAssignment(SyncGroupResponseMessage response)
    {
        var bufferReader = new BufferReader(response.Assignment);
        var version = bufferReader.ReadShort();
        var cpa = new ConsumerProtocolAssignment(ref bufferReader, (ApiVersion)version);

        return cpa;
    }

    private async Task JoinToGroupAsync(Subscription subscription, CancellationToken token)
    {
        try
        {
            var currentApiVersion = _kafkaCluster
                .GetClusterMetadata()
                .GetMaxCurrentApiVersion(ApiKeys.JoinGroup);

            var protocols = BuildSupportProtocols(subscription);

            //first request
            var joinGroupRequestMessage = JoinGroupRequestMessage.Build(currentApiVersion,
                _groupId,
                _groupInstanceId,
                protocols,
                _rebalanceTimeoutMs,
                _sessionTimeoutMs,
                MemberId);

            var response =
                await _coordinatorConnector.SendAsync<JoinGroupResponseMessage, JoinGroupRequestMessage>(joinGroupRequestMessage, false, token);

            // Ok. This is the correct error code for the first request. MemberId came to us - we repeat the same request with this value.
            if (response.Code == ErrorCodes.MemberIdRequired)
            {
                joinGroupRequestMessage.MemberId = response.MemberId;
                joinGroupRequestMessage.Reason = "Send with memberId";

                response = await _coordinatorConnector.SendAsync<JoinGroupResponseMessage, JoinGroupRequestMessage>(joinGroupRequestMessage,
                    false,
                    token);
            }

            // Other errors - we consider that it was not possible to join the group
            if (response.Code != ErrorCodes.None)
            {
                throw new ProtocolKafkaException(response.Code, "Консьюмеру не удалось присоединиться к группе");
            }

            MemberId = response.MemberId;
            IsLeader = response.Leader == MemberId;
            GenerationId = response.GenerationId;
            _protocolName = response.ProtocolName; // выбранный протокол координатором группы для всех консьюмеров

            subscription.GenerationId = GenerationId;
            _selectedPartitionAssignor = _supportPartitionAssignors[_protocolName];

            if (IsLeader && response.SkipAssignment)
            {
                ChangeState(CoordinatorState.JoinedToGroup);

                return;
            }
            var topicPartitions = IsLeader
                ? await LeaderElectedAsync(response.Members, token)
                : await FollowerAsync(token);

            ChangeState(CoordinatorState.JoinedToGroup);
            subscription.Assign(topicPartitions);

        }
        catch (ProtocolKafkaException exc)
        {
            _logger.LogError(exc, "Joining a group failed");

            _consumerMetrics.JoinToGroupFailed(_groupId);

            throw;
        }
        catch (Exception exc) when (_activeSessionAwaiter.Task.IsFaulted)
        {
            _logger.LogError(exc, "Coordinator for group {Group} not found", _groupId);
            _consumerMetrics.JoinToGroupFailed(_groupId);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Unknown error");
            _consumerMetrics.JoinToGroupFailed(_groupId);
        }
        finally
        {
            _consumerMetrics.JoinToGroupTotal(_groupId);
        }
    }

    private static JoinGroupRequestMessage.JoinGroupRequestProtocolCollection BuildSupportProtocols(Subscription subscription)
    {
        var protocolCollection = new JoinGroupRequestMessage.JoinGroupRequestProtocolCollection();

        var metadata = subscription.SerializeAsMetadata();

        foreach (var assignor in subscription.SupportPartitionAssignors)
        {
            var supportProtocol = new JoinGroupRequestMessage.JoinGroupRequestProtocolMessage
            {
                Metadata = metadata,
                Name = assignor.Name,
            };
            protocolCollection.Add(supportProtocol);
        }

        return protocolCollection;
    }

    private async Task<IReadOnlyCollection<TopicPartition>> FollowerAsync(CancellationToken token)
    {
        var request = new SyncGroupRequestMessage
        {
            MemberId = MemberId,
            GroupId = _groupId,
            GenerationId = GenerationId,
            ProtocolType = _PROTOCOL_TYPE,
            ProtocolName = _protocolName,
            Assignments = new List<SyncGroupRequestMessage.SyncGroupRequestAssignmentMessage>
            {
                new()
                {
                    MemberId = MemberId,
                }
            }
        };

        var calculateAssignment = await SyncGroupAsync(request, token);
        var topicPartitions = new List<TopicPartition>(calculateAssignment.AssignedPartitions.Count);

        foreach (var g in calculateAssignment.AssignedPartitions)
        {
            topicPartitions.AddRange(g.Partitions.Select(p => new TopicPartition(g.Topic, p)));
        }

        return topicPartitions;
    }

    private async Task<IReadOnlyCollection<TopicPartition>> LeaderElectedAsync(
        List<JoinGroupResponseMessage.JoinGroupResponseMemberMessage> resultMembers,
        CancellationToken token)
    {

        // Срабатывает, когда данные консьюмер был выбран лидером группы

        _logger.LogTrace("Данный консьюмер назначен лидером, выполняем ребалансировку");

        // Раз мы были назначены лидером - проводим балансировку группы
        var assignResult = await Balance(resultMembers, token);

        var request = new SyncGroupRequestMessage
        {
            MemberId = MemberId,
            GroupId = _groupId,
            GenerationId = GenerationId,
            ProtocolType = _PROTOCOL_TYPE,
            ProtocolName = _protocolName,
        };

        foreach (var result in assignResult)
        {
            var assignment = new ConsumerProtocolAssignment();

            foreach (var partitionsByTopic in result.Value.GroupBy(x => x.Topic))
            {
                assignment.AssignedPartitions.Add(new ConsumerProtocolAssignment.TopicPartitionMessage
                {
                    Topic = partitionsByTopic.Key,
                    Partitions = partitionsByTopic.Select(x => x.Partition.Value).ToList()
                });

            }

            using var ms = new MemoryStream();
            var writer = new BufferWriter(ms, 0);
            writer.WriteShort((short)ApiVersion.Version3);
            assignment.Write(writer, ApiVersion.Version3);

            request.Assignments.Add(new SyncGroupRequestMessage.SyncGroupRequestAssignmentMessage
            {
                MemberId = result.Key,
                Assignment = writer.WrittenSpan.ToArray()

            });
        }
        var calculateAssignment = await SyncGroupAsync(request, token);
        var topicPartitions = new List<TopicPartition>(calculateAssignment.AssignedPartitions.Count);

        foreach (var g in calculateAssignment.AssignedPartitions)
        {
            var topicMetadata = _kafkaCluster.GetTopicMetadata(g.Topic);
            topicPartitions.AddRange(g.Partitions.Select(p => new TopicPartition(topicMetadata.Name, p, topicMetadata.TopicId)));
        }

        return topicPartitions;
    }

    private async Task<IDictionary<string, List<TopicPartition>>> Balance(List<JoinGroupResponseMessage.JoinGroupResponseMemberMessage> resultMembers,
        CancellationToken token)
    {
        if (_selectedPartitionAssignor is null)
        {
            _logger.LogCritical("Не выбран текущий алгоритм балансировки");

            throw new KafkaException("Ошибка балансировки лидером");
        }

        var subscriptions = new Dictionary<string, Subscription>();

        var topics = new HashSet<string>();

        foreach (var member in resultMembers)
        {
            var subscription = Subscription.Deserialize(member.Metadata);
            subscription.GenerationId = GenerationId;
            subscriptions.Add(member.MemberId, subscription);

            foreach (var topic in subscription.Topics)
            {
                topics.Add(topic);
            }
        }

        var topicPartitions = await _kafkaCluster.GetTopicPartitionsAsync(topics, token);

        //Т.к. мы лидер группы, то тут проводим привязку партиций для всех членов группы
        var assignResult = _selectedPartitionAssignor.Assign(topicPartitions, subscriptions);

        return assignResult;
    }

    /// <summary>
    /// Пытается найти координатор для указанной группы
    /// </summary>
    /// <param name="token"></param>
    /// <exception cref="ProtocolKafkaException"></exception>
    internal async Task TryFindCoordinatorForGroupAsync(CancellationToken token)
    {
        //Т.к. у нас есть сохраненный коннектор с координатором, то мы уже ранее его находили и он не изменился
        if (_coordinatorConnector != KafkaConnector.Null)
        {
            ChangeState(CoordinatorState.WaitJoinToGroup);

            return;
        }
        ChangeState(CoordinatorState.SearchCoordinator);

        var currentApiVersion = _kafkaCluster
            .GetClusterMetadata()
            .GetMaxCurrentApiVersion(ApiKeys.FindCoordinator);

        var groups = new[]
        {
            _groupId
        };

        var findCoordinatorRequestMessage = FindCoordinatorRequestMessage.Build(currentApiVersion, groups);

        try
        {
            var findCoordinatorResponseMessage =
                await _kafkaCluster.SendAsync<FindCoordinatorResponseMessage, FindCoordinatorRequestMessage>(findCoordinatorRequestMessage, token);
            var coordinator = findCoordinatorResponseMessage.GetCoordinator();

            if (coordinator.Code == ErrorCodes.None)
            {
                _coordinatorConnector = _kafkaCluster.ProvideDedicateConnector(coordinator.NodeId);
                await _coordinatorConnector.OpenAsync(token);
                ChangeState(CoordinatorState.WaitJoinToGroup);
            }
            else if (coordinator.Code == ErrorCodes.GroupAuthorizationFailed)
            {
                throw new KafkaException($"Authorized fail for group {_groupId}");
            }
            else
            {
                throw new ProtocolKafkaException(coordinator.Code);
            }

        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Не удалось получить данные по координатору");

            throw;
        }
    }

    private async Task HeartbeatLoop()
    {
        do
        {
            if (_tokenSource.Token.IsCancellationRequested)
            {
                _logger.LogTrace("Цикл отправки heartbeat отменен");

                return;
            }

            try
            {
                //Если нет активной сессии, то ждем ее появления
                await _activeSessionAwaiter.Task;
                await HeartbeatAsync(_tokenSource.Token);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Неожиданная ошибка");
            }

        } while (await _timer.WaitForNextTickAsync(_tokenSource.Token));
    }

    private async Task HeartbeatAsync(CancellationToken token)
    {
        _logger.LogTrace("Sending heartbeat");

        _heartbeatRequest ??= new HeartbeatRequestMessage
        {
            GenerationId = GenerationId,
            GroupId = _groupId,
            MemberId = MemberId,
            GroupInstanceId = _groupInstanceId
        };
        var result = await _coordinatorConnector.SendAsync<HeartbeatResponseMessage, HeartbeatRequestMessage>(_heartbeatRequest, false, token);

        switch (result.Code)
        {
            case ErrorCodes.None:
                _logger.LogTrace("The response to the heartbeat was successful");

                return;
            case ErrorCodes.RebalanceInProgress:
                {
                    //todo 
                    return;
                }
            case ErrorCodes.ReassignmentInProgress:
                {
                    //todo
                    return;
                }
            case ErrorCodes.UnknownMemberId:
                {
                    //todo
                    return;
                }
            default:
                throw new ProtocolKafkaException(result.Code, "An unsupported error code came in response to a heartbeat");
        }
    }

    /// <summary>
    /// Покидаем группу
    /// </summary>
    private async Task LeaveGroupAsync(CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(MemberId))
        {
            return;
        }

        var request = new LeaveGroupRequestMessage
        {
            MemberId = MemberId,
            GroupId = _groupId
        };

        var result = await _coordinatorConnector.SendAsync<LeaveGroupResponseMessage, LeaveGroupRequestMessage>(request, false, token);

        if (result.Code.IsSuccessCode())
        {
            return;
        }
        //обработать код ошибки, если нужно
    }

    private bool ChangeState(CoordinatorState state)
    {
        if (_state == state)
        {
            return false;
        }

        _logger.CoordinatorChangeStateTrace(_state, state);
        _state = state;

        return true;
    }

    internal enum CoordinatorState
    {
        Initialization = 0,
        SearchCoordinator = 1,
        WaitJoinToGroup = 2,
        JoinedToGroup = 3,
    }
}