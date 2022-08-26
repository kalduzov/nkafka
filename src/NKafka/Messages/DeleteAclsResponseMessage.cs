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

public sealed class DeleteAclsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The results for each filter.
    /// </summary>
    public List<DeleteAclsFilterResultMessage> FilterResultsMessage { get; set; } = new ();

    public DeleteAclsResponseMessage()
    {
        ApiKey = ApiKeys.DeleteAcls;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public DeleteAclsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DeleteAcls;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class DeleteAclsFilterResultMessage: Message
    {
        /// <summary>
        /// The error code, or 0 if the filter succeeded.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The error message, or null if the filter succeeded.
        /// </summary>
        public string ErrorMessageMessage { get; set; } = "";

        /// <summary>
        /// The ACLs which matched this filter.
        /// </summary>
        public List<DeleteAclsMatchingAclMessage> MatchingAclsMessage { get; set; } = new ();

        public DeleteAclsFilterResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DeleteAclsFilterResultMessage(BufferReader reader, ApiVersions version)
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

    public sealed class DeleteAclsMatchingAclMessage: Message
    {
        /// <summary>
        /// The deletion error code, or 0 if the deletion succeeded.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The deletion error message, or null if the deletion succeeded.
        /// </summary>
        public string ErrorMessageMessage { get; set; } = "";

        /// <summary>
        /// The ACL resource type.
        /// </summary>
        public sbyte ResourceTypeMessage { get; set; } = 0;

        /// <summary>
        /// The ACL resource name.
        /// </summary>
        public string ResourceNameMessage { get; set; } = "";

        /// <summary>
        /// The ACL resource pattern type.
        /// </summary>
        public sbyte PatternTypeMessage { get; set; } = 3;

        /// <summary>
        /// The ACL principal.
        /// </summary>
        public string PrincipalMessage { get; set; } = "";

        /// <summary>
        /// The ACL host.
        /// </summary>
        public string HostMessage { get; set; } = "";

        /// <summary>
        /// The ACL operation.
        /// </summary>
        public sbyte OperationMessage { get; set; } = 0;

        /// <summary>
        /// The ACL permission type.
        /// </summary>
        public sbyte PermissionTypeMessage { get; set; } = 0;

        public DeleteAclsMatchingAclMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DeleteAclsMatchingAclMessage(BufferReader reader, ApiVersions version)
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
