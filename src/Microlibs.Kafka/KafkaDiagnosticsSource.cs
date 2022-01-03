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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Diagnostics;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka;

internal static class KafkaDiagnosticsSource
{
    private static readonly ActivitySource _activitySource = new("Microlibs.Kafka");

    private static readonly ActivitySource _internalActivitySource = new("Microlibs.Kafka.Internal");

    internal static Activity? ProduceMessage<TKey, TValue>(TopicPartition topicPartition, Message<TKey, TValue> message, bool isFireAndForget)
    {
        var activity = _activitySource
            .StartActivity(nameof(ProduceMessage))
            ?.AddTag("Partition", topicPartition.Partition.Value.ToString())
            .AddTag("Topic", topicPartition.Topic)
            .AddTag("FireAndForget", isFireAndForget.ToString())
            .AddTag("KeyType", typeof(TKey))
            .AddTag("ValueType", typeof(TValue));

        if (typeof(TKey).IsValueType)
        {
            activity?.AddTag("Key", message.Key?.ToString());
        }
        else
        {
            if (message.Key is not null)
            {
                activity?.AddTag("Key", message.Key);
            }
        }

        return activity;
    }

    public static Activity? InternalSendMessage(ApiKeys key, ApiVersions version, int requestId, int brokerId)
    {
        var activity = _internalActivitySource
            .StartActivity(nameof(InternalSendMessage))
            ?.AddTag("Key", key)
            .AddTag("Version", version)
            .AddTag("RequestId", requestId)
            .AddTag("BrokerId", brokerId);

        return activity;
    }
}