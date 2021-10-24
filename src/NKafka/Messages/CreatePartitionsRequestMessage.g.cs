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

public sealed partial class CreatePartitionsRequestMessage: IRequestMessage, IEquatable<CreatePartitionsRequestMessage>
{
    /// <inheritdoc />
    public ApiKeys ApiKey => ApiKeys.CreatePartitions;

    public const bool ONLY_CONTROLLER = true;

    /// <inheritdoc />
    public bool OnlyController => ONLY_CONTROLLER;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// Each topic that we want to create new partitions inside.
    /// </summary>
    public CreatePartitionsTopicCollection Topics { get; set; } = new ();

    /// <summary>
    /// The time in ms to wait for the partitions to be created.
    /// </summary>
    public int TimeoutMs { get; set; } = 0;

    /// <summary>
    /// If true, then validate the request, but don't actually increase the number of partitions.
    /// </summary>
    public bool ValidateOnly { get; set; } = false;

    public CreatePartitionsRequestMessage()
    {
    }

    public CreatePartitionsRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
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
                    var newCollection = new CreatePartitionsTopicCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new CreatePartitionsTopicMessage(reader, version));
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
                    var newCollection = new CreatePartitionsTopicCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new CreatePartitionsTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        TimeoutMs = reader.ReadInt();
        ValidateOnly = reader.ReadByte() != 0;
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
        writer.WriteBool(ValidateOnly);
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

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is CreatePartitionsRequestMessage other && Equals(other);
    }

    public bool Equals(CreatePartitionsRequestMessage? other)
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
        if (ValidateOnly != other.ValidateOnly)
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Topics, TimeoutMs, ValidateOnly);
        return hashCode;
    }

    public override string ToString()
    {
        return "CreatePartitionsRequestMessage("
            + "Topics=" + Topics.DeepToString()
            + ", TimeoutMs=" + TimeoutMs
            + ", ValidateOnly=" + (ValidateOnly ? "true" : "false")
            + ")";
    }

    public sealed partial class CreatePartitionsTopicMessage: IMessage, IEquatable<CreatePartitionsTopicMessage>
    {
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The new partition count.
        /// </summary>
        public int Count { get; set; } = 0;

        /// <summary>
        /// The new partition assignments.
        /// </summary>
        public List<CreatePartitionsAssignmentMessage> Assignments { get; set; } = new ();

        public CreatePartitionsTopicMessage()
        {
        }

        public CreatePartitionsTopicMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of CreatePartitionsTopicMessage");
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
            Count = reader.ReadInt();
            {
                if (version >= ApiVersion.Version2)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        Assignments = null;
                    }
                    else
                    {
                        var newCollection = new List<CreatePartitionsAssignmentMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new CreatePartitionsAssignmentMessage(reader, version));
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
                        Assignments = null;
                    }
                    else
                    {
                        var newCollection = new List<CreatePartitionsAssignmentMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new CreatePartitionsAssignmentMessage(reader, version));
                        }
                        Assignments = newCollection;
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
            writer.WriteInt(Count);
            if (version >= ApiVersion.Version2)
            {
                if (Assignments is null)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteVarUInt(Assignments.Count + 1);
                    foreach (var element in Assignments)
                    {
                        element.Write(writer, version);
                    }
                }
            }
            else
            {
                if (Assignments is null)
                {
                    writer.WriteInt(-1);
                }
                else
                {
                    writer.WriteInt(Assignments.Count);
                    foreach (var element in Assignments)
                    {
                        element.Write(writer, version);
                    }
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

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is CreatePartitionsTopicMessage other && Equals(other);
        }

        public bool Equals(CreatePartitionsTopicMessage? other)
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
            if (Count != other.Count)
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
            return "CreatePartitionsTopicMessage("
                + "Name=" + (string.IsNullOrWhiteSpace(Name) ? "null" : Name)
                + ", Count=" + Count
                + ", Assignments=" + (Assignments is null ? "null" : Assignments.DeepToString())
                + ")";
        }
    }

    public sealed partial class CreatePartitionsAssignmentMessage: IMessage, IEquatable<CreatePartitionsAssignmentMessage>
    {
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The assigned broker IDs.
        /// </summary>
        public List<int> BrokerIds { get; set; } = new ();

        public CreatePartitionsAssignmentMessage()
        {
        }

        public CreatePartitionsAssignmentMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of CreatePartitionsAssignmentMessage");
            }
            {
                int arrayLength;
                if (version >= ApiVersion.Version2)
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            if (version >= ApiVersion.Version2)
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

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is CreatePartitionsAssignmentMessage other && Equals(other);
        }

        public bool Equals(CreatePartitionsAssignmentMessage? other)
        {
            if (other is null)
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
            hashCode = HashCode.Combine(hashCode, BrokerIds);
            return hashCode;
        }

        public override string ToString()
        {
            return "CreatePartitionsAssignmentMessage("
                + "BrokerIds=" + BrokerIds.DeepToString()
                + ")";
        }
    }

    public sealed partial class CreatePartitionsTopicCollection: HashSet<CreatePartitionsTopicMessage>
    {
        public CreatePartitionsTopicCollection()
        {
        }

        public CreatePartitionsTopicCollection(int capacity)
            : base(capacity)
        {
        }
        public override bool Equals(object? obj)
        {
            return SetEquals((IEnumerable<CreatePartitionsTopicMessage>)obj);
        }
    }
}