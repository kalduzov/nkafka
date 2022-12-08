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

// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using System.Text;

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;

namespace NKafka.Messages;

public sealed class DescribeConfigsRequestMessage: IRequestMessage, IEquatable<DescribeConfigsRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version4;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.DescribeConfigs;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The resources whose configurations we want to describe.
    /// </summary>
    public List<DescribeConfigsResourceMessage> Resources { get; set; } = new();

    /// <summary>
    /// True if we should include all synonyms.
    /// </summary>
    public bool IncludeSynonyms { get; set; } = false;

    /// <summary>
    /// True if we should include configuration documentation.
    /// </summary>
    public bool IncludeDocumentation { get; set; } = false;

    public DescribeConfigsRequestMessage()
    {
    }

    public DescribeConfigsRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            if (version >= ApiVersion.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Resources was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribeConfigsResourceMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribeConfigsResourceMessage(reader, version));
                    }
                    Resources = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Resources was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribeConfigsResourceMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribeConfigsResourceMessage(reader, version));
                    }
                    Resources = newCollection;
                }
            }
        }
        if (version >= ApiVersion.Version1)
        {
            IncludeSynonyms = reader.ReadByte() != 0;
        }
        else
        {
            IncludeSynonyms = false;
        }
        if (version >= ApiVersion.Version3)
        {
            IncludeDocumentation = reader.ReadByte() != 0;
        }
        else
        {
            IncludeDocumentation = false;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version4)
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

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersion.Version4)
        {
            writer.WriteVarUInt(Resources.Count + 1);
            foreach (var element in Resources)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Resources.Count);
            foreach (var element in Resources)
            {
                element.Write(writer, version);
            }
        }
        if (version >= ApiVersion.Version1)
        {
            writer.WriteBool(IncludeSynonyms);
        }
        else
        {
            if (IncludeSynonyms)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default IncludeSynonyms at version {version}");
            }
        }
        if (version >= ApiVersion.Version3)
        {
            writer.WriteBool(IncludeDocumentation);
        }
        else
        {
            if (IncludeDocumentation)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default IncludeDocumentation at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version4)
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
        return ReferenceEquals(this, obj) || obj is DescribeConfigsRequestMessage other && Equals(other);
    }

    public bool Equals(DescribeConfigsRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Resources, IncludeSynonyms, IncludeDocumentation);
        return hashCode;
    }

    public override string ToString()
    {
        return "DescribeConfigsRequestMessage("
            + ", IncludeSynonyms=" + (IncludeSynonyms ? "true" : "false")
            + ", IncludeDocumentation=" + (IncludeDocumentation ? "true" : "false")
            + ")";
    }

    public sealed class DescribeConfigsResourceMessage: IMessage, IEquatable<DescribeConfigsResourceMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version4;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceType { get; set; } = 0;

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceName { get; set; } = string.Empty;

        /// <summary>
        /// The configuration keys to list, or null to list all configuration keys.
        /// </summary>
        public List<string> ConfigurationKeys { get; set; } = new();

        public DescribeConfigsResourceMessage()
        {
        }

        public DescribeConfigsResourceMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version4)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribeConfigsResourceMessage");
            }
            ResourceType = reader.ReadSByte();
            {
                int length;
                if (version >= ApiVersion.Version4)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field ResourceName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ResourceName had invalid length {length}");
                }
                else
                {
                    ResourceName = reader.ReadString(length);
                }
            }
            {
                if (version >= ApiVersion.Version4)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        ConfigurationKeys = null;
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
                                throw new Exception("non-nullable field ConfigurationKeys element was serialized as null");
                            }
                            else if (length > 0x7fff)
                            {
                                throw new Exception($"string field ConfigurationKeys element had invalid length {length}");
                            }
                            else
                            {
                                newCollection.Add(reader.ReadString(length));
                            }
                        }
                        ConfigurationKeys = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        ConfigurationKeys = null;
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
                                throw new Exception("non-nullable field ConfigurationKeys element was serialized as null");
                            }
                            else if (length > 0x7fff)
                            {
                                throw new Exception($"string field ConfigurationKeys element had invalid length {length}");
                            }
                            else
                            {
                                newCollection.Add(reader.ReadString(length));
                            }
                        }
                        ConfigurationKeys = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version4)
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteSByte(ResourceType);
            {
                var stringBytes = Encoding.UTF8.GetBytes(ResourceName);
                if (version >= ApiVersion.Version4)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version4)
            {
                if (ConfigurationKeys is null)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteVarUInt(ConfigurationKeys.Count + 1);
                    foreach (var element in ConfigurationKeys)
                    {
                        {
                            var stringBytes = Encoding.UTF8.GetBytes(element);
                            writer.WriteVarUInt(stringBytes.Length + 1);
                            writer.WriteBytes(stringBytes);
                        }
                    }
                }
            }
            else
            {
                if (ConfigurationKeys is null)
                {
                    writer.WriteInt(-1);
                }
                else
                {
                    writer.WriteInt(ConfigurationKeys.Count);
                    foreach (var element in ConfigurationKeys)
                    {
                        {
                            var stringBytes = Encoding.UTF8.GetBytes(element);
                            writer.WriteShort((short)stringBytes.Length);
                            writer.WriteBytes(stringBytes);
                        }
                    }
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version4)
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
            return ReferenceEquals(this, obj) || obj is DescribeConfigsResourceMessage other && Equals(other);
        }

        public bool Equals(DescribeConfigsResourceMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ResourceType, ResourceName, ConfigurationKeys);
            return hashCode;
        }

        public override string ToString()
        {
            return "DescribeConfigsResourceMessage("
                + "ResourceType=" + ResourceType
                + ")";
        }
    }
}