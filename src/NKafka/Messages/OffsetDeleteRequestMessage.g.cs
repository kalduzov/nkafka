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
/// Describes the contract for message OffsetDeleteRequestMessage
/// </summary>
public sealed partial class OffsetDeleteRequestMessage: IRequestMessage, IEquatable<OffsetDeleteRequestMessage>
{
    /// <inheritdoc />
    public ApiKeys ApiKey => ApiKeys.OffsetDelete;

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
    /// The unique group identifier.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// The topics to delete offsets for
    /// </summary>
    public OffsetDeleteRequestTopicCollection Topics { get; set; } = new ();

    /// <summary>
    /// The basic constructor of the message OffsetDeleteRequestMessage
    /// </summary>
    public OffsetDeleteRequestMessage()
    {
    }

    /// <summary>
    /// Base constructor for deserializing message OffsetDeleteRequestMessage
    /// </summary>
    public OffsetDeleteRequestMessage(ref BufferReader reader, ApiVersion version)
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
                var newCollection = new OffsetDeleteRequestTopicCollection(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new OffsetDeleteRequestTopicMessage(ref reader, version));
                }
                Topics = newCollection;
            }
        }
        UnknownTaggedFields = null;
    }

    /// <inheritdoc />
    public void Write(BufferWriter writer, ApiVersion version)
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

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is OffsetDeleteRequestMessage other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(OffsetDeleteRequestMessage? other)
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
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, GroupId, Topics);
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "OffsetDeleteRequestMessage("
            + "GroupId=" + (string.IsNullOrWhiteSpace(GroupId) ? "null" : GroupId)
            + ", Topics=" + Topics.DeepToString()
            + ")";
    }

    /// <summary>
    /// Describes the contract for message OffsetDeleteRequestTopicMessage
    /// </summary>
    public sealed partial class OffsetDeleteRequestTopicMessage: IMessage, IEquatable<OffsetDeleteRequestTopicMessage>
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
        /// Each partition to delete offsets for.
        /// </summary>
        public List<OffsetDeleteRequestPartitionMessage> Partitions { get; set; } = new ();

        /// <summary>
        /// The basic constructor of the message OffsetDeleteRequestTopicMessage
        /// </summary>
        public OffsetDeleteRequestTopicMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message OffsetDeleteRequestTopicMessage
        /// </summary>
        public OffsetDeleteRequestTopicMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
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
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new OffsetDeleteRequestPartitionMessage(ref reader, version));
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
        }

        /// <inheritdoc />
        public void Write(BufferWriter writer, ApiVersion version)
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

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetDeleteRequestTopicMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(OffsetDeleteRequestTopicMessage? other)
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
            if (Partitions is null)
            {
                if (other.Partitions is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Partitions.SequenceEqual(other.Partitions))
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
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "OffsetDeleteRequestTopicMessage("
                + "Name=" + (string.IsNullOrWhiteSpace(Name) ? "null" : Name)
                + ", Partitions=" + Partitions.DeepToString()
                + ")";
        }
    }

    /// <summary>
    /// Describes the contract for message OffsetDeleteRequestPartitionMessage
    /// </summary>
    public sealed partial class OffsetDeleteRequestPartitionMessage: IMessage, IEquatable<OffsetDeleteRequestPartitionMessage>
    {
        /// <inheritdoc />
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <inheritdoc />
        public int IncomingBufferLength { get; private set; } = 0;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The basic constructor of the message OffsetDeleteRequestPartitionMessage
        /// </summary>
        public OffsetDeleteRequestPartitionMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message OffsetDeleteRequestPartitionMessage
        /// </summary>
        public OffsetDeleteRequestPartitionMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetDeleteRequestPartitionMessage");
            }
            PartitionIndex = reader.ReadInt();
            UnknownTaggedFields = null;
        }

        /// <inheritdoc />
        public void Write(BufferWriter writer, ApiVersion version)
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

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetDeleteRequestPartitionMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(OffsetDeleteRequestPartitionMessage? other)
        {
            if (other is null)
            {
                return false;
            }
            if (PartitionIndex != other.PartitionIndex)
            {
                return false;
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "OffsetDeleteRequestPartitionMessage("
                + "PartitionIndex=" + PartitionIndex
                + ")";
        }
    }

    /// <summary>
    /// Describes the contract for message OffsetDeleteRequestTopicCollection
    /// </summary>
    public sealed partial class OffsetDeleteRequestTopicCollection: HashSet<OffsetDeleteRequestTopicMessage>
    {
        /// <summary>
        /// Basic collection constructor
        /// </summary>
        public OffsetDeleteRequestTopicCollection()
        {
        }

        /// <summary>
        /// Basic collection constructor with the ability to set capacity
        /// </summary>
        public OffsetDeleteRequestTopicCollection(int capacity)
            : base(capacity)
        {
        }
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return SetEquals((IEnumerable<OffsetDeleteRequestTopicMessage>)obj);
        }
    }
}
