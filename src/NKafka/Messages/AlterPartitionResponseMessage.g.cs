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

public sealed partial class AlterPartitionResponseMessage: IResponseMessage, IEquatable<AlterPartitionResponseMessage>
{
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The top level response error code
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// 
    /// </summary>
    public List<TopicDataMessage> Topics { get; set; } = new ();

    public AlterPartitionResponseMessage()
    {
    }

    public AlterPartitionResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ThrottleTimeMs = reader.ReadInt();
        ErrorCode = reader.ReadShort();
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Topics was serialized as null");
            }
            else
            {
                var newCollection = new List<TopicDataMessage>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new TopicDataMessage(reader, version));
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

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteShort((short)ErrorCode);
        writer.WriteVarUInt(Topics.Count + 1);
        foreach (var element in Topics)
        {
            element.Write(writer, version);
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is AlterPartitionResponseMessage other && Equals(other);
    }

    public bool Equals(AlterPartitionResponseMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ThrottleTimeMs != other.ThrottleTimeMs)
        {
            return false;
        }
        if (ErrorCode != other.ErrorCode)
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
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, Topics);
        return hashCode;
    }

    public override string ToString()
    {
        return "AlterPartitionResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
            + ", Topics=" + Topics.DeepToString()
            + ")";
    }

    public sealed partial class TopicDataMessage: IMessage, IEquatable<TopicDataMessage>
    {
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the topic
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the topic
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// 
        /// </summary>
        public List<PartitionDataMessage> Partitions { get; set; } = new ();

        public TopicDataMessage()
        {
        }

        public TopicDataMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version2)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TopicDataMessage");
            }
            if (version <= ApiVersion.Version1)
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field TopicName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field TopicName had invalid length {length}");
                }
                else
                {
                    TopicName = reader.ReadString(length);
                }
            }
            else
            {
                TopicName = string.Empty;
            }
            if (version >= ApiVersion.Version2)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<PartitionDataMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new PartitionDataMessage(reader, version));
                    }
                    Partitions = newCollection;
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            if (version <= ApiVersion.Version1)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(TopicName);
                    writer.WriteVarUInt(stringBytes.Length + 1);
                    writer.WriteBytes(stringBytes);
                }
            }
            if (version >= ApiVersion.Version2)
            {
                writer.WriteGuid(TopicId);
            }
            writer.WriteVarUInt(Partitions.Count + 1);
            foreach (var element in Partitions)
            {
                element.Write(writer, version);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is TopicDataMessage other && Equals(other);
        }

        public bool Equals(TopicDataMessage? other)
        {
            if (other is null)
            {
                return false;
            }
            if (TopicName is null)
            {
                if (other.TopicName is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!TopicName.Equals(other.TopicName))
                {
                    return false;
                }
            }
            if (!TopicId.Equals(other.TopicId))
            {
                return false;
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
            hashCode = HashCode.Combine(hashCode, TopicName, TopicId, Partitions);
            return hashCode;
        }

        public override string ToString()
        {
            return "TopicDataMessage("
                + "TopicName=" + (string.IsNullOrWhiteSpace(TopicName) ? "null" : TopicName)
                + ", TopicId=" + TopicId
                + ", Partitions=" + Partitions.DeepToString()
                + ")";
        }
    }

    public sealed partial class PartitionDataMessage: IMessage, IEquatable<PartitionDataMessage>
    {
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The partition level error code
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The broker ID of the leader.
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        /// <summary>
        /// The in-sync replica IDs.
        /// </summary>
        public List<int> Isr { get; set; } = new ();

        /// <summary>
        /// 1 if the partition is recovering from an unclean leader election; 0 otherwise.
        /// </summary>
        public sbyte LeaderRecoveryState { get; set; } = 0;

        /// <summary>
        /// The current epoch for the partition for KRaft controllers. The current ZK version for the legacy controllers.
        /// </summary>
        public int PartitionEpoch { get; set; } = 0;

        public PartitionDataMessage()
        {
        }

        public PartitionDataMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version2)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of PartitionDataMessage");
            }
            PartitionIndex = reader.ReadInt();
            ErrorCode = reader.ReadShort();
            LeaderId = reader.ReadInt();
            LeaderEpoch = reader.ReadInt();
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Isr was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Isr = newCollection;
                }
            }
            if (version >= ApiVersion.Version1)
            {
                LeaderRecoveryState = reader.ReadSByte();
            }
            else
            {
                LeaderRecoveryState = 0;
            }
            PartitionEpoch = reader.ReadInt();
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteShort((short)ErrorCode);
            writer.WriteInt(LeaderId);
            writer.WriteInt(LeaderEpoch);
            writer.WriteVarUInt(Isr.Count + 1);
            foreach (var element in Isr)
            {
                writer.WriteInt(element);
            }
            if (version >= ApiVersion.Version1)
            {
                writer.WriteSByte(LeaderRecoveryState);
            }
            writer.WriteInt(PartitionEpoch);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is PartitionDataMessage other && Equals(other);
        }

        public bool Equals(PartitionDataMessage? other)
        {
            if (other is null)
            {
                return false;
            }
            if (PartitionIndex != other.PartitionIndex)
            {
                return false;
            }
            if (ErrorCode != other.ErrorCode)
            {
                return false;
            }
            if (LeaderId != other.LeaderId)
            {
                return false;
            }
            if (LeaderEpoch != other.LeaderEpoch)
            {
                return false;
            }
            if (Isr is null)
            {
                if (other.Isr is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Isr.SequenceEqual(other.Isr))
                {
                    return false;
                }
            }
            if (LeaderRecoveryState != other.LeaderRecoveryState)
            {
                return false;
            }
            if (PartitionEpoch != other.PartitionEpoch)
            {
                return false;
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex, ErrorCode, LeaderId, LeaderEpoch, Isr, LeaderRecoveryState, PartitionEpoch);
            return hashCode;
        }

        public override string ToString()
        {
            return "PartitionDataMessage("
                + "PartitionIndex=" + PartitionIndex
                + ", ErrorCode=" + ErrorCode
                + ", LeaderId=" + LeaderId
                + ", LeaderEpoch=" + LeaderEpoch
                + ", Isr=" + Isr.DeepToString()
                + ", LeaderRecoveryState=" + LeaderRecoveryState
                + ", PartitionEpoch=" + PartitionEpoch
                + ")";
        }
    }
}
