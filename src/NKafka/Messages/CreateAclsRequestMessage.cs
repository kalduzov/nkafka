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

public partial class CreateAclsRequestMessage: RequestMessage
{
    /// <summary>
    /// The ACLs that we want to create.
    /// </summary>
    public IReadOnlyCollection<AclCreationMessage> Creations { get; set; }

    public CreateAclsRequestMessage()
    {
        ApiKey = ApiKeys.CreateAcls;
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

    public class AclCreationMessage: Message
    {
        /// <summary>
        /// The type of the resource.
        /// </summary>
        public sbyte ResourceType { get; set; }

        /// <summary>
        /// The resource name for the ACL.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// The pattern type for the ACL.
        /// </summary>
        public sbyte ResourcePatternType { get; set; } = 3;

        /// <summary>
        /// The principal for the ACL.
        /// </summary>
        public string Principal { get; set; }

        /// <summary>
        /// The host for the ACL.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The operation type for the ACL (read, write, etc.).
        /// </summary>
        public sbyte Operation { get; set; }

        /// <summary>
        /// The permission type for the ACL (allow, deny, etc.).
        /// </summary>
        public sbyte PermissionType { get; set; }

        public AclCreationMessage()
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