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

public sealed class FetchSnapshotResponseMessage: IResponseMessage, IEquatable<FetchSnapshotResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The top level response error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The topics to fetch.
    /// </summary>
    public List<TopicSnapshotMessage> Topics { get; set; } = new ();

    public FetchSnapshotResponseMessage()
    {
    }

    public FetchSnapshotResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
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
                var newCollection = new List<TopicSnapshotMessage>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new TopicSnapshotMessage(reader, version));
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
        return ReferenceEquals(this, obj) || obj is FetchSnapshotResponseMessage other && Equals(other);
    }

    public bool Equals(FetchSnapshotResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, Topics);
        return hashCode;
    }

    public override string ToString()
    {
        return "FetchSnapshotResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
            + ")";
    }

    public sealed class TopicSnapshotMessage: IMessage, IEquatable<TopicSnapshotMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the topic to fetch.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The partitions to fetch.
        /// </summary>
        public List<PartitionSnapshotMessage> Partitions { get; set; } = new ();

        public TopicSnapshotMessage()
        {
        }

        public TopicSnapshotMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TopicSnapshotMessage");
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
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<PartitionSnapshotMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new PartitionSnapshotMessage(reader, version));
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
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
            return ReferenceEquals(this, obj) || obj is TopicSnapshotMessage other && Equals(other);
        }

        public bool Equals(TopicSnapshotMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name, Partitions);
            return hashCode;
        }

        public override string ToString()
        {
            return "TopicSnapshotMessage("
                + ")";
        }
    }

    public sealed class PartitionSnapshotMessage: IMessage, IEquatable<PartitionSnapshotMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no fetch error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The snapshot endOffset and epoch fetched
        /// </summary>
        public SnapshotIdMessage SnapshotId { get; set; } = new ();

        /// <summary>
        /// 
        /// </summary>
        public LeaderIdAndEpochMessage CurrentLeader { get; set; } = new ();

        /// <summary>
        /// The total size of the snapshot.
        /// </summary>
        public long Size { get; set; } = 0;

        /// <summary>
        /// The starting byte position within the snapshot included in the Bytes field.
        /// </summary>
        public long Position { get; set; } = 0;

        /// <summary>
        /// Snapshot data in records format which may not be aligned on an offset boundary
        /// </summary>
        public RecordBatch? UnalignedRecords { get; set; } = null;

        public PartitionSnapshotMessage()
        {
        }

        public PartitionSnapshotMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of PartitionSnapshotMessage");
            }
            Index = reader.ReadInt();
            ErrorCode = reader.ReadShort();
            {
                SnapshotId = new SnapshotIdMessage(reader, version);
            }
            {
                CurrentLeader = new ();
            }
            Size = reader.ReadLong();
            Position = reader.ReadLong();
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field UnalignedRecords was serialized as null");
                }
                else
                {
                    UnalignedRecords = reader.ReadRecords(length);
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
                    case 0:
                    {
                        CurrentLeader = new LeaderIdAndEpochMessage(reader, version);
                        break;
                    }
                    default:
                        UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                        break;
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(Index);
            writer.WriteShort((short)ErrorCode);
            SnapshotId.Write(writer, version);
            if (!CurrentLeader.Equals(new ()))
            {
                numTaggedFields++;
            }
            writer.WriteLong(Size);
            writer.WriteLong(Position);
            writer.WriteVarUInt(UnalignedRecords.Length + 1);
            writer.WriteRecords(UnalignedRecords);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            {
                if (!CurrentLeader.Equals(new ()))
                {
                    writer.WriteVarUInt(0);
                    CurrentLeader.Write(writer, version);
                }
            }
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is PartitionSnapshotMessage other && Equals(other);
        }

        public bool Equals(PartitionSnapshotMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Index, ErrorCode, SnapshotId, CurrentLeader, Size, Position, UnalignedRecords);
            return hashCode;
        }

        public override string ToString()
        {
            return "PartitionSnapshotMessage("
                + "Index=" + Index
                + ", ErrorCode=" + ErrorCode
                + ", Size=" + Size
                + ", Position=" + Position
                + ")";
        }
    }

    public sealed class SnapshotIdMessage: IMessage, IEquatable<SnapshotIdMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public long EndOffset { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int Epoch { get; set; } = 0;

        public SnapshotIdMessage()
        {
        }

        public SnapshotIdMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of SnapshotIdMessage");
            }
            EndOffset = reader.ReadLong();
            Epoch = reader.ReadInt();
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
            writer.WriteLong(EndOffset);
            writer.WriteInt(Epoch);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is SnapshotIdMessage other && Equals(other);
        }

        public bool Equals(SnapshotIdMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, EndOffset, Epoch);
            return hashCode;
        }

        public override string ToString()
        {
            return "SnapshotIdMessage("
                + "EndOffset=" + EndOffset
                + ", Epoch=" + Epoch
                + ")";
        }
    }

    public sealed class LeaderIdAndEpochMessage: IMessage, IEquatable<LeaderIdAndEpochMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The ID of the current leader or -1 if the leader is unknown.
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The latest known leader epoch
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        public LeaderIdAndEpochMessage()
        {
        }

        public LeaderIdAndEpochMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of LeaderIdAndEpochMessage");
            }
            LeaderId = reader.ReadInt();
            LeaderEpoch = reader.ReadInt();
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
            writer.WriteInt(LeaderId);
            writer.WriteInt(LeaderEpoch);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is LeaderIdAndEpochMessage other && Equals(other);
        }

        public bool Equals(LeaderIdAndEpochMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, LeaderId, LeaderEpoch);
            return hashCode;
        }

        public override string ToString()
        {
            return "LeaderIdAndEpochMessage("
                + "LeaderId=" + LeaderId
                + ", LeaderEpoch=" + LeaderEpoch
                + ")";
        }
    }
}
