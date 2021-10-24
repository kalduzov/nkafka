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

public sealed partial class AllocateProducerIdsRequestMessage: IRequestMessage, IEquatable<AllocateProducerIdsRequestMessage>
{
    /// <inheritdoc />
    public ApiKeys ApiKey => ApiKeys.AllocateProducerIds;

    public const bool ONLY_CONTROLLER = true;

    /// <inheritdoc />
    public bool OnlyController => ONLY_CONTROLLER;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The ID of the requesting broker
    /// </summary>
    public int BrokerId { get; set; } = 0;

    /// <summary>
    /// The epoch of the requesting broker
    /// </summary>
    public long BrokerEpoch { get; set; } = -1;

    public AllocateProducerIdsRequestMessage()
    {
    }

    public AllocateProducerIdsRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        BrokerId = reader.ReadInt();
        BrokerEpoch = reader.ReadLong();
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
        var numTaggedFields = 0;
        writer.WriteInt(BrokerId);
        writer.WriteLong(BrokerEpoch);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is AllocateProducerIdsRequestMessage other && Equals(other);
    }

    public bool Equals(AllocateProducerIdsRequestMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (BrokerId != other.BrokerId)
        {
            return false;
        }
        if (BrokerEpoch != other.BrokerEpoch)
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, BrokerId, BrokerEpoch);
        return hashCode;
    }

    public override string ToString()
    {
        return "AllocateProducerIdsRequestMessage("
            + "BrokerId=" + BrokerId
            + ", BrokerEpoch=" + BrokerEpoch
            + ")";
    }
}
