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

public sealed partial class AddPartitionsToTxnRequestMessage: IRequestMessage, IEquatable<AddPartitionsToTxnRequestMessage>
{
    /// <inheritdoc />
    public ApiKeys ApiKey => ApiKeys.AddPartitionsToTxn;

    public const bool ONLY_CONTROLLER = false;

    /// <inheritdoc />
    public bool OnlyController => ONLY_CONTROLLER;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The transactional id corresponding to the transaction.
    /// </summary>
    public string TransactionalId { get; set; } = string.Empty;

    /// <summary>
    /// Current producer id in use by the transactional id.
    /// </summary>
    public long ProducerId { get; set; } = 0;

    /// <summary>
    /// Current epoch associated with the producer id.
    /// </summary>
    public short ProducerEpoch { get; set; } = 0;

    /// <summary>
    /// The partitions to add to the transaction.
    /// </summary>
    public AddPartitionsToTxnTopicCollection Topics { get; set; } = new ();

    public AddPartitionsToTxnRequestMessage()
    {
    }

    public AddPartitionsToTxnRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            int length;
            if (version >= ApiVersion.Version3)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field TransactionalId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field TransactionalId had invalid length {length}");
            }
            else
            {
                TransactionalId = reader.ReadString(length);
            }
        }
        ProducerId = reader.ReadLong();
        ProducerEpoch = reader.ReadShort();
        {
            if (version >= ApiVersion.Version3)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new AddPartitionsToTxnTopicCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new AddPartitionsToTxnTopicMessage(reader, version));
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
                    var newCollection = new AddPartitionsToTxnTopicCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new AddPartitionsToTxnTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version3)
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
            var stringBytes = Encoding.UTF8.GetBytes(TransactionalId);
            if (version >= ApiVersion.Version3)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        writer.WriteLong(ProducerId);
        writer.WriteShort(ProducerEpoch);
        if (version >= ApiVersion.Version3)
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
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version3)
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
        return ReferenceEquals(this, obj) || obj is AddPartitionsToTxnRequestMessage other && Equals(other);
    }

    public bool Equals(AddPartitionsToTxnRequestMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (TransactionalId is null)
        {
            if (other.TransactionalId is not null)
            {
                return false;
            }
        }
        else
        {
            if (!TransactionalId.Equals(other.TransactionalId))
            {
                return false;
            }
        }
        if (ProducerId != other.ProducerId)
        {
            return false;
        }
        if (ProducerEpoch != other.ProducerEpoch)
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
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, TransactionalId, ProducerId, ProducerEpoch, Topics);
        return hashCode;
    }

    public override string ToString()
    {
        return "AddPartitionsToTxnRequestMessage("
            + "TransactionalId=" + (string.IsNullOrWhiteSpace(TransactionalId) ? "null" : TransactionalId)
            + ", ProducerId=" + ProducerId
            + ", ProducerEpoch=" + ProducerEpoch
            + ", Topics=" + Topics.DeepToString()
            + ")";
    }

    public sealed partial class AddPartitionsToTxnTopicMessage: IMessage, IEquatable<AddPartitionsToTxnTopicMessage>
    {
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the topic.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The partition indexes to add to the transaction
        /// </summary>
        public List<int> Partitions { get; set; } = new ();

        public AddPartitionsToTxnTopicMessage()
        {
        }

        public AddPartitionsToTxnTopicMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of AddPartitionsToTxnTopicMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version3)
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
                if (version >= ApiVersion.Version3)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version3)
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
                if (version >= ApiVersion.Version3)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version3)
            {
                writer.WriteVarUInt(Partitions.Count + 1);
            }
            else
            {
                writer.WriteInt(Partitions.Count);
            }
            foreach (var element in Partitions)
            {
                writer.WriteInt(element);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version3)
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
            return ReferenceEquals(this, obj) || obj is AddPartitionsToTxnTopicMessage other && Equals(other);
        }

        public bool Equals(AddPartitionsToTxnTopicMessage? other)
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

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }

        public override string ToString()
        {
            return "AddPartitionsToTxnTopicMessage("
                + "Name=" + (string.IsNullOrWhiteSpace(Name) ? "null" : Name)
                + ", Partitions=" + Partitions.DeepToString()
                + ")";
        }
    }

    public sealed partial class AddPartitionsToTxnTopicCollection: HashSet<AddPartitionsToTxnTopicMessage>
    {
        public AddPartitionsToTxnTopicCollection()
        {
        }

        public AddPartitionsToTxnTopicCollection(int capacity)
            : base(capacity)
        {
        }
        public override bool Equals(object? obj)
        {
            return SetEquals((IEnumerable<AddPartitionsToTxnTopicMessage>)obj);
        }
    }
}
