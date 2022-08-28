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

public sealed class MetadataRequestMessage: IRequestMessage, IEquatable<MetadataRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version12;

    public ApiKeys ApiKey => ApiKeys.Metadata;

    public ApiVersions Version {get; set;}

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

    public MetadataRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            if (version >= ApiVersions.Version9)
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
                    for (var i = 0; i< arrayLength; i++)
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
                    if (version >= ApiVersions.Version1)
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
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new MetadataRequestTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        if (version >= ApiVersions.Version4)
        {
            AllowAutoTopicCreation = reader.ReadByte() != 0;
        }
        else
        {
            AllowAutoTopicCreation = true;
        }
        if (version >= ApiVersions.Version8 && version <= ApiVersions.Version10)
        {
            IncludeClusterAuthorizedOperations = reader.ReadByte() != 0;
        }
        else
        {
            IncludeClusterAuthorizedOperations = false;
        }
        if (version >= ApiVersions.Version8)
        {
            IncludeTopicAuthorizedOperations = reader.ReadByte() != 0;
        }
        else
        {
            IncludeTopicAuthorizedOperations = false;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version9)
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
        if (version >= ApiVersions.Version9)
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
                if (version >= ApiVersions.Version1)
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
        if (version >= ApiVersions.Version4)
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
        if (version >= ApiVersions.Version8 && version <= ApiVersions.Version10)
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
        if (version >= ApiVersions.Version8)
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
        if (version >= ApiVersions.Version9)
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

    public sealed class MetadataRequestTopicMessage: IMessage, IEquatable<MetadataRequestTopicMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version12;

        public ApiVersions Version {get; set;}

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

        public MetadataRequestTopicMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version12)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of MetadataRequestTopicMessage");
            }
            if (version >= ApiVersions.Version10)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            {
                int length;
                if (version >= ApiVersions.Version9)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    if (version >= ApiVersions.Version10)
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
            if (version >= ApiVersions.Version9)
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
            if (version >= ApiVersions.Version10)
            {
                writer.WriteGuid(TopicId);
            }
            if (Name is null)
            {
                if (version >= ApiVersions.Version10)
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
                if (version >= ApiVersions.Version9)
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
            if (version >= ApiVersions.Version9)
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
    }
}
