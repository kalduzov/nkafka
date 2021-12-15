using System;
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