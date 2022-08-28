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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class TxnOffsetCommitRequestMessage: RequestMessage, IEquatable<TxnOffsetCommitRequestMessage>
{
    /// <summary>
    /// The ID of the transaction.
    /// </summary>
    public string TransactionalId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the group.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

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
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string? GroupInstanceId { get; set; } = null;

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
        var numTaggedFields = 0;
        {
            var stringBytes = Encoding.UTF8.GetBytes(TransactionalId);
            if (version >= ApiVersions.Version3)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        {
            var stringBytes = Encoding.UTF8.GetBytes(GroupId);
            if (version >= ApiVersions.Version3)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        writer.WriteLong(ProducerId);
        writer.WriteShort(ProducerEpoch);
        if (version >= ApiVersions.Version3)
        {
            writer.WriteInt(GenerationId);
        }
        else
        {
            if (GenerationId != -1)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default GenerationId at version {version}");
            }
        }
        if (version >= ApiVersions.Version3)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(MemberId);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        else
        {
            if (!MemberId.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default MemberId at version {version}");
            }
        }
        if (version >= ApiVersions.Version3)
        {
            if (GroupInstanceId is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(GroupInstanceId);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        else
        {
            if (GroupInstanceId is not null)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default GroupInstanceId at version {version}");
            }
        }
        if (version >= ApiVersions.Version3)
        {
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Topics.Count);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version3)
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
        return ReferenceEquals(this, obj) || obj is TxnOffsetCommitRequestMessage other && Equals(other);
    }

    public bool Equals(TxnOffsetCommitRequestMessage? other)
    {
        return true;
    }

    public sealed class TxnOffsetCommitRequestTopicMessage: Message, IEquatable<TxnOffsetCommitRequestTopicMessage>
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

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
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersions.Version3)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version3)
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
            if (version >= ApiVersions.Version3)
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
            return ReferenceEquals(this, obj) || obj is TxnOffsetCommitRequestTopicMessage other && Equals(other);
        }

        public bool Equals(TxnOffsetCommitRequestTopicMessage? other)
        {
            return true;
        }
    }

    public sealed class TxnOffsetCommitRequestPartitionMessage: Message, IEquatable<TxnOffsetCommitRequestPartitionMessage>
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
        public string CommittedMetadata { get; set; } = string.Empty;

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
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteLong(CommittedOffset);
            if (version >= ApiVersions.Version2)
            {
                writer.WriteInt(CommittedLeaderEpoch);
            }
            if (CommittedMetadata is null)
            {
                if (version >= ApiVersions.Version3)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteShort(-1);
                }
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(CommittedMetadata);
                if (version >= ApiVersions.Version3)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version3)
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
            return ReferenceEquals(this, obj) || obj is TxnOffsetCommitRequestPartitionMessage other && Equals(other);
        }

        public bool Equals(TxnOffsetCommitRequestPartitionMessage? other)
        {
            return true;
        }
    }
}
