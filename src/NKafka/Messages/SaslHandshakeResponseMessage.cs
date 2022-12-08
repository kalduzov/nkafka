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

public sealed class SaslHandshakeResponseMessage: IResponseMessage, IEquatable<SaslHandshakeResponseMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version1;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The mechanisms enabled in the server.
    /// </summary>
    public List<string> Mechanisms { get; set; } = new();

    public SaslHandshakeResponseMessage()
    {
    }

    public SaslHandshakeResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ErrorCode = reader.ReadShort();
        {
            int arrayLength;
            arrayLength = reader.ReadInt();
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Mechanisms was serialized as null");
            }
            else
            {
                var newCollection = new List<string>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    int length;
                    length = reader.ReadShort();
                    if (length < 0)
                    {
                        throw new Exception("non-nullable field Mechanisms element was serialized as null");
                    }
                    else if (length > 0x7fff)
                    {
                        throw new Exception($"string field Mechanisms element had invalid length {length}");
                    }
                    else
                    {
                        newCollection.Add(reader.ReadString(length));
                    }
                }
                Mechanisms = newCollection;
            }
        }
        UnknownTaggedFields = null;
    }

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        writer.WriteShort((short)ErrorCode);
        writer.WriteInt(Mechanisms.Count);
        foreach (var element in Mechanisms)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(element);
                writer.WriteShort((short)stringBytes.Length);
                writer.WriteBytes(stringBytes);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (numTaggedFields > 0)
        {
            throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
        }
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is SaslHandshakeResponseMessage other && Equals(other);
    }

    public bool Equals(SaslHandshakeResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ErrorCode, Mechanisms);
        return hashCode;
    }

    public override string ToString()
    {
        return "SaslHandshakeResponseMessage("
            + "ErrorCode=" + ErrorCode
            + ")";
    }
}