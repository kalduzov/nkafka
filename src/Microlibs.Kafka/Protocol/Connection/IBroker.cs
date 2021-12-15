using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;

namespace Microlibs.Kafka.Protocol.Connection;

/// <summary>
///     Интерфейсы брокера
/// </summary>
public interface IBroker : IDisposable, IAsyncDisposable
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
    /// 
    /// </summary>
    string? Rack { get; }

    /// <summary>
    ///     Список партиций топиков
    /// </summary>
    IReadOnlyCollection<TopicPartition> TopicPartitions { get; }

    Task OpenAsync(CommonConfig commonConfig, CancellationToken token);

    /// <summary>
    /// Отправка сообщений брокеру по типу fire and forget
    /// </summary>
    void Send<TRequestMessage>(TRequestMessage message)
        where TRequestMessage : KafkaContent;

    /// <summary>
    ///     Отправка сообщения брокеру и ожидание получения результата этого сообщения
    /// </summary>
    Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
        where TResponseMessage : KafkaResponseMessage, new()
        where TRequestMessage : KafkaContent;
}