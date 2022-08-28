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

public sealed class LeaveGroupRequestMessage: IRequestMessage, IEquatable<LeaveGroupRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version5;

    public ApiKeys ApiKey => ApiKeys.LeaveGroup;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The ID of the group to leave.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// The member ID to remove from the group.
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// List of leaving member identities.
    /// </summary>
    public List<MemberIdentityMessage> Members { get; set; } = new ();

    public LeaveGroupRequestMessage()
    {
    }

    public LeaveGroupRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            int length;
            if (version >= ApiVersions.Version4)
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
        if (version <= ApiVersions.Version2)
        {
            int length;
            length = reader.ReadShort();
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
        else
        {
            MemberId = string.Empty;
        }
        if (version >= ApiVersions.Version3)
        {
            if (version >= ApiVersions.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Members was serialized as null");
                }
                else
                {
                    var newCollection = new List<MemberIdentityMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new MemberIdentityMessage(reader, version));
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
                    var newCollection = new List<MemberIdentityMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new MemberIdentityMessage(reader, version));
                    }
                    Members = newCollection;
                }
            }
        }
        else
        {
            Members = new ();
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version4)
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
        if (version <= ApiVersions.Version2)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(MemberId);
                writer.WriteShort((short)stringBytes.Length);
                writer.WriteBytes(stringBytes);
            }
        }
        else
        {
            if (!MemberId.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default MemberId at version {version}");
            }
        }
        if (version >= ApiVersions.Version3)
        {
            if (version >= ApiVersions.Version4)
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
        else
        {
            if (Members.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Members at version {version}");
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
        return ReferenceEquals(this, obj) || obj is LeaveGroupRequestMessage other && Equals(other);
    }

    public bool Equals(LeaveGroupRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, GroupId, MemberId, Members);
        return hashCode;
    }

    public sealed class MemberIdentityMessage: IMessage, IEquatable<MemberIdentityMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version5;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The member ID to remove from the group.
        /// </summary>
        public string MemberId { get; set; } = string.Empty;

        /// <summary>
        /// The group instance ID to remove from the group.
        /// </summary>
        public string? GroupInstanceId { get; set; } = null;

        /// <summary>
        /// The reason why the member left the group.
        /// </summary>
        public string? Reason { get; set; } = null;

        public MemberIdentityMessage()
        {
        }

        public MemberIdentityMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version5)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of MemberIdentityMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version4)
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
                if (version >= ApiVersions.Version4)
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
            if (version >= ApiVersions.Version5)
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    Reason = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Reason had invalid length {length}");
                }
                else
                {
                    Reason = reader.ReadString(length);
                }
            }
            else
            {
                Reason = null;
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version4)
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
            if (version < ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of MemberIdentityMessage");
            }
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
            if (version >= ApiVersions.Version5)
            {
                if (Reason is null)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Reason);
                    writer.WriteVarUInt(stringBytes.Length + 1);
                    writer.WriteBytes(stringBytes);
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
            return ReferenceEquals(this, obj) || obj is MemberIdentityMessage other && Equals(other);
        }

        public bool Equals(MemberIdentityMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, MemberId, GroupInstanceId, Reason);
            return hashCode;
        }
    }
}
