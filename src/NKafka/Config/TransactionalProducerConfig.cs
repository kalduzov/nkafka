// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

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
    /// Initializes a configuration with transaction-safe defaults.
    /// </summary>
    public TransactionalProducerConfig()
    {
        EnableIdempotence = true;
        Acks = Acks.All;
    }
}