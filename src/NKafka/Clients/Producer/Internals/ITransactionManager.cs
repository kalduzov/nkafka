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

using NKafka.Clients.Consumer;

namespace NKafka.Clients.Producer.Internals;

internal interface ITransactionManager
{
    internal bool IsTransactional { get; }

    internal Task InitializeTransactionsAsync(ProducerIdAndEpoch producerIdAndEpoch, bool keepPreparedTxn, CancellationToken cancellationToken);

    internal Task InitializeTransactionsAsync(bool keepPreparedTxn, CancellationToken cancellationToken)
    {
        return InitializeTransactionsAsync(ProducerIdAndEpoch.None, keepPreparedTxn, cancellationToken);
    }

    internal Task InitializeTransactionsAsync(ProducerIdAndEpoch producerIdAndEpoch, CancellationToken cancellationToken)
    {
        return InitializeTransactionsAsync(producerIdAndEpoch, false, cancellationToken);
    }

    internal void Begin();

    internal Task SendOffsetsToTransactionAsync(IReadOnlyCollection<TopicPartitionOffset> offsets,
        ConsumerGroupMetadata groupMetadata,
        CancellationToken token);

    internal Task CommitAsync(CancellationToken token);

    internal Task AbortAsync(CancellationToken token);

    internal void TryAddPartition(TopicPartition topicPartition);

    internal Task BumpIdempotentEpochAndResetIdIfNeededAsync(CancellationToken cancellationToken);
};