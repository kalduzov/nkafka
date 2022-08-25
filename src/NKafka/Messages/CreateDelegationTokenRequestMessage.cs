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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class CreateDelegationTokenRequestMessage: RequestMessage
{
    /// <summary>
    /// The principal type of the owner of the token. If it's null it defaults to the token request principal.
    /// </summary>
    public string OwnerPrincipalType { get; set; } = null!;

    /// <summary>
    /// The principal name of the owner of the token. If it's null it defaults to the token request principal.
    /// </summary>
    public string OwnerPrincipalName { get; set; } = null!;

    /// <summary>
    /// A list of those who are allowed to renew this token before it expires.
    /// </summary>
    public List<CreatableRenewers> Renewers { get; set; } = new();

    /// <summary>
    /// The maximum lifetime of the token in milliseconds, or -1 to use the server side default.
    /// </summary>
    public long MaxLifetimeMs { get; set; } = 0;

    public CreateDelegationTokenRequestMessage()
    {
        ApiKey = ApiKeys.CreateDelegationToken;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public CreateDelegationTokenRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.CreateDelegationToken;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class CreatableRenewers: Message
    {
        /// <summary>
        /// The type of the Kafka principal.
        /// </summary>
        public string PrincipalType { get; set; } = null!;

        /// <summary>
        /// The name of the Kafka principal.
        /// </summary>
        public string PrincipalName { get; set; } = null!;

        public CreatableRenewers()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public CreatableRenewers(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}