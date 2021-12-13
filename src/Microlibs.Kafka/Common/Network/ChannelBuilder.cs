using System;
using System.Buffers;

namespace Microlibs.Kafka.Common.Network;

public interface IChannelBuilder : IDisposable
{
    /// <summary>
    ///     Returns a Channel with TransportLayer and Authenticator configured.
    /// </summary>
    /// <param name="id">channel id</param>
    /// <param name="key">SelectionKey</param>
    /// <param name="maxReceiveSize">max size of a single receive buffer to allocate</param>
    /// <param name="memoryPool">memory pool from which to allocate buffers, or null for none</param>
    /// <param name="metadataRegistry"></param>
    /// <returns></returns>
    KafkaChannel BuildChannel(
        string id,
        SelectionKey key,
        int maxReceiveSize,
        MemoryPool<byte> memoryPool,
        ChannelMetadataRegistry metadataRegistry);
}