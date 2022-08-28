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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class DescribeAclsRequestMessage: RequestMessage, IEquatable<DescribeAclsRequestMessage>
{
    /// <summary>
    /// The resource type.
    /// </summary>
    public sbyte ResourceTypeFilter { get; set; } = 0;

    /// <summary>
    /// The resource name, or null to match any resource name.
    /// </summary>
    public string ResourceNameFilter { get; set; } = string.Empty;

    /// <summary>
    /// The resource pattern to match.
    /// </summary>
    public sbyte PatternTypeFilter { get; set; } = 3;

    /// <summary>
    /// The principal to match, or null to match any principal.
    /// </summary>
    public string PrincipalFilter { get; set; } = string.Empty;

    /// <summary>
    /// The host to match, or null to match any host.
    /// </summary>
    public string HostFilter { get; set; } = string.Empty;

    /// <summary>
    /// The operation to match.
    /// </summary>
    public sbyte Operation { get; set; } = 0;

    /// <summary>
    /// The permission type to match.
    /// </summary>
    public sbyte PermissionType { get; set; } = 0;

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

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteSByte(ResourceTypeFilter);
        if (ResourceNameFilter is null)
        {
            if (version >= ApiVersions.Version2)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteShort(-1);
            }
        }
        else
        {
            var stringBytes = Encoding.UTF8.GetBytes(ResourceNameFilter);
            if (version >= ApiVersions.Version2)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersions.Version1)
        {
            writer.WriteSByte(PatternTypeFilter);
        }
        else
        {
            if (PatternTypeFilter != 3)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default PatternTypeFilter at version {version}");
            }
        }
        if (PrincipalFilter is null)
        {
            if (version >= ApiVersions.Version2)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteShort(-1);
            }
        }
        else
        {
            var stringBytes = Encoding.UTF8.GetBytes(PrincipalFilter);
            if (version >= ApiVersions.Version2)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (HostFilter is null)
        {
            if (version >= ApiVersions.Version2)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteShort(-1);
            }
        }
        else
        {
            var stringBytes = Encoding.UTF8.GetBytes(HostFilter);
            if (version >= ApiVersions.Version2)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        writer.WriteSByte(Operation);
        writer.WriteSByte(PermissionType);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version2)
        {
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }
        else
        {
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is DescribeAclsRequestMessage other && Equals(other);
    }

    public bool Equals(DescribeAclsRequestMessage? other)
    {
        return true;
    }
}
