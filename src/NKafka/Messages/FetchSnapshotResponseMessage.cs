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

public sealed class FetchSnapshotResponseMessage: ResponseMessage, IEquatable<FetchSnapshotResponseMessage>
{
    /// <summary>
    /// The top level response error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// The topics to fetch.
    /// </summary>
    public List<TopicSnapshotMessage> Topics { get; set; } = new ();

    public FetchSnapshotResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public FetchSnapshotResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteShort(ErrorCode);
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

    public sealed class TopicSnapshotMessage: Message, IEquatable<TopicSnapshotMessage>
    {
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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public TopicSnapshotMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
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
    }

    public sealed class PartitionSnapshotMessage: Message, IEquatable<PartitionSnapshotMessage>
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no fetch error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public PartitionSnapshotMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(Index);
            writer.WriteShort(ErrorCode);
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
    }

    public sealed class SnapshotIdMessage: Message, IEquatable<SnapshotIdMessage>
    {
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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public SnapshotIdMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
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
    }

    public sealed class LeaderIdAndEpochMessage: Message, IEquatable<LeaderIdAndEpochMessage>
    {
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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public LeaderIdAndEpochMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
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
    }
}
