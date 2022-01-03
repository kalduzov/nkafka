﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Net;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Microlibs.Kafka.Tests")]

namespace Microlibs.Kafka;

internal static class Utils
{
    internal const string _LOGGER_PREFIX = "[Kafka] ";
    
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