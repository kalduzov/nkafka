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

public sealed partial class SaslAuthenticateResponseMessage: IResponseMessage, IEquatable<SaslAuthenticateResponseMessage>
{
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The error message, or null if there was no error.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// The SASL authentication bytes from the server, as defined by the SASL mechanism.
    /// </summary>
    public byte[] AuthBytes { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The SASL authentication bytes from the server, as defined by the SASL mechanism.
    /// </summary>
    public long SessionLifetimeMs { get; set; } = 0;

    public SaslAuthenticateResponseMessage()
    {
    }

    public SaslAuthenticateResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ErrorCode = reader.ReadShort();
        {
            int length;
            if (version >= ApiVersion.Version2)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                ErrorMessage = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ErrorMessage had invalid length {length}");
            }
            else
            {
                ErrorMessage = reader.ReadString(length);
            }
        }
        {
            int length;
            if (version >= ApiVersion.Version2)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadInt();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field AuthBytes was serialized as null");
            }
            else
            {
                AuthBytes = reader.ReadBytes(length);
            }
        }
        if (version >= ApiVersion.Version1)
        {
            SessionLifetimeMs = reader.ReadLong();
        }
        else
        {
            SessionLifetimeMs = 0;
        }
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
        writer.WriteShort((short)ErrorCode);
        if (ErrorMessage is null)
        {
            if (version >= ApiVersion.Version2)
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
            var stringBytes = Encoding.UTF8.GetBytes(ErrorMessage);
            if (version >= ApiVersion.Version2)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersion.Version2)
        {
            writer.WriteVarUInt(AuthBytes.Length + 1);
        }
        else
        {
            writer.WriteInt(AuthBytes.Length);
        }
        writer.WriteBytes(AuthBytes);
        if (version >= ApiVersion.Version1)
        {
            writer.WriteLong(SessionLifetimeMs);
        }
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
        return ReferenceEquals(this, obj) || obj is SaslAuthenticateResponseMessage other && Equals(other);
    }

    public bool Equals(SaslAuthenticateResponseMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ErrorCode != other.ErrorCode)
        {
            return false;
        }
        if (ErrorMessage is null)
        {
            if (other.ErrorMessage is not null)
            {
                return false;
            }
        }
        else
        {
            if (!ErrorMessage.Equals(other.ErrorMessage))
            {
                return false;
            }
        }
        if (!AuthBytes.SequenceEqual(other.AuthBytes))
        {
            return false;
        }
        if (SessionLifetimeMs != other.SessionLifetimeMs)
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ErrorCode, ErrorMessage, AuthBytes, SessionLifetimeMs);
        return hashCode;
    }

    public override string ToString()
    {
        return "SaslAuthenticateResponseMessage("
            + "ErrorCode=" + ErrorCode
            + ", ErrorMessage=" + (string.IsNullOrWhiteSpace(ErrorMessage) ? "null" : ErrorMessage)
            + ", AuthBytes=" + AuthBytes.DeepToString()
            + ", SessionLifetimeMs=" + SessionLifetimeMs
            + ")";
    }
}