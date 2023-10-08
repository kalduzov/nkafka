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
// <auto-generated> THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT. </auto-generated>

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

#nullable enable

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

/// <summary>
/// Describes the contract for message InitProducerIdRequestMessage
/// </summary>
public sealed partial class InitProducerIdRequestMessage: IRequestMessage, IEquatable<InitProducerIdRequestMessage>
{
    /// <inheritdoc />
    public ApiKeys ApiKey => ApiKeys.InitProducerId;

    /// <summary>
    /// Indicates whether the request is accessed by any broker or only by the controller
    /// </summary>
    public const bool ONLY_CONTROLLER = false;

    /// <inheritdoc />
    public bool OnlyController => ONLY_CONTROLLER;

    /// <inheritdoc />
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <inheritdoc />
    public int IncomingBufferLength { get; private set; } = 0;

    /// <summary>
    /// The transactional id, or null if the producer is not transactional.
    /// </summary>
    public string TransactionalId { get; set; } = string.Empty;

    /// <summary>
    /// The time in ms to wait before aborting idle transactions sent by this producer. This is only relevant if a TransactionalId has been defined.
    /// </summary>
    public int TransactionTimeoutMs { get; set; } = 0;

    /// <summary>
    /// The producer id. This is used to disambiguate requests if a transactional id is reused following its expiration.
    /// </summary>
    public long ProducerId { get; set; } = -1;

    /// <summary>
    /// The producer&#39;s current epoch. This will be checked against the producer epoch on the broker, and the request will return an error if they do not match.
    /// </summary>
    public short ProducerEpoch { get; set; } = -1;

    /// <summary>
    /// The basic constructor of the message InitProducerIdRequestMessage
    /// </summary>
    public InitProducerIdRequestMessage()
    {
    }

    /// <summary>
    /// Base constructor for deserializing message InitProducerIdRequestMessage
    /// </summary>
    public InitProducerIdRequestMessage(ref BufferReader reader, ApiVersion version)
        : this()
    {
        IncomingBufferLength = reader.Length;
        Read(ref reader, version);
    }

    /// <inheritdoc />
    public void Read(ref BufferReader reader, ApiVersion version)
    {
        {
            int length;
            if (version >= ApiVersion.Version2)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                TransactionalId = null;
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
        TransactionTimeoutMs = reader.ReadInt();
        if (version >= ApiVersion.Version3)
        {
            ProducerId = reader.ReadLong();
        }
        else
        {
            ProducerId = -1;
        }
        if (version >= ApiVersion.Version3)
        {
            ProducerEpoch = reader.ReadShort();
        }
        else
        {
            ProducerEpoch = -1;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version2)
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

    /// <inheritdoc />
    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        if (TransactionalId is null)
        {
            if (version >= ApiVersion.Version2)
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
            var stringBytes = Encoding.UTF8.GetBytes(TransactionalId);
            if (version >= ApiVersion.Version2)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        writer.WriteInt(TransactionTimeoutMs);
        if (version >= ApiVersion.Version3)
        {
            writer.WriteLong(ProducerId);
        }
        else
        {
            if (ProducerId != -1)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ProducerId at version {version}");
            }
        }
        if (version >= ApiVersion.Version3)
        {
            writer.WriteShort(ProducerEpoch);
        }
        else
        {
            if (ProducerEpoch != -1)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ProducerEpoch at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version2)
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

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is InitProducerIdRequestMessage other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(InitProducerIdRequestMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (TransactionalId is null)
        {
            if (other.TransactionalId is not null)
            {
                return false;
            }
        }
        else
        {
            if (!TransactionalId.Equals(other.TransactionalId))
            {
                return false;
            }
        }
        if (TransactionTimeoutMs != other.TransactionTimeoutMs)
        {
            return false;
        }
        if (ProducerId != other.ProducerId)
        {
            return false;
        }
        if (ProducerEpoch != other.ProducerEpoch)
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, TransactionalId, TransactionTimeoutMs, ProducerId, ProducerEpoch);
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "InitProducerIdRequestMessage("
            + "TransactionalId=" + (string.IsNullOrWhiteSpace(TransactionalId) ? "null" : TransactionalId)
            + ", TransactionTimeoutMs=" + TransactionTimeoutMs
            + ", ProducerId=" + ProducerId
            + ", ProducerEpoch=" + ProducerEpoch
            + ")";
    }
}
