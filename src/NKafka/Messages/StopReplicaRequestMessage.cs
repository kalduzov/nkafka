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
public sealed partial class StopReplicaRequestMessage: RequestMessage
{
    /// <summary>
    /// The controller id.
    /// </summary>
    public int ControllerId { get; set; } = 0;

    /// <summary>
    /// The controller epoch.
    /// </summary>
    public int ControllerEpoch { get; set; } = 0;

    /// <summary>
    /// The broker epoch.
    /// </summary>
    public long? BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// Whether these partitions should be deleted.
    /// </summary>
    public bool DeletePartitions { get; set; } = false;

    /// <summary>
    /// The partitions to stop.
    /// </summary>
    public List<StopReplicaPartitionV0> UngroupedPartitions { get; set; } = new();

    /// <summary>
    /// The topics to stop.
    /// </summary>
    public List<StopReplicaTopicV1> Topics { get; set; } = new();

    /// <summary>
    /// Each topic.
    /// </summary>
    public List<StopReplicaTopicState> TopicStates { get; set; } = new();

    public StopReplicaRequestMessage()
    {
        ApiKey = ApiKeys.StopReplica;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public StopReplicaRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.StopReplica;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class StopReplicaPartitionV0: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = null!;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        public StopReplicaPartitionV0()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public StopReplicaPartitionV0(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class StopReplicaTopicV1: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The partition indexes.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new();

        public StopReplicaTopicV1()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public StopReplicaTopicV1(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class StopReplicaTopicState: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = null!;

        /// <summary>
        /// The state of each partition
        /// </summary>
        public List<StopReplicaPartitionState> PartitionStates { get; set; } = new();

        public StopReplicaTopicState()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public StopReplicaTopicState(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class StopReplicaPartitionState: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        /// <summary>
        /// Whether this partition should be deleted.
        /// </summary>
        public bool DeletePartition { get; set; } = false;

        public StopReplicaPartitionState()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public StopReplicaPartitionState(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}