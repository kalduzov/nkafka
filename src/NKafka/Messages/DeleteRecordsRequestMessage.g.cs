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
/// Describes the contract for message DeleteRecordsRequestMessage
/// </summary>
public sealed partial class DeleteRecordsRequestMessage: IRequestMessage, IEquatable<DeleteRecordsRequestMessage>
{
    /// <inheritdoc />
    public ApiKeys ApiKey => ApiKeys.DeleteRecords;

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
    /// Each topic that we want to delete records from.
    /// </summary>
    public List<DeleteRecordsTopicMessage> Topics { get; set; } = new ();

    /// <summary>
    /// How long to wait for the deletion to complete, in milliseconds.
    /// </summary>
    public int TimeoutMs { get; set; } = 0;

    /// <summary>
    /// The basic constructor of the message DeleteRecordsRequestMessage
    /// </summary>
    public DeleteRecordsRequestMessage()
    {
    }

    /// <summary>
    /// Base constructor for deserializing message DeleteRecordsRequestMessage
    /// </summary>
    public DeleteRecordsRequestMessage(ref BufferReader reader, ApiVersion version)
        : this()
    {
        IncomingBufferLength = reader.Length;
        Read(ref reader, version);
    }

    /// <inheritdoc />
    public void Read(ref BufferReader reader, ApiVersion version)
    {
        {
            if (version >= ApiVersion.Version2)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new List<DeleteRecordsTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DeleteRecordsTopicMessage(ref reader, version));
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
                    var newCollection = new List<DeleteRecordsTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DeleteRecordsTopicMessage(ref reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        TimeoutMs = reader.ReadInt();
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
        if (version >= ApiVersion.Version2)
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
        writer.WriteInt(TimeoutMs);
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
        return ReferenceEquals(this, obj) || obj is DeleteRecordsRequestMessage other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(DeleteRecordsRequestMessage? other)
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
        if (TimeoutMs != other.TimeoutMs)
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Topics, TimeoutMs);
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "DeleteRecordsRequestMessage("
            + "Topics=" + Topics.DeepToString()
            + ", TimeoutMs=" + TimeoutMs
            + ")";
    }

    /// <summary>
    /// Describes the contract for message DeleteRecordsTopicMessage
    /// </summary>
    public sealed partial class DeleteRecordsTopicMessage: IMessage, IEquatable<DeleteRecordsTopicMessage>
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
        /// Each partition that we want to delete records from.
        /// </summary>
        public List<DeleteRecordsPartitionMessage> Partitions { get; set; } = new ();

        /// <summary>
        /// The basic constructor of the message DeleteRecordsTopicMessage
        /// </summary>
        public DeleteRecordsTopicMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message DeleteRecordsTopicMessage
        /// </summary>
        public DeleteRecordsTopicMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version2)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DeleteRecordsTopicMessage");
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
                if (version >= ApiVersion.Version2)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DeleteRecordsPartitionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DeleteRecordsPartitionMessage(ref reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DeleteRecordsPartitionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DeleteRecordsPartitionMessage(ref reader, version));
                        }
                        Partitions = newCollection;
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
                writer.WriteVarUInt(Partitions.Count + 1);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Partitions.Count);
                foreach (var element in Partitions)
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
            return ReferenceEquals(this, obj) || obj is DeleteRecordsTopicMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(DeleteRecordsTopicMessage? other)
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
            hashCode = HashCode.Combine(hashCode, Name, Partitions);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "DeleteRecordsTopicMessage("
                + "Name=" + (string.IsNullOrWhiteSpace(Name) ? "null" : Name)
                + ", Partitions=" + Partitions.DeepToString()
                + ")";
        }
    }

    /// <summary>
    /// Describes the contract for message DeleteRecordsPartitionMessage
    /// </summary>
    public sealed partial class DeleteRecordsPartitionMessage: IMessage, IEquatable<DeleteRecordsPartitionMessage>
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
        /// The deletion offset.
        /// </summary>
        public long Offset { get; set; } = 0;

        /// <summary>
        /// The basic constructor of the message DeleteRecordsPartitionMessage
        /// </summary>
        public DeleteRecordsPartitionMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message DeleteRecordsPartitionMessage
        /// </summary>
        public DeleteRecordsPartitionMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version2)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DeleteRecordsPartitionMessage");
            }
            PartitionIndex = reader.ReadInt();
            Offset = reader.ReadLong();
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
            writer.WriteInt(PartitionIndex);
            writer.WriteLong(Offset);
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
            return ReferenceEquals(this, obj) || obj is DeleteRecordsPartitionMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(DeleteRecordsPartitionMessage? other)
        {
            if (other is null)
            {
                return false;
            }
            if (PartitionIndex != other.PartitionIndex)
            {
                return false;
            }
            if (Offset != other.Offset)
            {
                return false;
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex, Offset);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "DeleteRecordsPartitionMessage("
                + "PartitionIndex=" + PartitionIndex
                + ", Offset=" + Offset
                + ")";
        }
    }
}
