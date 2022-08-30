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

public sealed class DescribeProducersResponseMessage: IResponseMessage, IEquatable<DescribeProducersResponseMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// Each topic in the response.
    /// </summary>
    public List<TopicResponseMessage> Topics { get; set; } = new ();

    public DescribeProducersResponseMessage()
    {
    }

    public DescribeProducersResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ThrottleTimeMs = reader.ReadInt();
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Topics was serialized as null");
            }
            else
            {
                var newCollection = new List<TopicResponseMessage>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new TopicResponseMessage(reader, version));
                }
                Topics = newCollection;
            }
        }
        UnknownTaggedFields = null;
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

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteVarUInt(Topics.Count + 1);
        foreach (var element in Topics)
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
        return ReferenceEquals(this, obj) || obj is DescribeProducersResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeProducersResponseMessage? other)
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
        return "DescribeProducersResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }

    public sealed class TopicResponseMessage: IMessage, IEquatable<TopicResponseMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Each partition in the response.
        /// </summary>
        public List<PartitionResponseMessage> Partitions { get; set; } = new ();

        public TopicResponseMessage()
        {
        }

        public TopicResponseMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TopicResponseMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
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
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<PartitionResponseMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new PartitionResponseMessage(reader, version));
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteVarUInt(Partitions.Count + 1);
            foreach (var element in Partitions)
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
            return ReferenceEquals(this, obj) || obj is TopicResponseMessage other && Equals(other);
        }

        public bool Equals(TopicResponseMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name, Partitions);
            return hashCode;
        }

        public override string ToString()
        {
            return "TopicResponseMessage("
                + ")";
        }
    }

    public sealed class PartitionResponseMessage: IMessage, IEquatable<PartitionResponseMessage>
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
        /// The partition error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The partition error message, which may be null if no additional details are available
        /// </summary>
        public string? ErrorMessage { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public List<ProducerStateMessage> ActiveProducers { get; set; } = new ();

        public PartitionResponseMessage()
        {
        }

        public PartitionResponseMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of PartitionResponseMessage");
            }
            PartitionIndex = reader.ReadInt();
            ErrorCode = reader.ReadShort();
            {
                int length;
                length = reader.ReadVarUInt() - 1;
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
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field ActiveProducers was serialized as null");
                }
                else
                {
                    var newCollection = new List<ProducerStateMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new ProducerStateMessage(reader, version));
                    }
                    ActiveProducers = newCollection;
                }
            }
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteShort((short)ErrorCode);
            if (ErrorMessage is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(ErrorMessage);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteVarUInt(ActiveProducers.Count + 1);
            foreach (var element in ActiveProducers)
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
            return ReferenceEquals(this, obj) || obj is PartitionResponseMessage other && Equals(other);
        }

        public bool Equals(PartitionResponseMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex, ErrorCode, ErrorMessage, ActiveProducers);
            return hashCode;
        }

        public override string ToString()
        {
            return "PartitionResponseMessage("
                + "PartitionIndex=" + PartitionIndex
                + ", ErrorCode=" + ErrorCode
                + ")";
        }
    }

    public sealed class ProducerStateMessage: IMessage, IEquatable<ProducerStateMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public long ProducerId { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int ProducerEpoch { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int LastSequence { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public long LastTimestamp { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public int CoordinatorEpoch { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long CurrentTxnStartOffset { get; set; } = -1;

        public ProducerStateMessage()
        {
        }

        public ProducerStateMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of ProducerStateMessage");
            }
            ProducerId = reader.ReadLong();
            ProducerEpoch = reader.ReadInt();
            LastSequence = reader.ReadInt();
            LastTimestamp = reader.ReadLong();
            CoordinatorEpoch = reader.ReadInt();
            CurrentTxnStartOffset = reader.ReadLong();
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteLong(ProducerId);
            writer.WriteInt(ProducerEpoch);
            writer.WriteInt(LastSequence);
            writer.WriteLong(LastTimestamp);
            writer.WriteInt(CoordinatorEpoch);
            writer.WriteLong(CurrentTxnStartOffset);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is ProducerStateMessage other && Equals(other);
        }

        public bool Equals(ProducerStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ProducerId, ProducerEpoch, LastSequence, LastTimestamp, CoordinatorEpoch, CurrentTxnStartOffset);
            return hashCode;
        }

        public override string ToString()
        {
            return "ProducerStateMessage("
                + "ProducerId=" + ProducerId
                + ", ProducerEpoch=" + ProducerEpoch
                + ", LastSequence=" + LastSequence
                + ", LastTimestamp=" + LastTimestamp
                + ", CoordinatorEpoch=" + CoordinatorEpoch
                + ", CurrentTxnStartOffset=" + CurrentTxnStartOffset
                + ")";
        }
    }
}
