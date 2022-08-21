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

public partial class DescribeAclsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; }

    /// <summary>
    /// The error message, or null if there was no error.
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Each Resource that is referenced in an ACL.
    /// </summary>
    public IReadOnlyCollection<DescribeAclsResourceMessage> Resources { get; set; }

    public DescribeAclsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public DescribeAclsResponseMessage(BufferReader reader, ApiVersions version)
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

    public class DescribeAclsResourceMessage: Message
    {
        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceType { get; set; }

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// The resource pattern type.
        /// </summary>
        public sbyte PatternType { get; set; } = 3;

        /// <summary>
        /// The ACLs.
        /// </summary>
        public IReadOnlyCollection<AclDescriptionMessage> Acls { get; set; }

        public DescribeAclsResourceMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public DescribeAclsResourceMessage(BufferReader reader, ApiVersions version)
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
    public class AclDescriptionMessage: Message
    {
        /// <summary>
        /// The ACL principal.
        /// </summary>
        public string Principal { get; set; }

        /// <summary>
        /// The ACL host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The ACL operation.
        /// </summary>
        public sbyte Operation { get; set; }

        /// <summary>
        /// The ACL permission type.
        /// </summary>
        public sbyte PermissionType { get; set; }

        public AclDescriptionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public AclDescriptionMessage(BufferReader reader, ApiVersions version)
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