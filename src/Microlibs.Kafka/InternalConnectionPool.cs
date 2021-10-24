using System;
using System.Collections.Generic;
using System.Linq;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Protocol;
using Microsoft.Extensions.ObjectPool;

namespace Microlibs.Kafka
{
    internal class InternalConnectionPool
    {
        private readonly CommonConfig _commonConfig;
        private readonly List<IConnection> _connections;

        public InternalConnectionPool(CommonConfig commonConfig)
        {
            _commonConfig = commonConfig;
            _connections = new List<IConnection>(commonConfig.BootstrapServers.Count);

            foreach (var bootstrapServer in commonConfig.BootstrapServers)
            {
                var servers = bootstrapServer.Split(",", StringSplitOptions.RemoveEmptyEntries);

                foreach (var server in servers)
                {
                    var hostsAndPorts = server.Split(":", StringSplitOptions.RemoveEmptyEntries);
                    var host = hostsAndPorts[0];
                    var port = int.Parse(hostsAndPorts[1]);

                    var connection = new InternalConnection(host, port, commonConfig.ClientId);
                    _connections.Add(connection);
                }
            }
        }

        public IReadOnlyCollection<IConnection> GetConnections()
        {
            return _connections;
        }
    }
}