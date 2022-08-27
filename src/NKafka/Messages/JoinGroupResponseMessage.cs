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

public sealed class JoinGroupResponseMessage: ResponseMessage
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// The generation ID of the group.
    /// </summary>
    public int GenerationId { get; set; } = -1;

    /// <summary>
    /// The group protocol name.
    /// </summary>
    public string? ProtocolType { get; set; } = null;

    /// <summary>
    /// The group protocol selected by the coordinator.
    /// </summary>
    public string ProtocolName { get; set; } = "";

    /// <summary>
    /// The leader of the group.
    /// </summary>
    public string Leader { get; set; } = "";

    /// <summary>
    /// True if the leader must skip running the assignment.
    /// </summary>
    public bool SkipAssignment { get; set; } = false;

    /// <summary>
    /// The member ID assigned by the group coordinator.
    /// </summary>
    public string MemberId { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    public List<JoinGroupResponseMemberMessage> Members { get; set; } = new ();

    public JoinGroupResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    public JoinGroupResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed class JoinGroupResponseMemberMessage: Message
    {
        /// <summary>
        /// The group member ID.
        /// </summary>
        public string MemberId { get; set; } = "";

        /// <summary>
        /// The unique identifier of the consumer instance provided by end user.
        /// </summary>
        public string? GroupInstanceId { get; set; } = null;

        /// <summary>
        /// The group member metadata.
        /// </summary>
        public byte[] Metadata { get; set; } = Array.Empty<byte>();

        public JoinGroupResponseMemberMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public JoinGroupResponseMemberMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}
