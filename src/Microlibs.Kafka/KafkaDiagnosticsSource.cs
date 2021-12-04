using System.Diagnostics;

namespace Microlibs.Kafka;

internal static class KafkaDiagnosticsSource
{
    private static readonly ActivitySource _activitySource = new("Microlibs.Kafka");

    internal static Activity? ProduceMessage<TKey, TValue>(TopicPartition topicPartition, Message<TKey, TValue> message, bool isFireAndForget)
    {
        var activity = _activitySource
            .CreateActivity(nameof(ProduceMessage), ActivityKind.Producer)
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

        activity?.Start();

        return activity;
    }
}