// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 * 
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Concurrent;

using NKafka.Collections;
using NKafka.Config;
using NKafka.Protocol.Records;

namespace NKafka.Clients.Producer.Internals;

internal sealed class RecordAccumulator
{
    private readonly ProducerConfig _config;
    private readonly int _deliveryTimeoutMs;
    private readonly ConcurrentDictionary<TopicPartition, Deque<ProducerBatch>> _batches;

    public RecordAccumulator(
        ProducerConfig config,
        int deliveryTimeoutMs)
    {
        _config = config;
        _deliveryTimeoutMs = deliveryTimeoutMs;
        _batches = new ConcurrentDictionary<TopicPartition, Deque<ProducerBatch>>();
    }

    public Task<RecordAppendResult> AppendAsync(
        TopicPartition topicPartition,
        Timestamp timestamp,
        byte[] key,
        byte[] value,
        Headers headers,
        CancellationToken token)
    {
        RecordAppendResult result;

        if (headers == null)
        {
            headers = Headers.Empty;
        }

        var deque = _batches.GetOrAdd(topicPartition, _ => new Deque<ProducerBatch>());

        lock (deque)
        {
            var lastBatch = deque.PeekFront();

            if (lastBatch is not null)
            {
                var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
                lastBatch.Add(timestamp, key, value, headers, tcs);
                result = new RecordAppendResult(tcs, false, false);

                return Task.FromResult(result);
            }
        }

        result = new RecordAppendResult(new TaskCompletionSource<int>(TaskCreationOptions.None), false, false);

        return Task.FromResult(result);
    }
}