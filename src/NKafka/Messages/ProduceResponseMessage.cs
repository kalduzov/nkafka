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

public sealed class ProduceResponseMessage: IResponseMessage, IEquatable<ProduceResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version9;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// Each produce response
    /// </summary>
    public TopicProduceResponseCollection Responses { get; set; } = new ();

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    public ProduceResponseMessage()
    {
    }

    public ProduceResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            if (version >= ApiVersions.Version9)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Responses was serialized as null");
                }
                else
                {
                    var newCollection = new TopicProduceResponseCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new TopicProduceResponseMessage(reader, version));
                    }
                    Responses = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Responses was serialized as null");
                }
                else
                {
                    var newCollection = new TopicProduceResponseCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new TopicProduceResponseMessage(reader, version));
                    }
                    Responses = newCollection;
                }
            }
        }
        if (version >= ApiVersions.Version1)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version9)
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

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version9)
        {
            writer.WriteVarUInt(Responses.Count + 1);
            foreach (var element in Responses)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Responses.Count);
            foreach (var element in Responses)
            {
                element.Write(writer, version);
            }
        }
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version9)
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
        return ReferenceEquals(this, obj) || obj is ProduceResponseMessage other && Equals(other);
    }

    public bool Equals(ProduceResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Responses, ThrottleTimeMs);
        return hashCode;
    }

    public override string ToString()
    {
        return "ProduceResponseMessage("
            + ", ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }

    public sealed class TopicProduceResponseMessage: IMessage, IEquatable<TopicProduceResponseMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version9;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Each partition that we produced to within the topic.
        /// </summary>
        public List<PartitionProduceResponseMessage> PartitionResponses { get; set; } = new ();

        public TopicProduceResponseMessage()
        {
        }

        public TopicProduceResponseMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version9)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TopicProduceResponseMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version9)
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
                if (version >= ApiVersions.Version9)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field PartitionResponses was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<PartitionProduceResponseMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new PartitionProduceResponseMessage(reader, version));
                        }
                        PartitionResponses = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field PartitionResponses was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<PartitionProduceResponseMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new PartitionProduceResponseMessage(reader, version));
                        }
                        PartitionResponses = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version9)
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version9)
            {
                writer.WriteVarUInt(PartitionResponses.Count + 1);
                foreach (var element in PartitionResponses)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(PartitionResponses.Count);
                foreach (var element in PartitionResponses)
                {
                    element.Write(writer, version);
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version9)
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
            return ReferenceEquals(this, obj) || obj is TopicProduceResponseMessage other && Equals(other);
        }

        public bool Equals(TopicProduceResponseMessage? other)
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
            return "TopicProduceResponseMessage("
                + ")";
        }
    }

    public sealed class PartitionProduceResponseMessage: IMessage, IEquatable<PartitionProduceResponseMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version9;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The base offset.
        /// </summary>
        public long BaseOffset { get; set; } = 0;

        /// <summary>
        /// The timestamp returned by broker after appending the messages. If CreateTime is used for the topic, the timestamp will be -1.  If LogAppendTime is used for the topic, the timestamp will be the broker local time when the messages are appended.
        /// </summary>
        public long LogAppendTimeMs { get; set; } = -1;

        /// <summary>
        /// The log start offset.
        /// </summary>
        public long LogStartOffset { get; set; } = -1;

        /// <summary>
        /// The batch indices of records that caused the batch to be dropped
        /// </summary>
        public List<BatchIndexAndErrorMessageMessage> RecordErrors { get; set; } = new ();

        /// <summary>
        /// The global error message summarizing the common root cause of the records that caused the batch to be dropped
        /// </summary>
        public string? ErrorMessage { get; set; } = null;

        public PartitionProduceResponseMessage()
        {
        }

        public PartitionProduceResponseMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version9)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of PartitionProduceResponseMessage");
            }
            Index = reader.ReadInt();
            ErrorCode = reader.ReadShort();
            BaseOffset = reader.ReadLong();
            if (version >= ApiVersions.Version2)
            {
                LogAppendTimeMs = reader.ReadLong();
            }
            else
            {
                LogAppendTimeMs = -1;
            }
            if (version >= ApiVersions.Version5)
            {
                LogStartOffset = reader.ReadLong();
            }
            else
            {
                LogStartOffset = -1;
            }
            if (version >= ApiVersions.Version8)
            {
                if (version >= ApiVersions.Version9)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field RecordErrors was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<BatchIndexAndErrorMessageMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new BatchIndexAndErrorMessageMessage(reader, version));
                        }
                        RecordErrors = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field RecordErrors was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<BatchIndexAndErrorMessageMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new BatchIndexAndErrorMessageMessage(reader, version));
                        }
                        RecordErrors = newCollection;
                    }
                }
            }
            else
            {
                RecordErrors = new ();
            }
            if (version >= ApiVersions.Version8)
            {
                int length;
                if (version >= ApiVersions.Version9)
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
            else
            {
                ErrorMessage = null;
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version9)
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(Index);
            writer.WriteShort((short)ErrorCode);
            writer.WriteLong(BaseOffset);
            if (version >= ApiVersions.Version2)
            {
                writer.WriteLong(LogAppendTimeMs);
            }
            if (version >= ApiVersions.Version5)
            {
                writer.WriteLong(LogStartOffset);
            }
            if (version >= ApiVersions.Version8)
            {
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(RecordErrors.Count + 1);
                    foreach (var element in RecordErrors)
                    {
                        element.Write(writer, version);
                    }
                }
                else
                {
                    writer.WriteInt(RecordErrors.Count);
                    foreach (var element in RecordErrors)
                    {
                        element.Write(writer, version);
                    }
                }
            }
            if (version >= ApiVersions.Version8)
            {
                if (ErrorMessage is null)
                {
                    if (version >= ApiVersions.Version9)
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
                    if (version >= ApiVersions.Version9)
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
            if (version >= ApiVersions.Version9)
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
            return ReferenceEquals(this, obj) || obj is PartitionProduceResponseMessage other && Equals(other);
        }

        public bool Equals(PartitionProduceResponseMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Index, ErrorCode, BaseOffset, LogAppendTimeMs, LogStartOffset, RecordErrors, ErrorMessage);
            return hashCode;
        }

        public override string ToString()
        {
            return "PartitionProduceResponseMessage("
                + "Index=" + Index
                + ", ErrorCode=" + ErrorCode
                + ", BaseOffset=" + BaseOffset
                + ", LogAppendTimeMs=" + LogAppendTimeMs
                + ", LogStartOffset=" + LogStartOffset
                + ")";
        }
    }

    public sealed class BatchIndexAndErrorMessageMessage: IMessage, IEquatable<BatchIndexAndErrorMessageMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version9;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The batch index of the record that cause the batch to be dropped
        /// </summary>
        public int BatchIndex { get; set; } = 0;

        /// <summary>
        /// The error message of the record that caused the batch to be dropped
        /// </summary>
        public string? BatchIndexErrorMessage { get; set; } = null;

        public BatchIndexAndErrorMessageMessage()
        {
        }

        public BatchIndexAndErrorMessageMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version9)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of BatchIndexAndErrorMessageMessage");
            }
            BatchIndex = reader.ReadInt();
            {
                int length;
                if (version >= ApiVersions.Version9)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    BatchIndexErrorMessage = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field BatchIndexErrorMessage had invalid length {length}");
                }
                else
                {
                    BatchIndexErrorMessage = reader.ReadString(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version9)
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of BatchIndexAndErrorMessageMessage");
            }
            var numTaggedFields = 0;
            writer.WriteInt(BatchIndex);
            if (BatchIndexErrorMessage is null)
            {
                if (version >= ApiVersions.Version9)
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
                var stringBytes = Encoding.UTF8.GetBytes(BatchIndexErrorMessage);
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version9)
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
            return ReferenceEquals(this, obj) || obj is BatchIndexAndErrorMessageMessage other && Equals(other);
        }

        public bool Equals(BatchIndexAndErrorMessageMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, BatchIndex, BatchIndexErrorMessage);
            return hashCode;
        }

        public override string ToString()
        {
            return "BatchIndexAndErrorMessageMessage("
                + "BatchIndex=" + BatchIndex
                + ")";
        }
    }

    public sealed class TopicProduceResponseCollection: HashSet<TopicProduceResponseMessage>
    {
        public TopicProduceResponseCollection()
        {
        }

        public TopicProduceResponseCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
