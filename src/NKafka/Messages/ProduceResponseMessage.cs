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

public sealed class ProduceResponseMessage: ResponseMessage, IEquatable<ProduceResponseMessage>
{
    /// <summary>
    /// Each produce response
    /// </summary>
    public TopicProduceResponseCollection Responses { get; set; } = new ();


    public ProduceResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    public ProduceResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
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

    public sealed class TopicProduceResponseMessage: Message, IEquatable<TopicProduceResponseMessage>
    {
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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public TopicProduceResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
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
    }

    public sealed class PartitionProduceResponseMessage: Message, IEquatable<PartitionProduceResponseMessage>
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public PartitionProduceResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(Index);
            writer.WriteShort(ErrorCode);
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
    }

    public sealed class BatchIndexAndErrorMessageMessage: Message, IEquatable<BatchIndexAndErrorMessageMessage>
    {
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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public BatchIndexAndErrorMessageMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
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
