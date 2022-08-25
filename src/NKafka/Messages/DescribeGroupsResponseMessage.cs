﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class DescribeGroupsResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each described group.
    /// </summary>
    public List<DescribedGroup> Groups { get; set; } = new();

    public DescribeGroupsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    public DescribeGroupsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class DescribedGroup: Message
    {
        /// <summary>
        /// The describe error, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The group ID string.
        /// </summary>
        public string GroupId { get; set; } = null!;

        /// <summary>
        /// The group state string, or the empty string.
        /// </summary>
        public string GroupState { get; set; } = null!;

        /// <summary>
        /// The group protocol type, or the empty string.
        /// </summary>
        public string ProtocolType { get; set; } = null!;

        /// <summary>
        /// The group protocol data, or the empty string.
        /// </summary>
        public string ProtocolData { get; set; } = null!;

        /// <summary>
        /// The group members.
        /// </summary>
        public List<DescribedGroupMember> Members { get; set; } = new();

        /// <summary>
        /// 32-bit bitfield to represent authorized operations for this group.
        /// </summary>
        public int AuthorizedOperations { get; set; } = -2147483648;

        public DescribedGroup()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version5;
        }

        public DescribedGroup(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version5;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class DescribedGroupMember: Message
    {
        /// <summary>
        /// The member ID assigned by the group coordinator.
        /// </summary>
        public string MemberId { get; set; } = null!;

        /// <summary>
        /// The unique identifier of the consumer instance provided by end user.
        /// </summary>
        public string? GroupInstanceId { get; set; } = "null";

        /// <summary>
        /// The client ID used in the member's latest join group request.
        /// </summary>
        public string ClientId { get; set; } = null!;

        /// <summary>
        /// The client host.
        /// </summary>
        public string ClientHost { get; set; } = null!;

        /// <summary>
        /// The metadata corresponding to the current group protocol in use.
        /// </summary>
        public byte[] MemberMetadata { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// The current assignment provided by the group leader.
        /// </summary>
        public byte[] MemberAssignment { get; set; } = Array.Empty<byte>();

        public DescribedGroupMember()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version5;
        }

        public DescribedGroupMember(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version5;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}