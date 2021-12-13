using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microlibs.Kafka.Protocol.Connection;

/// <summary>
///     Интерфейсы брокера
/// </summary>
public interface IBroker : IDisposable
{
    /// <summary>
    ///     Список топиков обслуживающихся на брокере
    /// </summary>
    IReadOnlyCollection<string> Topics { get; }

    /// <summary>
    ///     Флаг указывающий, что данный брокер является контроллером
    /// </summary>
    bool IsController { get; }

    /// <summary>
    ///     Идентикификатор брокера
    /// </summary>
    int Id { get; }

    /// <summary>
    ///     Точка подключения к брокеру
    /// </summary>
    public EndPoint EndPoint { get; set; }

    /// <summary>
    ///     Список партиций топиков
    /// </summary>
    IReadOnlyCollection<TopicPartition> TopicPartitions { get; }

    void Send(KafkaRequestMessage requestMessage);

    /// <summary>
    ///     Отправка сообщения в брокер и ожидание получения результата этого сообщения
    /// </summary>
    Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
        where TResponseMessage : KafkaResponseMessage
        where TRequestMessage : KafkaContent;
}