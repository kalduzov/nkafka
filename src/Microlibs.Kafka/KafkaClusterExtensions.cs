using System;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microlibs.Kafka;

public static class KafkaClusterExtensions
{
    /// <summary>
    ///     Инициализирует подсистему для нового кафка кластера.
    /// </summary>
    /// <param name="config">Конфигурация</param>
    /// <param name="loggerFactory">Экземпляр фабрики логирования, может быть null</param>
    /// <exception cref="ClusterKafkaException">Не удалось инициализировать кластер</exception>
    public static async Task<IKafkaCluster> CreateNewClusterAsync(this ClusterConfig config, ILoggerFactory? loggerFactory = null)
    {
        /*
        * Создание нового кластера состоит из нескольких шагов
        * 1. Валидируем конфигурацию. Нам важно работать с валидной конфигураций.
        * 2. Создаем новый структуру кластера c уже валидной конфигурации
        * 3. Когда структура создана и инициализирована - запускаем кластер
        */

        var loggerFactoryInstance = loggerFactory ?? NullLoggerFactory.Instance;
        var localLogger = loggerFactoryInstance.CreateLogger<KafkaCluster>();

        localLogger.LogDebug("Creating new cluster");

        config.Validate();

        var cluster = new KafkaCluster(config, loggerFactoryInstance);

        var cts = new CancellationTokenSource(config.ClusterInitTimeoutMs);

        try
        {
            await cluster.InitializationAsync(cts.Token);

            return cluster;
        }
        catch (OperationCanceledException exc)
        {
            if (!cts.IsCancellationRequested)
            {
                throw;
            }

            await cluster.DisposeAsync(); //Не удалось инициализировать кластер - очишаем ресурсы, которые, возможно, уже были частично инициалированы
            throw new ClusterKafkaException($"Не удалось инциализировать кластер за выделенное время {config.ClusterInitTimeoutMs}ms", exc);
        }
        finally
        {
            cts.Dispose();
        }
    }
}