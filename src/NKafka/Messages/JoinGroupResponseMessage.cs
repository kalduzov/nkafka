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

using NKafka.Exceptions;
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
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version2)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        writer.WriteShort(ErrorCode);
        writer.WriteInt(GenerationId);
        if (version >= ApiVersions.Version7)
        {
            if (ProtocolType is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(ProtocolType);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        if (ProtocolName is null)
        {
            if (version >= ApiVersions.Version7)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                throw new NullReferenceException();            }
        }
        else
        {
            var stringBytes = Encoding.UTF8.GetBytes(ProtocolName);
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        {
            var stringBytes = Encoding.UTF8.GetBytes(Leader);
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersions.Version9)
        {
            writer.WriteBool(SkipAssignment);
        }
        else
        {
            if (SkipAssignment)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default SkipAssignment at version {version}");
            }
        }
        {
            var stringBytes = Encoding.UTF8.GetBytes(MemberId);
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersions.Version6)
        {
            writer.WriteVarUInt(Members.Count + 1);
            foreach (var element in Members)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Members.Count);
            foreach (var element in Members)
            {
                element.Write(writer, version);
            }
        }
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
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(MemberId);
                if (version >= ApiVersions.Version6)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version5)
            {
                if (GroupInstanceId is null)
                {
                    if (version >= ApiVersions.Version6)
                    {
                        writer.WriteVarUInt(0);
                    }
                    else
                    {
                        writer.WriteShort(-1);
                    }
                }
                else
                {
                    var stringBytes = Encoding.UTF8.GetBytes(GroupInstanceId);
                    if (version >= ApiVersions.Version6)
                    {
                        writer.WriteVarUInt(stringBytes.Length + 1);
                    }
                    else
                    {
                        writer.WriteShort((short)stringBytes.Length);
                    }
                    writer.WriteBytes(stringBytes);
                }
            }
            else
            {
                if (GroupInstanceId is not null)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default GroupInstanceId at version {version}");
                }
            }
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(Metadata.Length + 1);
            }
            else
            {
                writer.WriteInt(Metadata.Length);
            }
            writer.WriteBytes(Metadata);
        }
    }
}
