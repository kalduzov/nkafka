// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Protocol;

namespace NKafka.Clients.Producer;

/// <summary>
/// Describes a Kafka or local error associated with a delivery result.
/// </summary>
public sealed record ProducerError
{
    /// <summary>Gets the Kafka error code or <see cref="ErrorCodes.ClientError"/>.</summary>
    public ErrorCodes ErrorCode { get; }

    /// <summary>Gets the local error cause.</summary>
    public ProducerLocalError LocalError { get; }

    /// <summary>Creates an error with a valid Kafka or local error classification.</summary>
    public ProducerError(ErrorCodes errorCode, ProducerLocalError localError)
    {
        if (errorCode == ErrorCodes.None)
        {
            throw new ArgumentException(nameof(errorCode));
        }

        if (errorCode == ErrorCodes.ClientError && localError == ProducerLocalError.None)
        {
            throw new ArgumentException(nameof(localError));
        }

        if (errorCode != ErrorCodes.ClientError && localError != ProducerLocalError.None)
        {
            throw new ArgumentException(nameof(localError));
        }

        ErrorCode = errorCode;
        LocalError = localError;
    }
}
