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

public sealed class AlterUserScramCredentialsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The results for deletions and alterations, one per affected user.
    /// </summary>
    public List<AlterUserScramCredentialsResultMessage> Results { get; set; } = new ();

    public AlterUserScramCredentialsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public AlterUserScramCredentialsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed class AlterUserScramCredentialsResultMessage: Message
    {
        /// <summary>
        /// The user name.
        /// </summary>
        public string User { get; set; } = "";

        /// <summary>
        /// The error code.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The error message, if any.
        /// </summary>
        public string ErrorMessage { get; set; } = "";

        public AlterUserScramCredentialsResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public AlterUserScramCredentialsResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}