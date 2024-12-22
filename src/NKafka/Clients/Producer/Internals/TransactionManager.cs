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

using NKafka.Clients.Consumer;
using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Messages;

namespace NKafka.Clients.Producer.Internals;

internal class TransactionManager(ProducerConfig config, ILoggerFactory loggerFactory): ITransactionManager
{
    private enum State
    {
        Uninitialized,
        Initializing,
        Ready,
        InTransaction,
        CommittingTransaction,
        AbortingTransaction,
        AbortableError,
        FatalError
    }

    private readonly string _transactionalId = config.TransactionalId;
    private readonly int _transactionTimeoutMs = config.TransactionTimeoutMs;
    private readonly bool _enableIdempotence = config.EnableIdempotence;
    private readonly ILogger<TransactionManager> _logger = loggerFactory.CreateLogger<TransactionManager>();

    private volatile State _currentState = State.Uninitialized;
    private volatile Exception? _lastError;
    private readonly HashSet<TopicPartition> _newPartitionsInTransaction = [];
    private readonly HashSet<TopicPartition> _partitionsInTransaction = [];
    private readonly HashSet<TopicPartition> _pendingPartitionsInTransaction = [];

    public bool IsTransactional => !string.IsNullOrEmpty(_transactionalId);

    public Task Init(CancellationToken token)
    {
        var request = new InitProducerIdRequestMessage
        {
            TransactionalId = _transactionalId,
            TransactionTimeoutMs = _transactionTimeoutMs,
            ProducerId = -1,
            ProducerEpoch = -1
        };

        return null;
    }

    public void Begin()
    {
        TransitionTo(State.InTransaction);
    }

    public Task SendOffsetsToTransaction(IReadOnlyCollection<TopicPartitionOffset> offsets,
        ConsumerGroupMetadata groupMetadata,
        CancellationToken token)
    {
        return null;
    }

    public Task Commit(CancellationToken token)
    {
        TransitionTo(State.CommittingTransaction);

        return null;
    }

    public Task Abort(CancellationToken token)
    {
        return null;
    }

    public void TryAddPartition(TopicPartition topicPartition)
    {
        if (IsTransactional)
        {
            if (!HasProducerId)
            {
                throw new ProduceException("No producer id specified");
            }

            if (_currentState == State.InTransaction)
            {
                throw new ProduceException("No producer id specified");
            }

            if (_partitionsInTransaction.Contains(topicPartition) || _newPartitionsInTransaction.Contains(topicPartition))
            {
                return;
            }
            _newPartitionsInTransaction.Add(topicPartition);
        }
    }

    public bool HasProducerId { get; set; } = false;

    private static bool IsTransitionValid(State from, State to)
    {
        return to switch
        {

            State.Uninitialized => from is State.Ready or State.AbortableError,
            State.Initializing => from is State.Uninitialized or State.AbortingTransaction,
            State.Ready => from is State.Initializing or State.CommittingTransaction or State.AbortingTransaction,
            State.InTransaction => from is State.Ready,
            State.CommittingTransaction => from is State.InTransaction,
            State.AbortingTransaction => from is State.InTransaction or State.AbortableError,
            State.AbortableError => from is State.InTransaction or State.CommittingTransaction or State.AbortableError or State.Initializing,
            _ => true
        };
    }

    private void TransitionTo(State target, Exception? exception = null)
    {
        if (!IsTransitionValid(_currentState, target))
        {
            throw new Exception($"Invalid state transition from {_currentState} to {target}");
        }

        if (target == State.FatalError || _currentState == State.AbortableError)
        {
            _lastError = exception ?? throw new ProducerException("Cannot transition to " + target + " with a null exception");

        }
        else
        {
            _lastError = null;
        }

        if (_lastError is not null)
        {
            _logger.LogDebug(_lastError, "Transition from state {Current} to error state {Target}", _currentState, target);
        }
        else
        {
            _logger.LogDebug("Transition from state {Current} to {Target}", _currentState, target);
        }
        _currentState = target;
    }
}