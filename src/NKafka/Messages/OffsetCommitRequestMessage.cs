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

public sealed class OffsetCommitRequestMessage: IRequestMessage, IEquatable<OffsetCommitRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

    public ApiKeys ApiKey => ApiKeys.OffsetCommit;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The unique group identifier.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// The generation of the group.
    /// </summary>
    public int GenerationId { get; set; } = -1;

    /// <summary>
    /// The member ID assigned by the group coordinator.
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string? GroupInstanceId { get; set; } = null;

    /// <summary>
    /// The time period in ms to retain the offset.
    /// </summary>
    public long RetentionTimeMs { get; set; } = -1;

    /// <summary>
    /// The topics to commit offsets for.
    /// </summary>
    public List<OffsetCommitRequestTopicMessage> Topics { get; set; } = new ();

    public OffsetCommitRequestMessage()
    {
    }

    public OffsetCommitRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            int length;
            if (version >= ApiVersions.Version8)
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
        if (version >= ApiVersions.Version1)
        {
            GenerationId = reader.ReadInt();
        }
        else
        {
            GenerationId = -1;
        }
        if (version >= ApiVersions.Version1)
        {
            int length;
            if (version >= ApiVersions.Version8)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field MemberId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field MemberId had invalid length {length}");
            }
            else
            {
                MemberId = reader.ReadString(length);
            }
        }
        else
        {
            MemberId = string.Empty;
        }
        if (version >= ApiVersions.Version7)
        {
            int length;
            if (version >= ApiVersions.Version8)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                GroupInstanceId = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field GroupInstanceId had invalid length {length}");
            }
            else
            {
                GroupInstanceId = reader.ReadString(length);
            }
        }
        else
        {
            GroupInstanceId = null;
        }
        if (version >= ApiVersions.Version2 && version <= ApiVersions.Version4)
        {
            RetentionTimeMs = reader.ReadLong();
        }
        else
        {
            RetentionTimeMs = -1;
        }
        {
            if (version >= ApiVersions.Version8)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new List<OffsetCommitRequestTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new OffsetCommitRequestTopicMessage(reader, version));
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
                    var newCollection = new List<OffsetCommitRequestTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new OffsetCommitRequestTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version8)
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
            var stringBytes = Encoding.UTF8.GetBytes(GroupId);
            if (version >= ApiVersions.Version8)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(GenerationId);
        }
        if (version >= ApiVersions.Version1)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(MemberId);
                if (version >= ApiVersions.Version8)
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
        if (version >= ApiVersions.Version7)
        {
            if (GroupInstanceId is null)
            {
                if (version >= ApiVersions.Version8)
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
                var stringBytes = Encoding.UTF8.GetBytes(GroupInstanceId);
                if (version >= ApiVersions.Version8)
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
            if (GroupInstanceId is not null)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default GroupInstanceId at version {version}");
            }
        }
        if (version >= ApiVersions.Version2 && version <= ApiVersions.Version4)
        {
            writer.WriteLong(RetentionTimeMs);
        }
        if (version >= ApiVersions.Version8)
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
        if (version >= ApiVersions.Version8)
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
        return ReferenceEquals(this, obj) || obj is OffsetCommitRequestMessage other && Equals(other);
    }

    public bool Equals(OffsetCommitRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, GroupId, GenerationId, MemberId, GroupInstanceId, RetentionTimeMs, Topics);
        return hashCode;
    }

    public sealed class OffsetCommitRequestTopicMessage: IMessage, IEquatable<OffsetCommitRequestTopicMessage>
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
        /// Each partition to commit offsets for.
        /// </summary>
        public List<OffsetCommitRequestPartitionMessage> Partitions { get; set; } = new ();

        public OffsetCommitRequestTopicMessage()
        {
        }

        public OffsetCommitRequestTopicMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetCommitRequestTopicMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version8)
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
                if (version >= ApiVersions.Version8)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<OffsetCommitRequestPartitionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new OffsetCommitRequestPartitionMessage(reader, version));
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
                        var newCollection = new List<OffsetCommitRequestPartitionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new OffsetCommitRequestPartitionMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version8)
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
                if (version >= ApiVersions.Version8)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version8)
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
            if (version >= ApiVersions.Version8)
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
            return ReferenceEquals(this, obj) || obj is OffsetCommitRequestTopicMessage other && Equals(other);
        }

        public bool Equals(OffsetCommitRequestTopicMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name, Partitions);
            return hashCode;
        }
    }

    public sealed class OffsetCommitRequestPartitionMessage: IMessage, IEquatable<OffsetCommitRequestPartitionMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The message offset to be committed.
        /// </summary>
        public long CommittedOffset { get; set; } = 0;

        /// <summary>
        /// The leader epoch of this partition.
        /// </summary>
        public int CommittedLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The timestamp of the commit.
        /// </summary>
        public long CommitTimestamp { get; set; } = -1;

        /// <summary>
        /// Any associated metadata the client wants to keep.
        /// </summary>
        public string CommittedMetadata { get; set; } = string.Empty;

        public OffsetCommitRequestPartitionMessage()
        {
        }

        public OffsetCommitRequestPartitionMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetCommitRequestPartitionMessage");
            }
            PartitionIndex = reader.ReadInt();
            CommittedOffset = reader.ReadLong();
            if (version >= ApiVersions.Version6)
            {
                CommittedLeaderEpoch = reader.ReadInt();
            }
            else
            {
                CommittedLeaderEpoch = -1;
            }
            if (version >= ApiVersions.Version1 && version <= ApiVersions.Version1)
            {
                CommitTimestamp = reader.ReadLong();
            }
            else
            {
                CommitTimestamp = -1;
            }
            {
                int length;
                if (version >= ApiVersions.Version8)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    CommittedMetadata = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field CommittedMetadata had invalid length {length}");
                }
                else
                {
                    CommittedMetadata = reader.ReadString(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version8)
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
            writer.WriteLong(CommittedOffset);
            if (version >= ApiVersions.Version6)
            {
                writer.WriteInt(CommittedLeaderEpoch);
            }
            if (version >= ApiVersions.Version1 && version <= ApiVersions.Version1)
            {
                writer.WriteLong(CommitTimestamp);
            }
            else
            {
                if (CommitTimestamp != -1)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default CommitTimestamp at version {version}");
                }
            }
            if (CommittedMetadata is null)
            {
                if (version >= ApiVersions.Version8)
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
                var stringBytes = Encoding.UTF8.GetBytes(CommittedMetadata);
                if (version >= ApiVersions.Version8)
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
            if (version >= ApiVersions.Version8)
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
            return ReferenceEquals(this, obj) || obj is OffsetCommitRequestPartitionMessage other && Equals(other);
        }

        public bool Equals(OffsetCommitRequestPartitionMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex, CommittedOffset, CommittedLeaderEpoch, CommitTimestamp, CommittedMetadata);
            return hashCode;
        }
    }
}
