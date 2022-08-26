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
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class DescribeGroupsResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each described group.
    /// </summary>
    public List<DescribedGroupMessage> GroupsMessage { get; set; } = new ();

    public DescribeGroupsResponseMessage()
    {
        ApiKey = ApiKeys.DescribeGroups;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    public DescribeGroupsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeGroups;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class DescribedGroupMessage: Message
    {
        /// <summary>
        /// The describe error, or 0 if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The group ID string.
        /// </summary>
        public string GroupIdMessage { get; set; } = "";

        /// <summary>
        /// The group state string, or the empty string.
        /// </summary>
        public string GroupStateMessage { get; set; } = "";

        /// <summary>
        /// The group protocol type, or the empty string.
        /// </summary>
        public string ProtocolTypeMessage { get; set; } = "";

        /// <summary>
        /// The group protocol data, or the empty string.
        /// </summary>
        public string ProtocolDataMessage { get; set; } = "";

        /// <summary>
        /// The group members.
        /// </summary>
        public List<DescribedGroupMemberMessage> MembersMessage { get; set; } = new ();

        /// <summary>
        /// 32-bit bitfield to represent authorized operations for this group.
        /// </summary>
        public int AuthorizedOperationsMessage { get; set; } = -2147483648;

        public DescribedGroupMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DescribedGroupMessage(BufferReader reader, ApiVersions version)
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

    public sealed class DescribedGroupMemberMessage: Message
    {
        /// <summary>
        /// The member ID assigned by the group coordinator.
        /// </summary>
        public string MemberIdMessage { get; set; } = "";

        /// <summary>
        /// The unique identifier of the consumer instance provided by end user.
        /// </summary>
        public string? GroupInstanceIdMessage { get; set; } = null;

        /// <summary>
        /// The client ID used in the member's latest join group request.
        /// </summary>
        public string ClientIdMessage { get; set; } = "";

        /// <summary>
        /// The client host.
        /// </summary>
        public string ClientHostMessage { get; set; } = "";

        /// <summary>
        /// The metadata corresponding to the current group protocol in use.
        /// </summary>
        public byte[] MemberMetadataMessage { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// The current assignment provided by the group leader.
        /// </summary>
        public byte[] MemberAssignmentMessage { get; set; } = Array.Empty<byte>();

        public DescribedGroupMemberMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DescribedGroupMemberMessage(BufferReader reader, ApiVersions version)
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
