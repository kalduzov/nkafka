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

public partial class OffsetCommitRequestMessage: RequestMessage
{
    /// <summary>
    /// The unique group identifier.
    /// </summary>
    public string GroupId { get; set; }

    /// <summary>
    /// The generation of the group.
    /// </summary>
    public int? GenerationId { get; set; } = -1;

    /// <summary>
    /// The member ID assigned by the group coordinator.
    /// </summary>
    public string? MemberId { get; set; }

    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string? GroupInstanceId { get; set; } = null;

    /// <summary>
    /// The time period in ms to retain the offset.
    /// </summary>
    public long? RetentionTimeMs { get; set; } = -1;

    /// <summary>
    /// The topics to commit offsets for.
    /// </summary>
    public IReadOnlyCollection<OffsetCommitRequestTopicMessage> Topics { get; set; }

    public OffsetCommitRequestMessage()
    {
        ApiKey = ApiKeys.OffsetCommit;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version8;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version8)
        {
        }
        else //no flexible version
        {
        }

    }

    public class OffsetCommitRequestTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Each partition to commit offsets for.
        /// </summary>
        public IReadOnlyCollection<OffsetCommitRequestPartitionMessage> Partitions { get; set; }

        public OffsetCommitRequestTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version8)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class OffsetCommitRequestPartitionMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The message offset to be committed.
        /// </summary>
        public long CommittedOffset { get; set; }

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
        public string CommittedMetadata { get; set; }

        public OffsetCommitRequestPartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version8)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}