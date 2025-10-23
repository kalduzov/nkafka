// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2024 Aleksey Kalduzov. All rights reserved
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

using Microsoft.Extensions.Logging;

using NKafka.Clients.Consumer;
using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka.Clients.Producer;

internal sealed partial class Producer
{
    /// <summary>
    /// Needs to be called before any other methods when the <see cref="ProducerConfig.TransactionalId"/> is set in the configuration.
    /// </summary>
    /// <param name="token"></param>
    public async Task InitTransactionsAsync(CancellationToken token)
    {
        ThrowIfNotTransactional();
        ThrowIfProducerClosed();

        var sw = Stopwatch.StartNew();

        try
        {
            await _transactionManager.InitAsync(token);
            _messagesSender.Wakeup();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Failed to initialize producer");

            throw;
        }
        finally
        {
            _producerMetrics.TransactionInit(sw.ElapsedMilliseconds);
        }

    }

    /// <summary>
    /// Should be called before the start of each new transaction.
    /// </summary>
    public void BeginTransaction()
    {
        ThrowIfNotTransactional();
        ThrowIfProducerClosed();

        var sw = Stopwatch.StartNew();

        try
        {
            _transactionManager.Begin();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "failed to initialize transaction");

            throw;
        }
        finally
        {
            _producerMetrics.BeginTxn(sw.ElapsedMilliseconds);
        }

    }

    /// <summary>
    /// Commits the ongoing transaction
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken token)
    {
        ThrowIfNotTransactional();
        ThrowIfProducerClosed();

        var sw = Stopwatch.StartNew();

        try
        {
            await _transactionManager.Commit(token);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Failed to send offsets to transaction");

            throw;
        }
        finally
        {
            _producerMetrics.CommitTxn(sw.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Aborts the ongoing transaction
    /// </summary>
    public async Task AbortTransactionAsync(CancellationToken token)
    {
        ThrowIfNotTransactional();
        ThrowIfProducerClosed();
        var sw = Stopwatch.StartNew();

        try
        {
            await _transactionManager.Abort(token);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Failed to send offsets to transaction");

            throw;
        }
        finally
        {
            _producerMetrics.AbortTxn(sw.ElapsedMilliseconds);
        }
    }

    /// <inheritdoc />
    public async Task SendOffsetsToTransactionAsync(IReadOnlyCollection<TopicPartitionOffset> offsets,
        ConsumerGroupMetadata groupMetadata,
        CancellationToken token)
    {
        ThrowIfInvalidGroupMetadata(groupMetadata);
        ThrowIfNotTransactional();
        ThrowIfProducerClosed();

        if (offsets.Count == 0)
        {
            return;
        }

        var sw = Stopwatch.StartNew();

        try
        {
            await _transactionManager.SendOffsetsToTransaction(offsets, groupMetadata, token);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Failed to send offsets to transaction");

            throw;
        }
        finally
        {
            _producerMetrics.SendOffsets(sw.ElapsedMilliseconds);
        }

    }

    private void ThrowIfInvalidGroupMetadata(ConsumerGroupMetadata groupMetadata)
    {
    }

    private static void ThrowIfNotTransactional()
    {
        throw new ProducerException("This producer is not in a transactional state.");
    }
}