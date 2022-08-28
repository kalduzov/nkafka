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

public sealed class LeaveGroupResponseMessage: IResponseMessage, IEquatable<LeaveGroupResponseMessage>
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
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// List of leaving member responses.
    /// </summary>
    public List<MemberResponseMessage> Members { get; set; } = new ();

    public LeaveGroupResponseMessage()
    {
    }

    public LeaveGroupResponseMessage(BufferReader reader, ApiVersions version)
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
        ErrorCode = reader.ReadShort();
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
                    var newCollection = new List<MemberResponseMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new MemberResponseMessage(reader, version));
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
                    var newCollection = new List<MemberResponseMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new MemberResponseMessage(reader, version));
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
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        writer.WriteShort((short)ErrorCode);
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
        return ReferenceEquals(this, obj) || obj is LeaveGroupResponseMessage other && Equals(other);
    }

    public bool Equals(LeaveGroupResponseMessage? other)
    {
        return true;
    }

    public sealed class MemberResponseMessage: IMessage, IEquatable<MemberResponseMessage>
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
        public string GroupInstanceId { get; set; } = string.Empty;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        public MemberResponseMessage()
        {
        }

        public MemberResponseMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version5)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of MemberResponseMessage");
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
            ErrorCode = reader.ReadShort();
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
                throw new UnsupportedVersionException($"Can't write version {version} of MemberResponseMessage");
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
            writer.WriteShort((short)ErrorCode);
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
            return ReferenceEquals(this, obj) || obj is MemberResponseMessage other && Equals(other);
        }

        public bool Equals(MemberResponseMessage? other)
        {
            return true;
        }
    }
}
