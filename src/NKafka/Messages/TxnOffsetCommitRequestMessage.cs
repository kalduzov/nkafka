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

public sealed class TxnOffsetCommitRequestMessage: RequestMessage
{
    /// <summary>
    /// The ID of the transaction.
    /// </summary>
    public string TransactionalId { get; set; } = "";

    /// <summary>
    /// The ID of the group.
    /// </summary>
    public string GroupId { get; set; } = "";

    /// <summary>
    /// The current producer ID in use by the transactional ID.
    /// </summary>
    public long ProducerId { get; set; } = 0;

    /// <summary>
    /// The current epoch associated with the producer ID.
    /// </summary>
    public short ProducerEpoch { get; set; } = 0;

    /// <summary>
    /// The generation of the consumer.
    /// </summary>
    public int GenerationId { get; set; } = -1;

    /// <summary>
    /// The member ID assigned by the group coordinator.
    /// </summary>
    public string MemberId { get; set; } = "";

    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string GroupInstanceId { get; set; } = null;

    /// <summary>
    /// Each topic that we want to commit offsets for.
    /// </summary>
    public List<TxnOffsetCommitRequestTopicMessage> Topics { get; set; } = new ();

    public TxnOffsetCommitRequestMessage()
    {
        ApiKey = ApiKeys.TxnOffsetCommit;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public TxnOffsetCommitRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.TxnOffsetCommit;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed class TxnOffsetCommitRequestTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The partitions inside the topic that we want to committ offsets for.
        /// </summary>
        public List<TxnOffsetCommitRequestPartitionMessage> Partitions { get; set; } = new ();

        public TxnOffsetCommitRequestTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public TxnOffsetCommitRequestTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class TxnOffsetCommitRequestPartitionMessage: Message
    {
        /// <summary>
        /// The index of the partition within the topic.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The message offset to be committed.
        /// </summary>
        public long CommittedOffset { get; set; } = 0;

        /// <summary>
        /// The leader epoch of the last consumed record.
        /// </summary>
        public int CommittedLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// Any associated metadata the client wants to keep.
        /// </summary>
        public string CommittedMetadata { get; set; } = "";

        public TxnOffsetCommitRequestPartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public TxnOffsetCommitRequestPartitionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}