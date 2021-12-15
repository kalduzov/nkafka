using Microlibs.Kafka.Clients.Producer;
using Microlibs.Kafka.Config;

namespace Microlibs.Kafka;

public static class ProducerExtensions
{
    private const string _DEFAULT_PRODUCER_NAME_FORMAT = "__DefaultProducer<{0},{1}>";

    /// <summary>
    ///     Создает нового продюсера с указанными типами ключа и сообщения
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <remarks>Если такой продюсер уже существует и не уничтожен - то возвращается он</remarks>
    public static IProducer<TKey, TValue> BuildProducer<TKey, TValue>(this IKafkaCluster kafkaCluster)
    {
        return BuildProducer<TKey, TValue>(kafkaCluster, null!);
    }

    /// <summary>
    ///     Создает нового продюсера с указанными типами ключа и сообщения
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <param name="kafkaCluster"></param>
    /// <param name="producerConfig">Producer specific configuration</param>
    /// <remarks>Если такой продюсер уже существует и не уничтожен - то возвращается он</remarks>
    public static IProducer<TKey, TValue> BuildProducer<TKey, TValue>(this IKafkaCluster kafkaCluster, ProducerConfig producerConfig)
    {
        var name = string.Format(_DEFAULT_PRODUCER_NAME_FORMAT, typeof(TKey).Name, typeof(TValue).Name);

        return kafkaCluster.BuildProducer<TKey, TValue>(name, producerConfig);
    }
}