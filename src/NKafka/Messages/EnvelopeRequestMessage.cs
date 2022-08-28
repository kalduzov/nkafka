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

public sealed class EnvelopeRequestMessage: IRequestMessage, IEquatable<EnvelopeRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

    public ApiKeys ApiKey => ApiKeys.Envelope;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The embedded request header and data.
    /// </summary>
    public byte[] RequestData { get; set; } = new byte[0];

    /// <summary>
    /// Value of the initial client principal when the request is redirected by a broker.
    /// </summary>
    public byte[] RequestPrincipal { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The original client's address in bytes.
    /// </summary>
    public byte[] ClientHostAddress { get; set; } = Array.Empty<byte>();

    public EnvelopeRequestMessage()
    {
    }

    public EnvelopeRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                throw new Exception("non-nullable field RequestData was serialized as null");
            }
            else
            {
                RequestData = reader.ReadBytes(length);
            }
        }
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                RequestPrincipal = null;
            }
            else
            {
                RequestPrincipal = reader.ReadBytes(length);
            }
        }
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                throw new Exception("non-nullable field ClientHostAddress was serialized as null");
            }
            else
            {
                ClientHostAddress = reader.ReadBytes(length);
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
        writer.WriteVarUInt(RequestData.Length + 1);
        writer.WriteBytes(RequestData);
        if (RequestPrincipal is null)
        {
            writer.WriteVarUInt(0);
        }
        else
        {
            writer.WriteVarUInt(RequestPrincipal.Length + 1);
            writer.WriteBytes(RequestPrincipal);
        }
        writer.WriteVarUInt(ClientHostAddress.Length + 1);
        writer.WriteBytes(ClientHostAddress);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is EnvelopeRequestMessage other && Equals(other);
    }

    public bool Equals(EnvelopeRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, RequestData, RequestPrincipal, ClientHostAddress);
        return hashCode;
    }

    public override string ToString()
    {
        return "EnvelopeRequestMessage("
            + ")";
    }
}
