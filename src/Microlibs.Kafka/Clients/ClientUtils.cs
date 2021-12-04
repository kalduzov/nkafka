using System;
using Microlibs.Kafka.Common.Network;
using Microlibs.Kafka.Config;
using Microsoft.Extensions.Logging;

namespace Microlibs.Kafka.Clients;

internal static class ClientUtils
{
    public static IChannelBuilder CreateChannelBuilder(ProducerConfig config, TimeSpan time, ILoggerFactory loggerFactory)
    {
        return null;
    }
}