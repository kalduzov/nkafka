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

public sealed class OffsetFetchRequestMessage: IRequestMessage, IEquatable<OffsetFetchRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

    public ApiKeys ApiKey => ApiKeys.OffsetFetch;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The group to fetch offsets for.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Each topic we would like to fetch offsets for, or null to fetch offsets for all topics.
    /// </summary>
    public List<OffsetFetchRequestTopicMessage> Topics { get; set; } = new ();

    /// <summary>
    /// Each group we would like to fetch offsets for
    /// </summary>
    public List<OffsetFetchRequestGroupMessage> Groups { get; set; } = new ();

    /// <summary>
    /// Whether broker should hold on returning unstable offsets but set a retriable error code for the partitions.
    /// </summary>
    public bool RequireStable { get; set; } = false;

    public OffsetFetchRequestMessage()
    {
    }

    public OffsetFetchRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        if (version <= ApiVersions.Version7)
        {
            int length;
            if (version >= ApiVersions.Version6)
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
        else
        {
            GroupId = string.Empty;
        }
        if (version <= ApiVersions.Version7)
        {
            if (version >= ApiVersions.Version6)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    Topics = null;
                }
                else
                {
                    var newCollection = new List<OffsetFetchRequestTopicMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchRequestTopicMessage(reader, version));
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
                    if (version >= ApiVersions.Version2)
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
                    var newCollection = new List<OffsetFetchRequestTopicMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchRequestTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        else
        {
            Topics = new ();
        }
        if (version >= ApiVersions.Version8)
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Groups was serialized as null");
            }
            else
            {
                var newCollection = new List<OffsetFetchRequestGroupMessage>(arrayLength);
                for (var i = 0; i< arrayLength; i++)
                {
                    newCollection.Add(new OffsetFetchRequestGroupMessage(reader, version));
                }
                Groups = newCollection;
            }
        }
        else
        {
            Groups = new ();
        }
        if (version >= ApiVersions.Version7)
        {
            RequireStable = reader.ReadByte() != 0;
        }
        else
        {
            RequireStable = false;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version6)
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
        if (version <= ApiVersions.Version7)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(GroupId);
                if (version >= ApiVersions.Version6)
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
            if (!GroupId.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default GroupId at version {version}");
            }
        }
        if (version <= ApiVersions.Version7)
        {
            if (version >= ApiVersions.Version6)
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
                    if (version >= ApiVersions.Version2)
                    {
                        writer.WriteInt(-1);
                    }
                    else
                    {
                        throw new NullReferenceException();                    }
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
        }
        else
        {
            if (Topics is null || Topics.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Topics at version {version}");
            }
        }
        if (version >= ApiVersions.Version8)
        {
            writer.WriteVarUInt(Groups.Count + 1);
            foreach (var element in Groups)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (Groups.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Groups at version {version}");
            }
        }
        if (version >= ApiVersions.Version7)
        {
            writer.WriteBool(RequireStable);
        }
        else
        {
            if (RequireStable)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default RequireStable at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version6)
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
        return ReferenceEquals(this, obj) || obj is OffsetFetchRequestMessage other && Equals(other);
    }

    public bool Equals(OffsetFetchRequestMessage? other)
    {
        return true;
    }

    public sealed class OffsetFetchRequestTopicMessage: IMessage, IEquatable<OffsetFetchRequestTopicMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The partition indexes we would like to fetch offsets for.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new ();

        public OffsetFetchRequestTopicMessage()
        {
        }

        public OffsetFetchRequestTopicMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            {
                int length;
                if (version >= ApiVersions.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
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
                if (version >= ApiVersions.Version6)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field PartitionIndexes was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    PartitionIndexes = newCollection;
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version6)
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
            if (version > ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of OffsetFetchRequestTopicMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersions.Version6)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(PartitionIndexes.Count + 1);
            }
            else
            {
                writer.WriteInt(PartitionIndexes.Count);
            }
            foreach (var element in PartitionIndexes)
            {
                writer.WriteInt(element);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version6)
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
            return ReferenceEquals(this, obj) || obj is OffsetFetchRequestTopicMessage other && Equals(other);
        }

        public bool Equals(OffsetFetchRequestTopicMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetFetchRequestGroupMessage: IMessage, IEquatable<OffsetFetchRequestGroupMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The group ID.
        /// </summary>
        public string groupId { get; set; } = string.Empty;

        /// <summary>
        /// Each topic we would like to fetch offsets for, or null to fetch offsets for all topics.
        /// </summary>
        public List<OffsetFetchRequestTopicsMessage> Topics { get; set; } = new ();

        public OffsetFetchRequestGroupMessage()
        {
        }

        public OffsetFetchRequestGroupMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetFetchRequestGroupMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field groupId was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field groupId had invalid length {length}");
                }
                else
                {
                    groupId = reader.ReadString(length);
                }
            }
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    Topics = null;
                }
                else
                {
                    var newCollection = new List<OffsetFetchRequestTopicsMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchRequestTopicsMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of OffsetFetchRequestGroupMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(groupId);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
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
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetFetchRequestGroupMessage other && Equals(other);
        }

        public bool Equals(OffsetFetchRequestGroupMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetFetchRequestTopicsMessage: IMessage, IEquatable<OffsetFetchRequestTopicsMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version8;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The partition indexes we would like to fetch offsets for.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new ();

        public OffsetFetchRequestTopicsMessage()
        {
        }

        public OffsetFetchRequestTopicsMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetFetchRequestTopicsMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
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
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field PartitionIndexes was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    PartitionIndexes = newCollection;
                }
            }
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteVarUInt(PartitionIndexes.Count + 1);
            foreach (var element in PartitionIndexes)
            {
                writer.WriteInt(element);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetFetchRequestTopicsMessage other && Equals(other);
        }

        public bool Equals(OffsetFetchRequestTopicsMessage? other)
        {
            return true;
        }
    }
}
