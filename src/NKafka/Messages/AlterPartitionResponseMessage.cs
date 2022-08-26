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

public sealed partial class AlterPartitionResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top level response error code
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public List<TopicDataMessage> Topics { get; set; } = new();

    public AlterPartitionResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public AlterPartitionResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed partial class TopicDataMessage: Message
    {
        /// <summary>
        /// The name of the topic
        /// </summary>
        public string? TopicName { get; set; } = null!;

        /// <summary>
        /// The ID of the topic
        /// </summary>
        public Guid? TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// 
        /// </summary>
        public List<PartitionDataMessage> Partitions { get; set; } = new();

        public TopicDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public TopicDataMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public sealed partial class PartitionDataMessage: Message
    {
        /// <summary>
        /// The partition index
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The partition level error code
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The broker ID of the leader.
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        /// <summary>
        /// The in-sync replica IDs.
        /// </summary>
        public List<int> Isr { get; set; } = new();

        /// <summary>
        /// 1 if the partition is recovering from an unclean leader election; 0 otherwise.
        /// </summary>
        public sbyte? LeaderRecoveryState { get; set; } = 0;

        /// <summary>
        /// The current epoch for the partition for KRaft controllers. The current ZK version for the legacy controllers.
        /// </summary>
        public int PartitionEpoch { get; set; } = 0;

        public PartitionDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public PartitionDataMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}