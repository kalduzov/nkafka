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

public sealed class CreateTopicsRequestMessage: IRequestMessage, IEquatable<CreateTopicsRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

    public ApiKeys ApiKey => ApiKeys.CreateTopics;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The topics to create.
    /// </summary>
    public CreatableTopicCollection Topics { get; set; } = new ();

    /// <summary>
    /// How long to wait in milliseconds before timing out the request.
    /// </summary>
    public int timeoutMs { get; set; } = 60000;

    /// <summary>
    /// If true, check that the topics can be created as specified, but don't create anything.
    /// </summary>
    public bool validateOnly { get; set; } = false;

    public CreateTopicsRequestMessage()
    {
    }

    public CreateTopicsRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            if (version >= ApiVersions.Version5)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new CreatableTopicCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new CreatableTopicMessage(reader, version));
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
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new CreatableTopicCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new CreatableTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        timeoutMs = reader.ReadInt();
        if (version >= ApiVersions.Version1)
        {
            validateOnly = reader.ReadByte() != 0;
        }
        else
        {
            validateOnly = false;
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
        if (version >= ApiVersions.Version5)
        {
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Topics.Count);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        writer.WriteInt(timeoutMs);
        if (version >= ApiVersions.Version1)
        {
            writer.WriteBool(validateOnly);
        }
        else
        {
            if (validateOnly)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default validateOnly at version {version}");
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
        return ReferenceEquals(this, obj) || obj is CreateTopicsRequestMessage other && Equals(other);
    }

    public bool Equals(CreateTopicsRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Topics, timeoutMs, validateOnly);
        return hashCode;
    }

    public sealed class CreatableTopicMessage: IMessage, IEquatable<CreatableTopicMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The number of partitions to create in the topic, or -1 if we are either specifying a manual partition assignment or using the default partitions.
        /// </summary>
        public int NumPartitions { get; set; } = 0;

        /// <summary>
        /// The number of replicas to create for each partition in the topic, or -1 if we are either specifying a manual partition assignment or using the default replication factor.
        /// </summary>
        public short ReplicationFactor { get; set; } = 0;

        /// <summary>
        /// The manual partition assignment, or the empty array if we are using automatic assignment.
        /// </summary>
        public CreatableReplicaAssignmentCollection Assignments { get; set; } = new ();

        /// <summary>
        /// The custom topic configurations to set.
        /// </summary>
        public CreateableTopicConfigCollection Configs { get; set; } = new ();

        public CreatableTopicMessage()
        {
        }

        public CreatableTopicMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of CreatableTopicMessage");
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
            NumPartitions = reader.ReadInt();
            ReplicationFactor = reader.ReadShort();
            {
                if (version >= ApiVersions.Version5)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Assignments was serialized as null");
                    }
                    else
                    {
                        var newCollection = new CreatableReplicaAssignmentCollection(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new CreatableReplicaAssignmentMessage(reader, version));
                        }
                        Assignments = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Assignments was serialized as null");
                    }
                    else
                    {
                        var newCollection = new CreatableReplicaAssignmentCollection(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new CreatableReplicaAssignmentMessage(reader, version));
                        }
                        Assignments = newCollection;
                    }
                }
            }
            {
                if (version >= ApiVersions.Version5)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Configs was serialized as null");
                    }
                    else
                    {
                        var newCollection = new CreateableTopicConfigCollection(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new CreateableTopicConfigMessage(reader, version));
                        }
                        Configs = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Configs was serialized as null");
                    }
                    else
                    {
                        var newCollection = new CreateableTopicConfigCollection(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new CreateableTopicConfigMessage(reader, version));
                        }
                        Configs = newCollection;
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
            writer.WriteInt(NumPartitions);
            writer.WriteShort(ReplicationFactor);
            if (version >= ApiVersions.Version5)
            {
                writer.WriteVarUInt(Assignments.Count + 1);
                foreach (var element in Assignments)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Assignments.Count);
                foreach (var element in Assignments)
                {
                    element.Write(writer, version);
                }
            }
            if (version >= ApiVersions.Version5)
            {
                writer.WriteVarUInt(Configs.Count + 1);
                foreach (var element in Configs)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Configs.Count);
                foreach (var element in Configs)
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
            return ReferenceEquals(this, obj) || obj is CreatableTopicMessage other && Equals(other);
        }

        public bool Equals(CreatableTopicMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }
    }

    public sealed class CreatableReplicaAssignmentMessage: IMessage, IEquatable<CreatableReplicaAssignmentMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The brokers to place the partition on.
        /// </summary>
        public List<int> BrokerIds { get; set; } = new ();

        public CreatableReplicaAssignmentMessage()
        {
        }

        public CreatableReplicaAssignmentMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of CreatableReplicaAssignmentMessage");
            }
            PartitionIndex = reader.ReadInt();
            {
                int arrayLength;
                if (version >= ApiVersions.Version5)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field BrokerIds was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    BrokerIds = newCollection;
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
            writer.WriteInt(PartitionIndex);
            if (version >= ApiVersions.Version5)
            {
                writer.WriteVarUInt(BrokerIds.Count + 1);
            }
            else
            {
                writer.WriteInt(BrokerIds.Count);
            }
            foreach (var element in BrokerIds)
            {
                writer.WriteInt(element);
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
            return ReferenceEquals(this, obj) || obj is CreatableReplicaAssignmentMessage other && Equals(other);
        }

        public bool Equals(CreatableReplicaAssignmentMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex);
            return hashCode;
        }
    }

    public sealed class CreatableReplicaAssignmentCollection: HashSet<CreatableReplicaAssignmentMessage>
    {
        public CreatableReplicaAssignmentCollection()
        {
        }

        public CreatableReplicaAssignmentCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class CreateableTopicConfigMessage: IMessage, IEquatable<CreateableTopicConfigMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The configuration name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The configuration value.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        public CreateableTopicConfigMessage()
        {
        }

        public CreateableTopicConfigMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of CreateableTopicConfigMessage");
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
                    Value = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Value had invalid length {length}");
                }
                else
                {
                    Value = reader.ReadString(length);
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
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
            if (Value is null)
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
                var stringBytes = Encoding.UTF8.GetBytes(Value);
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
            return ReferenceEquals(this, obj) || obj is CreateableTopicConfigMessage other && Equals(other);
        }

        public bool Equals(CreateableTopicConfigMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }
    }

    public sealed class CreateableTopicConfigCollection: HashSet<CreateableTopicConfigMessage>
    {
        public CreateableTopicConfigCollection()
        {
        }

        public CreateableTopicConfigCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class CreatableTopicCollection: HashSet<CreatableTopicMessage>
    {
        public CreatableTopicCollection()
        {
        }

        public CreatableTopicCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
