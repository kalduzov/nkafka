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

public sealed class CreateDelegationTokenResponseMessage: ResponseMessage, IEquatable<CreateDelegationTokenResponseMessage>
{
    /// <summary>
    /// The top-level error, or zero if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

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


    public CreateDelegationTokenResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public CreateDelegationTokenResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteShort(ErrorCode);
        {
            var stringBytes = Encoding.UTF8.GetBytes(PrincipalType);
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
        {
            var stringBytes = Encoding.UTF8.GetBytes(PrincipalName);
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
        if (version >= ApiVersions.Version3)
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
        if (version >= ApiVersions.Version3)
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
        if (version >= ApiVersions.Version2)
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
        return ReferenceEquals(this, obj) || obj is CreateDelegationTokenResponseMessage other && Equals(other);
    }

    public bool Equals(CreateDelegationTokenResponseMessage? other)
    {
        return true;
    }
}
