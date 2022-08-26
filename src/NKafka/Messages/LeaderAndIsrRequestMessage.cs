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

public sealed class LeaderAndIsrRequestMessage: RequestMessage
{
    /// <summary>
    /// The current controller ID.
    /// </summary>
    public int ControllerIdMessage { get; set; } = 0;

    /// <summary>
    /// The current controller epoch.
    /// </summary>
    public int ControllerEpochMessage { get; set; } = 0;

    /// <summary>
    /// The current broker epoch.
    /// </summary>
    public long BrokerEpochMessage { get; set; } = -1;

    /// <summary>
    /// The type that indicates whether all topics are included in the request
    /// </summary>
    public sbyte TypeMessage { get; set; } = 0;

    /// <summary>
    /// The state of each partition, in a v0 or v1 message.
    /// </summary>
    public List<LeaderAndIsrPartitionStateMessage> UngroupedPartitionStatesMessage { get; set; } = new ();

    /// <summary>
    /// Each topic.
    /// </summary>
    public List<LeaderAndIsrTopicStateMessage> TopicStatesMessage { get; set; } = new ();

    /// <summary>
    /// The current live leaders.
    /// </summary>
    public List<LeaderAndIsrLiveLeaderMessage> LiveLeadersMessage { get; set; } = new ();

    public LeaderAndIsrRequestMessage()
    {
        ApiKey = ApiKeys.LeaderAndIsr;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version6;
    }

    public LeaderAndIsrRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.LeaderAndIsr;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version6;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class LeaderAndIsrTopicStateMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicNameMessage { get; set; } = "";

        /// <summary>
        /// The unique topic ID.
        /// </summary>
        public Guid TopicIdMessage { get; set; } = Guid.Empty;

        /// <summary>
        /// The state of each partition
        /// </summary>
        public List<LeaderAndIsrPartitionStateMessage> PartitionStatesMessage { get; set; } = new ();

        public LeaderAndIsrTopicStateMessage()
        {
            LowestSupportedVersion = ApiVersions.Version2;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public LeaderAndIsrTopicStateMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version2;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class LeaderAndIsrLiveLeaderMessage: Message
    {
        /// <summary>
        /// The leader's broker ID.
        /// </summary>
        public int BrokerIdMessage { get; set; } = 0;

        /// <summary>
        /// The leader's hostname.
        /// </summary>
        public string HostNameMessage { get; set; } = "";

        /// <summary>
        /// The leader's port.
        /// </summary>
        public int PortMessage { get; set; } = 0;

        public LeaderAndIsrLiveLeaderMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public LeaderAndIsrLiveLeaderMessage(BufferReader reader, ApiVersions version)
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

    public sealed class LeaderAndIsrPartitionState: Message
    {
        /// <summary>
        /// The topic name.  This is only present in v0 or v1.
        /// </summary>
        public string TopicNameMessage { get; set; } = "";

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndexMessage { get; set; } = 0;

        /// <summary>
        /// The controller epoch.
        /// </summary>
        public int ControllerEpochMessage { get; set; } = 0;

        /// <summary>
        /// The broker ID of the leader.
        /// </summary>
        public int LeaderMessage { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int LeaderEpochMessage { get; set; } = 0;

        /// <summary>
        /// The in-sync replica IDs.
        /// </summary>
        public List<int> IsrMessage { get; set; } = new ();

        /// <summary>
        /// The current epoch for the partition. The epoch is a monotonically increasing value which is incremented after every partition change. (Since the LeaderAndIsr request is only used by the legacy controller, this corresponds to the zkVersion)
        /// </summary>
        public int PartitionEpochMessage { get; set; } = 0;

        /// <summary>
        /// The replica IDs.
        /// </summary>
        public List<int> ReplicasMessage { get; set; } = new ();

        /// <summary>
        /// The replica IDs that we are adding this partition to, or null if no replicas are being added.
        /// </summary>
        public List<int> AddingReplicasMessage { get; set; } = new ();

        /// <summary>
        /// The replica IDs that we are removing this partition from, or null if no replicas are being removed.
        /// </summary>
        public List<int> RemovingReplicasMessage { get; set; } = new ();

        /// <summary>
        /// Whether the replica should have existed on the broker or not.
        /// </summary>
        public bool IsNewMessage { get; set; } = false;

        /// <summary>
        /// 1 if the partition is recovering from an unclean leader election; 0 otherwise.
        /// </summary>
        public sbyte LeaderRecoveryStateMessage { get; set; } = 0;

        public LeaderAndIsrPartitionState()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public LeaderAndIsrPartitionState(BufferReader reader, ApiVersions version)
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
