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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public partial class FetchSnapshotRequestMessage: RequestMessage
{
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
    public int MaxBytes { get; set; } = 0x7fffffff;

    /// <summary>
    /// The topics to fetch
    /// </summary>
    public IReadOnlyCollection<TopicSnapshotMessage> Topics { get; set; }

    public FetchSnapshotRequestMessage()
    {
        ApiKey = ApiKeys.FetchSnapshot;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version0)
        {
        }
        else //no flexible version
        {
        }

    }

    public class TopicSnapshotMessage: Message
    {
        /// <summary>
        /// The name of the topic to fetch
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The partitions to fetch
        /// </summary>
        public IReadOnlyCollection<PartitionSnapshotMessage> Partitions { get; set; }

        public TopicSnapshotMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version0)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class PartitionSnapshotMessage: Message
    {
        /// <summary>
        /// The partition index
        /// </summary>
        public int Partition { get; set; }

        /// <summary>
        /// The current leader epoch of the partition, -1 for unknown leader epoch
        /// </summary>
        public int CurrentLeaderEpoch { get; set; }

        /// <summary>
        /// The snapshot endOffset and epoch to fetch
        /// </summary>
        public SnapshotIdMessage SnapshotId { get; set; }

        /// <summary>
        /// The byte position within the snapshot to start fetching from
        /// </summary>
        public long Position { get; set; }

        public PartitionSnapshotMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version0)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class SnapshotIdMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public long EndOffset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Epoch { get; set; }

        public SnapshotIdMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version0)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}