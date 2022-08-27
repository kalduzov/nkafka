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

public sealed class FetchResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top level response error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// The fetch session ID, or 0 if this is not part of a fetch session.
    /// </summary>
    public int SessionId { get; set; } = 0;

    /// <summary>
    /// The response topics.
    /// </summary>
    public List<FetchableTopicResponseMessage> Responses { get; set; } = new ();

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

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersions.Version7)
        {
            writer.WriteShort(ErrorCode);
        }
        if (version >= ApiVersions.Version7)
        {
            writer.WriteInt(SessionId);
        }
        else
        {
            if (SessionId != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default SessionId at version {version}");
            }
        }
        if (version >= ApiVersions.Version12)
        {
            writer.WriteVarUInt(Responses.Count + 1);
            foreach (var element in Responses)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Responses.Count);
            foreach (var element in Responses)
            {
                element.Write(writer, version);
            }
        }
    }

    public sealed class FetchableTopicResponseMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Topic { get; set; } = "";

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The topic partitions.
        /// </summary>
        public List<PartitionDataMessage> Partitions { get; set; } = new ();

        public FetchableTopicResponseMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public FetchableTopicResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            if (version <= ApiVersions.Version12)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Topic);
                    if (version >= ApiVersions.Version12)
                    {
                        writer.WriteVarUInt(stringBytes.Length + 1);
                    }
                    else
                    {
                        writer.WriteShort((short)stringBytes.Length);
                    }
                    writer.WriteBytes(stringBytes);
                }
            }
            if (version >= ApiVersions.Version13)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersions.Version12)
            {
                writer.WriteVarUInt(Partitions.Count + 1);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Partitions.Count);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
        }
    }

    public sealed class PartitionDataMessage: Message
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
        public long LastStableOffset { get; set; } = -1;

        /// <summary>
        /// The current log start offset.
        /// </summary>
        public long LogStartOffset { get; set; } = -1;

        /// <summary>
        /// In case divergence is detected based on the `LastFetchedEpoch` and `FetchOffset` in the request, this field indicates the largest epoch and its end offset such that subsequent records are known to diverge
        /// </summary>
        public EpochEndOffsetMessage DivergingEpoch { get; set; } = new ();

        /// <summary>
        /// 
        /// </summary>
        public LeaderIdAndEpochMessage CurrentLeader { get; set; } = new ();

        /// <summary>
        /// In the case of fetching an offset less than the LogStartOffset, this is the end offset and epoch that should be used in the FetchSnapshot request.
        /// </summary>
        public SnapshotIdMessage SnapshotId { get; set; } = new ();

        /// <summary>
        /// The aborted transactions.
        /// </summary>
        public List<AbortedTransactionMessage> AbortedTransactions { get; set; } = new ();

        /// <summary>
        /// The preferred read replica for the consumer to use on its next fetch request
        /// </summary>
        public int PreferredReadReplica { get; set; } = -1;

        /// <summary>
        /// The record data.
        /// </summary>
        public RecordBatch? Records { get; set; } = null;

        public PartitionDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public PartitionDataMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteShort(ErrorCode);
            writer.WriteLong(HighWatermark);
            if (version >= ApiVersions.Version4)
            {
                writer.WriteLong(LastStableOffset);
            }
            if (version >= ApiVersions.Version5)
            {
                writer.WriteLong(LogStartOffset);
            }
            if (version >= ApiVersions.Version12)
            {
                if (DivergingEpoch.Equals(new ()))
                {
                    numTaggedFields++;
                }
            }
            else
            {
                if (DivergingEpoch.Equals(new ()))
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default DivergingEpoch at version {version}");
                }
            }
            if (version >= ApiVersions.Version12)
            {
                if (CurrentLeader.Equals(new ()))
                {
                    numTaggedFields++;
                }
            }
            else
            {
                if (CurrentLeader.Equals(new ()))
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default CurrentLeader at version {version}");
                }
            }
            if (version >= ApiVersions.Version12)
            {
                if (SnapshotId.Equals(new ()))
                {
                    numTaggedFields++;
                }
            }
            else
            {
                if (SnapshotId.Equals(new ()))
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default SnapshotId at version {version}");
                }
            }
            if (version >= ApiVersions.Version4)
            {
                if (version >= ApiVersions.Version12)
                {
                    if (AbortedTransactions is null)
                    {
                        writer.WriteVarUInt(0);
                    }
                    else
                    {
                        writer.WriteVarUInt(AbortedTransactions.Count + 1);
                        foreach (var element in AbortedTransactions)
                        {
                            element.Write(writer, version);
                        }
                    }
                }
                else
                {
                    if (AbortedTransactions is null)
                    {
                        writer.WriteInt(-1);
                    }
                    else
                    {
                        writer.WriteInt(AbortedTransactions.Count);
                        foreach (var element in AbortedTransactions)
                        {
                            element.Write(writer, version);
                        }
                    }
                }
            }
            if (version >= ApiVersions.Version11)
            {
                writer.WriteInt(PreferredReadReplica);
            }
            else
            {
                if (PreferredReadReplica != -1)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default PreferredReadReplica at version {version}");
                }
            }
            if (Records is null)
            {
                if (version >= ApiVersions.Version12)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteInt(-1);
                }
            }
            else
            {
                if (version >= ApiVersions.Version12)
                {
                    writer.WriteVarUInt(Records.Length + 1);
                }
                else
                {
                    writer.WriteInt(Records.Length);
                }
                writer.WriteRecords(Records);
            }
        }
    }

    public sealed class EpochEndOffsetMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public int Epoch { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public long EndOffset { get; set; } = -1;

        public EpochEndOffsetMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public EpochEndOffsetMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version12)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of EpochEndOffsetMessage");
            }
            var numTaggedFields = 0;
            writer.WriteInt(Epoch);
            writer.WriteLong(EndOffset);
        }
    }

    public sealed class LeaderIdAndEpochMessage: Message
    {
        /// <summary>
        /// The ID of the current leader or -1 if the leader is unknown.
        /// </summary>
        public int LeaderId { get; set; } = -1;

        /// <summary>
        /// The latest known leader epoch
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        public LeaderIdAndEpochMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public LeaderIdAndEpochMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version12)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of LeaderIdAndEpochMessage");
            }
            var numTaggedFields = 0;
            writer.WriteInt(LeaderId);
            writer.WriteInt(LeaderEpoch);
        }
    }

    public sealed class SnapshotIdMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public long EndOffset { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public int Epoch { get; set; } = -1;

        public SnapshotIdMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public SnapshotIdMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version12)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of SnapshotIdMessage");
            }
            var numTaggedFields = 0;
            writer.WriteLong(EndOffset);
            writer.WriteInt(Epoch);
        }
    }

    public sealed class AbortedTransactionMessage: Message
    {
        /// <summary>
        /// The producer id associated with the aborted transaction.
        /// </summary>
        public long ProducerId { get; set; } = 0;

        /// <summary>
        /// The first offset in the aborted transaction.
        /// </summary>
        public long FirstOffset { get; set; } = 0;

        public AbortedTransactionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public AbortedTransactionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version4)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of AbortedTransactionMessage");
            }
            var numTaggedFields = 0;
            writer.WriteLong(ProducerId);
            writer.WriteLong(FirstOffset);
        }
    }
}
