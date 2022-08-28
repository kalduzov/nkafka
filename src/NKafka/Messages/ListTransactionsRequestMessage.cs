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

public sealed class ListTransactionsRequestMessage: IRequestMessage, IEquatable<ListTransactionsRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

    public ApiKeys ApiKey => ApiKeys.ListTransactions;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The transaction states to filter by: if empty, all transactions are returned; if non-empty, then only transactions matching one of the filtered states will be returned
    /// </summary>
    public List<string> StateFilters { get; set; } = new ();

    /// <summary>
    /// The producerIds to filter by: if empty, all transactions will be returned; if non-empty, only transactions which match one of the filtered producerIds will be returned
    /// </summary>
    public List<long> ProducerIdFilters { get; set; } = new ();

    public ListTransactionsRequestMessage()
    {
    }

    public ListTransactionsRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field StateFilters was serialized as null");
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
                        throw new Exception("non-nullable field StateFilters element was serialized as null");
                    }
                    else if (length > 0x7fff)
                    {
                        throw new Exception($"string field StateFilters element had invalid length {length}");
                    }
                    else
                    {
                        newCollection.Add(reader.ReadString(length));
                    }
                }
                StateFilters = newCollection;
            }
        }
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field ProducerIdFilters was serialized as null");
            }
            else
            {
                var newCollection = new List<long>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(reader.ReadLong());
                }
                ProducerIdFilters = newCollection;
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
        writer.WriteVarUInt(StateFilters.Count + 1);
        foreach (var element in StateFilters)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(element);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        writer.WriteVarUInt(ProducerIdFilters.Count + 1);
        foreach (var element in ProducerIdFilters)
        {
            writer.WriteLong(element);
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ListTransactionsRequestMessage other && Equals(other);
    }

    public bool Equals(ListTransactionsRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, StateFilters, ProducerIdFilters);
        return hashCode;
    }

    public override string ToString()
    {
        return "ListTransactionsRequestMessage("
            + ")";
    }
}
