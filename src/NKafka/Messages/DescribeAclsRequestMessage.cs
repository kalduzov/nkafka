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

public sealed class DescribeAclsRequestMessage: RequestMessage
{
    /// <summary>
    /// The resource type.
    /// </summary>
    public sbyte ResourceTypeFilterMessage { get; set; } = 0;

    /// <summary>
    /// The resource name, or null to match any resource name.
    /// </summary>
    public string ResourceNameFilterMessage { get; set; } = "";

    /// <summary>
    /// The resource pattern to match.
    /// </summary>
    public sbyte PatternTypeFilterMessage { get; set; } = 3;

    /// <summary>
    /// The principal to match, or null to match any principal.
    /// </summary>
    public string PrincipalFilterMessage { get; set; } = "";

    /// <summary>
    /// The host to match, or null to match any host.
    /// </summary>
    public string HostFilterMessage { get; set; } = "";

    /// <summary>
    /// The operation to match.
    /// </summary>
    public sbyte OperationMessage { get; set; } = 0;

    /// <summary>
    /// The permission type to match.
    /// </summary>
    public sbyte PermissionTypeMessage { get; set; } = 0;

    public DescribeAclsRequestMessage()
    {
        ApiKey = ApiKeys.DescribeAcls;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public DescribeAclsRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeAcls;
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
