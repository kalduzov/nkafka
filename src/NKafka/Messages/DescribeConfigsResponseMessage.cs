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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class DescribeConfigsResponseMessage: IResponseMessage, IEquatable<DescribeConfigsResponseMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version4;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The results for each resource.
    /// </summary>
    public List<DescribeConfigsResultMessage> Results { get; set; } = new ();

    public DescribeConfigsResponseMessage()
    {
    }

    public DescribeConfigsResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ThrottleTimeMs = reader.ReadInt();
        {
            if (version >= ApiVersion.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Results was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribeConfigsResultMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribeConfigsResultMessage(reader, version));
                    }
                    Results = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Results was serialized as null");
                }
                else
                {
                    var newCollection = new List<DescribeConfigsResultMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new DescribeConfigsResultMessage(reader, version));
                    }
                    Results = newCollection;
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
        writer.WriteInt(ThrottleTimeMs);
        if (version >= ApiVersion.Version4)
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
        return ReferenceEquals(this, obj) || obj is DescribeConfigsResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeConfigsResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, Results);
        return hashCode;
    }

    public override string ToString()
    {
        return "DescribeConfigsResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }

    public sealed class DescribeConfigsResultMessage: IMessage, IEquatable<DescribeConfigsResultMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version4;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The error code, or 0 if we were able to successfully describe the configurations.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

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
        }

        public DescribeConfigsResultMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version4)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribeConfigsResultMessage");
            }
            ErrorCode = reader.ReadShort();
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
                    ErrorMessage = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ErrorMessage had invalid length {length}");
                }
                else
                {
                    ErrorMessage = reader.ReadString(length);
                }
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
                        throw new Exception("non-nullable field Configs was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DescribeConfigsResourceResultMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DescribeConfigsResourceResultMessage(reader, version));
                        }
                        Configs = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Configs was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DescribeConfigsResourceResultMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DescribeConfigsResourceResultMessage(reader, version));
                        }
                        Configs = newCollection;
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
            writer.WriteShort((short)ErrorCode);
            if (ErrorMessage is null)
            {
                if (version >= ApiVersion.Version4)
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
            return ReferenceEquals(this, obj) || obj is DescribeConfigsResultMessage other && Equals(other);
        }

        public bool Equals(DescribeConfigsResultMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ErrorCode, ErrorMessage, ResourceType, ResourceName, Configs);
            return hashCode;
        }

        public override string ToString()
        {
            return "DescribeConfigsResultMessage("
                + "ErrorCode=" + ErrorCode
                + ", ResourceType=" + ResourceType
                + ")";
        }
    }

    public sealed class DescribeConfigsResourceResultMessage: IMessage, IEquatable<DescribeConfigsResourceResultMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version4;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

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
        }

        public DescribeConfigsResourceResultMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version4)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribeConfigsResourceResultMessage");
            }
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
                    throw new Exception("non-nullable field Name was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Name had invalid length {length}");
                }
                else
                {
                    Name = reader.ReadString(length);
                }
            }
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
                    Value = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Value had invalid length {length}");
                }
                else
                {
                    Value = reader.ReadString(length);
                }
            }
            ReadOnly = reader.ReadByte() != 0;
            if (version <= ApiVersion.Version0)
            {
                IsDefault = reader.ReadByte() != 0;
            }
            else
            {
                IsDefault = false;
            }
            if (version >= ApiVersion.Version1)
            {
                ConfigSource = reader.ReadSByte();
            }
            else
            {
                ConfigSource = -1;
            }
            IsSensitive = reader.ReadByte() != 0;
            if (version >= ApiVersion.Version1)
            {
                if (version >= ApiVersion.Version4)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Synonyms was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DescribeConfigsSynonymMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DescribeConfigsSynonymMessage(reader, version));
                        }
                        Synonyms = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Synonyms was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<DescribeConfigsSynonymMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new DescribeConfigsSynonymMessage(reader, version));
                        }
                        Synonyms = newCollection;
                    }
                }
            }
            else
            {
                Synonyms = new ();
            }
            if (version >= ApiVersion.Version3)
            {
                ConfigType = reader.ReadSByte();
            }
            else
            {
                ConfigType = 0;
            }
            if (version >= ApiVersion.Version3)
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
                    Documentation = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Documentation had invalid length {length}");
                }
                else
                {
                    Documentation = reader.ReadString(length);
                }
            }
            else
            {
                Documentation = string.Empty;
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
            if (Value is null)
            {
                if (version >= ApiVersion.Version4)
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
            writer.WriteBool(ReadOnly);
            if (version <= ApiVersion.Version0)
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
            if (version >= ApiVersion.Version1)
            {
                writer.WriteSByte(ConfigSource);
            }
            writer.WriteBool(IsSensitive);
            if (version >= ApiVersion.Version1)
            {
                if (version >= ApiVersion.Version4)
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
            if (version >= ApiVersion.Version3)
            {
                writer.WriteSByte(ConfigType);
            }
            if (version >= ApiVersion.Version3)
            {
                if (Documentation is null)
                {
                    if (version >= ApiVersion.Version4)
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
            return ReferenceEquals(this, obj) || obj is DescribeConfigsResourceResultMessage other && Equals(other);
        }

        public bool Equals(DescribeConfigsResourceResultMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name, Value, ReadOnly, IsDefault, ConfigSource, IsSensitive, Synonyms);
            hashCode = HashCode.Combine(hashCode, ConfigType, Documentation);
            return hashCode;
        }

        public override string ToString()
        {
            return "DescribeConfigsResourceResultMessage("
                + ", ReadOnly=" + (ReadOnly ? "true" : "false")
                + ", IsDefault=" + (IsDefault ? "true" : "false")
                + ", ConfigSource=" + ConfigSource
                + ", IsSensitive=" + (IsSensitive ? "true" : "false")
                + ", ConfigType=" + ConfigType
                + ")";
        }
    }

    public sealed class DescribeConfigsSynonymMessage: IMessage, IEquatable<DescribeConfigsSynonymMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version4;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

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
        }

        public DescribeConfigsSynonymMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version4)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribeConfigsSynonymMessage");
            }
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
                    throw new Exception("non-nullable field Name was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Name had invalid length {length}");
                }
                else
                {
                    Name = reader.ReadString(length);
                }
            }
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
                    Value = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Value had invalid length {length}");
                }
                else
                {
                    Value = reader.ReadString(length);
                }
            }
            Source = reader.ReadSByte();
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
            if (version < ApiVersion.Version1)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of DescribeConfigsSynonymMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
            if (Value is null)
            {
                if (version >= ApiVersion.Version4)
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
            writer.WriteSByte(Source);
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
            return ReferenceEquals(this, obj) || obj is DescribeConfigsSynonymMessage other && Equals(other);
        }

        public bool Equals(DescribeConfigsSynonymMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name, Value, Source);
            return hashCode;
        }

        public override string ToString()
        {
            return "DescribeConfigsSynonymMessage("
                + ", Source=" + Source
                + ")";
        }
    }
}
