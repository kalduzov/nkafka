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

public sealed class SyncGroupRequestMessage: RequestMessage
{
    /// <summary>
    /// The unique group identifier.
    /// </summary>
    public string GroupIdMessage { get; set; } = "";

    /// <summary>
    /// The generation of the group.
    /// </summary>
    public int GenerationIdMessage { get; set; } = 0;

    /// <summary>
    /// The member ID assigned by the group.
    /// </summary>
    public string MemberIdMessage { get; set; } = "";

    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string GroupInstanceIdMessage { get; set; } = null;

    /// <summary>
    /// The group protocol type.
    /// </summary>
    public string? ProtocolTypeMessage { get; set; } = null;

    /// <summary>
    /// The group protocol name.
    /// </summary>
    public string? ProtocolNameMessage { get; set; } = null;

    /// <summary>
    /// Each assignment.
    /// </summary>
    public List<SyncGroupRequestAssignmentMessage> AssignmentsMessage { get; set; } = new ();

    public SyncGroupRequestMessage()
    {
        ApiKey = ApiKeys.SyncGroup;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    public SyncGroupRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.SyncGroup;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class SyncGroupRequestAssignmentMessage: Message
    {
        /// <summary>
        /// The ID of the member to assign.
        /// </summary>
        public string MemberIdMessage { get; set; } = "";

        /// <summary>
        /// The member assignment.
        /// </summary>
        public byte[] AssignmentMessage { get; set; } = Array.Empty<byte>();

        public SyncGroupRequestAssignmentMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public SyncGroupRequestAssignmentMessage(BufferReader reader, ApiVersions version)
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
