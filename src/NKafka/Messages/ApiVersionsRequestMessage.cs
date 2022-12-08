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

// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using System.Text;

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;

namespace NKafka.Messages;

public sealed class ApiVersionsRequestMessage: IRequestMessage, IEquatable<ApiVersionsRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.ApiVersions;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The name of the client.
    /// </summary>
    public string ClientSoftwareName { get; set; } = string.Empty;

    /// <summary>
    /// The version of the client.
    /// </summary>
    public string ClientSoftwareVersion { get; set; } = string.Empty;

    public ApiVersionsRequestMessage()
    {
    }

    public ApiVersionsRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        if (version >= ApiVersion.Version3)
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                throw new Exception("non-nullable field ClientSoftwareName was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ClientSoftwareName had invalid length {length}");
            }
            else
            {
                ClientSoftwareName = reader.ReadString(length);
            }
        }
        else
        {
            ClientSoftwareName = string.Empty;
        }
        if (version >= ApiVersion.Version3)
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                throw new Exception("non-nullable field ClientSoftwareVersion was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ClientSoftwareVersion had invalid length {length}");
            }
            else
            {
                ClientSoftwareVersion = reader.ReadString(length);
            }
        }
        else
        {
            ClientSoftwareVersion = string.Empty;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version3)
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
        if (version >= ApiVersion.Version3)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(ClientSoftwareName);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        if (version >= ApiVersion.Version3)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(ClientSoftwareVersion);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version3)
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
        return ReferenceEquals(this, obj) || obj is ApiVersionsRequestMessage other && Equals(other);
    }

    public bool Equals(ApiVersionsRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ClientSoftwareName, ClientSoftwareVersion);
        return hashCode;
    }

    public override string ToString()
    {
        return "ApiVersionsRequestMessage("
            + ")";
    }
}