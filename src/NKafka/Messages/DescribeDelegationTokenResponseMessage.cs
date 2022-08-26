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
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// The tokens.
    /// </summary>
    public List<DescribedDelegationTokenMessage> TokensMessage { get; set; } = new ();


    public DescribeDelegationTokenResponseMessage()
    {
        ApiKey = ApiKeys.DescribeDelegationToken;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public DescribeDelegationTokenResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeDelegationToken;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class DescribedDelegationTokenMessage: Message
    {
        /// <summary>
        /// The token principal type.
        /// </summary>
        public string PrincipalTypeMessage { get; set; } = "";

        /// <summary>
        /// The token principal name.
        /// </summary>
        public string PrincipalNameMessage { get; set; } = "";

        /// <summary>
        /// The principal type of the requester of the token.
        /// </summary>
        public string TokenRequesterPrincipalTypeMessage { get; set; } = "";

        /// <summary>
        /// The principal type of the requester of the token.
        /// </summary>
        public string TokenRequesterPrincipalNameMessage { get; set; } = "";

        /// <summary>
        /// The token issue timestamp in milliseconds.
        /// </summary>
        public long IssueTimestampMessage { get; set; } = 0;

        /// <summary>
        /// The token expiry timestamp in milliseconds.
        /// </summary>
        public long ExpiryTimestampMessage { get; set; } = 0;

        /// <summary>
        /// The token maximum timestamp length in milliseconds.
        /// </summary>
        public long MaxTimestampMessage { get; set; } = 0;

        /// <summary>
        /// The token ID.
        /// </summary>
        public string TokenIdMessage { get; set; } = "";

        /// <summary>
        /// The token HMAC.
        /// </summary>
        public byte[] HmacMessage { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Those who are able to renew this token before it expires.
        /// </summary>
        public List<DescribedDelegationTokenRenewerMessage> RenewersMessage { get; set; } = new ();

        public DescribedDelegationTokenMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DescribedDelegationTokenMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class DescribedDelegationTokenRenewerMessage: Message
    {
        /// <summary>
        /// The renewer principal type
        /// </summary>
        public string PrincipalTypeMessage { get; set; } = "";

        /// <summary>
        /// The renewer principal name
        /// </summary>
        public string PrincipalNameMessage { get; set; } = "";

        public DescribedDelegationTokenRenewerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DescribedDelegationTokenRenewerMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }
}
