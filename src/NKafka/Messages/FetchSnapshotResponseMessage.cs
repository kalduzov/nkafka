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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class FetchSnapshotResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top level response error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// The topics to fetch.
    /// </summary>
    public List<TopicSnapshot> Topics { get; set; } = new();

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

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class TopicSnapshot: Message
    {
        /// <summary>
        /// The name of the topic to fetch.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The partitions to fetch.
        /// </summary>
        public List<PartitionSnapshot> Partitions { get; set; } = new();

        public TopicSnapshot()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public TopicSnapshot(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class PartitionSnapshot: Message
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
        public SnapshotId SnapshotId { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public LeaderIdAndEpoch CurrentLeader { get; set; } = new();

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
        public RecordBatch UnalignedRecords { get; set; } = new();

        public PartitionSnapshot()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public PartitionSnapshot(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class SnapshotId: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public long EndOffset { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int Epoch { get; set; } = 0;

        public SnapshotId()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public SnapshotId(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class LeaderIdAndEpoch: Message
    {
        /// <summary>
        /// The ID of the current leader or -1 if the leader is unknown.
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The latest known leader epoch
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        public LeaderIdAndEpoch()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public LeaderIdAndEpoch(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}