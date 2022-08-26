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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class FetchSnapshotRequestMessage: RequestMessage
{
    /// <summary>
    /// The clusterId if known, this is used to validate metadata fetches prior to broker registration
    /// </summary>
    public string ClusterId { get; set; } = null;

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
        ApiKey = ApiKeys.FetchSnapshot;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public FetchSnapshotRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.FetchSnapshot;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed class TopicSnapshotMessage: Message
    {
        /// <summary>
        /// The name of the topic to fetch
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The partitions to fetch
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
        }
    }

    public sealed class PartitionSnapshotMessage: Message
    {
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
        }
    }

    public sealed class SnapshotIdMessage: Message
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
        }
    }
}