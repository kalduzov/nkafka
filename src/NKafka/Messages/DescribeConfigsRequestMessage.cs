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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class DescribeConfigsRequestMessage: RequestMessage, IEquatable<DescribeConfigsRequestMessage>
{
    /// <summary>
    /// The resources whose configurations we want to describe.
    /// </summary>
    public List<DescribeConfigsResourceMessage> Resources { get; set; } = new ();

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
        ApiKey = ApiKeys.DescribeConfigs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public DescribeConfigsRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeConfigs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version4)
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
        if (version >= ApiVersions.Version1)
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
        if (version >= ApiVersions.Version3)
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
        if (version >= ApiVersions.Version4)
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

    public sealed class DescribeConfigsResourceMessage: Message, IEquatable<DescribeConfigsResourceMessage>
    {
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
        public List<string> ConfigurationKeys { get; set; } = new ();

        public DescribeConfigsResourceMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public DescribeConfigsResourceMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteSByte(ResourceType);
            {
                var stringBytes = Encoding.UTF8.GetBytes(ResourceName);
                if (version >= ApiVersions.Version4)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version4)
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
            if (version >= ApiVersions.Version4)
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
    }
}
