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

public sealed class DeleteAclsRequestMessage: IRequestMessage, IEquatable<DeleteAclsRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

    public ApiKeys ApiKey => ApiKeys.DeleteAcls;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The filters to use when deleting ACLs.
    /// </summary>
    public List<DeleteAclsFilterMessage> Filters { get; set; } = new ();

    public DeleteAclsRequestMessage()
    {
    }

    public DeleteAclsRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            if (version >= ApiVersions.Version2)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Filters was serialized as null");
                }
                else
                {
                    var newCollection = new List<DeleteAclsFilterMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DeleteAclsFilterMessage(reader, version));
                    }
                    Filters = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Filters was serialized as null");
                }
                else
                {
                    var newCollection = new List<DeleteAclsFilterMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DeleteAclsFilterMessage(reader, version));
                    }
                    Filters = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version2)
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
            writer.WriteVarUInt(Filters.Count + 1);
            foreach (var element in Filters)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Filters.Count);
            foreach (var element in Filters)
            {
                element.Write(writer, version);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version2)
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
        return ReferenceEquals(this, obj) || obj is DeleteAclsRequestMessage other && Equals(other);
    }

    public bool Equals(DeleteAclsRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Filters);
        return hashCode;
    }

    public override string ToString()
    {
        return "DeleteAclsRequestMessage("
            + ")";
    }

    public sealed class DeleteAclsFilterMessage: IMessage, IEquatable<DeleteAclsFilterMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceTypeFilter { get; set; } = 0;

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceNameFilter { get; set; } = string.Empty;

        /// <summary>
        /// The pattern type.
        /// </summary>
        public sbyte PatternTypeFilter { get; set; } = 3;

        /// <summary>
        /// The principal filter, or null to accept all principals.
        /// </summary>
        public string PrincipalFilter { get; set; } = string.Empty;

        /// <summary>
        /// The host filter, or null to accept all hosts.
        /// </summary>
        public string HostFilter { get; set; } = string.Empty;

        /// <summary>
        /// The ACL operation.
        /// </summary>
        public sbyte Operation { get; set; } = 0;

        /// <summary>
        /// The permission type.
        /// </summary>
        public sbyte PermissionType { get; set; } = 0;

        public DeleteAclsFilterMessage()
        {
        }

        public DeleteAclsFilterMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DeleteAclsFilterMessage");
            }
            ResourceTypeFilter = reader.ReadSByte();
            {
                int length;
                if (version >= ApiVersions.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    ResourceNameFilter = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ResourceNameFilter had invalid length {length}");
                }
                else
                {
                    ResourceNameFilter = reader.ReadString(length);
                }
            }
            if (version >= ApiVersions.Version1)
            {
                PatternTypeFilter = reader.ReadSByte();
            }
            else
            {
                PatternTypeFilter = 3;
            }
            {
                int length;
                if (version >= ApiVersions.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    PrincipalFilter = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field PrincipalFilter had invalid length {length}");
                }
                else
                {
                    PrincipalFilter = reader.ReadString(length);
                }
            }
            {
                int length;
                if (version >= ApiVersions.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    HostFilter = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field HostFilter had invalid length {length}");
                }
                else
                {
                    HostFilter = reader.ReadString(length);
                }
            }
            Operation = reader.ReadSByte();
            PermissionType = reader.ReadSByte();
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version2)
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
            writer.WriteSByte(ResourceTypeFilter);
            if (ResourceNameFilter is null)
            {
                if (version >= ApiVersions.Version2)
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
                var stringBytes = Encoding.UTF8.GetBytes(ResourceNameFilter);
                if (version >= ApiVersions.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version1)
            {
                writer.WriteSByte(PatternTypeFilter);
            }
            else
            {
                if (PatternTypeFilter != 3)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default PatternTypeFilter at version {version}");
                }
            }
            if (PrincipalFilter is null)
            {
                if (version >= ApiVersions.Version2)
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
                var stringBytes = Encoding.UTF8.GetBytes(PrincipalFilter);
                if (version >= ApiVersions.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (HostFilter is null)
            {
                if (version >= ApiVersions.Version2)
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
                var stringBytes = Encoding.UTF8.GetBytes(HostFilter);
                if (version >= ApiVersions.Version2)
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
            if (version >= ApiVersions.Version2)
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
            return ReferenceEquals(this, obj) || obj is DeleteAclsFilterMessage other && Equals(other);
        }

        public bool Equals(DeleteAclsFilterMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ResourceTypeFilter, ResourceNameFilter, PatternTypeFilter, PrincipalFilter, HostFilter, Operation, PermissionType);
            return hashCode;
        }

        public override string ToString()
        {
            return "DeleteAclsFilterMessage("
                + "ResourceTypeFilter=" + ResourceTypeFilter
                + ", PatternTypeFilter=" + PatternTypeFilter
                + ", Operation=" + Operation
                + ", PermissionType=" + PermissionType
                + ")";
        }
    }
}
