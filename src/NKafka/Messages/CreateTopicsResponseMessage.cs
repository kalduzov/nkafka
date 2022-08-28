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

public sealed class CreateTopicsResponseMessage: ResponseMessage, IEquatable<CreateTopicsResponseMessage>
{
    /// <summary>
    /// Results for each topic we tried to create.
    /// </summary>
    public CreatableTopicResultCollection Topics { get; set; } = new ();

    public CreateTopicsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public CreateTopicsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version2)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersions.Version5)
        {
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Topics.Count);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
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
        return ReferenceEquals(this, obj) || obj is CreateTopicsResponseMessage other && Equals(other);
    }

    public bool Equals(CreateTopicsResponseMessage? other)
    {
        return true;
    }

    public sealed class CreatableTopicResultMessage: Message, IEquatable<CreatableTopicResultMessage>
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The error message, or null if there was no error.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Optional topic config error returned if configs are not returned in the response.
        /// </summary>
        public short TopicConfigErrorCode { get; set; } = 0;

        /// <summary>
        /// Number of partitions of the topic.
        /// </summary>
        public int NumPartitions { get; set; } = -1;

        /// <summary>
        /// Replication factor of the topic.
        /// </summary>
        public short ReplicationFactor { get; set; } = -1;

        /// <summary>
        /// Configuration of the topic.
        /// </summary>
        public List<CreatableTopicConfigsMessage> Configs { get; set; } = new ();

        public CreatableTopicResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public CreatableTopicResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersions.Version5)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version7)
            {
                writer.WriteGuid(TopicId);
            }
            writer.WriteShort(ErrorCode);
            if (version >= ApiVersions.Version1)
            {
                if (ErrorMessage is null)
                {
                    if (version >= ApiVersions.Version5)
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
                    if (version >= ApiVersions.Version5)
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
            if (version >= ApiVersions.Version5)
            {
                if (TopicConfigErrorCode != 0)
                {
                    numTaggedFields++;
                }
            }
            if (version >= ApiVersions.Version5)
            {
                writer.WriteInt(NumPartitions);
            }
            if (version >= ApiVersions.Version5)
            {
                writer.WriteShort(ReplicationFactor);
            }
            if (version >= ApiVersions.Version5)
            {
                if (Configs is null)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteVarUInt(Configs.Count + 1);
                    foreach (var element in Configs)
                    {
                        element.Write(writer, version);
                    }
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version5)
            {
                writer.WriteVarUInt(numTaggedFields);
                {
                    if (TopicConfigErrorCode != 0)
                    {
                        writer.WriteVarUInt(0);
                        writer.WriteVarUInt(2);
                        writer.WriteShort(TopicConfigErrorCode);
                    }
                }
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
            return ReferenceEquals(this, obj) || obj is CreatableTopicResultMessage other && Equals(other);
        }

        public bool Equals(CreatableTopicResultMessage? other)
        {
            return true;
        }
    }

    public sealed class CreatableTopicConfigsMessage: Message, IEquatable<CreatableTopicConfigsMessage>
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
        /// The configuration source.
        /// </summary>
        public sbyte ConfigSource { get; set; } = -1;

        /// <summary>
        /// True if this configuration is sensitive.
        /// </summary>
        public bool IsSensitive { get; set; } = false;

        public CreatableTopicConfigsMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public CreatableTopicConfigsMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version5)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of CreatableTopicConfigsMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            if (Value is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(Value);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteBool(ReadOnly);
            writer.WriteSByte(ConfigSource);
            writer.WriteBool(IsSensitive);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is CreatableTopicConfigsMessage other && Equals(other);
        }

        public bool Equals(CreatableTopicConfigsMessage? other)
        {
            return true;
        }
    }

    public sealed class CreatableTopicResultCollection: HashSet<CreatableTopicResultMessage>
    {
        public CreatableTopicResultCollection()
        {
        }

        public CreatableTopicResultCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
