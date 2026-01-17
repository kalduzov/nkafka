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
using NKafka.Protocol;

namespace NKafka.Clients.Producer.Internals;

/// <inheritdoc />
internal class TransactionManager: ITransactionManager
{
    private readonly IKafkaCluster _kafkaCluster;

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

    private readonly string _transactionalId;
    private readonly int _transactionTimeoutMs;
    private readonly bool _enableIdempotence;
    private readonly ILogger<TransactionManager> _logger;

    private volatile State _currentState = State.Uninitialized;
    private volatile Exception? _lastError;
    private readonly HashSet<TopicPartition> _newPartitionsInTransaction = [];
    private readonly HashSet<TopicPartition> _partitionsInTransaction = [];
    private readonly HashSet<TopicPartition> _pendingPartitionsInTransaction = [];
    private bool _clientSideEpochBumpRequired;
    private ProducerIdAndEpoch _producerIdAndEpoch;
    private bool _isEpochBump;

    public TransactionManager(ProducerConfig config, ILoggerFactory loggerFactory, IKafkaCluster kafkaCluster)
    {
        _kafkaCluster = kafkaCluster;
        _transactionalId = config.TransactionalId;
        _transactionTimeoutMs = config.TransactionTimeoutMs;
        _enableIdempotence = config.EnableIdempotence;
        _logger = loggerFactory.CreateLogger<TransactionManager>();
        _producerIdAndEpoch = ProducerIdAndEpoch.None;

    }

    public bool IsTransactional => !string.IsNullOrEmpty(_transactionalId);

    public async Task InitializeTransactionsAsync(ProducerIdAndEpoch producerIdAndEpoch, bool keepPreparedTxn, CancellationToken cancellationToken)
    {
        var isEpochBump = producerIdAndEpoch != ProducerIdAndEpoch.None;

        if (!isEpochBump)
        {
            TransitionTo(State.Initializing);

            _logger.LogInformation("Invoking InitProducerId for the first time in order to acquire a producer ID");

            if (keepPreparedTxn)
            {
                _logger.LogInformation("Invoking InitProducerId with keepPreparedTxn set to true for 2PC transactions");
            }
        }
        else
        {
            _logger.LogInformation("Invoking InitProducerId with current producer ID and epoch {ProducerIdAndEpoch} in order to bump the epoch",
                producerIdAndEpoch);
        }

        var request = new InitProducerIdRequestMessage
        {
            TransactionalId = _transactionalId,
            TransactionTimeoutMs = _transactionTimeoutMs,
            ProducerId = producerIdAndEpoch.ProducerId,
            ProducerEpoch = producerIdAndEpoch.Epoch
        };

        var result = await _kafkaCluster.SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(request, cancellationToken);

        switch (result.Code)
        {

        }
    }

    public void Begin()
    {
        TransitionTo(State.InTransaction);
    }

    public Task SendOffsetsToTransactionAsync(IReadOnlyCollection<TopicPartitionOffset> offsets,
        ConsumerGroupMetadata groupMetadata,
        CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken token)
    {
        TransitionTo(State.CommittingTransaction);

        return Task.CompletedTask;
    }

    public Task AbortAsync(CancellationToken token)
    {
        return Task.CompletedTask;
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

    public async Task BumpIdempotentEpochAndResetIdIfNeededAsync(CancellationToken cancellationToken)
    {
        if (!IsTransactional)
        {
            return;
        }

        if (_clientSideEpochBumpRequired)
        {
            await BumpIdempotentProducerEpochAsync();
        }

        if (_currentState != State.Initializing && !_producerIdAndEpoch.IsValid)
        {
            TransitionTo(State.Initializing);
            var request = new InitProducerIdRequestMessage
            {
                TransactionalId = null!,
                TransactionTimeoutMs = int.MaxValue
            };
            var response = await _kafkaCluster.SendAsync<InitProducerIdRequestMessage, InitProducerIdResponseMessage>(request, cancellationToken);

            switch (response.Code)
            {
                case ErrorCodes.None:
                    {
                        _producerIdAndEpoch = new ProducerIdAndEpoch(response.ProducerId, response.ProducerEpoch);
                        TransitionTo(State.Ready);

                        if (_isEpochBump)
                        {
                            ResetSequenceNumbers();
                        }

                        break;
                    }
                case ErrorCodes.NotCoordinator:
                case ErrorCodes.CoordinatorNotAvailable:
                    {
                        await LookupCoordinatorAsync(FindCoordinatorRequestMessage.CoordinatorType.Transaction);

                        break;
                    }
            }
        }
    }

    private async Task LookupCoordinatorAsync(FindCoordinatorRequestMessage.CoordinatorType transaction)
    {
    }

    private void ResetSequenceNumbers()
    {
    }

    private async Task BumpIdempotentProducerEpochAsync()
    {
    }

    public bool HasProducerId { get; set; } = false;

    private static bool IsTransitionValid(State source, State target)
    {
        return target switch
        {

            State.Uninitialized => source is State.Ready or State.AbortableError,
            State.Initializing => source is State.Uninitialized or State.AbortingTransaction,
            State.Ready => source is State.Initializing or State.CommittingTransaction or State.AbortingTransaction,
            State.InTransaction => source is State.Ready,
            State.CommittingTransaction => source is State.InTransaction,
            State.AbortingTransaction => source is State.InTransaction or State.AbortableError,
            State.AbortableError => source is State.InTransaction or State.CommittingTransaction or State.AbortableError or State.Initializing,
            _ => true
        };
    }

    private void TransitionTo(State target, Exception? exception = null)
    {
        if (!IsTransitionValid(_currentState, target))
        {
            throw new TransactionException($"Invalid state transition from {_currentState} to {target}");
        }

        if (target == State.FatalError || _currentState == State.AbortableError)
        {
            _lastError = exception ?? throw new TransactionException("Cannot transition to " + target + " with a null exception");

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