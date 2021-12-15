using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka.Clients.Producer
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public interface IProducer<TKey, TValue> : IProducer
    {
        /// <summary>
        /// </summary>
        Task FlushAsync(CancellationToken cancellationToken);

        void Flush(TimeSpan timeout);

        IReadOnlyCollection<PartitionInfo> PartitionsFor(string topic);

        void Close(TimeSpan timeout);

        Task CloseAsync(CancellationToken token);

        #region Produce

        /// <summary>
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topicName, Message<TKey, TValue> message, CancellationToken token = default)
        {
            return ProduceAsync(new TopicPartition(topicName, Partition.Any), message, token);
        }

        /// <summary>
        /// </summary>
        /// <param name="topicPartition"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<DeliveryResult<TKey, TValue>> ProduceAsync(
            TopicPartition topicPartition,
            Message<TKey, TValue> message,
            CancellationToken token = default);

        /// <summary>
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="messages"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<DeliveryResult<TKey, TValue>>> ProduceAsync(
            string topicName,
            IEnumerable<Message<TKey, TValue>> messages,
            CancellationToken token = default)
        {
            return ProduceAsync(new TopicPartition(topicName, Partition.Any), messages, token);
        }

        /// <summary>
        /// </summary>
        /// <param name="topicPartition"></param>
        /// <param name="messages"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<DeliveryResult<TKey, TValue>>> ProduceAsync(
            TopicPartition topicPartition,
            IEnumerable<Message<TKey, TValue>> messages,
            CancellationToken token = default);

        /// <summary>
        ///     Fire and forget produce message
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="message"></param>
        void Produce(string topicName, Message<TKey, TValue> message)
        {
            Produce(new TopicPartition(topicName, Partition.Any), message);
        }

        /// <summary>
        ///     Fire and forget produce message
        /// </summary>
        /// <param name="topicPartition"></param>
        /// <param name="message"></param>
        void Produce(TopicPartition topicPartition, Message<TKey, TValue> message);

        /// <summary>
        ///     Fire and forget produce message
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="messages"></param>
        void Produce(string topicName, IEnumerable<Message<TKey, TValue>> messages)
        {
            Produce(new TopicPartition(topicName, Partition.Any), messages);
        }

        /// <summary>
        ///     Fire and forget produce message
        /// </summary>
        /// <param name="topicPartition"></param>
        /// <param name="messages"></param>
        void Produce(TopicPartition topicPartition, IEnumerable<Message<TKey, TValue>> messages);

        /// <summary>
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="message"></param>
        /// <param name="deliveryCallback"></param>
        void Produce(string topicName, Message<TKey, TValue> message, Action<DeliveryResult<TKey, TValue>> deliveryCallback)
        {
            Produce(new TopicPartition(topicName, Partition.Any), message, deliveryCallback);
        }

        /// <summary>
        /// </summary>
        /// <param name="topicPartition"></param>
        /// <param name="message"></param>
        /// <param name="deliveryCallback"></param>
        void Produce(TopicPartition topicPartition, Message<TKey, TValue> message, Action<DeliveryResult<TKey, TValue>> deliveryCallback);

        #endregion Produce
    }

    public class RecordMetadata
    {
    }

    public class ConsumerGroupMetadata
    {
    }

    public class OffsetAndMetadata
    {
    }
}

/// <summary>
///     Маркерный интерфейс для продюсера
/// </summary>
public interface IProducer : IDisposable, IAsyncDisposable
{
}