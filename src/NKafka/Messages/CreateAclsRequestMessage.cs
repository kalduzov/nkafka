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
public sealed partial class CreateAclsRequestMessage: RequestMessage
{
    /// <summary>
    /// The ACLs that we want to create.
    /// </summary>
    public List<AclCreation> Creations { get; set; } = new();

    public CreateAclsRequestMessage()
    {
        ApiKey = ApiKeys.CreateAcls;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public CreateAclsRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.CreateAcls;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class AclCreation: Message
    {
        /// <summary>
        /// The type of the resource.
        /// </summary>
        public sbyte ResourceType { get; set; } = 0;

        /// <summary>
        /// The resource name for the ACL.
        /// </summary>
        public string ResourceName { get; set; } = null!;

        /// <summary>
        /// The pattern type for the ACL.
        /// </summary>
        public sbyte ResourcePatternType { get; set; } = 3;

        /// <summary>
        /// The principal for the ACL.
        /// </summary>
        public string Principal { get; set; } = null!;

        /// <summary>
        /// The host for the ACL.
        /// </summary>
        public string Host { get; set; } = null!;

        /// <summary>
        /// The operation type for the ACL (read, write, etc.).
        /// </summary>
        public sbyte Operation { get; set; } = 0;

        /// <summary>
        /// The permission type for the ACL (allow, deny, etc.).
        /// </summary>
        public sbyte PermissionType { get; set; } = 0;

        public AclCreation()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public AclCreation(BufferReader reader, ApiVersions version)
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