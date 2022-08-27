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

public sealed class DescribeDelegationTokenResponseMessage: ResponseMessage
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// The tokens.
    /// </summary>
    public List<DescribedDelegationTokenMessage> Tokens { get; set; } = new ();


    public DescribeDelegationTokenResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public DescribeDelegationTokenResponseMessage(BufferReader reader, ApiVersions version)
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
    }

    public sealed class DescribedDelegationTokenMessage: Message
    {
        /// <summary>
        /// The token principal type.
        /// </summary>
        public string PrincipalType { get; set; } = "";

        /// <summary>
        /// The token principal name.
        /// </summary>
        public string PrincipalName { get; set; } = "";

        /// <summary>
        /// The principal type of the requester of the token.
        /// </summary>
        public string TokenRequesterPrincipalType { get; set; } = "";

        /// <summary>
        /// The principal type of the requester of the token.
        /// </summary>
        public string TokenRequesterPrincipalName { get; set; } = "";

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
        public string TokenId { get; set; } = "";

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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public DescribedDelegationTokenMessage(BufferReader reader, ApiVersions version)
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
                if (TokenRequesterPrincipalType.Equals(""))
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
                if (TokenRequesterPrincipalName.Equals(""))
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
        }
    }

    public sealed class DescribedDelegationTokenRenewerMessage: Message
    {
        /// <summary>
        /// The renewer principal type
        /// </summary>
        public string PrincipalType { get; set; } = "";

        /// <summary>
        /// The renewer principal name
        /// </summary>
        public string PrincipalName { get; set; } = "";

        public DescribedDelegationTokenRenewerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public DescribedDelegationTokenRenewerMessage(BufferReader reader, ApiVersions version)
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
        }
    }
}
