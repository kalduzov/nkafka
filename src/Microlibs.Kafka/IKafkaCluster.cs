using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Clients.Producer;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Protocol.Connection;

namespace Microlibs.Kafka;

/// <summary>
///     Описывает API работы с kakfa кластером
/// </summary>
public interface IKafkaCluster : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     Конфигурация кластера
    /// </summary>
    ClusterConfig Config { get; }

    /// <summary>
    ///     Список топиков в кластере
    /// </summary>
    /// <remarks>Возвращаются все топики, которые были запрошены для кластера</remarks>
    IReadOnlyCollection<string> Topics { get; }

    /// <summary>
    ///     Закрыт кластер или открыт
    /// </summary>
    /// <remarks>Из закрытого или уничтоженного кластера невозможно получить никакую информацию</remarks>
    bool Closed { get; }

    /// <summary>
    ///     Информация о брокере, который является контроллером
    /// </summary>
    IBroker Controller { get; }

    /// <summary>
    ///     Список всех брокеров кластера
    /// </summary>
    IReadOnlyCollection<IBroker> Brokers { get; }

    /// <summary>
    ///     Создает продюсера связанного с текущим кластером
    /// </summary>
    /// <param name="name">Имя продюсера</param>
    /// <param name="producerConfig">Конфигурация продюсера</param>
    /// <remarks>
    ///     Если продюсер с таким именем уже сушествует, то вернется его экземпляр. Если нет, то будет создан новый
    ///     продюсер
    /// </remarks>
    IProducer<TKey, TValue> BuildProducer<TKey, TValue>(string name, ProducerConfig? producerConfig = null);

    /// <summary>
    ///     Создает консьюмера связанного с текущим кластером
    /// </summary>
    /// <param name="consumeGroupName">Название группы консьюмера</param>
    /// <param name="consumerConfig">Конфигурация консьюмера</param>
    /// <remarks>Метод всегда возвращает новый консьюмер, привязанный к конкретной группе</remarks>
    IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(string consumeGroupName, ConsumerConfig? consumerConfig = null);

    /// <summary>
    ///     Обновляет метаданные по указанным топикам
    /// </summary>
    /// <param name="topics">Список топиков, по которым необходимо получить информацию из брокеров</param>
    Task RefreshMetadataAsync(CancellationToken token, params string[] topics);
}