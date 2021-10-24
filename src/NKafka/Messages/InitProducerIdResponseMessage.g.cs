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
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed partial class InitProducerIdResponseMessage: IResponseMessage, IEquatable<InitProducerIdResponseMessage>
{
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The current producer id.
    /// </summary>
    public long ProducerId { get; set; } = -1;

    /// <summary>
    /// The current epoch associated with the producer id.
    /// </summary>
    public short ProducerEpoch { get; set; } = 0;

    public InitProducerIdResponseMessage()
    {
    }

    public InitProducerIdResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ThrottleTimeMs = reader.ReadInt();
        ErrorCode = reader.ReadShort();
        ProducerId = reader.ReadLong();
        ProducerEpoch = reader.ReadShort();
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

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteShort((short)ErrorCode);
        writer.WriteLong(ProducerId);
        writer.WriteShort(ProducerEpoch);
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

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is InitProducerIdResponseMessage other && Equals(other);
    }

    public bool Equals(InitProducerIdResponseMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ThrottleTimeMs != other.ThrottleTimeMs)
        {
            return false;
        }
        if (ErrorCode != other.ErrorCode)
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

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, ProducerId, ProducerEpoch);
        return hashCode;
    }

    public override string ToString()
    {
        return "InitProducerIdResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
            + ", ProducerId=" + ProducerId
            + ", ProducerEpoch=" + ProducerEpoch
            + ")";
    }
}