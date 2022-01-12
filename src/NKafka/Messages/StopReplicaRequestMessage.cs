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

public partial class StopReplicaRequestMessage: RequestMessage
{
    /// <summary>
    /// The controller id.
    /// </summary>
    public int ControllerId { get; set; }

    /// <summary>
    /// The controller epoch.
    /// </summary>
    public int ControllerEpoch { get; set; }

    /// <summary>
    /// The broker epoch.
    /// </summary>
    public long? BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// Whether these partitions should be deleted.
    /// </summary>
    public bool DeletePartitions { get; set; }

    /// <summary>
    /// The partitions to stop.
    /// </summary>
    public IReadOnlyCollection<StopReplicaPartitionV0Message> UngroupedPartitions { get; set; }

    /// <summary>
    /// The topics to stop.
    /// </summary>
    public IReadOnlyCollection<StopReplicaTopicV1Message> Topics { get; set; }

    /// <summary>
    /// Each topic.
    /// </summary>
    public IReadOnlyCollection<StopReplicaTopicStateMessage> TopicStates { get; set; }

    public StopReplicaRequestMessage()
    {
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

    public class StopReplicaPartitionV0Message: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; }

        public StopReplicaPartitionV0Message()
        {
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
    public class StopReplicaTopicV1Message: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The partition indexes.
        /// </summary>
        public IReadOnlyCollection<int> PartitionIndexes { get; set; }

        public StopReplicaTopicV1Message()
        {
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
    public class StopReplicaTopicStateMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// The state of each partition
        /// </summary>
        public IReadOnlyCollection<StopReplicaPartitionStateMessage> PartitionStates { get; set; }

        public StopReplicaTopicStateMessage()
        {
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
    public class StopReplicaPartitionStateMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        /// <summary>
        /// Whether this partition should be deleted.
        /// </summary>
        public bool DeletePartition { get; set; }

        public StopReplicaPartitionStateMessage()
        {
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