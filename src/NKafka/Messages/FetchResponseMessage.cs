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

public sealed class FetchResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top level response error code.
    /// </summary>
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// The fetch session ID, or 0 if this is not part of a fetch session.
    /// </summary>
    public int SessionIdMessage { get; set; } = 0;

    /// <summary>
    /// The response topics.
    /// </summary>
    public List<FetchableTopicResponseMessage> ResponsesMessage { get; set; } = new ();

    public FetchResponseMessage()
    {
        ApiKey = ApiKeys.Fetch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version13;
    }

    public FetchResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.Fetch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version13;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class FetchableTopicResponseMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicMessage { get; set; } = "";

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicIdMessage { get; set; } = Guid.Empty;

        /// <summary>
        /// The topic partitions.
        /// </summary>
        public List<PartitionDataMessage> PartitionsMessage { get; set; } = new ();

        public FetchableTopicResponseMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public FetchableTopicResponseMessage(BufferReader reader, ApiVersions version)
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

    public sealed class PartitionDataMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndexMessage { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no fetch error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The current high water mark.
        /// </summary>
        public long HighWatermarkMessage { get; set; } = 0;

        /// <summary>
        /// The last stable offset (or LSO) of the partition. This is the last offset such that the state of all transactional records prior to this offset have been decided (ABORTED or COMMITTED)
        /// </summary>
        public long LastStableOffsetMessage { get; set; } = -1;

        /// <summary>
        /// The current log start offset.
        /// </summary>
        public long LogStartOffsetMessage { get; set; } = -1;

        /// <summary>
        /// In case divergence is detected based on the `LastFetchedEpoch` and `FetchOffset` in the request, this field indicates the largest epoch and its end offset such that subsequent records are known to diverge
        /// </summary>
        public List<EpochEndOffset> DivergingEpochMessage { get; set; } = new ();

        /// <summary>
        /// 
        /// </summary>
        public List<LeaderIdAndEpoch> CurrentLeaderMessage { get; set; } = new ();

        /// <summary>
        /// In the case of fetching an offset less than the LogStartOffset, this is the end offset and epoch that should be used in the FetchSnapshot request.
        /// </summary>
        public List<SnapshotId> SnapshotIdMessage { get; set; } = new ();

        /// <summary>
        /// The aborted transactions.
        /// </summary>
        public List<AbortedTransactionMessage> AbortedTransactionsMessage { get; set; } = new ();

        /// <summary>
        /// The preferred read replica for the consumer to use on its next fetch request
        /// </summary>
        public int PreferredReadReplicaMessage { get; set; } = -1;

        /// <summary>
        /// The record data.
        /// </summary>
        public RecordBatch RecordsMessage { get; set; } = null;

        public PartitionDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public PartitionDataMessage(BufferReader reader, ApiVersions version)
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

    public sealed class EpochEndOffsetMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public int EpochMessage { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public long EndOffsetMessage { get; set; } = -1;

        public EpochEndOffsetMessage()
        {
            LowestSupportedVersion = ApiVersions.Version1;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public EpochEndOffsetMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version1;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class LeaderIdAndEpochMessage: Message
    {
        /// <summary>
        /// The ID of the current leader or -1 if the leader is unknown.
        /// </summary>
        public int LeaderIdMessage { get; set; } = -1;

        /// <summary>
        /// The latest known leader epoch
        /// </summary>
        public int LeaderEpochMessage { get; set; } = -1;

        public LeaderIdAndEpochMessage()
        {
            LowestSupportedVersion = ApiVersions.Version1;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public LeaderIdAndEpochMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version1;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class SnapshotIdMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public long EndOffsetMessage { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public int EpochMessage { get; set; } = -1;

        public SnapshotIdMessage()
        {
            LowestSupportedVersion = ApiVersions.Version1;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public SnapshotIdMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version1;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class AbortedTransactionMessage: Message
    {
        /// <summary>
        /// The producer id associated with the aborted transaction.
        /// </summary>
        public long ProducerIdMessage { get; set; } = 0;

        /// <summary>
        /// The first offset in the aborted transaction.
        /// </summary>
        public long FirstOffsetMessage { get; set; } = 0;

        public AbortedTransactionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version4;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public AbortedTransactionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version4;
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
