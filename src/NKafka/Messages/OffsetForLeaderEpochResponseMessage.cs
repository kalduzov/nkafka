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

public sealed class OffsetForLeaderEpochResponseMessage: IResponseMessage, IEquatable<OffsetForLeaderEpochResponseMessage>
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
    /// Each topic we fetched offsets for.
    /// </summary>
    public OffsetForLeaderTopicResultCollection Topics { get; set; } = new ();

    public OffsetForLeaderEpochResponseMessage()
    {
    }

    public OffsetForLeaderEpochResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        if (version >= ApiVersion.Version2)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        {
            if (version >= ApiVersion.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new OffsetForLeaderTopicResultCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new OffsetForLeaderTopicResultMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new OffsetForLeaderTopicResultCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new OffsetForLeaderTopicResultMessage(reader, version));
                    }
                    Topics = newCollection;
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
        if (version >= ApiVersion.Version2)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersion.Version4)
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
        return ReferenceEquals(this, obj) || obj is OffsetForLeaderEpochResponseMessage other && Equals(other);
    }

    public bool Equals(OffsetForLeaderEpochResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, Topics);
        return hashCode;
    }

    public override string ToString()
    {
        return "OffsetForLeaderEpochResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }

    public sealed class OffsetForLeaderTopicResultMessage: IMessage, IEquatable<OffsetForLeaderTopicResultMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version4;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// Each partition in the topic we fetched offsets for.
        /// </summary>
        public List<EpochEndOffsetMessage> Partitions { get; set; } = new ();

        public OffsetForLeaderTopicResultMessage()
        {
        }

        public OffsetForLeaderTopicResultMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version4)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetForLeaderTopicResultMessage");
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
                    throw new Exception("non-nullable field Topic was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Topic had invalid length {length}");
                }
                else
                {
                    Topic = reader.ReadString(length);
                }
            }
            {
                if (version >= ApiVersion.Version4)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<EpochEndOffsetMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new EpochEndOffsetMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<EpochEndOffsetMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new EpochEndOffsetMessage(reader, version));
                        }
                        Partitions = newCollection;
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Topic);
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
                writer.WriteVarUInt(Partitions.Count + 1);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Partitions.Count);
                foreach (var element in Partitions)
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
            return ReferenceEquals(this, obj) || obj is OffsetForLeaderTopicResultMessage other && Equals(other);
        }

        public bool Equals(OffsetForLeaderTopicResultMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Topic);
            return hashCode;
        }

        public override string ToString()
        {
            return "OffsetForLeaderTopicResultMessage("
                + ")";
        }
    }

    public sealed class EpochEndOffsetMessage: IMessage, IEquatable<EpochEndOffsetMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version4;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The error code 0, or if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int Partition { get; set; } = 0;

        /// <summary>
        /// The leader epoch of the partition.
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The end offset of the epoch.
        /// </summary>
        public long EndOffset { get; set; } = -1;

        public EpochEndOffsetMessage()
        {
        }

        public EpochEndOffsetMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version4)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of EpochEndOffsetMessage");
            }
            ErrorCode = reader.ReadShort();
            Partition = reader.ReadInt();
            if (version >= ApiVersion.Version1)
            {
                LeaderEpoch = reader.ReadInt();
            }
            else
            {
                LeaderEpoch = -1;
            }
            EndOffset = reader.ReadLong();
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
            writer.WriteInt(Partition);
            if (version >= ApiVersion.Version1)
            {
                writer.WriteInt(LeaderEpoch);
            }
            writer.WriteLong(EndOffset);
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
            return ReferenceEquals(this, obj) || obj is EpochEndOffsetMessage other && Equals(other);
        }

        public bool Equals(EpochEndOffsetMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ErrorCode, Partition, LeaderEpoch, EndOffset);
            return hashCode;
        }

        public override string ToString()
        {
            return "EpochEndOffsetMessage("
                + "ErrorCode=" + ErrorCode
                + ", Partition=" + Partition
                + ", LeaderEpoch=" + LeaderEpoch
                + ", EndOffset=" + EndOffset
                + ")";
        }
    }

    public sealed class OffsetForLeaderTopicResultCollection: HashSet<OffsetForLeaderTopicResultMessage>
    {
        public OffsetForLeaderTopicResultCollection()
        {
        }

        public OffsetForLeaderTopicResultCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
