// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 * 
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Net;
using System.Text;

using NKafka.Connection;

namespace NKafka;

internal static class Utils
{
    internal const string LOGGER_PREFIX = "[Kafka] ";

    internal static EndPoint BuildEndPoint(string bootstrapServer)
    {
        var (host, port) = GetHostAndPort(bootstrapServer);

        return BuildEndPoint(host, port);
    }

    internal static EndPoint BuildEndPoint(string host, int port)
    {
        if (string.IsNullOrWhiteSpace(host) || port == -1)
        {
            return new NoEndPoint();
        }

        if (IPAddress.TryParse(host, out var ipAddress))
        {
            return new IPEndPoint(ipAddress, port);
        }

        return new DnsEndPoint(host, port);
    }

    internal static (string Host, int Port) GetHostAndPort(string server)
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

    public static string MkString<TKey, TValue>(
        Dictionary<TKey, TValue> map,
        string begin,
        string end,
        string keyValueSeparator,
        string elementSeparator)
        where TKey : notnull
    {
        var sb = new StringBuilder();
        sb.Append(begin);

        var prefix = string.Empty;

        foreach (var (key, value) in map)
        {
            sb.Append(prefix).Append(key).Append(keyValueSeparator).Append(value);
            prefix = elementSeparator;
        }

        sb.Append(end);

        return sb.ToString();
    }

    public static Dictionary<string, string> ParseMap(string extensions, string keyValueSeparator, string elementSeparator)
    {
        var map = new Dictionary<string, string>(0);

        if (!string.IsNullOrWhiteSpace(extensions))
        {
            var attributeValues = extensions.Split(elementSeparator);

            foreach (var value in attributeValues)
            {
                var array = value.Split(keyValueSeparator, 2);
                map.Add(array[0], array[1]);
            }
        }

        return map;
    }
}