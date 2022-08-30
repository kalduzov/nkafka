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

public sealed class FetchSnapshotRequestMessage: IRequestMessage, IEquatable<FetchSnapshotRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.FetchSnapshot;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The clusterId if known, this is used to validate metadata fetches prior to broker registration
    /// </summary>
    public string? ClusterId { get; set; } = null;

    /// <summary>
    /// The broker ID of the follower
    /// </summary>
    public int ReplicaId { get; set; } = -1;

    /// <summary>
    /// The maximum bytes to fetch from all of the snapshots
    /// </summary>
    public int MaxBytes { get; set; } = 2147483647;

    /// <summary>
    /// The topics to fetch
    /// </summary>
    public List<TopicSnapshotMessage> Topics { get; set; } = new ();

    public FetchSnapshotRequestMessage()
    {
    }

    public FetchSnapshotRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            ClusterId = null;
        }
        ReplicaId = reader.ReadInt();
        MaxBytes = reader.ReadInt();
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
                case 0:
                {
                    int length;
                    length = reader.ReadVarUInt() - 1;
                    if (length < 0)
                    {
                        ClusterId = null;
                    }
                    else if (length > 0x7fff)
                    {
                        throw new Exception($"string field ClusterId had invalid length {length}");
                    }
                    else
                    {
                        ClusterId = reader.ReadString(length);
                    }
                    break;
                }
                default:
                    UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                    break;
            }
        }
    }

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        if (ClusterId is not null)
        {
            numTaggedFields++;
        }
        writer.WriteInt(ReplicaId);
        writer.WriteInt(MaxBytes);
        writer.WriteVarUInt(Topics.Count + 1);
        foreach (var element in Topics)
        {
            element.Write(writer, version);
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        if (ClusterId is not null)
        {
            writer.WriteVarUInt(0);
            var stringBytes = Encoding.UTF8.GetBytes(ClusterId);
            writer.WriteVarUInt(stringBytes.Length + (stringBytes.Length + 1).SizeOfVarUInt());
            writer.WriteVarUInt(stringBytes.Length + 1);
            writer.WriteBytes(stringBytes);
        }
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is FetchSnapshotRequestMessage other && Equals(other);
    }

    public bool Equals(FetchSnapshotRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ClusterId, ReplicaId, MaxBytes, Topics);
        return hashCode;
    }

    public override string ToString()
    {
        return "FetchSnapshotRequestMessage("
            + ", ReplicaId=" + ReplicaId
            + ", MaxBytes=" + MaxBytes
            + ")";
    }

    public sealed class TopicSnapshotMessage: IMessage, IEquatable<TopicSnapshotMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the topic to fetch
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The partitions to fetch
        /// </summary>
        public List<PartitionSnapshotMessage> Partitions { get; set; } = new ();

        public TopicSnapshotMessage()
        {
        }

        public TopicSnapshotMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
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

        public void Write(BufferWriter writer, ApiVersion version)
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
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index
        /// </summary>
        public int Partition { get; set; } = 0;

        /// <summary>
        /// The current leader epoch of the partition, -1 for unknown leader epoch
        /// </summary>
        public int CurrentLeaderEpoch { get; set; } = 0;

        /// <summary>
        /// The snapshot endOffset and epoch to fetch
        /// </summary>
        public SnapshotIdMessage SnapshotId { get; set; } = new ();

        /// <summary>
        /// The byte position within the snapshot to start fetching from
        /// </summary>
        public long Position { get; set; } = 0;

        public PartitionSnapshotMessage()
        {
        }

        public PartitionSnapshotMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of PartitionSnapshotMessage");
            }
            Partition = reader.ReadInt();
            CurrentLeaderEpoch = reader.ReadInt();
            {
                SnapshotId = new SnapshotIdMessage(reader, version);
            }
            Position = reader.ReadLong();
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
            writer.WriteInt(Partition);
            writer.WriteInt(CurrentLeaderEpoch);
            SnapshotId.Write(writer, version);
            writer.WriteLong(Position);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
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
            hashCode = HashCode.Combine(hashCode, Partition, CurrentLeaderEpoch, SnapshotId, Position);
            return hashCode;
        }

        public override string ToString()
        {
            return "PartitionSnapshotMessage("
                + "Partition=" + Partition
                + ", CurrentLeaderEpoch=" + CurrentLeaderEpoch
                + ", Position=" + Position
                + ")";
        }
    }

    public sealed class SnapshotIdMessage: IMessage, IEquatable<SnapshotIdMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

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

        public SnapshotIdMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
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

        public void Write(BufferWriter writer, ApiVersion version)
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
}
