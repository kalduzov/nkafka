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

public sealed class LeaderAndIsrResponseMessage: ResponseMessage, IEquatable<LeaderAndIsrResponseMessage>
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// Each partition in v0 to v4 message.
    /// </summary>
    public List<LeaderAndIsrPartitionErrorMessage> PartitionErrors { get; set; } = new ();

    /// <summary>
    /// Each topic
    /// </summary>
    public LeaderAndIsrTopicErrorCollection Topics { get; set; } = new ();

    public LeaderAndIsrResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version6;
    }

    public LeaderAndIsrResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version6;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteShort(ErrorCode);
        if (version <= ApiVersions.Version4)
        {
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(PartitionErrors.Count + 1);
                foreach (var element in PartitionErrors)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(PartitionErrors.Count);
                foreach (var element in PartitionErrors)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (PartitionErrors.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default PartitionErrors at version {version}");
            }
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
            if (Topics.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Topics at version {version}");
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
        return ReferenceEquals(this, obj) || obj is LeaderAndIsrResponseMessage other && Equals(other);
    }

    public bool Equals(LeaderAndIsrResponseMessage? other)
    {
        return true;
    }

    public sealed class LeaderAndIsrTopicErrorMessage: Message, IEquatable<LeaderAndIsrTopicErrorMessage>
    {
        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// Each partition.
        /// </summary>
        public List<LeaderAndIsrPartitionErrorMessage> PartitionErrors { get; set; } = new ();

        public LeaderAndIsrTopicErrorMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version6;
        }

        public LeaderAndIsrTopicErrorMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version6;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version5)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of LeaderAndIsrTopicErrorMessage");
            }
            var numTaggedFields = 0;
            writer.WriteGuid(TopicId);
            writer.WriteVarUInt(PartitionErrors.Count + 1);
            foreach (var element in PartitionErrors)
            {
                element.Write(writer, version);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is LeaderAndIsrTopicErrorMessage other && Equals(other);
        }

        public bool Equals(LeaderAndIsrTopicErrorMessage? other)
        {
            return true;
        }
    }

    public sealed class LeaderAndIsrTopicErrorCollection: HashSet<LeaderAndIsrTopicErrorMessage>
    {
        public LeaderAndIsrTopicErrorCollection()
        {
        }

        public LeaderAndIsrTopicErrorCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class LeaderAndIsrPartitionErrorMessage: Message, IEquatable<LeaderAndIsrPartitionErrorMessage>
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The partition error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        public LeaderAndIsrPartitionErrorMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version6;
        }

        public LeaderAndIsrPartitionErrorMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version6;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            if (version <= ApiVersions.Version4)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(TopicName);
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
            writer.WriteInt(PartitionIndex);
            writer.WriteShort(ErrorCode);
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
            return ReferenceEquals(this, obj) || obj is LeaderAndIsrPartitionErrorMessage other && Equals(other);
        }

        public bool Equals(LeaderAndIsrPartitionErrorMessage? other)
        {
            return true;
        }
    }
}
