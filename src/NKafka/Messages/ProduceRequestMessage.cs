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

public sealed class ProduceRequestMessage: IRequestMessage, IEquatable<ProduceRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version9;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.Produce;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The transactional ID, or null if the producer is not transactional.
    /// </summary>
    public string? TransactionalId { get; set; } = null;

    /// <summary>
    /// The number of acknowledgments the producer requires the leader to have received before considering a request complete. Allowed values: 0 for no acknowledgments, 1 for only the leader and -1 for the full ISR.
    /// </summary>
    public short Acks { get; set; } = 0;

    /// <summary>
    /// The timeout to await a response in milliseconds.
    /// </summary>
    public int TimeoutMs { get; set; } = 0;

    /// <summary>
    /// Each topic to produce to.
    /// </summary>
    public TopicProduceDataCollection TopicData { get; set; } = new ();

    public ProduceRequestMessage()
    {
    }

    public ProduceRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        if (version >= ApiVersion.Version3)
        {
            int length;
            if (version >= ApiVersion.Version9)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                TransactionalId = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field TransactionalId had invalid length {length}");
            }
            else
            {
                TransactionalId = reader.ReadString(length);
            }
        }
        else
        {
            TransactionalId = null;
        }
        Acks = reader.ReadShort();
        TimeoutMs = reader.ReadInt();
        {
            if (version >= ApiVersion.Version9)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field TopicData was serialized as null");
                }
                else
                {
                    var newCollection = new TopicProduceDataCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new TopicProduceDataMessage(reader, version));
                    }
                    TopicData = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field TopicData was serialized as null");
                }
                else
                {
                    var newCollection = new TopicProduceDataCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new TopicProduceDataMessage(reader, version));
                    }
                    TopicData = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version9)
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
        if (version >= ApiVersion.Version3)
        {
            if (TransactionalId is null)
            {
                if (version >= ApiVersion.Version9)
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
                var stringBytes = Encoding.UTF8.GetBytes(TransactionalId);
                if (version >= ApiVersion.Version9)
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
        else
        {
            if (TransactionalId is not null)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default TransactionalId at version {version}");
            }
        }
        writer.WriteShort(Acks);
        writer.WriteInt(TimeoutMs);
        if (version >= ApiVersion.Version9)
        {
            writer.WriteVarUInt(TopicData.Count + 1);
            foreach (var element in TopicData)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(TopicData.Count);
            foreach (var element in TopicData)
            {
                element.Write(writer, version);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version9)
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
        return ReferenceEquals(this, obj) || obj is ProduceRequestMessage other && Equals(other);
    }

    public bool Equals(ProduceRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, TransactionalId, Acks, TimeoutMs, TopicData);
        return hashCode;
    }

    public override string ToString()
    {
        return "ProduceRequestMessage("
            + ", Acks=" + Acks
            + ", TimeoutMs=" + TimeoutMs
            + ")";
    }

    public sealed class TopicProduceDataMessage: IMessage, IEquatable<TopicProduceDataMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version9;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Each partition to produce to.
        /// </summary>
        public List<PartitionProduceDataMessage> PartitionData { get; set; } = new ();

        public TopicProduceDataMessage()
        {
        }

        public TopicProduceDataMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version9)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TopicProduceDataMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version9)
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
                if (version >= ApiVersion.Version9)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field PartitionData was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<PartitionProduceDataMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new PartitionProduceDataMessage(reader, version));
                        }
                        PartitionData = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field PartitionData was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<PartitionProduceDataMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new PartitionProduceDataMessage(reader, version));
                        }
                        PartitionData = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version9)
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
                if (version >= ApiVersion.Version9)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version9)
            {
                writer.WriteVarUInt(PartitionData.Count + 1);
                foreach (var element in PartitionData)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(PartitionData.Count);
                foreach (var element in PartitionData)
                {
                    element.Write(writer, version);
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version9)
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
            return ReferenceEquals(this, obj) || obj is TopicProduceDataMessage other && Equals(other);
        }

        public bool Equals(TopicProduceDataMessage? other)
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
            return "TopicProduceDataMessage("
                + ")";
        }
    }

    public sealed class PartitionProduceDataMessage: IMessage, IEquatable<PartitionProduceDataMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version9;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// The record data to be produced.
        /// </summary>
        public RecordBatch? Records { get; set; } = null;

        public PartitionProduceDataMessage()
        {
        }

        public PartitionProduceDataMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version9)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of PartitionProduceDataMessage");
            }
            Index = reader.ReadInt();
            {
                int length;
                if (version >= ApiVersion.Version9)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadInt();
                }
                if (length < 0)
                {
                    Records = null;
                }
                else
                {
                    Records = reader.ReadRecords(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version9)
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
            writer.WriteInt(Index);
            if (Records is null)
            {
                if (version >= ApiVersion.Version9)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteInt(-1);
                }
            }
            else
            {
                if (version >= ApiVersion.Version9)
                {
                    writer.WriteVarUInt(Records.Length + 1);
                }
                else
                {
                    writer.WriteInt(Records.Length);
                }
                writer.WriteRecords(Records);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version9)
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
            return ReferenceEquals(this, obj) || obj is PartitionProduceDataMessage other && Equals(other);
        }

        public bool Equals(PartitionProduceDataMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Index, Records);
            return hashCode;
        }

        public override string ToString()
        {
            return "PartitionProduceDataMessage("
                + "Index=" + Index
                + ")";
        }
    }

    public sealed class TopicProduceDataCollection: HashSet<TopicProduceDataMessage>
    {
        public TopicProduceDataCollection()
        {
        }

        public TopicProduceDataCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
