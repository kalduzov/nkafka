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
/// Describes the contract for message SyncGroupResponseMessage
/// </summary>
public sealed partial class SyncGroupResponseMessage: IResponseMessage, IEquatable<SyncGroupResponseMessage>
{
    /// <inheritdoc />
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <inheritdoc />
    public int IncomingBufferLength { get; private set; } = 0;

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
    /// The group protocol type.
    /// </summary>
    public string? ProtocolType { get; set; } = null;

    /// <summary>
    /// The group protocol name.
    /// </summary>
    public string? ProtocolName { get; set; } = null;

    /// <summary>
    /// The member assignment.
    /// </summary>
    public byte[] Assignment { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The basic constructor of the message SyncGroupResponseMessage
    /// </summary>
    public SyncGroupResponseMessage()
    {
    }

    /// <summary>
    /// Base constructor for deserializing message SyncGroupResponseMessage
    /// </summary>
    public SyncGroupResponseMessage(ref BufferReader reader, ApiVersion version)
        : this()
    {
        IncomingBufferLength = reader.Length;
        Read(ref reader, version);
    }

    /// <inheritdoc />
    public void Read(ref BufferReader reader, ApiVersion version)
    {
        if (version >= ApiVersion.Version1)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        ErrorCode = reader.ReadShort();
        if (version >= ApiVersion.Version5)
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                ProtocolType = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ProtocolType had invalid length {length}");
            }
            else
            {
                ProtocolType = reader.ReadString(length);
            }
        }
        else
        {
            ProtocolType = null;
        }
        if (version >= ApiVersion.Version5)
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                ProtocolName = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ProtocolName had invalid length {length}");
            }
            else
            {
                ProtocolName = reader.ReadString(length);
            }
        }
        else
        {
            ProtocolName = null;
        }
        {
            int length;
            if (version >= ApiVersion.Version4)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadInt();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field Assignment was serialized as null");
            }
            else
            {
                Assignment = reader.ReadBytes(length);
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version4)
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
        if (version >= ApiVersion.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        writer.WriteShort((short)ErrorCode);
        if (version >= ApiVersion.Version5)
        {
            if (ProtocolType is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(ProtocolType);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        if (version >= ApiVersion.Version5)
        {
            if (ProtocolName is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(ProtocolName);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        if (version >= ApiVersion.Version4)
        {
            writer.WriteVarUInt(Assignment.Length + 1);
        }
        else
        {
            writer.WriteInt(Assignment.Length);
        }
        writer.WriteBytes(Assignment);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version4)
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
        return ReferenceEquals(this, obj) || obj is SyncGroupResponseMessage other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(SyncGroupResponseMessage? other)
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
        if (ProtocolType is null)
        {
            if (other.ProtocolType is not null)
            {
                return false;
            }
        }
        else
        {
            if (!ProtocolType.Equals(other.ProtocolType))
            {
                return false;
            }
        }
        if (ProtocolName is null)
        {
            if (other.ProtocolName is not null)
            {
                return false;
            }
        }
        else
        {
            if (!ProtocolName.Equals(other.ProtocolName))
            {
                return false;
            }
        }
        if (!Assignment.SequenceEqual(other.Assignment))
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, ProtocolType, ProtocolName, Assignment);
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "SyncGroupResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
            + ", ProtocolType=" + (string.IsNullOrWhiteSpace(ProtocolType) ? "null" : ProtocolType)
            + ", ProtocolName=" + (string.IsNullOrWhiteSpace(ProtocolName) ? "null" : ProtocolName)
            + ", Assignment=" + Assignment.DeepToString()
            + ")";
    }
}
