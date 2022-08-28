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

public sealed class OffsetDeleteRequestMessage: IRequestMessage, IEquatable<OffsetDeleteRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

    public ApiKeys ApiKey => ApiKeys.OffsetDelete;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The unique group identifier.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// The topics to delete offsets for
    /// </summary>
    public OffsetDeleteRequestTopicCollection Topics { get; set; } = new ();

    public OffsetDeleteRequestMessage()
    {
    }

    public OffsetDeleteRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            int length;
            length = reader.ReadShort();
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
            int arrayLength;
            arrayLength = reader.ReadInt();
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Topics was serialized as null");
            }
            else
            {
                OffsetDeleteRequestTopicCollection newCollection = new(arrayLength);
                for (var i = 0; i< arrayLength; i++)
                {
                    newCollection.Add(new OffsetDeleteRequestTopicMessage(reader, version));
                }
                Topics = newCollection;
            }
        }
        UnknownTaggedFields = null;
    }

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        {
            var stringBytes = Encoding.UTF8.GetBytes(GroupId);
            writer.WriteShort((short)stringBytes.Length);
            writer.WriteBytes(stringBytes);
        }
        writer.WriteInt(Topics.Count);
        foreach (var element in Topics)
        {
            element.Write(writer, version);
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (numTaggedFields > 0)
        {
            throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
        }
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is OffsetDeleteRequestMessage other && Equals(other);
    }

    public bool Equals(OffsetDeleteRequestMessage? other)
    {
        return true;
    }

    public sealed class OffsetDeleteRequestTopicMessage: IMessage, IEquatable<OffsetDeleteRequestTopicMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Each partition to delete offsets for.
        /// </summary>
        public List<OffsetDeleteRequestPartitionMessage> Partitions { get; set; } = new ();

        public OffsetDeleteRequestTopicMessage()
        {
        }

        public OffsetDeleteRequestTopicMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetDeleteRequestTopicMessage");
            }
            {
                int length;
                length = reader.ReadShort();
                if (length < 0)
                {
                    throw new Exception("non-nullable field Name was serialized as null");
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
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<OffsetDeleteRequestPartitionMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetDeleteRequestPartitionMessage(reader, version));
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteShort((short)stringBytes.Length);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteInt(Partitions.Count);
            foreach (var element in Partitions)
            {
                element.Write(writer, version);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetDeleteRequestTopicMessage other && Equals(other);
        }

        public bool Equals(OffsetDeleteRequestTopicMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetDeleteRequestPartitionMessage: IMessage, IEquatable<OffsetDeleteRequestPartitionMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        public OffsetDeleteRequestPartitionMessage()
        {
        }

        public OffsetDeleteRequestPartitionMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetDeleteRequestPartitionMessage");
            }
            PartitionIndex = reader.ReadInt();
            UnknownTaggedFields = null;
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetDeleteRequestPartitionMessage other && Equals(other);
        }

        public bool Equals(OffsetDeleteRequestPartitionMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetDeleteRequestTopicCollection: HashSet<OffsetDeleteRequestTopicMessage>
    {
        public OffsetDeleteRequestTopicCollection()
        {
        }

        public OffsetDeleteRequestTopicCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
