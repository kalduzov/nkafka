using Microlibs.Kafka.Exceptions;

namespace Microlibs.Kafka.Config;

/// <summary>
///     Конфигурация кластера
/// </summary>
public record ClusterConfig : CommonConfig
{
    /// <summary>
    ///     Таймаут обновления данных по брокерам в ms
    /// </summary>
    /// <remarks>Default - 1000ms</remarks>
    public int BrokerUpdateTimeoutMs { get; set; } = 1000;

    /// <summary>
    ///     Сколько времени ждать инициализации кластера в ms
    /// </summary>
    /// <remarks>Default - 15 sec</remarks>
    public int ClusterInitTimeoutMs { get; set; } = 15000;

    /// <summary>
    ///     Валидирует настройки и кидает исключение, если настройки не верные или отсутствуют обязательные
    /// </summary>
    /// <exception cref="KafkaConfigException">Throw if configuration is not valid</exception>
    internal override void Validate()
    {
        base.Validate();
    }
}