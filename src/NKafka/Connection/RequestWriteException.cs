namespace NKafka.Connection;

/// <summary>
/// Indicates that writing a Kafka request started but did not complete successfully.
/// </summary>
internal sealed class RequestWriteException(Exception innerException)
    : Exception("Writing the Kafka request to the connection failed.", innerException);
