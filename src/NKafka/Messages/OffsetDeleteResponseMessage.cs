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

public sealed class OffsetDeleteResponseMessage: IResponseMessage, IEquatable<OffsetDeleteResponseMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The top-level error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The responses for each topic.
    /// </summary>
    public OffsetDeleteResponseTopicCollection Topics { get; set; } = new ();

    public OffsetDeleteResponseMessage()
    {
    }

    public OffsetDeleteResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ErrorCode = reader.ReadShort();
        ThrottleTimeMs = reader.ReadInt();
        {
            int arrayLength;
            arrayLength = reader.ReadInt();
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Topics was serialized as null");
            }
            else
            {
                var newCollection = new OffsetDeleteResponseTopicCollection(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new OffsetDeleteResponseTopicMessage(reader, version));
                }
                Topics = newCollection;
            }
        }
        UnknownTaggedFields = null;
    }

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        writer.WriteShort((short)ErrorCode);
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteInt(Topics.Count);
        foreach (var element in Topics)
        {
            element.Write(writer, version);
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (numTaggedFields > 0)
        {
            throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
        }
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is OffsetDeleteResponseMessage other && Equals(other);
    }

    public bool Equals(OffsetDeleteResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ErrorCode, ThrottleTimeMs, Topics);
        return hashCode;
    }

    public override string ToString()
    {
        return "OffsetDeleteResponseMessage("
            + "ErrorCode=" + ErrorCode
            + ", ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }

    public sealed class OffsetDeleteResponseTopicMessage: IMessage, IEquatable<OffsetDeleteResponseTopicMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The responses for each partition in the topic.
        /// </summary>
        public OffsetDeleteResponsePartitionCollection Partitions { get; set; } = new ();

        public OffsetDeleteResponseTopicMessage()
        {
        }

        public OffsetDeleteResponseTopicMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetDeleteResponseTopicMessage");
            }
            {
                int length;
                length = reader.ReadShort();
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
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new OffsetDeleteResponsePartitionCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new OffsetDeleteResponsePartitionMessage(reader, version));
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
        }

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteShort((short)stringBytes.Length);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteInt(Partitions.Count);
            foreach (var element in Partitions)
            {
                element.Write(writer, version);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetDeleteResponseTopicMessage other && Equals(other);
        }

        public bool Equals(OffsetDeleteResponseTopicMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }

        public override string ToString()
        {
            return "OffsetDeleteResponseTopicMessage("
                + ")";
        }
    }

    public sealed class OffsetDeleteResponsePartitionMessage: IMessage, IEquatable<OffsetDeleteResponsePartitionMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        public OffsetDeleteResponsePartitionMessage()
        {
        }

        public OffsetDeleteResponsePartitionMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetDeleteResponsePartitionMessage");
            }
            PartitionIndex = reader.ReadInt();
            ErrorCode = reader.ReadShort();
            UnknownTaggedFields = null;
        }

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteShort((short)ErrorCode);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetDeleteResponsePartitionMessage other && Equals(other);
        }

        public bool Equals(OffsetDeleteResponsePartitionMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex);
            return hashCode;
        }

        public override string ToString()
        {
            return "OffsetDeleteResponsePartitionMessage("
                + "PartitionIndex=" + PartitionIndex
                + ", ErrorCode=" + ErrorCode
                + ")";
        }
    }

    public sealed class OffsetDeleteResponsePartitionCollection: HashSet<OffsetDeleteResponsePartitionMessage>
    {
        public OffsetDeleteResponsePartitionCollection()
        {
        }

        public OffsetDeleteResponsePartitionCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class OffsetDeleteResponseTopicCollection: HashSet<OffsetDeleteResponseTopicMessage>
    {
        public OffsetDeleteResponseTopicCollection()
        {
        }

        public OffsetDeleteResponseTopicCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
