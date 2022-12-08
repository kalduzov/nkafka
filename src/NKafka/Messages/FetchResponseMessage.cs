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

// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using System.Text;

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;

namespace NKafka.Messages;

public sealed class FetchResponseMessage: IResponseMessage, IEquatable<FetchResponseMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version13;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The top level response error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The fetch session ID, or 0 if this is not part of a fetch session.
    /// </summary>
    public int SessionId { get; set; } = 0;

    /// <summary>
    /// The response topics.
    /// </summary>
    public List<FetchableTopicResponseMessage> Responses { get; set; } = new();

    public FetchResponseMessage()
    {
    }

    public FetchResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        if (version >= ApiVersion.Version1)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        if (version >= ApiVersion.Version7)
        {
            ErrorCode = reader.ReadShort();
        }
        else
        {
            ErrorCode = 0;
        }
        if (version >= ApiVersion.Version7)
        {
            SessionId = reader.ReadInt();
        }
        else
        {
            SessionId = 0;
        }
        {
            if (version >= ApiVersion.Version12)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Responses was serialized as null");
                }
                else
                {
                    var newCollection = new List<FetchableTopicResponseMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new FetchableTopicResponseMessage(reader, version));
                    }
                    Responses = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Responses was serialized as null");
                }
                else
                {
                    var newCollection = new List<FetchableTopicResponseMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new FetchableTopicResponseMessage(reader, version));
                    }
                    Responses = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version12)
        {
            var numTaggedFields = reader.ReadVarUInt();
            for (var t = 0; t < numTaggedFields; t++)
            {
                var tag = reader.ReadVarUInt();
                var size = reader.ReadVarUInt();
                switch (tag)
                {
                    default:
                        UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                        break;
                }
            }
        }
    }

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersion.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersion.Version7)
        {
            writer.WriteShort((short)ErrorCode);
        }
        if (version >= ApiVersion.Version7)
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
        if (version >= ApiVersion.Version12)
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
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version12)
        {
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }
        else
        {
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is FetchResponseMessage other && Equals(other);
    }

    public bool Equals(FetchResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, SessionId, Responses);
        return hashCode;
    }

    public override string ToString()
    {
        return "FetchResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
            + ", SessionId=" + SessionId
            + ")";
    }

    public sealed class FetchableTopicResponseMessage: IMessage, IEquatable<FetchableTopicResponseMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version13;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The topic partitions.
        /// </summary>
        public List<PartitionDataMessage> Partitions { get; set; } = new();

        public FetchableTopicResponseMessage()
        {
        }

        public FetchableTopicResponseMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version13)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of FetchableTopicResponseMessage");
            }
            if (version <= ApiVersion.Version12)
            {
                int length;
                if (version >= ApiVersion.Version12)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Topic was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Topic had invalid length {length}");
                }
                else
                {
                    Topic = reader.ReadString(length);
                }
            }
            else
            {
                Topic = string.Empty;
            }
            if (version >= ApiVersion.Version13)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            {
                if (version >= ApiVersion.Version12)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<PartitionDataMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new PartitionDataMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<PartitionDataMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new PartitionDataMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version12)
            {
                var numTaggedFields = reader.ReadVarUInt();
                for (var t = 0; t < numTaggedFields; t++)
                {
                    var tag = reader.ReadVarUInt();
                    var size = reader.ReadVarUInt();
                    switch (tag)
                    {
                        default:
                            UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                            break;
                    }
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            if (version <= ApiVersion.Version12)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Topic);
                    if (version >= ApiVersion.Version12)
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
            if (version >= ApiVersion.Version13)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersion.Version12)
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
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version12)
            {
                writer.WriteVarUInt(numTaggedFields);
                rawWriter.WriteRawTags(writer, int.MaxValue);
            }
            else
            {
                if (numTaggedFields > 0)
                {
                    throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
                }
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is FetchableTopicResponseMessage other && Equals(other);
        }

        public bool Equals(FetchableTopicResponseMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Topic, TopicId, Partitions);
            return hashCode;
        }

        public override string ToString()
        {
            return "FetchableTopicResponseMessage("
                + ", TopicId=" + TopicId
                + ")";
        }
    }

    public sealed class PartitionDataMessage: IMessage, IEquatable<PartitionDataMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version13;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no fetch error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

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
        public EpochEndOffsetMessage DivergingEpoch { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public LeaderIdAndEpochMessage CurrentLeader { get; set; } = new();

        /// <summary>
        /// In the case of fetching an offset less than the LogStartOffset, this is the end offset and epoch that should be used in the FetchSnapshot request.
        /// </summary>
        public SnapshotIdMessage SnapshotId { get; set; } = new();

        /// <summary>
        /// The aborted transactions.
        /// </summary>
        public List<AbortedTransactionMessage> AbortedTransactions { get; set; } = new();

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
        }

        public PartitionDataMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version13)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of PartitionDataMessage");
            }
            PartitionIndex = reader.ReadInt();
            ErrorCode = reader.ReadShort();
            HighWatermark = reader.ReadLong();
            if (version >= ApiVersion.Version4)
            {
                LastStableOffset = reader.ReadLong();
            }
            else
            {
                LastStableOffset = -1;
            }
            if (version >= ApiVersion.Version5)
            {
                LogStartOffset = reader.ReadLong();
            }
            else
            {
                LogStartOffset = -1;
            }
            {
                DivergingEpoch = new();
            }
            {
                CurrentLeader = new();
            }
            {
                SnapshotId = new();
            }
            if (version >= ApiVersion.Version4)
            {
                if (version >= ApiVersion.Version12)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        AbortedTransactions = null;
                    }
                    else
                    {
                        var newCollection = new List<AbortedTransactionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new AbortedTransactionMessage(reader, version));
                        }
                        AbortedTransactions = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        AbortedTransactions = null;
                    }
                    else
                    {
                        var newCollection = new List<AbortedTransactionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new AbortedTransactionMessage(reader, version));
                        }
                        AbortedTransactions = newCollection;
                    }
                }
            }
            else
            {
                AbortedTransactions = new();
            }
            if (version >= ApiVersion.Version11)
            {
                PreferredReadReplica = reader.ReadInt();
            }
            else
            {
                PreferredReadReplica = -1;
            }
            {
                int length;
                if (version >= ApiVersion.Version12)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadInt();
                }
                if (length < 0)
                {
                    Records = null;
                }
                else
                {
                    Records = reader.ReadRecords(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version12)
            {
                var numTaggedFields = reader.ReadVarUInt();
                for (var t = 0; t < numTaggedFields; t++)
                {
                    var tag = reader.ReadVarUInt();
                    var size = reader.ReadVarUInt();
                    switch (tag)
                    {
                        case 0:
                            {
                                DivergingEpoch = new EpochEndOffsetMessage(reader, version);
                                break;
                            }
                        case 1:
                            {
                                CurrentLeader = new LeaderIdAndEpochMessage(reader, version);
                                break;
                            }
                        case 2:
                            {
                                SnapshotId = new SnapshotIdMessage(reader, version);
                                break;
                            }
                        default:
                            UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                            break;
                    }
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteShort((short)ErrorCode);
            writer.WriteLong(HighWatermark);
            if (version >= ApiVersion.Version4)
            {
                writer.WriteLong(LastStableOffset);
            }
            if (version >= ApiVersion.Version5)
            {
                writer.WriteLong(LogStartOffset);
            }
            if (version >= ApiVersion.Version12)
            {
                if (!DivergingEpoch.Equals(new()))
                {
                    numTaggedFields++;
                }
            }
            else
            {
                if (!DivergingEpoch.Equals(new()))
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default DivergingEpoch at version {version}");
                }
            }
            if (version >= ApiVersion.Version12)
            {
                if (!CurrentLeader.Equals(new()))
                {
                    numTaggedFields++;
                }
            }
            else
            {
                if (!CurrentLeader.Equals(new()))
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default CurrentLeader at version {version}");
                }
            }
            if (version >= ApiVersion.Version12)
            {
                if (!SnapshotId.Equals(new()))
                {
                    numTaggedFields++;
                }
            }
            else
            {
                if (!SnapshotId.Equals(new()))
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default SnapshotId at version {version}");
                }
            }
            if (version >= ApiVersion.Version4)
            {
                if (version >= ApiVersion.Version12)
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
            if (version >= ApiVersion.Version11)
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
                if (version >= ApiVersion.Version12)
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
                if (version >= ApiVersion.Version12)
                {
                    writer.WriteVarUInt(Records.Length + 1);
                }
                else
                {
                    writer.WriteInt(Records.Length);
                }
                writer.WriteRecords(Records);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version12)
            {
                writer.WriteVarUInt(numTaggedFields);
                {
                    if (!DivergingEpoch.Equals(new()))
                    {
                        writer.WriteVarUInt(0);
                        DivergingEpoch.Write(writer, version);
                    }
                }
                {
                    if (!CurrentLeader.Equals(new()))
                    {
                        writer.WriteVarUInt(1);
                        CurrentLeader.Write(writer, version);
                    }
                }
                {
                    if (!SnapshotId.Equals(new()))
                    {
                        writer.WriteVarUInt(2);
                        SnapshotId.Write(writer, version);
                    }
                }
                rawWriter.WriteRawTags(writer, int.MaxValue);
            }
            else
            {
                if (numTaggedFields > 0)
                {
                    throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
                }
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is PartitionDataMessage other && Equals(other);
        }

        public bool Equals(PartitionDataMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex, ErrorCode, HighWatermark, LastStableOffset, LogStartOffset, DivergingEpoch, CurrentLeader);
            hashCode = HashCode.Combine(hashCode, SnapshotId, AbortedTransactions, PreferredReadReplica, Records);
            return hashCode;
        }

        public override string ToString()
        {
            return "PartitionDataMessage("
                + "PartitionIndex=" + PartitionIndex
                + ", ErrorCode=" + ErrorCode
                + ", HighWatermark=" + HighWatermark
                + ", LastStableOffset=" + LastStableOffset
                + ", LogStartOffset=" + LogStartOffset
                + ", PreferredReadReplica=" + PreferredReadReplica
                + ")";
        }
    }

    public sealed class EpochEndOffsetMessage: IMessage, IEquatable<EpochEndOffsetMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version13;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

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
        }

        public EpochEndOffsetMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version13)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of EpochEndOffsetMessage");
            }
            Epoch = reader.ReadInt();
            EndOffset = reader.ReadLong();
            UnknownTaggedFields = null;
            var numTaggedFields = reader.ReadVarUInt();
            for (var t = 0; t < numTaggedFields; t++)
            {
                var tag = reader.ReadVarUInt();
                var size = reader.ReadVarUInt();
                switch (tag)
                {
                    default:
                        UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                        break;
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersion version)
        {
            if (version < ApiVersion.Version12)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of EpochEndOffsetMessage");
            }
            var numTaggedFields = 0;
            writer.WriteInt(Epoch);
            writer.WriteLong(EndOffset);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is EpochEndOffsetMessage other && Equals(other);
        }

        public bool Equals(EpochEndOffsetMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Epoch, EndOffset);
            return hashCode;
        }

        public override string ToString()
        {
            return "EpochEndOffsetMessage("
                + "Epoch=" + Epoch
                + ", EndOffset=" + EndOffset
                + ")";
        }
    }

    public sealed class LeaderIdAndEpochMessage: IMessage, IEquatable<LeaderIdAndEpochMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version13;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

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
        }

        public LeaderIdAndEpochMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version13)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of LeaderIdAndEpochMessage");
            }
            LeaderId = reader.ReadInt();
            LeaderEpoch = reader.ReadInt();
            UnknownTaggedFields = null;
            var numTaggedFields = reader.ReadVarUInt();
            for (var t = 0; t < numTaggedFields; t++)
            {
                var tag = reader.ReadVarUInt();
                var size = reader.ReadVarUInt();
                switch (tag)
                {
                    default:
                        UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                        break;
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersion version)
        {
            if (version < ApiVersion.Version12)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of LeaderIdAndEpochMessage");
            }
            var numTaggedFields = 0;
            writer.WriteInt(LeaderId);
            writer.WriteInt(LeaderEpoch);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is LeaderIdAndEpochMessage other && Equals(other);
        }

        public bool Equals(LeaderIdAndEpochMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, LeaderId, LeaderEpoch);
            return hashCode;
        }

        public override string ToString()
        {
            return "LeaderIdAndEpochMessage("
                + "LeaderId=" + LeaderId
                + ", LeaderEpoch=" + LeaderEpoch
                + ")";
        }
    }

    public sealed class SnapshotIdMessage: IMessage, IEquatable<SnapshotIdMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version13;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

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
        }

        public SnapshotIdMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version13)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of SnapshotIdMessage");
            }
            EndOffset = reader.ReadLong();
            Epoch = reader.ReadInt();
            UnknownTaggedFields = null;
            var numTaggedFields = reader.ReadVarUInt();
            for (var t = 0; t < numTaggedFields; t++)
            {
                var tag = reader.ReadVarUInt();
                var size = reader.ReadVarUInt();
                switch (tag)
                {
                    default:
                        UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                        break;
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersion version)
        {
            if (version < ApiVersion.Version12)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of SnapshotIdMessage");
            }
            var numTaggedFields = 0;
            writer.WriteLong(EndOffset);
            writer.WriteInt(Epoch);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is SnapshotIdMessage other && Equals(other);
        }

        public bool Equals(SnapshotIdMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, EndOffset, Epoch);
            return hashCode;
        }

        public override string ToString()
        {
            return "SnapshotIdMessage("
                + "EndOffset=" + EndOffset
                + ", Epoch=" + Epoch
                + ")";
        }
    }

    public sealed class AbortedTransactionMessage: IMessage, IEquatable<AbortedTransactionMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version13;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

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
        }

        public AbortedTransactionMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version13)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of AbortedTransactionMessage");
            }
            ProducerId = reader.ReadLong();
            FirstOffset = reader.ReadLong();
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version12)
            {
                var numTaggedFields = reader.ReadVarUInt();
                for (var t = 0; t < numTaggedFields; t++)
                {
                    var tag = reader.ReadVarUInt();
                    var size = reader.ReadVarUInt();
                    switch (tag)
                    {
                        default:
                            UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                            break;
                    }
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersion version)
        {
            if (version < ApiVersion.Version4)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of AbortedTransactionMessage");
            }
            var numTaggedFields = 0;
            writer.WriteLong(ProducerId);
            writer.WriteLong(FirstOffset);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version12)
            {
                writer.WriteVarUInt(numTaggedFields);
                rawWriter.WriteRawTags(writer, int.MaxValue);
            }
            else
            {
                if (numTaggedFields > 0)
                {
                    throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
                }
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is AbortedTransactionMessage other && Equals(other);
        }

        public bool Equals(AbortedTransactionMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ProducerId, FirstOffset);
            return hashCode;
        }

        public override string ToString()
        {
            return "AbortedTransactionMessage("
                + "ProducerId=" + ProducerId
                + ", FirstOffset=" + FirstOffset
                + ")";
        }
    }
}