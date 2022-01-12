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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public partial class JoinGroupRequestMessage: RequestMessage
{
    /// <summary>
    /// The group identifier.
    /// </summary>
    public string GroupId { get; set; }

    /// <summary>
    /// The coordinator considers the consumer dead if it receives no heartbeat after this timeout in milliseconds.
    /// </summary>
    public int SessionTimeoutMs { get; set; }

    /// <summary>
    /// The maximum time in milliseconds that the coordinator will wait for each member to rejoin when rebalancing the group.
    /// </summary>
    public int? RebalanceTimeoutMs { get; set; } = -1;

    /// <summary>
    /// The member id assigned by the group coordinator.
    /// </summary>
    public string MemberId { get; set; }

    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string? GroupInstanceId { get; set; } = null;

    /// <summary>
    /// The unique name the for class of protocols implemented by the group we want to join.
    /// </summary>
    public string ProtocolType { get; set; }

    /// <summary>
    /// The list of protocols that the member supports.
    /// </summary>
    public IReadOnlyCollection<JoinGroupRequestProtocolMessage> Protocols { get; set; }

    public JoinGroupRequestMessage()
    {
        ApiKey = ApiKeys.JoinGroup;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class JoinGroupRequestProtocolMessage: Message
    {
        /// <summary>
        /// The protocol name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The protocol metadata.
        /// </summary>
        public byte[] Metadata { get; set; }

        public JoinGroupRequestProtocolMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}