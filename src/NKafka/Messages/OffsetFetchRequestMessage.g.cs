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
/// Describes the contract for message OffsetFetchRequestMessage
/// </summary>
public sealed partial class OffsetFetchRequestMessage: IRequestMessage, IEquatable<OffsetFetchRequestMessage>
{
    /// <inheritdoc />
    public ApiKeys ApiKey => ApiKeys.OffsetFetch;

    /// <summary>
    /// Indicates whether the request is accessed by any broker or only by the controller
    /// </summary>
    public const bool ONLY_CONTROLLER = false;

    /// <inheritdoc />
    public bool OnlyController => ONLY_CONTROLLER;

    /// <inheritdoc />
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <inheritdoc />
    public int IncomingBufferLength { get; private set; } = 0;

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

    /// <summary>
    /// The basic constructor of the message OffsetFetchRequestMessage
    /// </summary>
    public OffsetFetchRequestMessage()
    {
    }

    /// <summary>
    /// Base constructor for deserializing message OffsetFetchRequestMessage
    /// </summary>
    public OffsetFetchRequestMessage(ref BufferReader reader, ApiVersion version)
        : this()
    {
        IncomingBufferLength = reader.Length;
        Read(ref reader, version);
    }

    /// <inheritdoc />
    public void Read(ref BufferReader reader, ApiVersion version)
    {
        if (version <= ApiVersion.Version7)
        {
            int length;
            if (version >= ApiVersion.Version6)
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
        if (version <= ApiVersion.Version7)
        {
            if (version >= ApiVersion.Version6)
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
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchRequestTopicMessage(ref reader, version));
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
                    if (version >= ApiVersion.Version2)
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
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchRequestTopicMessage(ref reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        else
        {
            Topics = new ();
        }
        if (version >= ApiVersion.Version8)
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
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new OffsetFetchRequestGroupMessage(ref reader, version));
                }
                Groups = newCollection;
            }
        }
        else
        {
            Groups = new ();
        }
        if (version >= ApiVersion.Version7)
        {
            RequireStable = reader.ReadByte() != 0;
        }
        else
        {
            RequireStable = false;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version6)
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
        if (version <= ApiVersion.Version7)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(GroupId);
                if (version >= ApiVersion.Version6)
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
        if (version <= ApiVersion.Version7)
        {
            if (version >= ApiVersion.Version6)
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
                    if (version >= ApiVersion.Version2)
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
        if (version >= ApiVersion.Version8)
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
        if (version >= ApiVersion.Version7)
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
        if (version >= ApiVersion.Version6)
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
        return ReferenceEquals(this, obj) || obj is OffsetFetchRequestMessage other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(OffsetFetchRequestMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (GroupId is null)
        {
            if (other.GroupId is not null)
            {
                return false;
            }
        }
        else
        {
            if (!GroupId.Equals(other.GroupId))
            {
                return false;
            }
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
        if (Groups is null)
        {
            if (other.Groups is not null)
            {
                return false;
            }
        }
        else
        {
            if (!Groups.SequenceEqual(other.Groups))
            {
                return false;
            }
        }
        if (RequireStable != other.RequireStable)
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, GroupId, Topics, Groups, RequireStable);
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "OffsetFetchRequestMessage("
            + "GroupId=" + (string.IsNullOrWhiteSpace(GroupId) ? "null" : GroupId)
            + ", Topics=" + (Topics is null ? "null" : Topics.DeepToString())
            + ", Groups=" + Groups.DeepToString()
            + ", RequireStable=" + (RequireStable ? "true" : "false")
            + ")";
    }

    /// <summary>
    /// Describes the contract for message OffsetFetchRequestTopicMessage
    /// </summary>
    public sealed partial class OffsetFetchRequestTopicMessage: IMessage, IEquatable<OffsetFetchRequestTopicMessage>
    {
        /// <inheritdoc />
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <inheritdoc />
        public int IncomingBufferLength { get; private set; } = 0;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The partition indexes we would like to fetch offsets for.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new ();

        /// <summary>
        /// The basic constructor of the message OffsetFetchRequestTopicMessage
        /// </summary>
        public OffsetFetchRequestTopicMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message OffsetFetchRequestTopicMessage
        /// </summary>
        public OffsetFetchRequestTopicMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            {
                int length;
                if (version >= ApiVersion.Version6)
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
                if (version >= ApiVersion.Version6)
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
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    PartitionIndexes = newCollection;
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version6)
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
            if (version > ApiVersion.Version7)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of OffsetFetchRequestTopicMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersion.Version6)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version6)
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
            if (version >= ApiVersion.Version6)
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
            return ReferenceEquals(this, obj) || obj is OffsetFetchRequestTopicMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(OffsetFetchRequestTopicMessage? other)
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
            if (PartitionIndexes is null)
            {
                if (other.PartitionIndexes is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!PartitionIndexes.SequenceEqual(other.PartitionIndexes))
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
            hashCode = HashCode.Combine(hashCode, Name, PartitionIndexes);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "OffsetFetchRequestTopicMessage("
                + "Name=" + (string.IsNullOrWhiteSpace(Name) ? "null" : Name)
                + ", PartitionIndexes=" + PartitionIndexes.DeepToString()
                + ")";
        }
    }

    /// <summary>
    /// Describes the contract for message OffsetFetchRequestGroupMessage
    /// </summary>
    public sealed partial class OffsetFetchRequestGroupMessage: IMessage, IEquatable<OffsetFetchRequestGroupMessage>
    {
        /// <inheritdoc />
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <inheritdoc />
        public int IncomingBufferLength { get; private set; } = 0;

        /// <summary>
        /// The group ID.
        /// </summary>
        public string groupId { get; set; } = string.Empty;

        /// <summary>
        /// Each topic we would like to fetch offsets for, or null to fetch offsets for all topics.
        /// </summary>
        public List<OffsetFetchRequestTopicsMessage> Topics { get; set; } = new ();

        /// <summary>
        /// The basic constructor of the message OffsetFetchRequestGroupMessage
        /// </summary>
        public OffsetFetchRequestGroupMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message OffsetFetchRequestGroupMessage
        /// </summary>
        public OffsetFetchRequestGroupMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version8)
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
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchRequestTopicsMessage(ref reader, version));
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

        /// <inheritdoc />
        public void Write(BufferWriter writer, ApiVersion version)
        {
            if (version < ApiVersion.Version8)
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

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetFetchRequestGroupMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(OffsetFetchRequestGroupMessage? other)
        {
            if (other is null)
            {
                return false;
            }
            if (groupId is null)
            {
                if (other.groupId is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!groupId.Equals(other.groupId))
                {
                    return false;
                }
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
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, groupId, Topics);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "OffsetFetchRequestGroupMessage("
                + "groupId=" + (string.IsNullOrWhiteSpace(groupId) ? "null" : groupId)
                + ", Topics=" + (Topics is null ? "null" : Topics.DeepToString())
                + ")";
        }
    }

    /// <summary>
    /// Describes the contract for message OffsetFetchRequestTopicsMessage
    /// </summary>
    public sealed partial class OffsetFetchRequestTopicsMessage: IMessage, IEquatable<OffsetFetchRequestTopicsMessage>
    {
        /// <inheritdoc />
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <inheritdoc />
        public int IncomingBufferLength { get; private set; } = 0;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The partition indexes we would like to fetch offsets for.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new ();

        /// <summary>
        /// The basic constructor of the message OffsetFetchRequestTopicsMessage
        /// </summary>
        public OffsetFetchRequestTopicsMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message OffsetFetchRequestTopicsMessage
        /// </summary>
        public OffsetFetchRequestTopicsMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version8)
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
                    for (var i = 0; i < arrayLength; i++)
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

        /// <inheritdoc />
        public void Write(BufferWriter writer, ApiVersion version)
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

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetFetchRequestTopicsMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(OffsetFetchRequestTopicsMessage? other)
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
            if (PartitionIndexes is null)
            {
                if (other.PartitionIndexes is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!PartitionIndexes.SequenceEqual(other.PartitionIndexes))
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
            hashCode = HashCode.Combine(hashCode, Name, PartitionIndexes);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "OffsetFetchRequestTopicsMessage("
                + "Name=" + (string.IsNullOrWhiteSpace(Name) ? "null" : Name)
                + ", PartitionIndexes=" + PartitionIndexes.DeepToString()
                + ")";
        }
    }
}
