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

public sealed class DescribeQuorumResponseMessage: IResponseMessage, IEquatable<DescribeQuorumResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version1;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The top level error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// 
    /// </summary>
    public List<TopicDataMessage> Topics { get; set; } = new ();

    public DescribeQuorumResponseMessage()
    {
    }

    public DescribeQuorumResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
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

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
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
        return ReferenceEquals(this, obj) || obj is DescribeQuorumResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeQuorumResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ErrorCode, Topics);
        return hashCode;
    }

    public override string ToString()
    {
        return "DescribeQuorumResponseMessage("
            + "ErrorCode=" + ErrorCode
            + ")";
    }

    public sealed class TopicDataMessage: IMessage, IEquatable<TopicDataMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version1;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public List<PartitionDataMessage> Partitions { get; set; } = new ();

        public TopicDataMessage()
        {
        }

        public TopicDataMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TopicDataMessage");
            }
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(TopicName);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
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
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, TopicName, Partitions);
            return hashCode;
        }

        public override string ToString()
        {
            return "TopicDataMessage("
                + ")";
        }
    }

    public sealed class PartitionDataMessage: IMessage, IEquatable<PartitionDataMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version1;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The ID of the current leader or -1 if the leader is unknown.
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The latest known leader epoch
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long HighWatermark { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public List<ReplicaStateMessage> CurrentVoters { get; set; } = new ();

        /// <summary>
        /// 
        /// </summary>
        public List<ReplicaStateMessage> Observers { get; set; } = new ();

        public PartitionDataMessage()
        {
        }

        public PartitionDataMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of PartitionDataMessage");
            }
            PartitionIndex = reader.ReadInt();
            ErrorCode = reader.ReadShort();
            LeaderId = reader.ReadInt();
            LeaderEpoch = reader.ReadInt();
            HighWatermark = reader.ReadLong();
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field CurrentVoters was serialized as null");
                }
                else
                {
                    var newCollection = new List<ReplicaStateMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new ReplicaStateMessage(reader, version));
                    }
                    CurrentVoters = newCollection;
                }
            }
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Observers was serialized as null");
                }
                else
                {
                    var newCollection = new List<ReplicaStateMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new ReplicaStateMessage(reader, version));
                    }
                    Observers = newCollection;
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
            writer.WriteInt(PartitionIndex);
            writer.WriteShort((short)ErrorCode);
            writer.WriteInt(LeaderId);
            writer.WriteInt(LeaderEpoch);
            writer.WriteLong(HighWatermark);
            writer.WriteVarUInt(CurrentVoters.Count + 1);
            foreach (var element in CurrentVoters)
            {
                element.Write(writer, version);
            }
            writer.WriteVarUInt(Observers.Count + 1);
            foreach (var element in Observers)
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
            return ReferenceEquals(this, obj) || obj is PartitionDataMessage other && Equals(other);
        }

        public bool Equals(PartitionDataMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex, ErrorCode, LeaderId, LeaderEpoch, HighWatermark, CurrentVoters, Observers);
            return hashCode;
        }

        public override string ToString()
        {
            return "PartitionDataMessage("
                + "PartitionIndex=" + PartitionIndex
                + ", ErrorCode=" + ErrorCode
                + ", LeaderId=" + LeaderId
                + ", LeaderEpoch=" + LeaderEpoch
                + ", HighWatermark=" + HighWatermark
                + ")";
        }
    }

    public sealed class ReplicaStateMessage: IMessage, IEquatable<ReplicaStateMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version1;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public int ReplicaId { get; set; } = 0;

        /// <summary>
        /// The last known log end offset of the follower or -1 if it is unknown
        /// </summary>
        public long LogEndOffset { get; set; } = 0;

        /// <summary>
        /// The last known leader wall clock time time when a follower fetched from the leader. This is reported as -1 both for the current leader or if it is unknown for a voter
        /// </summary>
        public long LastFetchTimestamp { get; set; } = -1;

        /// <summary>
        /// The leader wall clock append time of the offset for which the follower made the most recent fetch request. This is reported as the current time for the leader and -1 if unknown for a voter
        /// </summary>
        public long LastCaughtUpTimestamp { get; set; } = -1;

        public ReplicaStateMessage()
        {
        }

        public ReplicaStateMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of ReplicaStateMessage");
            }
            ReplicaId = reader.ReadInt();
            LogEndOffset = reader.ReadLong();
            if (version >= ApiVersions.Version1)
            {
                LastFetchTimestamp = reader.ReadLong();
            }
            else
            {
                LastFetchTimestamp = -1;
            }
            if (version >= ApiVersions.Version1)
            {
                LastCaughtUpTimestamp = reader.ReadLong();
            }
            else
            {
                LastCaughtUpTimestamp = -1;
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
            writer.WriteInt(ReplicaId);
            writer.WriteLong(LogEndOffset);
            if (version >= ApiVersions.Version1)
            {
                writer.WriteLong(LastFetchTimestamp);
            }
            if (version >= ApiVersions.Version1)
            {
                writer.WriteLong(LastCaughtUpTimestamp);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is ReplicaStateMessage other && Equals(other);
        }

        public bool Equals(ReplicaStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ReplicaId, LogEndOffset, LastFetchTimestamp, LastCaughtUpTimestamp);
            return hashCode;
        }

        public override string ToString()
        {
            return "ReplicaStateMessage("
                + "ReplicaId=" + ReplicaId
                + ", LogEndOffset=" + LogEndOffset
                + ", LastFetchTimestamp=" + LastFetchTimestamp
                + ", LastCaughtUpTimestamp=" + LastCaughtUpTimestamp
                + ")";
        }
    }
}
