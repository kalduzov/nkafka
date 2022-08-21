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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public partial class DescribeDelegationTokenResponseMessage: ResponseMessage
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; }

    /// <summary>
    /// The tokens.
    /// </summary>
    public IReadOnlyCollection<DescribedDelegationTokenMessage> Tokens { get; set; }


    public DescribeDelegationTokenResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public DescribeDelegationTokenResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version2)
        {
        }
        else //no flexible version
        {
        }

    }

    public class DescribedDelegationTokenMessage: Message
    {
        /// <summary>
        /// The token principal type.
        /// </summary>
        public string PrincipalType { get; set; }

        /// <summary>
        /// The token principal name.
        /// </summary>
        public string PrincipalName { get; set; }

        /// <summary>
        /// The token issue timestamp in milliseconds.
        /// </summary>
        public long IssueTimestamp { get; set; }

        /// <summary>
        /// The token expiry timestamp in milliseconds.
        /// </summary>
        public long ExpiryTimestamp { get; set; }

        /// <summary>
        /// The token maximum timestamp length in milliseconds.
        /// </summary>
        public long MaxTimestamp { get; set; }

        /// <summary>
        /// The token ID.
        /// </summary>
        public string TokenId { get; set; }

        /// <summary>
        /// The token HMAC.
        /// </summary>
        public byte[] Hmac { get; set; }

        /// <summary>
        /// Those who are able to renew this token before it expires.
        /// </summary>
        public IReadOnlyCollection<DescribedDelegationTokenRenewerMessage> Renewers { get; set; }

        public DescribedDelegationTokenMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public DescribedDelegationTokenMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version2)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class DescribedDelegationTokenRenewerMessage: Message
    {
        /// <summary>
        /// The renewer principal type
        /// </summary>
        public string PrincipalType { get; set; }

        /// <summary>
        /// The renewer principal name
        /// </summary>
        public string PrincipalName { get; set; }

        public DescribedDelegationTokenRenewerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public DescribedDelegationTokenRenewerMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version2)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}