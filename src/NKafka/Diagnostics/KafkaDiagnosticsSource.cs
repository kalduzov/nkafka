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

using System.Diagnostics;
using System.Net;
using System.Reflection;

using FastEnumUtility;

using NKafka.Protocol;

namespace NKafka.Diagnostics;

internal static class KafkaDiagnosticsSource
{
    private static readonly ActivitySource _activitySource;

    private static readonly ActivitySource _internalActivitySource;

    static KafkaDiagnosticsSource()
    {
        var assembly = typeof(KafkaDiagnosticsSource).Assembly;
        var version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "0.0.0";
        _activitySource = new ActivitySource("NKafka", version);
        _internalActivitySource = new ActivitySource("NKafka.Internal", version);
    }

    internal static Activity? ProduceMessage<TKey, TValue>(TopicPartition topicPartition, Message<TKey, TValue> message, bool isFireAndForget)
    {
        var activity = _activitySource
            .StartActivity(nameof(ProduceMessage))
            ?.AddTag("Partition", topicPartition.Partition.Value.ToString())
            .AddTag("Topic", topicPartition.Topic)
            .AddTag("FireAndForget", isFireAndForget.ToString())
            .AddTag("KeyType", typeof(TKey))
            .AddTag("ValueType", typeof(TValue)).SetStatus(ActivityStatusCode.Ok);

        if (typeof(TKey).IsValueType)
        {
            activity?.AddTag("Key", message.Key?.ToString());
        }
        else
        {
            if (message.Key is not null)
            {
                activity?.AddTag("Key", message.Key.ToString());
            }
        }

        return activity;
    }

    public static Activity? InternalSendMessage(
        ApiKeys key,
        ApiVersion version,
        int requestId,
        int brokerId,
        EndPoint endPoint)
    {
        var activity = _internalActivitySource
            .StartActivity(nameof(InternalSendMessage))
            ?.AddTag("Key", key.FastToString())
            .AddTag("Version", version.FastToString())
            .AddTag("RequestId", requestId.ToString())
            .AddTag("BrokerId", brokerId.ToString())
            .AddTag("Address", endPoint)
            .SetStatus(ActivityStatusCode.Ok);

        return activity;
    }

    public static Activity? UpdateMetadata()
    {
        var activity = _activitySource
            .StartActivity(nameof(UpdateMetadata), ActivityKind.Client)?.SetStatus(ActivityStatusCode.Ok);

        return activity;
    }
}