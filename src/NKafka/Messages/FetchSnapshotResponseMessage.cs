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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class FetchSnapshotResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top level response error code.
    /// </summary>
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// The topics to fetch.
    /// </summary>
    public List<TopicSnapshotMessage> TopicsMessage { get; set; } = new ();

    public FetchSnapshotResponseMessage()
    {
        ApiKey = ApiKeys.FetchSnapshot;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public FetchSnapshotResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.FetchSnapshot;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class TopicSnapshotMessage: Message
    {
        /// <summary>
        /// The name of the topic to fetch.
        /// </summary>
        public string NameMessage { get; set; } = "";

        /// <summary>
        /// The partitions to fetch.
        /// </summary>
        public List<PartitionSnapshotMessage> PartitionsMessage { get; set; } = new ();

        public TopicSnapshotMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public TopicSnapshotMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class PartitionSnapshotMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int IndexMessage { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no fetch error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The snapshot endOffset and epoch fetched
        /// </summary>
        public List<SnapshotId> SnapshotIdMessage { get; set; } = new ();

        /// <summary>
        /// 
        /// </summary>
        public List<LeaderIdAndEpoch> CurrentLeaderMessage { get; set; } = new ();

        /// <summary>
        /// The total size of the snapshot.
        /// </summary>
        public long SizeMessage { get; set; } = 0;

        /// <summary>
        /// The starting byte position within the snapshot included in the Bytes field.
        /// </summary>
        public long PositionMessage { get; set; } = 0;

        /// <summary>
        /// Snapshot data in records format which may not be aligned on an offset boundary
        /// </summary>
        public RecordBatch UnalignedRecordsMessage { get; set; } = null;

        public PartitionSnapshotMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public PartitionSnapshotMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class SnapshotIdMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public long EndOffsetMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int EpochMessage { get; set; } = 0;

        public SnapshotIdMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public SnapshotIdMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class LeaderIdAndEpochMessage: Message
    {
        /// <summary>
        /// The ID of the current leader or -1 if the leader is unknown.
        /// </summary>
        public int LeaderIdMessage { get; set; } = 0;

        /// <summary>
        /// The latest known leader epoch
        /// </summary>
        public int LeaderEpochMessage { get; set; } = 0;

        public LeaderIdAndEpochMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public LeaderIdAndEpochMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }
}
