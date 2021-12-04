namespace Microlibs.Kafka;

public interface IHeader
{
    /// <summary>
    ///     The header key.
    /// </summary>
    string Key { get; }

    /// <summary>
    ///     The serialized header value data.
    /// </summary>
    byte[] GetValueBytes();
}