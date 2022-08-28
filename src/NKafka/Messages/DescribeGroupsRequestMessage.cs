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

public sealed class DescribeGroupsRequestMessage: IRequestMessage, IEquatable<DescribeGroupsRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version5;

    public ApiKeys ApiKey => ApiKeys.DescribeGroups;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The names of the groups to describe
    /// </summary>
    public List<string> Groups { get; set; } = new ();

    /// <summary>
    /// Whether to include authorized operations.
    /// </summary>
    public bool IncludeAuthorizedOperations { get; set; } = false;

    public DescribeGroupsRequestMessage()
    {
    }

    public DescribeGroupsRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            if (version >= ApiVersions.Version5)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Groups was serialized as null");
                }
                else
                {
                    var newCollection = new List<string>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        int length;
                        length = reader.ReadVarUInt() - 1;
                        if (length < 0)
                        {
                            throw new Exception("non-nullable field Groups element was serialized as null");
                        }
                        else if (length > 0x7fff)
                        {
                            throw new Exception($"string field Groups element had invalid length {length}");
                        }
                        else
                        {
                            newCollection.Add(reader.ReadString(length));
                        }
                    }
                    Groups = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Groups was serialized as null");
                }
                else
                {
                    var newCollection = new List<string>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        int length;
                        length = reader.ReadShort();
                        if (length < 0)
                        {
                            throw new Exception("non-nullable field Groups element was serialized as null");
                        }
                        else if (length > 0x7fff)
                        {
                            throw new Exception($"string field Groups element had invalid length {length}");
                        }
                        else
                        {
                            newCollection.Add(reader.ReadString(length));
                        }
                    }
                    Groups = newCollection;
                }
            }
        }
        if (version >= ApiVersions.Version3)
        {
            IncludeAuthorizedOperations = reader.ReadByte() != 0;
        }
        else
        {
            IncludeAuthorizedOperations = false;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version5)
        {
            var numTaggedFields = reader.ReadVarUInt();
            for (var t = 0; t < numTaggedFields; t++)
            {
                var tag = reader.ReadVarUInt();
                var size = reader.ReadVarUInt();
                switch (tag)
                {
                    default:
                        UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                        break;
                }
            }
        }
    }

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version5)
        {
            writer.WriteVarUInt(Groups.Count + 1);
            foreach (var element in Groups)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(element);
                    writer.WriteVarUInt(stringBytes.Length + 1);
                    writer.WriteBytes(stringBytes);
                }
            }
        }
        else
        {
            writer.WriteInt(Groups.Count);
            foreach (var element in Groups)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(element);
                    writer.WriteShort((short)stringBytes.Length);
                    writer.WriteBytes(stringBytes);
                }
            }
        }
        if (version >= ApiVersions.Version3)
        {
            writer.WriteBool(IncludeAuthorizedOperations);
        }
        else
        {
            if (IncludeAuthorizedOperations)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default IncludeAuthorizedOperations at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version5)
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
        return ReferenceEquals(this, obj) || obj is DescribeGroupsRequestMessage other && Equals(other);
    }

    public bool Equals(DescribeGroupsRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Groups, IncludeAuthorizedOperations);
        return hashCode;
    }

    public override string ToString()
    {
        return "DescribeGroupsRequestMessage("
            + ", IncludeAuthorizedOperations=" + (IncludeAuthorizedOperations ? "true" : "false")
            + ")";
    }
}
