using System.Collections.Generic;
using System.Net;

namespace Microlibs.Kafka.Clients;

internal class Metadata
{
    private MetadataCache _cache;
    private bool _needFullUpdate;
    private int _updateVersion;

    public void Bootstrap(IReadOnlyCollection<IPEndPoint> addresses)
    {
        _needFullUpdate = true;
        _updateVersion += 1;
        _cache = MetadataCache.Bootstrap(addresses);
    }
}