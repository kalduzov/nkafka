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

public sealed class DescribeDelegationTokenResponseMessage: IResponseMessage, IEquatable<DescribeDelegationTokenResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The tokens.
    /// </summary>
    public List<DescribedDelegationTokenMessage> Tokens { get; set; } = new ();

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    public DescribeDelegationTokenResponseMessage()
    {
    }

    public DescribeDelegationTokenResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        ErrorCode = reader.ReadShort();
        {
            if (version >= ApiVersions.Version2)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Tokens was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribedDelegationTokenMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribedDelegationTokenMessage(reader, version));
                    }
                    Tokens = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Tokens was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribedDelegationTokenMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribedDelegationTokenMessage(reader, version));
                    }
                    Tokens = newCollection;
                }
            }
        }
        ThrottleTimeMs = reader.ReadInt();
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
        writer.WriteShort((short)ErrorCode);
        if (version >= ApiVersions.Version2)
        {
            writer.WriteVarUInt(Tokens.Count + 1);
            foreach (var element in Tokens)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Tokens.Count);
            foreach (var element in Tokens)
            {
                element.Write(writer, version);
            }
        }
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
        return ReferenceEquals(this, obj) || obj is DescribeDelegationTokenResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeDelegationTokenResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ErrorCode, Tokens, ThrottleTimeMs);
        return hashCode;
    }

    public override string ToString()
    {
        return "DescribeDelegationTokenResponseMessage("
            + "ErrorCode=" + ErrorCode
            + ", ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }

    public sealed class DescribedDelegationTokenMessage: IMessage, IEquatable<DescribedDelegationTokenMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The token principal type.
        /// </summary>
        public string PrincipalType { get; set; } = string.Empty;

        /// <summary>
        /// The token principal name.
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
        /// The token issue timestamp in milliseconds.
        /// </summary>
        public long IssueTimestamp { get; set; } = 0;

        /// <summary>
        /// The token expiry timestamp in milliseconds.
        /// </summary>
        public long ExpiryTimestamp { get; set; } = 0;

        /// <summary>
        /// The token maximum timestamp length in milliseconds.
        /// </summary>
        public long MaxTimestamp { get; set; } = 0;

        /// <summary>
        /// The token ID.
        /// </summary>
        public string TokenId { get; set; } = string.Empty;

        /// <summary>
        /// The token HMAC.
        /// </summary>
        public byte[] Hmac { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Those who are able to renew this token before it expires.
        /// </summary>
        public List<DescribedDelegationTokenRenewerMessage> Renewers { get; set; } = new ();

        public DescribedDelegationTokenMessage()
        {
        }

        public DescribedDelegationTokenMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribedDelegationTokenMessage");
            }
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
            if (version >= ApiVersions.Version3)
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
            if (version >= ApiVersions.Version3)
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
            IssueTimestamp = reader.ReadLong();
            ExpiryTimestamp = reader.ReadLong();
            MaxTimestamp = reader.ReadLong();
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
                if (version >= ApiVersions.Version2)
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
            {
                if (version >= ApiVersions.Version2)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Renewers was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DescribedDelegationTokenRenewerMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DescribedDelegationTokenRenewerMessage(reader, version));
                        }
                        Renewers = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Renewers was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DescribedDelegationTokenRenewerMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DescribedDelegationTokenRenewerMessage(reader, version));
                        }
                        Renewers = newCollection;
                    }
                }
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
            writer.WriteLong(IssueTimestamp);
            writer.WriteLong(ExpiryTimestamp);
            writer.WriteLong(MaxTimestamp);
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
            if (version >= ApiVersions.Version2)
            {
                writer.WriteVarUInt(Renewers.Count + 1);
                foreach (var element in Renewers)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Renewers.Count);
                foreach (var element in Renewers)
                {
                    element.Write(writer, version);
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
            return ReferenceEquals(this, obj) || obj is DescribedDelegationTokenMessage other && Equals(other);
        }

        public bool Equals(DescribedDelegationTokenMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PrincipalType, PrincipalName, TokenRequesterPrincipalType, TokenRequesterPrincipalName, IssueTimestamp, ExpiryTimestamp, MaxTimestamp);
            hashCode = HashCode.Combine(hashCode, TokenId, Hmac, Renewers);
            return hashCode;
        }

        public override string ToString()
        {
            return "DescribedDelegationTokenMessage("
                + ", IssueTimestamp=" + IssueTimestamp
                + ", ExpiryTimestamp=" + ExpiryTimestamp
                + ", MaxTimestamp=" + MaxTimestamp
                + ")";
        }
    }

    public sealed class DescribedDelegationTokenRenewerMessage: IMessage, IEquatable<DescribedDelegationTokenRenewerMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The renewer principal type
        /// </summary>
        public string PrincipalType { get; set; } = string.Empty;

        /// <summary>
        /// The renewer principal name
        /// </summary>
        public string PrincipalName { get; set; } = string.Empty;

        public DescribedDelegationTokenRenewerMessage()
        {
        }

        public DescribedDelegationTokenRenewerMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribedDelegationTokenRenewerMessage");
            }
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
            return ReferenceEquals(this, obj) || obj is DescribedDelegationTokenRenewerMessage other && Equals(other);
        }

        public bool Equals(DescribedDelegationTokenRenewerMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PrincipalType, PrincipalName);
            return hashCode;
        }

        public override string ToString()
        {
            return "DescribedDelegationTokenRenewerMessage("
                + ")";
        }
    }
}
