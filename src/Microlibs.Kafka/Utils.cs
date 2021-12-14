using System;
using System.Net;
using System.Net.Sockets;

namespace Microlibs.Kafka;

internal static class Utils
{
    internal static EndPoint BuildBrokerEndPoint(string bootstrapServer)
    {
        var (host, port) = GetHostAndPort(bootstrapServer);

        return BuildEndPoint(host, port);
    }

    internal static EndPoint BuildEndPoint(string host, int port)
    {
        if (IPAddress.TryParse(host, out var ipAddress))
        {
            return new IPEndPoint(ipAddress, port);
        }

        return new DnsEndPoint(host, port);
    }

    private static (string, int) GetHostAndPort(string server)
    {
        var hostsAndPorts = server.Split(":", StringSplitOptions.RemoveEmptyEntries);

        var host = hostsAndPorts[0];
        ValidateHost(host);

        if (int.TryParse(hostsAndPorts[1], out var port))
        {
            ValidatePort(port);
        }
        else
        {
            throw new ArgumentException($"Port {port} is not integer");
        }

        return (host, port);
    }

    private static void ValidatePort(int port)
    {
        if (port <= 1024)
        {
            throw new ArgumentException($"Port {port} is incorrect");
        }
    }

    private static void ValidateHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentException($"Host {host} is incorrect");
        }
    }
}