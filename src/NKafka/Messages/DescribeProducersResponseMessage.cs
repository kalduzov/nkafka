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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class DescribeProducersResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each topic in the response.
    /// </summary>
    public List<TopicResponse> Topics { get; set; } = new();

    public DescribeProducersResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public DescribeProducersResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class TopicResponse: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Each partition in the response.
        /// </summary>
        public List<PartitionResponse> Partitions { get; set; } = new();

        public TopicResponse()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public TopicResponse(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class PartitionResponse: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The partition error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The partition error message, which may be null if no additional details are available
        /// </summary>
        public string? ErrorMessage { get; set; } = "null";

        /// <summary>
        /// 
        /// </summary>
        public List<ProducerState> ActiveProducers { get; set; } = new();

        public PartitionResponse()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public PartitionResponse(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class ProducerState: Message
    {
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

        public ProducerState()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public ProducerState(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}