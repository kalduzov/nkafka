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
public sealed partial class FetchResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top level response error code.
    /// </summary>
    public short? ErrorCode { get; set; } = 0;

    /// <summary>
    /// The fetch session ID, or 0 if this is not part of a fetch session.
    /// </summary>
    public int SessionId { get; set; } = 0;

    /// <summary>
    /// The response topics.
    /// </summary>
    public List<FetchableTopicResponse> Responses { get; set; } = new();

    public FetchResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version13;
    }

    public FetchResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version13;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class FetchableTopicResponse: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string? Topic { get; set; } = null!;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid? TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The topic partitions.
        /// </summary>
        public List<PartitionData> Partitions { get; set; } = new();

        public FetchableTopicResponse()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public FetchableTopicResponse(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class PartitionData: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no fetch error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The current high water mark.
        /// </summary>
        public long HighWatermark { get; set; } = 0;

        /// <summary>
        /// The last stable offset (or LSO) of the partition. This is the last offset such that the state of all transactional records prior to this offset have been decided (ABORTED or COMMITTED)
        /// </summary>
        public long? LastStableOffset { get; set; } = -1;

        /// <summary>
        /// The current log start offset.
        /// </summary>
        public long? LogStartOffset { get; set; } = -1;

        /// <summary>
        /// In case divergence is detected based on the `LastFetchedEpoch` and `FetchOffset` in the request, this field indicates the largest epoch and its end offset such that subsequent records are known to diverge
        /// </summary>
        public EpochEndOffset DivergingEpoch { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public LeaderIdAndEpoch CurrentLeader { get; set; } = new();

        /// <summary>
        /// In the case of fetching an offset less than the LogStartOffset, this is the end offset and epoch that should be used in the FetchSnapshot request.
        /// </summary>
        public SnapshotId SnapshotId { get; set; } = new();

        /// <summary>
        /// The aborted transactions.
        /// </summary>
        public List<AbortedTransaction>? AbortedTransactions { get; set; } = new();

        /// <summary>
        /// The preferred read replica for the consumer to use on its next fetch request
        /// </summary>
        public int PreferredReadReplica { get; set; } = -1;

        /// <summary>
        /// The record data.
        /// </summary>
        public RecordBatch Records { get; set; } = new();

        public PartitionData()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public PartitionData(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class EpochEndOffset: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public int Epoch { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public long EndOffset { get; set; } = -1;

        public EpochEndOffset()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public EpochEndOffset(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class LeaderIdAndEpoch: Message
    {
        /// <summary>
        /// The ID of the current leader or -1 if the leader is unknown.
        /// </summary>
        public int LeaderId { get; set; } = -1;

        /// <summary>
        /// The latest known leader epoch
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        public LeaderIdAndEpoch()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public LeaderIdAndEpoch(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class SnapshotId: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public long EndOffset { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public int Epoch { get; set; } = -1;

        public SnapshotId()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public SnapshotId(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class AbortedTransaction: Message
    {
        /// <summary>
        /// The producer id associated with the aborted transaction.
        /// </summary>
        public long ProducerId { get; set; } = 0;

        /// <summary>
        /// The first offset in the aborted transaction.
        /// </summary>
        public long FirstOffset { get; set; } = 0;

        public AbortedTransaction()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public AbortedTransaction(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}