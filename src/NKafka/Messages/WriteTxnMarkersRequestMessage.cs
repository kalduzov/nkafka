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

public sealed class WriteTxnMarkersRequestMessage: IRequestMessage, IEquatable<WriteTxnMarkersRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version1;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.WriteTxnMarkers;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The transaction markers to be written.
    /// </summary>
    public List<WritableTxnMarkerMessage> Markers { get; set; } = new ();

    public WriteTxnMarkersRequestMessage()
    {
    }

    public WriteTxnMarkersRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            if (version >= ApiVersion.Version1)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Markers was serialized as null");
                }
                else
                {
                    var newCollection = new List<WritableTxnMarkerMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new WritableTxnMarkerMessage(reader, version));
                    }
                    Markers = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Markers was serialized as null");
                }
                else
                {
                    var newCollection = new List<WritableTxnMarkerMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new WritableTxnMarkerMessage(reader, version));
                    }
                    Markers = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version1)
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
        if (version >= ApiVersion.Version1)
        {
            writer.WriteVarUInt(Markers.Count + 1);
            foreach (var element in Markers)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Markers.Count);
            foreach (var element in Markers)
            {
                element.Write(writer, version);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version1)
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
        return ReferenceEquals(this, obj) || obj is WriteTxnMarkersRequestMessage other && Equals(other);
    }

    public bool Equals(WriteTxnMarkersRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Markers);
        return hashCode;
    }

    public override string ToString()
    {
        return "WriteTxnMarkersRequestMessage("
            + ")";
    }

    public sealed class WritableTxnMarkerMessage: IMessage, IEquatable<WritableTxnMarkerMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version1;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The current producer ID.
        /// </summary>
        public long ProducerId { get; set; } = 0;

        /// <summary>
        /// The current epoch associated with the producer ID.
        /// </summary>
        public short ProducerEpoch { get; set; } = 0;

        /// <summary>
        /// The result of the transaction to write to the partitions (false = ABORT, true = COMMIT).
        /// </summary>
        public bool TransactionResult { get; set; } = false;

        /// <summary>
        /// Each topic that we want to write transaction marker(s) for.
        /// </summary>
        public List<WritableTxnMarkerTopicMessage> Topics { get; set; } = new ();

        /// <summary>
        /// Epoch associated with the transaction state partition hosted by this transaction coordinator
        /// </summary>
        public int CoordinatorEpoch { get; set; } = 0;

        public WritableTxnMarkerMessage()
        {
        }

        public WritableTxnMarkerMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of WritableTxnMarkerMessage");
            }
            ProducerId = reader.ReadLong();
            ProducerEpoch = reader.ReadShort();
            TransactionResult = reader.ReadByte() != 0;
            {
                if (version >= ApiVersion.Version1)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Topics was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<WritableTxnMarkerTopicMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new WritableTxnMarkerTopicMessage(reader, version));
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
                        var newCollection = new List<WritableTxnMarkerTopicMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new WritableTxnMarkerTopicMessage(reader, version));
                        }
                        Topics = newCollection;
                    }
                }
            }
            CoordinatorEpoch = reader.ReadInt();
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version1)
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
            writer.WriteLong(ProducerId);
            writer.WriteShort(ProducerEpoch);
            writer.WriteBool(TransactionResult);
            if (version >= ApiVersion.Version1)
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
            writer.WriteInt(CoordinatorEpoch);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version1)
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
            return ReferenceEquals(this, obj) || obj is WritableTxnMarkerMessage other && Equals(other);
        }

        public bool Equals(WritableTxnMarkerMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ProducerId, ProducerEpoch, TransactionResult, Topics, CoordinatorEpoch);
            return hashCode;
        }

        public override string ToString()
        {
            return "WritableTxnMarkerMessage("
                + "ProducerId=" + ProducerId
                + ", ProducerEpoch=" + ProducerEpoch
                + ", TransactionResult=" + (TransactionResult ? "true" : "false")
                + ", CoordinatorEpoch=" + CoordinatorEpoch
                + ")";
        }
    }

    public sealed class WritableTxnMarkerTopicMessage: IMessage, IEquatable<WritableTxnMarkerTopicMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version1;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The indexes of the partitions to write transaction markers for.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new ();

        public WritableTxnMarkerTopicMessage()
        {
        }

        public WritableTxnMarkerTopicMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of WritableTxnMarkerTopicMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version1)
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
                int arrayLength;
                if (version >= ApiVersion.Version1)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field PartitionIndexes was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    PartitionIndexes = newCollection;
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version1)
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
                if (version >= ApiVersion.Version1)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version1)
            {
                writer.WriteVarUInt(PartitionIndexes.Count + 1);
            }
            else
            {
                writer.WriteInt(PartitionIndexes.Count);
            }
            foreach (var element in PartitionIndexes)
            {
                writer.WriteInt(element);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version1)
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
            return ReferenceEquals(this, obj) || obj is WritableTxnMarkerTopicMessage other && Equals(other);
        }

        public bool Equals(WritableTxnMarkerTopicMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name, PartitionIndexes);
            return hashCode;
        }

        public override string ToString()
        {
            return "WritableTxnMarkerTopicMessage("
                + ")";
        }
    }
}
