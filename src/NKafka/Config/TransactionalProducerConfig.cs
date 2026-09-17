// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Exceptions;
using CEM = NKafka.Resources.ConfigExceptionMessages;

namespace NKafka.Config;

/// <summary>
/// Configuration for a transactional producer.
/// </summary>
/// <remarks>
/// This type currently reuses the producer configuration model while the public configuration
/// split is migrated. Its defaults enable the Kafka settings required by transactions.
/// </remarks>
public record TransactionalProducerConfig: ProducerConfig
{
    /// <summary>
    /// Gets or sets the unique identifier for this transactional producer.
    /// </summary>
    public string TransactionalId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction timeout in milliseconds.
    /// </summary>
    public int TransactionTimeoutMs { get; set; } = 60000;

    /// <summary>
    /// Initializes a configuration with transaction-safe defaults.
    /// </summary>
    public TransactionalProducerConfig()
    {
        EnableIdempotence = true;
        Acks = Acks.All;
    }

    /// <summary>
    /// Validates settings required for transactional production.
    /// </summary>
    /// <exception cref="KafkaConfigException">A required setting is missing or incompatible with transactions.</exception>
    internal override void Validate()
    {
        base.Validate();

        if (string.IsNullOrEmpty(TransactionalId))
        {
            throw new KafkaConfigException(nameof(TransactionalId), TransactionalId, CEM.TransactionalProducerConfig_TransactionalIdRequired);
        }

        if (TransactionTimeoutMs <= 0)
        {
            throw new KafkaConfigException(nameof(TransactionTimeoutMs), TransactionTimeoutMs, CEM.TransactionalProducerConfig_TransactionTimeoutInvalid);
        }

        if (!EnableIdempotence)
        {
            throw new KafkaConfigException(nameof(EnableIdempotence), EnableIdempotence, CEM.TransactionalProducerConfig_IdempotenceRequired);
        }

        if (Acks != Acks.All)
        {
            throw new KafkaConfigException(nameof(Acks), Acks, CEM.TransactionalProducerConfig_AcksRequired);
        }
    }
}
