using Microlibs.Kafka.Exceptions;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka.Config;

/**
 * Thrown if the user supplies an invalid configuration
 */
public class ConfigException : ProtocolKafkaException
{
    public ConfigException(string message)
        : base(StatusCodes.None, message)
    {
    }

    public ConfigException(string name, object value, string? message = null)
        : base(StatusCodes.None, "Invalid value " + value + " for configuration " + name + (message == null ? "" : ": " + message))
    {
    }
}