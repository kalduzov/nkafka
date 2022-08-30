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

public sealed class MetadataRequestMessage: IRequestMessage, IEquatable<MetadataRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version12;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.Metadata;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The topics to fetch metadata for.
    /// </summary>
    public List<MetadataRequestTopicMessage> Topics { get; set; } = new ();

    /// <summary>
    /// If this is true, the broker may auto-create topics that we requested which do not already exist, if it is configured to do so.
    /// </summary>
    public bool AllowAutoTopicCreation { get; set; } = true;

    /// <summary>
    /// Whether to include cluster authorized operations.
    /// </summary>
    public bool IncludeClusterAuthorizedOperations { get; set; } = false;

    /// <summary>
    /// Whether to include topic authorized operations.
    /// </summary>
    public bool IncludeTopicAuthorizedOperations { get; set; } = false;

    public MetadataRequestMessage()
    {
    }

    public MetadataRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            if (version >= ApiVersion.Version9)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    Topics = null;
                }
                else
                {
                    var newCollection = new List<MetadataRequestTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new MetadataRequestTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    if (version >= ApiVersion.Version1)
                    {
                        Topics = null;
                    }
                    else
                    {
                        throw new Exception("non-nullable field Topics was serialized as null");
                    }
                }
                else
                {
                    var newCollection = new List<MetadataRequestTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new MetadataRequestTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        if (version >= ApiVersion.Version4)
        {
            AllowAutoTopicCreation = reader.ReadByte() != 0;
        }
        else
        {
            AllowAutoTopicCreation = true;
        }
        if (version >= ApiVersion.Version8 && version <= ApiVersion.Version10)
        {
            IncludeClusterAuthorizedOperations = reader.ReadByte() != 0;
        }
        else
        {
            IncludeClusterAuthorizedOperations = false;
        }
        if (version >= ApiVersion.Version8)
        {
            IncludeTopicAuthorizedOperations = reader.ReadByte() != 0;
        }
        else
        {
            IncludeTopicAuthorizedOperations = false;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version9)
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
        if (version >= ApiVersion.Version9)
        {
            if (Topics is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteVarUInt(Topics.Count + 1);
                foreach (var element in Topics)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (Topics is null)
            {
                if (version >= ApiVersion.Version1)
                {
                    writer.WriteInt(-1);
                }
                else
                {
                    throw new NullReferenceException();                }
            }
            else
            {
                writer.WriteInt(Topics.Count);
                foreach (var element in Topics)
                {
                    element.Write(writer, version);
                }
            }
        }
        if (version >= ApiVersion.Version4)
        {
            writer.WriteBool(AllowAutoTopicCreation);
        }
        else
        {
            if (!AllowAutoTopicCreation)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default AllowAutoTopicCreation at version {version}");
            }
        }
        if (version >= ApiVersion.Version8 && version <= ApiVersion.Version10)
        {
            writer.WriteBool(IncludeClusterAuthorizedOperations);
        }
        else
        {
            if (IncludeClusterAuthorizedOperations)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default IncludeClusterAuthorizedOperations at version {version}");
            }
        }
        if (version >= ApiVersion.Version8)
        {
            writer.WriteBool(IncludeTopicAuthorizedOperations);
        }
        else
        {
            if (IncludeTopicAuthorizedOperations)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default IncludeTopicAuthorizedOperations at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version9)
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
        return ReferenceEquals(this, obj) || obj is MetadataRequestMessage other && Equals(other);
    }

    public bool Equals(MetadataRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Topics, AllowAutoTopicCreation, IncludeClusterAuthorizedOperations, IncludeTopicAuthorizedOperations);
        return hashCode;
    }

    public override string ToString()
    {
        return "MetadataRequestMessage("
            + ", AllowAutoTopicCreation=" + (AllowAutoTopicCreation ? "true" : "false")
            + ", IncludeClusterAuthorizedOperations=" + (IncludeClusterAuthorizedOperations ? "true" : "false")
            + ", IncludeTopicAuthorizedOperations=" + (IncludeTopicAuthorizedOperations ? "true" : "false")
            + ")";
    }

    public sealed class MetadataRequestTopicMessage: IMessage, IEquatable<MetadataRequestTopicMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version12;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic id.
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        public MetadataRequestTopicMessage()
        {
        }

        public MetadataRequestTopicMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version12)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of MetadataRequestTopicMessage");
            }
            if (version >= ApiVersion.Version10)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            {
                int length;
                if (version >= ApiVersion.Version9)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    if (version >= ApiVersion.Version10)
                    {
                        Name = null;
                    }
                    else
                    {
                        throw new Exception("non-nullable field Name was serialized as null");
                    }
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Name had invalid length {length}");
                }
                else
                {
                    Name = reader.ReadString(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version9)
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
            if (version >= ApiVersion.Version10)
            {
                writer.WriteGuid(TopicId);
            }
            if (Name is null)
            {
                if (version >= ApiVersion.Version10)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    throw new NullReferenceException();                }
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersion.Version9)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version9)
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
            return ReferenceEquals(this, obj) || obj is MetadataRequestTopicMessage other && Equals(other);
        }

        public bool Equals(MetadataRequestTopicMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, TopicId, Name);
            return hashCode;
        }

        public override string ToString()
        {
            return "MetadataRequestTopicMessage("
                + "TopicId=" + TopicId
                + ")";
        }
    }
}
