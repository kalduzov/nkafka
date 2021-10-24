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
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed partial class CreateTopicsRequestMessage: IRequestMessage, IEquatable<CreateTopicsRequestMessage>
{
    /// <inheritdoc />
    public ApiKeys ApiKey => ApiKeys.CreateTopics;

    public const bool ONLY_CONTROLLER = true;

    /// <inheritdoc />
    public bool OnlyController => ONLY_CONTROLLER;

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

    public CreateTopicsRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            if (version >= ApiVersion.Version5)
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
        if (version >= ApiVersion.Version1)
        {
            validateOnly = reader.ReadByte() != 0;
        }
        else
        {
            validateOnly = false;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version5)
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
        if (version >= ApiVersion.Version5)
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
        if (version >= ApiVersion.Version1)
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
        if (version >= ApiVersion.Version5)
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
        if (other is null)
        {
            return false;
        }
        if (Topics is null)
        {
            if (other.Topics is not null)
            {
                return false;
            }
        }
        else
        {
            if (!Topics.SequenceEqual(other.Topics))
            {
                return false;
            }
        }
        if (timeoutMs != other.timeoutMs)
        {
            return false;
        }
        if (validateOnly != other.validateOnly)
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Topics, timeoutMs, validateOnly);
        return hashCode;
    }

    public override string ToString()
    {
        return "CreateTopicsRequestMessage("
            + "Topics=" + Topics.DeepToString()
            + ", timeoutMs=" + timeoutMs
            + ", validateOnly=" + (validateOnly ? "true" : "false")
            + ")";
    }

    public sealed partial class CreatableTopicMessage: IMessage, IEquatable<CreatableTopicMessage>
    {
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

        public CreatableTopicMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of CreatableTopicMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version5)
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
                if (version >= ApiVersion.Version5)
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
                if (version >= ApiVersion.Version5)
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
            if (version >= ApiVersion.Version5)
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersion.Version5)
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
            if (version >= ApiVersion.Version5)
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
            if (version >= ApiVersion.Version5)
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
            if (version >= ApiVersion.Version5)
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
            if (other is null)
            {
                return false;
            }
            if (Name is null)
            {
                if (other.Name is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Name.Equals(other.Name))
                {
                    return false;
                }
            }
            if (NumPartitions != other.NumPartitions)
            {
                return false;
            }
            if (ReplicationFactor != other.ReplicationFactor)
            {
                return false;
            }
            if (Assignments is null)
            {
                if (other.Assignments is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Assignments.SequenceEqual(other.Assignments))
                {
                    return false;
                }
            }
            if (Configs is null)
            {
                if (other.Configs is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Configs.SequenceEqual(other.Configs))
                {
                    return false;
                }
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }

        public override string ToString()
        {
            return "CreatableTopicMessage("
                + "Name=" + (string.IsNullOrWhiteSpace(Name) ? "null" : Name)
                + ", NumPartitions=" + NumPartitions
                + ", ReplicationFactor=" + ReplicationFactor
                + ", Assignments=" + Assignments.DeepToString()
                + ", Configs=" + Configs.DeepToString()
                + ")";
        }
    }

    public sealed partial class CreatableReplicaAssignmentMessage: IMessage, IEquatable<CreatableReplicaAssignmentMessage>
    {
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

        public CreatableReplicaAssignmentMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of CreatableReplicaAssignmentMessage");
            }
            PartitionIndex = reader.ReadInt();
            {
                int arrayLength;
                if (version >= ApiVersion.Version5)
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
            if (version >= ApiVersion.Version5)
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
            writer.WriteInt(PartitionIndex);
            if (version >= ApiVersion.Version5)
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
            if (version >= ApiVersion.Version5)
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
            if (other is null)
            {
                return false;
            }
            if (PartitionIndex != other.PartitionIndex)
            {
                return false;
            }
            if (BrokerIds is null)
            {
                if (other.BrokerIds is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!BrokerIds.SequenceEqual(other.BrokerIds))
                {
                    return false;
                }
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex);
            return hashCode;
        }

        public override string ToString()
        {
            return "CreatableReplicaAssignmentMessage("
                + "PartitionIndex=" + PartitionIndex
                + ", BrokerIds=" + BrokerIds.DeepToString()
                + ")";
        }
    }

    public sealed partial class CreatableReplicaAssignmentCollection: HashSet<CreatableReplicaAssignmentMessage>
    {
        public CreatableReplicaAssignmentCollection()
        {
        }

        public CreatableReplicaAssignmentCollection(int capacity)
            : base(capacity)
        {
        }
        public override bool Equals(object? obj)
        {
            return SetEquals((IEnumerable<CreatableReplicaAssignmentMessage>)obj);
        }
    }

    public sealed partial class CreateableTopicConfigMessage: IMessage, IEquatable<CreateableTopicConfigMessage>
    {
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

        public CreateableTopicConfigMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of CreateableTopicConfigMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version5)
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
                if (version >= ApiVersion.Version5)
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
            if (version >= ApiVersion.Version5)
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersion.Version5)
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
                if (version >= ApiVersion.Version5)
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
                if (version >= ApiVersion.Version5)
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
            if (version >= ApiVersion.Version5)
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
            if (other is null)
            {
                return false;
            }
            if (Name is null)
            {
                if (other.Name is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Name.Equals(other.Name))
                {
                    return false;
                }
            }
            if (Value is null)
            {
                if (other.Value is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Value.Equals(other.Value))
                {
                    return false;
                }
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }

        public override string ToString()
        {
            return "CreateableTopicConfigMessage("
                + "Name=" + (string.IsNullOrWhiteSpace(Name) ? "null" : Name)
                + ", Value=" + (string.IsNullOrWhiteSpace(Value) ? "null" : Value)
                + ")";
        }
    }

    public sealed partial class CreateableTopicConfigCollection: HashSet<CreateableTopicConfigMessage>
    {
        public CreateableTopicConfigCollection()
        {
        }

        public CreateableTopicConfigCollection(int capacity)
            : base(capacity)
        {
        }
        public override bool Equals(object? obj)
        {
            return SetEquals((IEnumerable<CreateableTopicConfigMessage>)obj);
        }
    }

    public sealed partial class CreatableTopicCollection: HashSet<CreatableTopicMessage>
    {
        public CreatableTopicCollection()
        {
        }

        public CreatableTopicCollection(int capacity)
            : base(capacity)
        {
        }
        public override bool Equals(object? obj)
        {
            return SetEquals((IEnumerable<CreatableTopicMessage>)obj);
        }
    }
}