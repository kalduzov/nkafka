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

public sealed class ListTransactionsResponseMessage: IResponseMessage, IEquatable<ListTransactionsResponseMessage>
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
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// Set of state filters provided in the request which were unknown to the transaction coordinator
    /// </summary>
    public List<string> UnknownStateFilters { get; set; } = new ();

    /// <summary>
    /// 
    /// </summary>
    public List<TransactionStateMessage> TransactionStates { get; set; } = new ();

    public ListTransactionsResponseMessage()
    {
    }

    public ListTransactionsResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        ThrottleTimeMs = reader.ReadInt();
        ErrorCode = reader.ReadShort();
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field UnknownStateFilters was serialized as null");
            }
            else
            {
                var newCollection = new List<string>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    int length;
                    length = reader.ReadVarUInt() - 1;
                    if (length < 0)
                    {
                        throw new Exception("non-nullable field UnknownStateFilters element was serialized as null");
                    }
                    else if (length > 0x7fff)
                    {
                        throw new Exception($"string field UnknownStateFilters element had invalid length {length}");
                    }
                    else
                    {
                        newCollection.Add(reader.ReadString(length));
                    }
                }
                UnknownStateFilters = newCollection;
            }
        }
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
        writer.WriteShort((short)ErrorCode);
        writer.WriteVarUInt(UnknownStateFilters.Count + 1);
        foreach (var element in UnknownStateFilters)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(element);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
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
        return ReferenceEquals(this, obj) || obj is ListTransactionsResponseMessage other && Equals(other);
    }

    public bool Equals(ListTransactionsResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, UnknownStateFilters, TransactionStates);
        return hashCode;
    }

    public override string ToString()
    {
        return "ListTransactionsResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
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
        public string TransactionalId { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public long ProducerId { get; set; } = 0;

        /// <summary>
        /// The current transaction state of the producer
        /// </summary>
        public string TransactionState { get; set; } = string.Empty;

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
            ProducerId = reader.ReadLong();
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
                var stringBytes = Encoding.UTF8.GetBytes(TransactionalId);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteLong(ProducerId);
            {
                var stringBytes = Encoding.UTF8.GetBytes(TransactionState);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
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
            hashCode = HashCode.Combine(hashCode, TransactionalId, ProducerId, TransactionState);
            return hashCode;
        }

        public override string ToString()
        {
            return "TransactionStateMessage("
                + ", ProducerId=" + ProducerId
                + ")";
        }
    }
}
