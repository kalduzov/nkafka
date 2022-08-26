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
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//
// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class DescribeClusterResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top-level error code, or 0 if there was no error
    /// </summary>
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// The top-level error message, or null if there was no error.
    /// </summary>
    public string ErrorMessageMessage { get; set; } = null;

    /// <summary>
    /// The cluster ID that responding broker belongs to.
    /// </summary>
    public string ClusterIdMessage { get; set; } = "";

    /// <summary>
    /// The ID of the controller broker.
    /// </summary>
    public int ControllerIdMessage { get; set; } = -1;

    /// <summary>
    /// Each broker in the response.
    /// </summary>
    public List<DescribeClusterBrokerMessage> BrokersMessage { get; set; } = new ();

    /// <summary>
    /// 32-bit bitfield to represent authorized operations for this cluster.
    /// </summary>
    public int ClusterAuthorizedOperationsMessage { get; set; } = -2147483648;

    public DescribeClusterResponseMessage()
    {
        ApiKey = ApiKeys.DescribeCluster;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public DescribeClusterResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeCluster;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class DescribeClusterBrokerMessage: Message
    {
        /// <summary>
        /// The broker ID.
        /// </summary>
        public Dictionary<int,> BrokerIdMessage { get; set; } = 0;

        /// <summary>
        /// The broker hostname.
        /// </summary>
        public Dictionary<string,> HostMessage { get; set; } = "";

        /// <summary>
        /// The broker port.
        /// </summary>
        public Dictionary<int,> PortMessage { get; set; } = 0;

        /// <summary>
        /// The rack of the broker, or null if it has not been assigned to a rack.
        /// </summary>
        public Dictionary<string,> RackMessage { get; set; } = null;

        public DescribeClusterBrokerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DescribeClusterBrokerMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }
}
