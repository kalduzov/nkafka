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
// <auto-generated> THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT. </auto-generated>

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

#nullable enable

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

/// <summary>
/// Describes the contract for message DescribeAclsResponseMessage
/// </summary>
public sealed partial class DescribeAclsResponseMessage: IResponseMessage, IEquatable<DescribeAclsResponseMessage>
{
    /// <inheritdoc />
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <inheritdoc />
    public int IncomingBufferLength { get; private set; } = 0;

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
    /// The error message, or null if there was no error.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Each Resource that is referenced in an ACL.
    /// </summary>
    public List<DescribeAclsResourceMessage> Resources { get; set; } = new ();

    /// <summary>
    /// The basic constructor of the message DescribeAclsResponseMessage
    /// </summary>
    public DescribeAclsResponseMessage()
    {
    }

    /// <summary>
    /// Base constructor for deserializing message DescribeAclsResponseMessage
    /// </summary>
    public DescribeAclsResponseMessage(ref BufferReader reader, ApiVersion version)
        : this()
    {
        IncomingBufferLength = reader.Length;
        Read(ref reader, version);
    }

    /// <inheritdoc />
    public void Read(ref BufferReader reader, ApiVersion version)
    {
        ThrottleTimeMs = reader.ReadInt();
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
                    throw new Exception("non-nullable field Resources was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribeAclsResourceMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribeAclsResourceMessage(ref reader, version));
                    }
                    Resources = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Resources was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribeAclsResourceMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribeAclsResourceMessage(ref reader, version));
                    }
                    Resources = newCollection;
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

    /// <inheritdoc />
    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(ThrottleTimeMs);
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
            writer.WriteVarUInt(Resources.Count + 1);
            foreach (var element in Resources)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Resources.Count);
            foreach (var element in Resources)
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

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is DescribeAclsResponseMessage other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(DescribeAclsResponseMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ThrottleTimeMs != other.ThrottleTimeMs)
        {
            return false;
        }
        if (ErrorCode != other.ErrorCode)
        {
            return false;
        }
        if (ErrorMessage is null)
        {
            if (other.ErrorMessage is not null)
            {
                return false;
            }
        }
        else
        {
            if (!ErrorMessage.Equals(other.ErrorMessage))
            {
                return false;
            }
        }
        if (Resources is null)
        {
            if (other.Resources is not null)
            {
                return false;
            }
        }
        else
        {
            if (!Resources.SequenceEqual(other.Resources))
            {
                return false;
            }
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, ErrorMessage, Resources);
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "DescribeAclsResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
            + ", ErrorMessage=" + (string.IsNullOrWhiteSpace(ErrorMessage) ? "null" : ErrorMessage)
            + ", Resources=" + Resources.DeepToString()
            + ")";
    }

    /// <summary>
    /// Describes the contract for message DescribeAclsResourceMessage
    /// </summary>
    public sealed partial class DescribeAclsResourceMessage: IMessage, IEquatable<DescribeAclsResourceMessage>
    {
        /// <inheritdoc />
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <inheritdoc />
        public int IncomingBufferLength { get; private set; } = 0;

        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceType { get; set; } = 0;

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceName { get; set; } = string.Empty;

        /// <summary>
        /// The resource pattern type.
        /// </summary>
        public sbyte PatternType { get; set; } = 3;

        /// <summary>
        /// The ACLs.
        /// </summary>
        public List<AclDescriptionMessage> Acls { get; set; } = new ();

        /// <summary>
        /// The basic constructor of the message DescribeAclsResourceMessage
        /// </summary>
        public DescribeAclsResourceMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message DescribeAclsResourceMessage
        /// </summary>
        public DescribeAclsResourceMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribeAclsResourceMessage");
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
                if (version >= ApiVersion.Version2)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Acls was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<AclDescriptionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new AclDescriptionMessage(ref reader, version));
                        }
                        Acls = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Acls was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<AclDescriptionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new AclDescriptionMessage(ref reader, version));
                        }
                        Acls = newCollection;
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

        /// <inheritdoc />
        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
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
            if (version >= ApiVersion.Version2)
            {
                writer.WriteVarUInt(Acls.Count + 1);
                foreach (var element in Acls)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Acls.Count);
                foreach (var element in Acls)
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

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is DescribeAclsResourceMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(DescribeAclsResourceMessage? other)
        {
            if (other is null)
            {
                return false;
            }
            if (ResourceType != other.ResourceType)
            {
                return false;
            }
            if (ResourceName is null)
            {
                if (other.ResourceName is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!ResourceName.Equals(other.ResourceName))
                {
                    return false;
                }
            }
            if (PatternType != other.PatternType)
            {
                return false;
            }
            if (Acls is null)
            {
                if (other.Acls is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Acls.SequenceEqual(other.Acls))
                {
                    return false;
                }
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ResourceType, ResourceName, PatternType, Acls);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "DescribeAclsResourceMessage("
                + "ResourceType=" + ResourceType
                + ", ResourceName=" + (string.IsNullOrWhiteSpace(ResourceName) ? "null" : ResourceName)
                + ", PatternType=" + PatternType
                + ", Acls=" + Acls.DeepToString()
                + ")";
        }
    }

    /// <summary>
    /// Describes the contract for message AclDescriptionMessage
    /// </summary>
    public sealed partial class AclDescriptionMessage: IMessage, IEquatable<AclDescriptionMessage>
    {
        /// <inheritdoc />
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <inheritdoc />
        public int IncomingBufferLength { get; private set; } = 0;

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

        /// <summary>
        /// The basic constructor of the message AclDescriptionMessage
        /// </summary>
        public AclDescriptionMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message AclDescriptionMessage
        /// </summary>
        public AclDescriptionMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of AclDescriptionMessage");
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

        /// <inheritdoc />
        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
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

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is AclDescriptionMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(AclDescriptionMessage? other)
        {
            if (other is null)
            {
                return false;
            }
            if (Principal is null)
            {
                if (other.Principal is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Principal.Equals(other.Principal))
                {
                    return false;
                }
            }
            if (Host is null)
            {
                if (other.Host is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Host.Equals(other.Host))
                {
                    return false;
                }
            }
            if (Operation != other.Operation)
            {
                return false;
            }
            if (PermissionType != other.PermissionType)
            {
                return false;
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Principal, Host, Operation, PermissionType);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "AclDescriptionMessage("
                + "Principal=" + (string.IsNullOrWhiteSpace(Principal) ? "null" : Principal)
                + ", Host=" + (string.IsNullOrWhiteSpace(Host) ? "null" : Host)
                + ", Operation=" + Operation
                + ", PermissionType=" + PermissionType
                + ")";
        }
    }
}
