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

public sealed class DescribeProducersResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each topic in the response.
    /// </summary>
    public List<TopicResponseMessage> TopicsMessage { get; set; } = new ();

    public DescribeProducersResponseMessage()
    {
        ApiKey = ApiKeys.DescribeProducers;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public DescribeProducersResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeProducers;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class TopicResponseMessage: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string NameMessage { get; set; } = "";

        /// <summary>
        /// Each partition in the response.
        /// </summary>
        public List<PartitionResponseMessage> PartitionsMessage { get; set; } = new ();

        public TopicResponseMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public TopicResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class PartitionResponseMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndexMessage { get; set; } = 0;

        /// <summary>
        /// The partition error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The partition error message, which may be null if no additional details are available
        /// </summary>
        public string ErrorMessageMessage { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public List<ProducerStateMessage> ActiveProducersMessage { get; set; } = new ();

        public PartitionResponseMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public PartitionResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class ProducerStateMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public long ProducerIdMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int ProducerEpochMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int LastSequenceMessage { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public long LastTimestampMessage { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public int CoordinatorEpochMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long CurrentTxnStartOffsetMessage { get; set; } = -1;

        public ProducerStateMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public ProducerStateMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }
}
