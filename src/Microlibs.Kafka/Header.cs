using System;

namespace Microlibs.Kafka;

/// <summary>
///     Represents a kafka message header.
/// </summary>
/// <remarks>
///     Message headers are supported by v0.11 brokers and above.
/// </remarks>
public class Header : IHeader
{
    private readonly byte[] val;

    /// <summary>
    ///     Create a new Header instance.
    /// </summary>
    /// <param name="key">
    ///     The header key.
    /// </param>
    /// <param name="value">
    ///     The header value (may be null).
    /// </param>
    public Header(string key, byte[] value)
    {
        if (key == null)
        {
            throw new ArgumentNullException("Kafka message header key cannot be null.");
        }

        Key = key;
        val = value;
    }

    /// <summary>
    ///     The header key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Get the serialized header value data.
    /// </summary>
    public byte[] GetValueBytes()
    {
        return val;
    }
}