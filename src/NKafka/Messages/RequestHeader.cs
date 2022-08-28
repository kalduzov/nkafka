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

public sealed class RequestHeader: IMessage, IEquatable<RequestHeader>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version2;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The API key of this request.
    /// </summary>
    public short RequestApiKey { get; set; } = 0;

    /// <summary>
    /// The API version of this request.
    /// </summary>
    public short RequestApiVersion { get; set; } = 0;

    /// <summary>
    /// The correlation ID of this request.
    /// </summary>
    public int CorrelationId { get; set; } = 0;

    /// <summary>
    /// The client ID string.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    public RequestHeader()
    {
    }

    public RequestHeader(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        RequestApiKey = reader.ReadShort();
        RequestApiVersion = reader.ReadShort();
        CorrelationId = reader.ReadInt();
        if (version >= ApiVersions.Version1)
        {
            int length;
            length = reader.ReadShort();
            if (length < 0)
            {
                ClientId = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ClientId had invalid length {length}");
            }
            else
            {
                ClientId = reader.ReadString(length);
            }
        }
        else
        {
            ClientId = string.Empty;
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
        writer.WriteShort(RequestApiKey);
        writer.WriteShort(RequestApiVersion);
        writer.WriteInt(CorrelationId);
        if (version >= ApiVersions.Version1)
        {
            if (ClientId is null)
            {
                writer.WriteShort(-1);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(ClientId);
                writer.WriteShort((short)stringBytes.Length);
                writer.WriteBytes(stringBytes);
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
        return ReferenceEquals(this, obj) || obj is RequestHeader other && Equals(other);
    }

    public bool Equals(RequestHeader? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, RequestApiKey, RequestApiVersion, CorrelationId, ClientId);
        return hashCode;
    }

    public override string ToString()
    {
        return "RequestHeader("
            + "RequestApiKey=" + RequestApiKey
            + ", RequestApiVersion=" + RequestApiVersion
            + ", CorrelationId=" + CorrelationId
            + ")";
    }
}
