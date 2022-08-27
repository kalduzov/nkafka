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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class ProduceResponseMessage: ResponseMessage
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
    }

    public sealed class TopicProduceResponseMessage: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string Name { get; set; } = "";

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
        }
    }

    public sealed class PartitionProduceResponseMessage: Message
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
        }
    }

    public sealed class BatchIndexAndErrorMessageMessage: Message
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
