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

public sealed class DescribeTransactionsResponseMessage: IResponseMessage, IEquatable<DescribeTransactionsResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public List<TransactionStateMessage> TransactionStates { get; set; } = new ();

    public DescribeTransactionsResponseMessage()
    {
    }

    public DescribeTransactionsResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        ThrottleTimeMs = reader.ReadInt();
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field TransactionStates was serialized as null");
            }
            else
            {
                var newCollection = new List<TransactionStateMessage>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new TransactionStateMessage(reader, version));
                }
                TransactionStates = newCollection;
            }
        }
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

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteVarUInt(TransactionStates.Count + 1);
        foreach (var element in TransactionStates)
        {
            element.Write(writer, version);
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is DescribeTransactionsResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeTransactionsResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, TransactionStates);
        return hashCode;
    }

    public override string ToString()
    {
        return "DescribeTransactionsResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }

    public sealed class TransactionStateMessage: IMessage, IEquatable<TransactionStateMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// 
        /// </summary>
        public string TransactionalId { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string TransactionState { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public int TransactionTimeoutMs { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long TransactionStartTimeMs { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long ProducerId { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public short ProducerEpoch { get; set; } = 0;

        /// <summary>
        /// The set of partitions included in the current transaction (if active). When a transaction is preparing to commit or abort, this will include only partitions which do not have markers.
        /// </summary>
        public TopicDataCollection Topics { get; set; } = new ();

        public TransactionStateMessage()
        {
        }

        public TransactionStateMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TransactionStateMessage");
            }
            ErrorCode = reader.ReadShort();
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field TransactionalId was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field TransactionalId had invalid length {length}");
                }
                else
                {
                    TransactionalId = reader.ReadString(length);
                }
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field TransactionState was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field TransactionState had invalid length {length}");
                }
                else
                {
                    TransactionState = reader.ReadString(length);
                }
            }
            TransactionTimeoutMs = reader.ReadInt();
            TransactionStartTimeMs = reader.ReadLong();
            ProducerId = reader.ReadLong();
            ProducerEpoch = reader.ReadShort();
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new TopicDataCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new TopicDataMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteShort((short)ErrorCode);
            {
                var stringBytes = Encoding.UTF8.GetBytes(TransactionalId);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            {
                var stringBytes = Encoding.UTF8.GetBytes(TransactionState);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteInt(TransactionTimeoutMs);
            writer.WriteLong(TransactionStartTimeMs);
            writer.WriteLong(ProducerId);
            writer.WriteShort(ProducerEpoch);
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is TransactionStateMessage other && Equals(other);
        }

        public bool Equals(TransactionStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ErrorCode, TransactionalId, TransactionState, TransactionTimeoutMs, TransactionStartTimeMs, ProducerId, ProducerEpoch);
            hashCode = HashCode.Combine(hashCode, Topics);
            return hashCode;
        }

        public override string ToString()
        {
            return "TransactionStateMessage("
                + "ErrorCode=" + ErrorCode
                + ", TransactionTimeoutMs=" + TransactionTimeoutMs
                + ", TransactionStartTimeMs=" + TransactionStartTimeMs
                + ", ProducerId=" + ProducerId
                + ", ProducerEpoch=" + ProducerEpoch
                + ")";
        }
    }

    public sealed class TopicDataMessage: IMessage, IEquatable<TopicDataMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public List<int> Partitions { get; set; } = new ();

        public TopicDataMessage()
        {
        }

        public TopicDataMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TopicDataMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
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
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Partitions = newCollection;
                }
            }
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Topic);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteVarUInt(Partitions.Count + 1);
            foreach (var element in Partitions)
            {
                writer.WriteInt(element);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is TopicDataMessage other && Equals(other);
        }

        public bool Equals(TopicDataMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Topic);
            return hashCode;
        }

        public override string ToString()
        {
            return "TopicDataMessage("
                + ")";
        }
    }

    public sealed class TopicDataCollection: HashSet<TopicDataMessage>
    {
        public TopicDataCollection()
        {
        }

        public TopicDataCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
