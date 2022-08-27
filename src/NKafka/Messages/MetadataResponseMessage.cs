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

public sealed class MetadataResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each broker in the response.
    /// </summary>
    public MetadataResponseBrokerCollection Brokers { get; set; } = new ();

    /// <summary>
    /// The cluster ID that responding broker belongs to.
    /// </summary>
    public string? ClusterId { get; set; } = null;

    /// <summary>
    /// The ID of the controller broker.
    /// </summary>
    public int ControllerId { get; set; } = -1;

    /// <summary>
    /// Each topic in the response.
    /// </summary>
    public MetadataResponseTopicCollection Topics { get; set; } = new ();

    /// <summary>
    /// 32-bit bitfield to represent authorized operations for this cluster.
    /// </summary>
    public int ClusterAuthorizedOperations { get; set; } = -2147483648;

    public MetadataResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version12;
    }

    public MetadataResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version12;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed class MetadataResponseBrokerMessage: Message
    {
        /// <summary>
        /// The broker ID.
        /// </summary>
        public int NodeId { get; set; } = 0;

        /// <summary>
        /// The broker hostname.
        /// </summary>
        public string Host { get; set; } = "";

        /// <summary>
        /// The broker port.
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>
        /// The rack of the broker, or null if it has not been assigned to a rack.
        /// </summary>
        public string? Rack { get; set; } = null;

        public MetadataResponseBrokerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        public MetadataResponseBrokerMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class MetadataResponseBrokerCollection: HashSet<MetadataResponseBrokerMessage>
    {
        public MetadataResponseBrokerCollection()
        {
        }

        public MetadataResponseBrokerCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class MetadataResponseTopicMessage: Message
    {
        /// <summary>
        /// The topic error, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The topic id.
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// True if the topic is internal.
        /// </summary>
        public bool IsInternal { get; set; } = false;

        /// <summary>
        /// Each partition in the topic.
        /// </summary>
        public List<MetadataResponsePartitionMessage> Partitions { get; set; } = new ();

        /// <summary>
        /// 32-bit bitfield to represent authorized operations for this topic.
        /// </summary>
        public int TopicAuthorizedOperations { get; set; } = -2147483648;

        public MetadataResponseTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        public MetadataResponseTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class MetadataResponsePartitionMessage: Message
    {
        /// <summary>
        /// The partition error, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The ID of the leader broker.
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The leader epoch of this partition.
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The set of all nodes that host this partition.
        /// </summary>
        public List<int> ReplicaNodes { get; set; } = new ();

        /// <summary>
        /// The set of nodes that are in sync with the leader for this partition.
        /// </summary>
        public List<int> IsrNodes { get; set; } = new ();

        /// <summary>
        /// The set of offline replicas of this partition.
        /// </summary>
        public List<int> OfflineReplicas { get; set; } = new ();

        public MetadataResponsePartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        public MetadataResponsePartitionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class MetadataResponseTopicCollection: HashSet<MetadataResponseTopicMessage>
    {
        public MetadataResponseTopicCollection()
        {
        }

        public MetadataResponseTopicCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
