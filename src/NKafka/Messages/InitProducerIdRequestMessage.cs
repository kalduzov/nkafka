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

public sealed class InitProducerIdRequestMessage: IRequestMessage, IEquatable<InitProducerIdRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version4;

    public ApiKeys ApiKey => ApiKeys.InitProducerId;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

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
    /// The producer's current epoch. This will be checked against the producer epoch on the broker, and the request will return an error if they do not match.
    /// </summary>
    public short ProducerEpoch { get; set; } = -1;

    public InitProducerIdRequestMessage()
    {
    }

    public InitProducerIdRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            int length;
            if (version >= ApiVersions.Version2)
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
        if (version >= ApiVersions.Version3)
        {
            ProducerId = reader.ReadLong();
        }
        else
        {
            ProducerId = -1;
        }
        if (version >= ApiVersions.Version3)
        {
            ProducerEpoch = reader.ReadShort();
        }
        else
        {
            ProducerEpoch = -1;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version2)
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

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (TransactionalId is null)
        {
            if (version >= ApiVersions.Version2)
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
            if (version >= ApiVersions.Version2)
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
        if (version >= ApiVersions.Version3)
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
        if (version >= ApiVersions.Version3)
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
        if (version >= ApiVersions.Version2)
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
        return ReferenceEquals(this, obj) || obj is InitProducerIdRequestMessage other && Equals(other);
    }

    public bool Equals(InitProducerIdRequestMessage? other)
    {
        return true;
    }
}
