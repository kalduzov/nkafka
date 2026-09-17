// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace NKafka.Clients.Producer;

/// <summary>
/// Identifies a local producer error that has no Kafka protocol error code.
/// </summary>
public enum ProducerLocalError
{
    /// <summary>No local error.</summary>
    None = 0,

    /// <summary>The message was cancelled before acceptance.</summary>
    Cancelled,

    /// <summary>The message was not accepted before the enqueue timeout.</summary>
    EnqueueTimedOut,

    /// <summary>The accepted message exceeded its delivery timeout.</summary>
    DeliveryTimedOut,

    /// <summary>The transport failed after the message could have been transmitted.</summary>
    TransportFailure,

    /// <summary>The producer was closing.</summary>
    ProducerClosing,

    /// <summary>The record exceeds the configured request size.</summary>
    RecordTooLarge
}
