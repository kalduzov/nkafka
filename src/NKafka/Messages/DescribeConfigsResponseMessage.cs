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

public sealed class DescribeConfigsResponseMessage: ResponseMessage, IEquatable<DescribeConfigsResponseMessage>
{
    /// <summary>
    /// The results for each resource.
    /// </summary>
    public List<DescribeConfigsResultMessage> Results { get; set; } = new ();

    public DescribeConfigsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public DescribeConfigsResponseMessage(BufferReader reader, ApiVersions version)
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
        writer.WriteInt(ThrottleTimeMs);
        if (version >= ApiVersions.Version4)
        {
            writer.WriteVarUInt(Results.Count + 1);
            foreach (var element in Results)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Results.Count);
            foreach (var element in Results)
            {
                element.Write(writer, version);
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
        return ReferenceEquals(this, obj) || obj is DescribeConfigsResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeConfigsResponseMessage? other)
    {
        return true;
    }

    public sealed class DescribeConfigsResultMessage: Message, IEquatable<DescribeConfigsResultMessage>
    {
        /// <summary>
        /// The error code, or 0 if we were able to successfully describe the configurations.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The error message, or null if we were able to successfully describe the configurations.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceType { get; set; } = 0;

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceName { get; set; } = string.Empty;

        /// <summary>
        /// Each listed configuration.
        /// </summary>
        public List<DescribeConfigsResourceResultMessage> Configs { get; set; } = new ();

        public DescribeConfigsResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public DescribeConfigsResultMessage(BufferReader reader, ApiVersions version)
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
            writer.WriteShort(ErrorCode);
            if (ErrorMessage is null)
            {
                if (version >= ApiVersions.Version4)
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
                var stringBytes = Encoding.UTF8.GetBytes(ErrorMessage);
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
                writer.WriteVarUInt(Configs.Count + 1);
                foreach (var element in Configs)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Configs.Count);
                foreach (var element in Configs)
                {
                    element.Write(writer, version);
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
            return ReferenceEquals(this, obj) || obj is DescribeConfigsResultMessage other && Equals(other);
        }

        public bool Equals(DescribeConfigsResultMessage? other)
        {
            return true;
        }
    }

    public sealed class DescribeConfigsResourceResultMessage: Message, IEquatable<DescribeConfigsResourceResultMessage>
    {
        /// <summary>
        /// The configuration name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The configuration value.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// True if the configuration is read-only.
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// True if the configuration is not set.
        /// </summary>
        public bool IsDefault { get; set; } = false;

        /// <summary>
        /// The configuration source.
        /// </summary>
        public sbyte ConfigSource { get; set; } = -1;

        /// <summary>
        /// True if this configuration is sensitive.
        /// </summary>
        public bool IsSensitive { get; set; } = false;

        /// <summary>
        /// The synonyms for this configuration key.
        /// </summary>
        public List<DescribeConfigsSynonymMessage> Synonyms { get; set; } = new ();

        /// <summary>
        /// The configuration data type. Type can be one of the following values - BOOLEAN, STRING, INT, SHORT, LONG, DOUBLE, LIST, CLASS, PASSWORD
        /// </summary>
        public sbyte ConfigType { get; set; } = 0;

        /// <summary>
        /// The configuration documentation.
        /// </summary>
        public string Documentation { get; set; } = string.Empty;

        public DescribeConfigsResourceResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public DescribeConfigsResourceResultMessage(BufferReader reader, ApiVersions version)
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
            if (Value is null)
            {
                if (version >= ApiVersions.Version4)
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
                var stringBytes = Encoding.UTF8.GetBytes(Value);
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
            writer.WriteBool(ReadOnly);
            if (version <= ApiVersions.Version0)
            {
                writer.WriteBool(IsDefault);
            }
            else
            {
                if (IsDefault)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default IsDefault at version {version}");
                }
            }
            if (version >= ApiVersions.Version1)
            {
                writer.WriteSByte(ConfigSource);
            }
            writer.WriteBool(IsSensitive);
            if (version >= ApiVersions.Version1)
            {
                if (version >= ApiVersions.Version4)
                {
                    writer.WriteVarUInt(Synonyms.Count + 1);
                    foreach (var element in Synonyms)
                    {
                        element.Write(writer, version);
                    }
                }
                else
                {
                    writer.WriteInt(Synonyms.Count);
                    foreach (var element in Synonyms)
                    {
                        element.Write(writer, version);
                    }
                }
            }
            if (version >= ApiVersions.Version3)
            {
                writer.WriteSByte(ConfigType);
            }
            if (version >= ApiVersions.Version3)
            {
                if (Documentation is null)
                {
                    if (version >= ApiVersions.Version4)
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
                    var stringBytes = Encoding.UTF8.GetBytes(Documentation);
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
            return ReferenceEquals(this, obj) || obj is DescribeConfigsResourceResultMessage other && Equals(other);
        }

        public bool Equals(DescribeConfigsResourceResultMessage? other)
        {
            return true;
        }
    }

    public sealed class DescribeConfigsSynonymMessage: Message, IEquatable<DescribeConfigsSynonymMessage>
    {
        /// <summary>
        /// The synonym name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The synonym value.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// The synonym source.
        /// </summary>
        public sbyte Source { get; set; } = 0;

        public DescribeConfigsSynonymMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public DescribeConfigsSynonymMessage(BufferReader reader, ApiVersions version)
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
            if (version < ApiVersions.Version1)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of DescribeConfigsSynonymMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
            if (Value is null)
            {
                if (version >= ApiVersions.Version4)
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
                var stringBytes = Encoding.UTF8.GetBytes(Value);
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
            writer.WriteSByte(Source);
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
            return ReferenceEquals(this, obj) || obj is DescribeConfigsSynonymMessage other && Equals(other);
        }

        public bool Equals(DescribeConfigsSynonymMessage? other)
        {
            return true;
        }
    }
}
