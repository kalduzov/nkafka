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

public sealed partial class CreateDelegationTokenResponseMessage: IResponseMessage, IEquatable<CreateDelegationTokenResponseMessage>
{
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The top-level error, or zero if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The principal type of the token owner.
    /// </summary>
    public string PrincipalType { get; set; } = string.Empty;

    /// <summary>
    /// The name of the token owner.
    /// </summary>
    public string PrincipalName { get; set; } = string.Empty;

    /// <summary>
    /// The principal type of the requester of the token.
    /// </summary>
    public string TokenRequesterPrincipalType { get; set; } = string.Empty;

    /// <summary>
    /// The principal type of the requester of the token.
    /// </summary>
    public string TokenRequesterPrincipalName { get; set; } = string.Empty;

    /// <summary>
    /// When this token was generated.
    /// </summary>
    public long IssueTimestampMs { get; set; } = 0;

    /// <summary>
    /// When this token expires.
    /// </summary>
    public long ExpiryTimestampMs { get; set; } = 0;

    /// <summary>
    /// The maximum lifetime of this token.
    /// </summary>
    public long MaxTimestampMs { get; set; } = 0;

    /// <summary>
    /// The token UUID.
    /// </summary>
    public string TokenId { get; set; } = string.Empty;

    /// <summary>
    /// HMAC of the delegation token.
    /// </summary>
    public byte[] Hmac { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    public CreateDelegationTokenResponseMessage()
    {
    }

    public CreateDelegationTokenResponseMessage(BufferReader reader, ApiVersion version)
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
                throw new Exception("non-nullable field PrincipalType was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field PrincipalType had invalid length {length}");
            }
            else
            {
                PrincipalType = reader.ReadString(length);
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
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field PrincipalName was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field PrincipalName had invalid length {length}");
            }
            else
            {
                PrincipalName = reader.ReadString(length);
            }
        }
        if (version >= ApiVersion.Version3)
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                throw new Exception("non-nullable field TokenRequesterPrincipalType was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field TokenRequesterPrincipalType had invalid length {length}");
            }
            else
            {
                TokenRequesterPrincipalType = reader.ReadString(length);
            }
        }
        else
        {
            TokenRequesterPrincipalType = string.Empty;
        }
        if (version >= ApiVersion.Version3)
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                throw new Exception("non-nullable field TokenRequesterPrincipalName was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field TokenRequesterPrincipalName had invalid length {length}");
            }
            else
            {
                TokenRequesterPrincipalName = reader.ReadString(length);
            }
        }
        else
        {
            TokenRequesterPrincipalName = string.Empty;
        }
        IssueTimestampMs = reader.ReadLong();
        ExpiryTimestampMs = reader.ReadLong();
        MaxTimestampMs = reader.ReadLong();
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
                throw new Exception("non-nullable field TokenId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field TokenId had invalid length {length}");
            }
            else
            {
                TokenId = reader.ReadString(length);
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
                throw new Exception("non-nullable field Hmac was serialized as null");
            }
            else
            {
                Hmac = reader.ReadBytes(length);
            }
        }
        ThrottleTimeMs = reader.ReadInt();
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
        {
            var stringBytes = Encoding.UTF8.GetBytes(PrincipalType);
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
        {
            var stringBytes = Encoding.UTF8.GetBytes(PrincipalName);
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
        if (version >= ApiVersion.Version3)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(TokenRequesterPrincipalType);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        else
        {
            if (!TokenRequesterPrincipalType.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default TokenRequesterPrincipalType at version {version}");
            }
        }
        if (version >= ApiVersion.Version3)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(TokenRequesterPrincipalName);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        else
        {
            if (!TokenRequesterPrincipalName.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default TokenRequesterPrincipalName at version {version}");
            }
        }
        writer.WriteLong(IssueTimestampMs);
        writer.WriteLong(ExpiryTimestampMs);
        writer.WriteLong(MaxTimestampMs);
        {
            var stringBytes = Encoding.UTF8.GetBytes(TokenId);
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
            writer.WriteVarUInt(Hmac.Length + 1);
        }
        else
        {
            writer.WriteInt(Hmac.Length);
        }
        writer.WriteBytes(Hmac);
        writer.WriteInt(ThrottleTimeMs);
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
        return ReferenceEquals(this, obj) || obj is CreateDelegationTokenResponseMessage other && Equals(other);
    }

    public bool Equals(CreateDelegationTokenResponseMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ErrorCode != other.ErrorCode)
        {
            return false;
        }
        if (PrincipalType is null)
        {
            if (other.PrincipalType is not null)
            {
                return false;
            }
        }
        else
        {
            if (!PrincipalType.Equals(other.PrincipalType))
            {
                return false;
            }
        }
        if (PrincipalName is null)
        {
            if (other.PrincipalName is not null)
            {
                return false;
            }
        }
        else
        {
            if (!PrincipalName.Equals(other.PrincipalName))
            {
                return false;
            }
        }
        if (TokenRequesterPrincipalType is null)
        {
            if (other.TokenRequesterPrincipalType is not null)
            {
                return false;
            }
        }
        else
        {
            if (!TokenRequesterPrincipalType.Equals(other.TokenRequesterPrincipalType))
            {
                return false;
            }
        }
        if (TokenRequesterPrincipalName is null)
        {
            if (other.TokenRequesterPrincipalName is not null)
            {
                return false;
            }
        }
        else
        {
            if (!TokenRequesterPrincipalName.Equals(other.TokenRequesterPrincipalName))
            {
                return false;
            }
        }
        if (IssueTimestampMs != other.IssueTimestampMs)
        {
            return false;
        }
        if (ExpiryTimestampMs != other.ExpiryTimestampMs)
        {
            return false;
        }
        if (MaxTimestampMs != other.MaxTimestampMs)
        {
            return false;
        }
        if (TokenId is null)
        {
            if (other.TokenId is not null)
            {
                return false;
            }
        }
        else
        {
            if (!TokenId.Equals(other.TokenId))
            {
                return false;
            }
        }
        if (!Hmac.SequenceEqual(other.Hmac))
        {
            return false;
        }
        if (ThrottleTimeMs != other.ThrottleTimeMs)
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ErrorCode, PrincipalType, PrincipalName, TokenRequesterPrincipalType, TokenRequesterPrincipalName, IssueTimestampMs, ExpiryTimestampMs);
        hashCode = HashCode.Combine(hashCode, MaxTimestampMs, TokenId, Hmac, ThrottleTimeMs);
        return hashCode;
    }

    public override string ToString()
    {
        return "CreateDelegationTokenResponseMessage("
            + "ErrorCode=" + ErrorCode
            + ", PrincipalType=" + (string.IsNullOrWhiteSpace(PrincipalType) ? "null" : PrincipalType)
            + ", PrincipalName=" + (string.IsNullOrWhiteSpace(PrincipalName) ? "null" : PrincipalName)
            + ", TokenRequesterPrincipalType=" + (string.IsNullOrWhiteSpace(TokenRequesterPrincipalType) ? "null" : TokenRequesterPrincipalType)
            + ", TokenRequesterPrincipalName=" + (string.IsNullOrWhiteSpace(TokenRequesterPrincipalName) ? "null" : TokenRequesterPrincipalName)
            + ", IssueTimestampMs=" + IssueTimestampMs
            + ", ExpiryTimestampMs=" + ExpiryTimestampMs
            + ", MaxTimestampMs=" + MaxTimestampMs
            + ", TokenId=" + (string.IsNullOrWhiteSpace(TokenId) ? "null" : TokenId)
            + ", Hmac=" + Hmac.DeepToString()
            + ", ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }
}