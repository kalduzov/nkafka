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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class DeleteAclsResponseMessage: IResponseMessage, IEquatable<DeleteAclsResponseMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The results for each filter.
    /// </summary>
    public List<DeleteAclsFilterResultMessage> FilterResults { get; set; } = new ();

    public DeleteAclsResponseMessage()
    {
    }

    public DeleteAclsResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ThrottleTimeMs = reader.ReadInt();
        {
            if (version >= ApiVersion.Version2)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field FilterResults was serialized as null");
                }
                else
                {
                    var newCollection = new List<DeleteAclsFilterResultMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DeleteAclsFilterResultMessage(reader, version));
                    }
                    FilterResults = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field FilterResults was serialized as null");
                }
                else
                {
                    var newCollection = new List<DeleteAclsFilterResultMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DeleteAclsFilterResultMessage(reader, version));
                    }
                    FilterResults = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version2)
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
        writer.WriteInt(ThrottleTimeMs);
        if (version >= ApiVersion.Version2)
        {
            writer.WriteVarUInt(FilterResults.Count + 1);
            foreach (var element in FilterResults)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(FilterResults.Count);
            foreach (var element in FilterResults)
            {
                element.Write(writer, version);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version2)
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
        return ReferenceEquals(this, obj) || obj is DeleteAclsResponseMessage other && Equals(other);
    }

    public bool Equals(DeleteAclsResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, FilterResults);
        return hashCode;
    }

    public override string ToString()
    {
        return "DeleteAclsResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }

    public sealed class DeleteAclsFilterResultMessage: IMessage, IEquatable<DeleteAclsFilterResultMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The error code, or 0 if the filter succeeded.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The error message, or null if the filter succeeded.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// The ACLs which matched this filter.
        /// </summary>
        public List<DeleteAclsMatchingAclMessage> MatchingAcls { get; set; } = new ();

        public DeleteAclsFilterResultMessage()
        {
        }

        public DeleteAclsFilterResultMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DeleteAclsFilterResultMessage");
            }
            ErrorCode = reader.ReadShort();
            {
                int length;
                if (version >= ApiVersion.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    ErrorMessage = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ErrorMessage had invalid length {length}");
                }
                else
                {
                    ErrorMessage = reader.ReadString(length);
                }
            }
            {
                if (version >= ApiVersion.Version2)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field MatchingAcls was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DeleteAclsMatchingAclMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DeleteAclsMatchingAclMessage(reader, version));
                        }
                        MatchingAcls = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field MatchingAcls was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DeleteAclsMatchingAclMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DeleteAclsMatchingAclMessage(reader, version));
                        }
                        MatchingAcls = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version2)
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
            writer.WriteShort((short)ErrorCode);
            if (ErrorMessage is null)
            {
                if (version >= ApiVersion.Version2)
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
                var stringBytes = Encoding.UTF8.GetBytes(ErrorMessage);
                if (version >= ApiVersion.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version2)
            {
                writer.WriteVarUInt(MatchingAcls.Count + 1);
                foreach (var element in MatchingAcls)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(MatchingAcls.Count);
                foreach (var element in MatchingAcls)
                {
                    element.Write(writer, version);
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version2)
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
            return ReferenceEquals(this, obj) || obj is DeleteAclsFilterResultMessage other && Equals(other);
        }

        public bool Equals(DeleteAclsFilterResultMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ErrorCode, ErrorMessage, MatchingAcls);
            return hashCode;
        }

        public override string ToString()
        {
            return "DeleteAclsFilterResultMessage("
                + "ErrorCode=" + ErrorCode
                + ")";
        }
    }

    public sealed class DeleteAclsMatchingAclMessage: IMessage, IEquatable<DeleteAclsMatchingAclMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The deletion error code, or 0 if the deletion succeeded.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The deletion error message, or null if the deletion succeeded.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// The ACL resource type.
        /// </summary>
        public sbyte ResourceType { get; set; } = 0;

        /// <summary>
        /// The ACL resource name.
        /// </summary>
        public string ResourceName { get; set; } = string.Empty;

        /// <summary>
        /// The ACL resource pattern type.
        /// </summary>
        public sbyte PatternType { get; set; } = 3;

        /// <summary>
        /// The ACL principal.
        /// </summary>
        public string Principal { get; set; } = string.Empty;

        /// <summary>
        /// The ACL host.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The ACL operation.
        /// </summary>
        public sbyte Operation { get; set; } = 0;

        /// <summary>
        /// The ACL permission type.
        /// </summary>
        public sbyte PermissionType { get; set; } = 0;

        public DeleteAclsMatchingAclMessage()
        {
        }

        public DeleteAclsMatchingAclMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DeleteAclsMatchingAclMessage");
            }
            ErrorCode = reader.ReadShort();
            {
                int length;
                if (version >= ApiVersion.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    ErrorMessage = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ErrorMessage had invalid length {length}");
                }
                else
                {
                    ErrorMessage = reader.ReadString(length);
                }
            }
            ResourceType = reader.ReadSByte();
            {
                int length;
                if (version >= ApiVersion.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field ResourceName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ResourceName had invalid length {length}");
                }
                else
                {
                    ResourceName = reader.ReadString(length);
                }
            }
            if (version >= ApiVersion.Version1)
            {
                PatternType = reader.ReadSByte();
            }
            else
            {
                PatternType = 3;
            }
            {
                int length;
                if (version >= ApiVersion.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Principal was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Principal had invalid length {length}");
                }
                else
                {
                    Principal = reader.ReadString(length);
                }
            }
            {
                int length;
                if (version >= ApiVersion.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Host was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Host had invalid length {length}");
                }
                else
                {
                    Host = reader.ReadString(length);
                }
            }
            Operation = reader.ReadSByte();
            PermissionType = reader.ReadSByte();
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version2)
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
            writer.WriteShort((short)ErrorCode);
            if (ErrorMessage is null)
            {
                if (version >= ApiVersion.Version2)
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
                var stringBytes = Encoding.UTF8.GetBytes(ErrorMessage);
                if (version >= ApiVersion.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            writer.WriteSByte(ResourceType);
            {
                var stringBytes = Encoding.UTF8.GetBytes(ResourceName);
                if (version >= ApiVersion.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version1)
            {
                writer.WriteSByte(PatternType);
            }
            else
            {
                if (PatternType != 3)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default PatternType at version {version}");
                }
            }
            {
                var stringBytes = Encoding.UTF8.GetBytes(Principal);
                if (version >= ApiVersion.Version2)
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
                var stringBytes = Encoding.UTF8.GetBytes(Host);
                if (version >= ApiVersion.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            writer.WriteSByte(Operation);
            writer.WriteSByte(PermissionType);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version2)
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
            return ReferenceEquals(this, obj) || obj is DeleteAclsMatchingAclMessage other && Equals(other);
        }

        public bool Equals(DeleteAclsMatchingAclMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ErrorCode, ErrorMessage, ResourceType, ResourceName, PatternType, Principal, Host);
            hashCode = HashCode.Combine(hashCode, Operation, PermissionType);
            return hashCode;
        }

        public override string ToString()
        {
            return "DeleteAclsMatchingAclMessage("
                + "ErrorCode=" + ErrorCode
                + ", ResourceType=" + ResourceType
                + ", PatternType=" + PatternType
                + ", Operation=" + Operation
                + ", PermissionType=" + PermissionType
                + ")";
        }
    }
}
