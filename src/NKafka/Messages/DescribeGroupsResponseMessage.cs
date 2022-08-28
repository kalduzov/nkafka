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

public sealed class DescribeGroupsResponseMessage: IResponseMessage, IEquatable<DescribeGroupsResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version5;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// Each described group.
    /// </summary>
    public List<DescribedGroupMessage> Groups { get; set; } = new ();

    public DescribeGroupsResponseMessage()
    {
    }

    public DescribeGroupsResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        if (version >= ApiVersions.Version1)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        {
            if (version >= ApiVersions.Version5)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Groups was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribedGroupMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribedGroupMessage(reader, version));
                    }
                    Groups = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Groups was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribedGroupMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribedGroupMessage(reader, version));
                    }
                    Groups = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version5)
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
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersions.Version5)
        {
            writer.WriteVarUInt(Groups.Count + 1);
            foreach (var element in Groups)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Groups.Count);
            foreach (var element in Groups)
            {
                element.Write(writer, version);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version5)
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
        return ReferenceEquals(this, obj) || obj is DescribeGroupsResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeGroupsResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, Groups);
        return hashCode;
    }

    public sealed class DescribedGroupMessage: IMessage, IEquatable<DescribedGroupMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version5;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The describe error, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The group ID string.
        /// </summary>
        public string GroupId { get; set; } = string.Empty;

        /// <summary>
        /// The group state string, or the empty string.
        /// </summary>
        public string GroupState { get; set; } = string.Empty;

        /// <summary>
        /// The group protocol type, or the empty string.
        /// </summary>
        public string ProtocolType { get; set; } = string.Empty;

        /// <summary>
        /// The group protocol data, or the empty string.
        /// </summary>
        public string ProtocolData { get; set; } = string.Empty;

        /// <summary>
        /// The group members.
        /// </summary>
        public List<DescribedGroupMemberMessage> Members { get; set; } = new ();

        /// <summary>
        /// 32-bit bitfield to represent authorized operations for this group.
        /// </summary>
        public int AuthorizedOperations { get; set; } = -2147483648;

        public DescribedGroupMessage()
        {
        }

        public DescribedGroupMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version5)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribedGroupMessage");
            }
            ErrorCode = reader.ReadShort();
            {
                int length;
                if (version >= ApiVersions.Version5)
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
            {
                int length;
                if (version >= ApiVersions.Version5)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field GroupState was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field GroupState had invalid length {length}");
                }
                else
                {
                    GroupState = reader.ReadString(length);
                }
            }
            {
                int length;
                if (version >= ApiVersions.Version5)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field ProtocolType was serialized as null");
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
            {
                int length;
                if (version >= ApiVersions.Version5)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field ProtocolData was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ProtocolData had invalid length {length}");
                }
                else
                {
                    ProtocolData = reader.ReadString(length);
                }
            }
            {
                if (version >= ApiVersions.Version5)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Members was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DescribedGroupMemberMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DescribedGroupMemberMessage(reader, version));
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
                        var newCollection = new List<DescribedGroupMemberMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DescribedGroupMemberMessage(reader, version));
                        }
                        Members = newCollection;
                    }
                }
            }
            if (version >= ApiVersions.Version3)
            {
                AuthorizedOperations = reader.ReadInt();
            }
            else
            {
                AuthorizedOperations = -2147483648;
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version5)
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
            writer.WriteShort((short)ErrorCode);
            {
                var stringBytes = Encoding.UTF8.GetBytes(GroupId);
                if (version >= ApiVersions.Version5)
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
                var stringBytes = Encoding.UTF8.GetBytes(GroupState);
                if (version >= ApiVersions.Version5)
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
                var stringBytes = Encoding.UTF8.GetBytes(ProtocolType);
                if (version >= ApiVersions.Version5)
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
                var stringBytes = Encoding.UTF8.GetBytes(ProtocolData);
                if (version >= ApiVersions.Version5)
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
            if (version >= ApiVersions.Version3)
            {
                writer.WriteInt(AuthorizedOperations);
            }
            else
            {
                if (AuthorizedOperations != -2147483648)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default AuthorizedOperations at version {version}");
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version5)
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
            return ReferenceEquals(this, obj) || obj is DescribedGroupMessage other && Equals(other);
        }

        public bool Equals(DescribedGroupMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ErrorCode, GroupId, GroupState, ProtocolType, ProtocolData, Members, AuthorizedOperations);
            return hashCode;
        }
    }

    public sealed class DescribedGroupMemberMessage: IMessage, IEquatable<DescribedGroupMemberMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version5;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The member ID assigned by the group coordinator.
        /// </summary>
        public string MemberId { get; set; } = string.Empty;

        /// <summary>
        /// The unique identifier of the consumer instance provided by end user.
        /// </summary>
        public string? GroupInstanceId { get; set; } = null;

        /// <summary>
        /// The client ID used in the member's latest join group request.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// The client host.
        /// </summary>
        public string ClientHost { get; set; } = string.Empty;

        /// <summary>
        /// The metadata corresponding to the current group protocol in use.
        /// </summary>
        public byte[] MemberMetadata { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// The current assignment provided by the group leader.
        /// </summary>
        public byte[] MemberAssignment { get; set; } = Array.Empty<byte>();

        public DescribedGroupMemberMessage()
        {
        }

        public DescribedGroupMemberMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version5)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribedGroupMemberMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version5)
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
            if (version >= ApiVersions.Version4)
            {
                int length;
                if (version >= ApiVersions.Version5)
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
                if (version >= ApiVersions.Version5)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field ClientId was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ClientId had invalid length {length}");
                }
                else
                {
                    ClientId = reader.ReadString(length);
                }
            }
            {
                int length;
                if (version >= ApiVersions.Version5)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field ClientHost was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ClientHost had invalid length {length}");
                }
                else
                {
                    ClientHost = reader.ReadString(length);
                }
            }
            {
                int length;
                if (version >= ApiVersions.Version5)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadInt();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field MemberMetadata was serialized as null");
                }
                else
                {
                    MemberMetadata = reader.ReadBytes(length);
                }
            }
            {
                int length;
                if (version >= ApiVersions.Version5)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadInt();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field MemberAssignment was serialized as null");
                }
                else
                {
                    MemberAssignment = reader.ReadBytes(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version5)
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
                if (version >= ApiVersions.Version5)
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
                if (GroupInstanceId is null)
                {
                    if (version >= ApiVersions.Version5)
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
                    if (version >= ApiVersions.Version5)
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(ClientId);
                if (version >= ApiVersions.Version5)
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
                var stringBytes = Encoding.UTF8.GetBytes(ClientHost);
                if (version >= ApiVersions.Version5)
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
                writer.WriteVarUInt(MemberMetadata.Length + 1);
            }
            else
            {
                writer.WriteInt(MemberMetadata.Length);
            }
            writer.WriteBytes(MemberMetadata);
            if (version >= ApiVersions.Version5)
            {
                writer.WriteVarUInt(MemberAssignment.Length + 1);
            }
            else
            {
                writer.WriteInt(MemberAssignment.Length);
            }
            writer.WriteBytes(MemberAssignment);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version5)
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
            return ReferenceEquals(this, obj) || obj is DescribedGroupMemberMessage other && Equals(other);
        }

        public bool Equals(DescribedGroupMemberMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, MemberId, GroupInstanceId, ClientId, ClientHost, MemberMetadata, MemberAssignment);
            return hashCode;
        }
    }
}
