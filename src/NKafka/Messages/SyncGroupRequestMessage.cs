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

// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using System.Text;

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;

namespace NKafka.Messages;

public sealed class SyncGroupRequestMessage: IRequestMessage, IEquatable<SyncGroupRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version5;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.SyncGroup;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

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
    public List<SyncGroupRequestAssignmentMessage> Assignments { get; set; } = new();

    public SyncGroupRequestMessage()
    {
    }

    public SyncGroupRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            int length;
            if (version >= ApiVersion.Version4)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field GroupId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field GroupId had invalid length {length}");
            }
            else
            {
                GroupId = reader.ReadString(length);
            }
        }
        GenerationId = reader.ReadInt();
        {
            int length;
            if (version >= ApiVersion.Version4)
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
        if (version >= ApiVersion.Version3)
        {
            int length;
            if (version >= ApiVersion.Version4)
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
        if (version >= ApiVersion.Version5)
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
        if (version >= ApiVersion.Version5)
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                ProtocolName = null;
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
        else
        {
            ProtocolName = null;
        }
        {
            if (version >= ApiVersion.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Assignments was serialized as null");
                }
                else
                {
                    var newCollection = new List<SyncGroupRequestAssignmentMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new SyncGroupRequestAssignmentMessage(reader, version));
                    }
                    Assignments = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Assignments was serialized as null");
                }
                else
                {
                    var newCollection = new List<SyncGroupRequestAssignmentMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new SyncGroupRequestAssignmentMessage(reader, version));
                    }
                    Assignments = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version4)
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

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        {
            var stringBytes = Encoding.UTF8.GetBytes(GroupId);
            if (version >= ApiVersion.Version4)
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
            if (version >= ApiVersion.Version4)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersion.Version3)
        {
            if (GroupInstanceId is null)
            {
                if (version >= ApiVersion.Version4)
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
                if (version >= ApiVersion.Version4)
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
        if (version >= ApiVersion.Version5)
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
        if (version >= ApiVersion.Version5)
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
        if (version >= ApiVersion.Version4)
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
        if (version >= ApiVersion.Version4)
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

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, GroupId, GenerationId, MemberId, GroupInstanceId, ProtocolType, ProtocolName, Assignments);
        return hashCode;
    }

    public override string ToString()
    {
        return "SyncGroupRequestMessage("
            + ", GenerationId=" + GenerationId
            + ")";
    }

    public sealed class SyncGroupRequestAssignmentMessage: IMessage, IEquatable<SyncGroupRequestAssignmentMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version5;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

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
        }

        public SyncGroupRequestAssignmentMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version5)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of SyncGroupRequestAssignmentMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version4)
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
                int length;
                if (version >= ApiVersion.Version4)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadInt();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Assignment was serialized as null");
                }
                else
                {
                    Assignment = reader.ReadBytes(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version4)
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(MemberId);
                if (version >= ApiVersion.Version4)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version4)
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
            if (version >= ApiVersion.Version4)
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

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, MemberId, Assignment);
            return hashCode;
        }

        public override string ToString()
        {
            return "SyncGroupRequestAssignmentMessage("
                + ")";
        }
    }
}