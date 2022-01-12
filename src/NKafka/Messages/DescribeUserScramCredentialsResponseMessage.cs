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

public partial class DescribeUserScramCredentialsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The message-level error code, 0 except for user authorization or infrastructure issues.
    /// </summary>
    public short ErrorCode { get; set; }

    /// <summary>
    /// The message-level error message, if any.
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// The results for descriptions, one per user.
    /// </summary>
    public IReadOnlyCollection<DescribeUserScramCredentialsResultMessage> Results { get; set; }

    public DescribeUserScramCredentialsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public DescribeUserScramCredentialsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class DescribeUserScramCredentialsResultMessage: Message
    {
        /// <summary>
        /// The user name.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The user-level error code.
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// The user-level error message, if any.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The mechanism and related information associated with the user's SCRAM credentials.
        /// </summary>
        public IReadOnlyCollection<CredentialInfoMessage> CredentialInfos { get; set; }

        public DescribeUserScramCredentialsResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public DescribeUserScramCredentialsResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class CredentialInfoMessage: Message
    {
        /// <summary>
        /// The SCRAM mechanism.
        /// </summary>
        public sbyte Mechanism { get; set; }

        /// <summary>
        /// The number of iterations used in the SCRAM credential.
        /// </summary>
        public int Iterations { get; set; }

        public CredentialInfoMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public CredentialInfoMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}