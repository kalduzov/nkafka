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

public sealed class JoinGroupResponseMessage: IResponseMessage, IEquatable<JoinGroupResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version9;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

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
    public string ProtocolName { get; set; } = string.Empty;

    /// <summary>
    /// The leader of the group.
    /// </summary>
    public string Leader { get; set; } = string.Empty;

    /// <summary>
    /// True if the leader must skip running the assignment.
    /// </summary>
    public bool SkipAssignment { get; set; } = false;

    /// <summary>
    /// The member ID assigned by the group coordinator.
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public List<JoinGroupResponseMemberMessage> Members { get; set; } = new ();

    public JoinGroupResponseMessage()
    {
    }

    public JoinGroupResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        if (version >= ApiVersions.Version2)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        ErrorCode = reader.ReadShort();
        GenerationId = reader.ReadInt();
        if (version >= ApiVersions.Version7)
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                ProtocolType = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ProtocolType had invalid length {length}");
            }
            else
            {
                ProtocolType = reader.ReadString(length);
            }
        }
        else
        {
            ProtocolType = null;
        }
        {
            int length;
            if (version >= ApiVersions.Version6)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                if (version >= ApiVersions.Version7)
                {
                    ProtocolName = null;
                }
                else
                {
                    throw new Exception("non-nullable field ProtocolName was serialized as null");
                }
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ProtocolName had invalid length {length}");
            }
            else
            {
                ProtocolName = reader.ReadString(length);
            }
        }
        {
            int length;
            if (version >= ApiVersions.Version6)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field Leader was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field Leader had invalid length {length}");
            }
            else
            {
                Leader = reader.ReadString(length);
            }
        }
        if (version >= ApiVersions.Version9)
        {
            SkipAssignment = reader.ReadByte() != 0;
        }
        else
        {
            SkipAssignment = false;
        }
        {
            int length;
            if (version >= ApiVersions.Version6)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field MemberId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field MemberId had invalid length {length}");
            }
            else
            {
                MemberId = reader.ReadString(length);
            }
        }
        {
            if (version >= ApiVersions.Version6)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Members was serialized as null");
                }
                else
                {
                    var newCollection = new List<JoinGroupResponseMemberMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new JoinGroupResponseMemberMessage(reader, version));
                    }
                    Members = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Members was serialized as null");
                }
                else
                {
                    var newCollection = new List<JoinGroupResponseMemberMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new JoinGroupResponseMemberMessage(reader, version));
                    }
                    Members = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version6)
        {
            var numTaggedFields = reader.ReadVarUInt();
            for (var t = 0; t < numTaggedFields; t++)
            {
                var tag = reader.ReadVarUInt();
                var size = reader.ReadVarUInt();
                switch (tag)
                {
                    default:
                        UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                        break;
                }
            }
        }
    }

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version2)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        writer.WriteShort((short)ErrorCode);
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
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version6)
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
        return ReferenceEquals(this, obj) || obj is JoinGroupResponseMessage other && Equals(other);
    }

    public bool Equals(JoinGroupResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, GenerationId, ProtocolType, ProtocolName, Leader, SkipAssignment);
        hashCode = HashCode.Combine(hashCode, MemberId, Members);
        return hashCode;
    }

    public sealed class JoinGroupResponseMemberMessage: IMessage, IEquatable<JoinGroupResponseMemberMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version9;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The group member ID.
        /// </summary>
        public string MemberId { get; set; } = string.Empty;

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
        }

        public JoinGroupResponseMemberMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version9)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of JoinGroupResponseMemberMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field MemberId was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field MemberId had invalid length {length}");
                }
                else
                {
                    MemberId = reader.ReadString(length);
                }
            }
            if (version >= ApiVersions.Version5)
            {
                int length;
                if (version >= ApiVersions.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    GroupInstanceId = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field GroupInstanceId had invalid length {length}");
                }
                else
                {
                    GroupInstanceId = reader.ReadString(length);
                }
            }
            else
            {
                GroupInstanceId = null;
            }
            {
                int length;
                if (version >= ApiVersions.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadInt();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Metadata was serialized as null");
                }
                else
                {
                    Metadata = reader.ReadBytes(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version6)
            {
                var numTaggedFields = reader.ReadVarUInt();
                for (var t = 0; t < numTaggedFields; t++)
                {
                    var tag = reader.ReadVarUInt();
                    var size = reader.ReadVarUInt();
                    switch (tag)
                    {
                        default:
                            UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                            break;
                    }
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersions version)
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
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version6)
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
            return ReferenceEquals(this, obj) || obj is JoinGroupResponseMemberMessage other && Equals(other);
        }

        public bool Equals(JoinGroupResponseMemberMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, MemberId, GroupInstanceId, Metadata);
            return hashCode;
        }
    }
}
