using System;
using System.Collections.Concurrent;

namespace Microlibs.Kafka.Protocol.Connection
{
    internal sealed class KafkaConnectionPoolManager : IDisposable
    {
        private readonly ConcurrentDictionary<ClusterConnectionKey, ClusterConnectionPool> _pools;
        private object _syncObj => _pools;

        public KafkaConnectionPoolManager()
        {
            _pools = new ConcurrentDictionary<ClusterConnectionKey, ClusterConnectionPool>();
        }

        public void Dispose()
        {
        }

        internal readonly struct ClusterConnectionKey : IEquatable<ClusterConnectionKey>
        {
            public readonly string ClusterId;

            public ClusterConnectionKey(string clusterId)
            {
                ClusterId = clusterId;
            }

            public override int GetHashCode()
                => HashCode.Combine(ClusterId);

            public override bool Equals(object? obj)
                => obj is ClusterConnectionKey cck && Equals(cck);

            public bool Equals(ClusterConnectionKey other)
                => ClusterId.Equals(other.ClusterId, StringComparison.Ordinal);
        }
    }
}