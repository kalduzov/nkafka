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

public sealed class SyncGroupResponseMessage: IResponseMessage, IEquatable<SyncGroupResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version5;

    public ApiVersions Version {get; set;}

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

    public SyncGroupResponseMessage()
    {
    }

    public SyncGroupResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        if (version >= ApiVersions.Version1)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        ErrorCode = reader.ReadShort();
        if (version >= ApiVersions.Version5)
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
        if (version >= ApiVersions.Version5)
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
            if (version >= ApiVersions.Version4)
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
        if (version >= ApiVersions.Version4)
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
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        writer.WriteShort((short)ErrorCode);
        if (version >= ApiVersions.Version5)
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
        if (version >= ApiVersions.Version5)
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
        if (version >= ApiVersions.Version4)
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
        if (version >= ApiVersions.Version4)
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
        return ReferenceEquals(this, obj) || obj is SyncGroupResponseMessage other && Equals(other);
    }

    public bool Equals(SyncGroupResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, ProtocolType, ProtocolName, Assignment);
        return hashCode;
    }

    public override string ToString()
    {
        return "SyncGroupResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
            + ")";
    }
}
