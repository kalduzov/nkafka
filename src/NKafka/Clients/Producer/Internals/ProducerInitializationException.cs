// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Exceptions;

namespace NKafka.Clients.Producer.Internals;

internal sealed class ProducerInitializationException(ProducerError error)
    : ProducerException("The idempotent producer could not be initialized.")
{
    internal ProducerError Error { get; } = error;
}
