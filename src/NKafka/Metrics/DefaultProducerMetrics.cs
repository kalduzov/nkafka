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
using System.Diagnostics.Metrics;

namespace NKafka.Metrics;

internal class DefaultProducerMetrics: IProducerMetrics
{
    private const string _PREFIX = "producer-";

    private Histogram<long>? _flushDuration;
    private Histogram<int>? _appendBytes;
    private readonly Meter _producerMetrics = new("NKafka.Metrics.Producer");

    internal DefaultProducerMetrics()
    {
        RegisterMetrics();
    }

    internal DefaultProducerMetrics(IMeterFactory meterFactory)
    {
        _producerMetrics = meterFactory.Create("NKafka.Metrics.Producer");

        RegisterMetrics();
    }

    private void RegisterMetrics()
    {
        _flushDuration = _producerMetrics.CreateHistogram<long>(
            name: $"{_PREFIX}flush-duration",
            unit: "ms",
            description: "Total time producer has spent in flush in milliseconds.");

        _appendBytes = _producerMetrics.CreateHistogram<int>(
            name: $"{_PREFIX}append-bytes",
            description: "");
    }

    /// <inheritdoc />
    public void Flush(long duration)
    {
        if (_flushDuration?.Enabled ?? false)
        {
            _flushDuration.Record(duration);
        }
    }

    /// <inheritdoc />
    public void AppendBytes(TopicPartition topicPartition, int appendBytes)
    {
        if (_appendBytes?.Enabled ?? false)
        {
            _appendBytes.Record(appendBytes,
                new TagList
                {
                    {
                        "topic", topicPartition.Topic
                    },
                    {
                        "partition", topicPartition.Partition
                    }
                });
        }
    }

    public void TransactionInit(long duration)
    {
    }

    public void BeginTxn(long duration)
    {
    }
}