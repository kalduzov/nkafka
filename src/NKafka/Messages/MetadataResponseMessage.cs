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

public sealed class MetadataResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each broker in the response.
    /// </summary>
    public List<MetadataResponseBrokerMessage> BrokersMessage { get; set; } = new ();

    /// <summary>
    /// The cluster ID that responding broker belongs to.
    /// </summary>
    public string? ClusterIdMessage { get; set; } = null;

    /// <summary>
    /// The ID of the controller broker.
    /// </summary>
    public int ControllerIdMessage { get; set; } = -1;

    /// <summary>
    /// Each topic in the response.
    /// </summary>
    public List<MetadataResponseTopicMessage> TopicsMessage { get; set; } = new ();

    /// <summary>
    /// 32-bit bitfield to represent authorized operations for this cluster.
    /// </summary>
    public int ClusterAuthorizedOperationsMessage { get; set; } = -2147483648;

    public MetadataResponseMessage()
    {
        ApiKey = ApiKeys.Metadata;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version12;
    }

    public MetadataResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.Metadata;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version12;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class MetadataResponseBrokerMessage: Message
    {
        /// <summary>
        /// The broker ID.
        /// </summary>
        public Dictionary<int,> NodeIdMessage { get; set; } = 0;

        /// <summary>
        /// The broker hostname.
        /// </summary>
        public Dictionary<string,> HostMessage { get; set; } = "";

        /// <summary>
        /// The broker port.
        /// </summary>
        public Dictionary<int,> PortMessage { get; set; } = 0;

        /// <summary>
        /// The rack of the broker, or null if it has not been assigned to a rack.
        /// </summary>
        public Dictionary<string,>? RackMessage { get; set; } = null;

        public MetadataResponseBrokerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public MetadataResponseBrokerMessage(BufferReader reader, ApiVersions version)
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

    public sealed class MetadataResponseTopicMessage: Message
    {
        /// <summary>
        /// The topic error, or 0 if there was no error.
        /// </summary>
        public Dictionary<short,> ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The topic name.
        /// </summary>
        public Dictionary<string,> NameMessage { get; set; } = "";

        /// <summary>
        /// The topic id.
        /// </summary>
        public Dictionary<Guid,> TopicIdMessage { get; set; } = Guid.Empty;

        /// <summary>
        /// True if the topic is internal.
        /// </summary>
        public Dictionary<bool,> IsInternalMessage { get; set; } = false;

        /// <summary>
        /// Each partition in the topic.
        /// </summary>
        public List<MetadataResponsePartitionMessage> PartitionsMessage { get; set; } = new ();

        /// <summary>
        /// 32-bit bitfield to represent authorized operations for this topic.
        /// </summary>
        public Dictionary<int,> TopicAuthorizedOperationsMessage { get; set; } = -2147483648;

        public MetadataResponseTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public MetadataResponseTopicMessage(BufferReader reader, ApiVersions version)
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

    public sealed class MetadataResponsePartitionMessage: Message
    {
        /// <summary>
        /// The partition error, or 0 if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndexMessage { get; set; } = 0;

        /// <summary>
        /// The ID of the leader broker.
        /// </summary>
        public int LeaderIdMessage { get; set; } = 0;

        /// <summary>
        /// The leader epoch of this partition.
        /// </summary>
        public int LeaderEpochMessage { get; set; } = -1;

        /// <summary>
        /// The set of all nodes that host this partition.
        /// </summary>
        public List<int> ReplicaNodesMessage { get; set; } = new ();

        /// <summary>
        /// The set of nodes that are in sync with the leader for this partition.
        /// </summary>
        public List<int> IsrNodesMessage { get; set; } = new ();

        /// <summary>
        /// The set of offline replicas of this partition.
        /// </summary>
        public List<int> OfflineReplicasMessage { get; set; } = new ();

        public MetadataResponsePartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public MetadataResponsePartitionMessage(BufferReader reader, ApiVersions version)
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
