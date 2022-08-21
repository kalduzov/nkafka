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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public partial class ProduceResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each produce response
    /// </summary>
    public IReadOnlyCollection<TopicProduceResponseMessage> Responses { get; set; }


    public ProduceResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    public ProduceResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version9)
        {
        }
        else //no flexible version
        {
        }

    }

    public class TopicProduceResponseMessage: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Each partition that we produced to within the topic.
        /// </summary>
        public IReadOnlyCollection<PartitionProduceResponseMessage> PartitionResponses { get; set; }

        public TopicProduceResponseMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public TopicProduceResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version9)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class PartitionProduceResponseMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// The base offset.
        /// </summary>
        public long BaseOffset { get; set; }

        /// <summary>
        /// The timestamp returned by broker after appending the messages. If CreateTime is used for the topic, the timestamp will be -1.  If LogAppendTime is used for the topic, the timestamp will be the broker local time when the messages are appended.
        /// </summary>
        public long? LogAppendTimeMs { get; set; } = -1;

        /// <summary>
        /// The log start offset.
        /// </summary>
        public long? LogStartOffset { get; set; } = -1;

        /// <summary>
        /// The batch indices of records that caused the batch to be dropped
        /// </summary>
        public IReadOnlyCollection<BatchIndexAndErrorMessageMessage>? RecordErrors { get; set; }

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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version9)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class BatchIndexAndErrorMessageMessage: Message
    {
        /// <summary>
        /// The batch index of the record that cause the batch to be dropped
        /// </summary>
        public int BatchIndex { get; set; }

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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version9)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}