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

public partial class DeleteAclsRequestMessage: RequestMessage
{
    /// <summary>
    /// The filters to use when deleting ACLs.
    /// </summary>
    public IReadOnlyCollection<DeleteAclsFilterMessage> Filters { get; set; }

    public DeleteAclsRequestMessage()
    {
        ApiKey = ApiKeys.DeleteAcls;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class DeleteAclsFilterMessage: Message
    {
        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceTypeFilter { get; set; }

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceNameFilter { get; set; }

        /// <summary>
        /// The pattern type.
        /// </summary>
        public sbyte PatternTypeFilter { get; set; } = 3;

        /// <summary>
        /// The principal filter, or null to accept all principals.
        /// </summary>
        public string PrincipalFilter { get; set; }

        /// <summary>
        /// The host filter, or null to accept all hosts.
        /// </summary>
        public string HostFilter { get; set; }

        /// <summary>
        /// The ACL operation.
        /// </summary>
        public sbyte Operation { get; set; }

        /// <summary>
        /// The permission type.
        /// </summary>
        public sbyte PermissionType { get; set; }

        public DeleteAclsFilterMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}