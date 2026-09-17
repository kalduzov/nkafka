// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka;

/// <summary>
/// Convenience methods for creating transactional producers.
/// </summary>
/// <remarks>
/// These methods validate and prepare configuration, then delegate creation and initialization to the cluster.
/// The caller owns the returned producer and must dispose it asynchronously.
/// </remarks>
public static class TransactionalProducerExtensions
{
    extension(IKafkaCluster cluster)
    {
        /// <summary>
        /// Creates a transactional producer with the specified identifier.
        /// </summary>
        public Task<ITransactionalProducer> CreateTransactionalProducerAsync(string transactionalId,
            CancellationToken cancellationToken)
        {
            ValidateTransactionalId(transactionalId);

            return cluster.CreateTransactionalProducerAsync(
                new TransactionalProducerConfig { TransactionalId = transactionalId },
                cancellationToken);
        }

        /// <summary>
        /// Creates a transactional producer using the specified identifier and configuration.
        /// </summary>
        public Task<ITransactionalProducer> CreateTransactionalProducerAsync(string transactionalId,
            TransactionalProducerConfig config,
            CancellationToken cancellationToken)
        {
            ValidateTransactionalId(transactionalId);
            ArgumentNullException.ThrowIfNull(config);

            if (!string.IsNullOrEmpty(config.TransactionalId)
                && !string.Equals(config.TransactionalId, transactionalId, StringComparison.Ordinal))
            {
                throw new KafkaConfigException(
                    nameof(config.TransactionalId),
                    config.TransactionalId,
                    "The configured TransactionalId differs from the explicit identifier.");
            }

            var effectiveConfig = config with { TransactionalId = transactionalId };
            return cluster.CreateTransactionalProducerAsync(effectiveConfig, cancellationToken);
        }

        /// <summary>
        /// Creates a transactional producer using a configuration callback.
        /// </summary>
        public Task<ITransactionalProducer> CreateTransactionalProducerAsync(string transactionalId,
            Action<TransactionalProducerConfig> configure,
            CancellationToken cancellationToken)
        {
            ValidateTransactionalId(transactionalId);
            ArgumentNullException.ThrowIfNull(configure);

            var config = new TransactionalProducerConfig();
            configure(config);

            return cluster.CreateTransactionalProducerAsync(transactionalId, config, cancellationToken);
        }
    }

    private static void ValidateTransactionalId(string transactionalId)
    {
        if (string.IsNullOrEmpty(transactionalId))
        {
            throw new KafkaConfigException(
                nameof(TransactionalProducerConfig.TransactionalId),
                transactionalId,
                "TransactionalId must not be empty.");
        }
    }
}
