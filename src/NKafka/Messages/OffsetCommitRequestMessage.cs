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

public sealed partial class OffsetCommitRequestMessage: RequestMessage
{
    /// <summary>
    /// The unique group identifier.
    /// </summary>
    public string GroupId { get; set; } = null!;

    /// <summary>
    /// The generation of the group.
    /// </summary>
    public int? GenerationId { get; set; } = -1;

    /// <summary>
    /// The member ID assigned by the group coordinator.
    /// </summary>
    public string? MemberId { get; set; } = null!;

    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string? GroupInstanceId { get; set; } = "null";

    /// <summary>
    /// The time period in ms to retain the offset.
    /// </summary>
    public long? RetentionTimeMs { get; set; } = -1;

    /// <summary>
    /// The topics to commit offsets for.
    /// </summary>
    public List<OffsetCommitRequestTopicMessage> Topics { get; set; } = new();

    public OffsetCommitRequestMessage()
    {
        ApiKey = ApiKeys.OffsetCommit;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version8;
    }

    public OffsetCommitRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.OffsetCommit;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version8;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed partial class OffsetCommitRequestTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Each partition to commit offsets for.
        /// </summary>
        public List<OffsetCommitRequestPartitionMessage> Partitions { get; set; } = new();

        public OffsetCommitRequestTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetCommitRequestTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public sealed partial class OffsetCommitRequestPartitionMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The message offset to be committed.
        /// </summary>
        public long CommittedOffset { get; set; } = 0;

        /// <summary>
        /// The leader epoch of this partition.
        /// </summary>
        public int? CommittedLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The timestamp of the commit.
        /// </summary>
        public long CommitTimestamp { get; set; } = -1;

        /// <summary>
        /// Any associated metadata the client wants to keep.
        /// </summary>
        public string CommittedMetadata { get; set; } = null!;

        public OffsetCommitRequestPartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetCommitRequestPartitionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}