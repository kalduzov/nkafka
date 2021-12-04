using System;
using Microlibs.Kafka.Clients.Producer;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Protocol.Connection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microlibs.Kafka
{
    /// <summary>
    /// 
    /// </summary>

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class KafkaCluster : IKafkaCluster
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly KafkaConnectionPoolManager _connectionPoolManager;

        private KafkaCluster(CommonConfig config, ILoggerFactory? loggerFactory)
        {
            Config = config;
            _loggerFactory = loggerFactory ?? new NullLoggerFactory();
            _connectionPoolManager = new KafkaConnectionPoolManager();
        }

        /// <summary>
        /// Инициализирует подсистему для нового кафка кластера. 
        /// </summary>
        /// <param name="config">Конфигурация</param>
        /// <param name="loggerFactory">Экземпляр фабрики логирования</param>
        public static IKafkaCluster Create(CommonConfig config, ILoggerFactory? loggerFactory = null)
        {
            return new KafkaCluster(config, loggerFactory ?? NullLoggerFactory.Instance);
        }

        public CommonConfig Config { get; }

        /// <summary>
        /// Создает новый продюсер для работы с кластером или возвращает уже существующий
        /// </summary>
        /// <param name="producerConfig">Конфигурация продюсера</param>
        /// <remarks>Библиотека </remarks>
        public IProducer<TKey, TValue> BuildProducer<TKey, TValue>(string name, ProducerConfig? producerConfig = null)
        {
            var config = producerConfig ?? ProducerConfig.BaseFrom(Config);

            return new Producer<TKey, TValue>(name, config, null, null, null, null, DateTime.Today.TimeOfDay, _loggerFactory);
        }

        public void Dispose()
        {
            _connectionPoolManager.Dispose();
        }
    }
}