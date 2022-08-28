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

public sealed class SyncGroupRequestMessage: RequestMessage, IEquatable<SyncGroupRequestMessage>
{
    /// <summary>
    /// The unique group identifier.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// The generation of the group.
    /// </summary>
    public int GenerationId { get; set; } = 0;

    /// <summary>
    /// The member ID assigned by the group.
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string? GroupInstanceId { get; set; } = null;

    /// <summary>
    /// The group protocol type.
    /// </summary>
    public string? ProtocolType { get; set; } = null;

    /// <summary>
    /// The group protocol name.
    /// </summary>
    public string? ProtocolName { get; set; } = null;

    /// <summary>
    /// Each assignment.
    /// </summary>
    public List<SyncGroupRequestAssignmentMessage> Assignments { get; set; } = new ();

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

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        {
            var stringBytes = Encoding.UTF8.GetBytes(GroupId);
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        writer.WriteInt(GenerationId);
        {
            var stringBytes = Encoding.UTF8.GetBytes(MemberId);
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersions.Version3)
        {
            if (GroupInstanceId is null)
            {
                if (version >= ApiVersions.Version4)
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
                if (version >= ApiVersions.Version4)
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
        if (version >= ApiVersions.Version5)
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
        if (version >= ApiVersions.Version5)
        {
            if (ProtocolName is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(ProtocolName);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        if (version >= ApiVersions.Version4)
        {
            writer.WriteVarUInt(Assignments.Count + 1);
            foreach (var element in Assignments)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Assignments.Count);
            foreach (var element in Assignments)
            {
                element.Write(writer, version);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version4)
        {
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }
        else
        {
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is SyncGroupRequestMessage other && Equals(other);
    }

    public bool Equals(SyncGroupRequestMessage? other)
    {
        return true;
    }

    public sealed class SyncGroupRequestAssignmentMessage: Message, IEquatable<SyncGroupRequestAssignmentMessage>
    {
        /// <summary>
        /// The ID of the member to assign.
        /// </summary>
        public string MemberId { get; set; } = string.Empty;

        /// <summary>
        /// The member assignment.
        /// </summary>
        public byte[] Assignment { get; set; } = Array.Empty<byte>();

        public SyncGroupRequestAssignmentMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version5;
        }

        public SyncGroupRequestAssignmentMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version5;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(MemberId);
                if (version >= ApiVersions.Version4)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(Assignment.Length + 1);
            }
            else
            {
                writer.WriteInt(Assignment.Length);
            }
            writer.WriteBytes(Assignment);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(numTaggedFields);
                rawWriter.WriteRawTags(writer, int.MaxValue);
            }
            else
            {
                if (numTaggedFields > 0)
                {
                    throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
                }
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is SyncGroupRequestAssignmentMessage other && Equals(other);
        }

        public bool Equals(SyncGroupRequestAssignmentMessage? other)
        {
            return true;
        }
    }
}
