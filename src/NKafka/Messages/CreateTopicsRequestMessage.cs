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

public sealed class CreateTopicsRequestMessage: RequestMessage
{
    /// <summary>
    /// The topics to create.
    /// </summary>
    public CreatableTopicCollection Topics { get; set; } = new ();

    /// <summary>
    /// How long to wait in milliseconds before timing out the request.
    /// </summary>
    public int timeoutMs { get; set; } = 60000;

    /// <summary>
    /// If true, check that the topics can be created as specified, but don't create anything.
    /// </summary>
    public bool validateOnly { get; set; } = false;

    public CreateTopicsRequestMessage()
    {
        ApiKey = ApiKeys.CreateTopics;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public CreateTopicsRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.CreateTopics;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed class CreatableTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The number of partitions to create in the topic, or -1 if we are either specifying a manual partition assignment or using the default partitions.
        /// </summary>
        public int NumPartitions { get; set; } = 0;

        /// <summary>
        /// The number of replicas to create for each partition in the topic, or -1 if we are either specifying a manual partition assignment or using the default replication factor.
        /// </summary>
        public short ReplicationFactor { get; set; } = 0;

        /// <summary>
        /// The manual partition assignment, or the empty array if we are using automatic assignment.
        /// </summary>
        public CreatableReplicaAssignmentCollection Assignments { get; set; } = new ();

        /// <summary>
        /// The custom topic configurations to set.
        /// </summary>
        public CreateableTopicConfigCollection Configs { get; set; } = new ();

        public CreatableTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public CreatableTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class CreatableReplicaAssignmentMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The brokers to place the partition on.
        /// </summary>
        public List<int> BrokerIds { get; set; } = new ();

        public CreatableReplicaAssignmentMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public CreatableReplicaAssignmentMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class CreatableReplicaAssignmentCollection: HashSet<CreatableReplicaAssignmentMessage>
    {
        public CreatableReplicaAssignmentCollection()
        {
        }

        public CreatableReplicaAssignmentCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class CreateableTopicConfigMessage: Message
    {
        /// <summary>
        /// The configuration name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The configuration value.
        /// </summary>
        public string Value { get; set; } = "";

        public CreateableTopicConfigMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public CreateableTopicConfigMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class CreateableTopicConfigCollection: HashSet<CreateableTopicConfigMessage>
    {
        public CreateableTopicConfigCollection()
        {
        }

        public CreateableTopicConfigCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class CreatableTopicCollection: HashSet<CreatableTopicMessage>
    {
        public CreatableTopicCollection()
        {
        }

        public CreatableTopicCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
