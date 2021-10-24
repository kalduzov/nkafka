//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Net;

namespace NKafka;

/// <summary>
///  Information about a Kafka node
/// </summary>
public record Node
{
    /// <summary>
    /// No node information
    /// </summary>
    public static Node NoNode { get; } = new(-1, "", -1);

    /// <summary>
    /// The node id of this node
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The host name for this node
    /// </summary>
    public string Host { get; }

    /// <summary>
    /// The port for this node
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// The rack for this node
    /// </summary>
    public string? Rack { get; }

    /// <summary>
    /// The EndPoint struct for this node
    /// </summary>
    public EndPoint EndPoint { get; }

    /// <summary>
    ///  Information about a Kafka node
    /// </summary>
    /// <param name="id"> The node id of this node</param>
    /// <param name="host">The host name for this node</param>
    /// <param name="port">The port for this node</param>
    /// <param name="rack">The rack for this node</param>
    public Node(int id, string host, int port, string? rack = null)
    {
        Id = id;
        Host = host;
        Port = port;
        Rack = rack;
        EndPoint = Utils.BuildEndPoint(host, port);
    }
}