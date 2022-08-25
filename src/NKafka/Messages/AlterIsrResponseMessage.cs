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
public sealed partial class AlterIsrResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top level response error code
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public List<TopicData> Topics { get; set; } = new();

    public AlterIsrResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public AlterIsrResponseMessage(BufferReader reader, ApiVersions version)
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

    public class TopicData: Message
    {
        /// <summary>
        /// The name of the topic
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public List<PartitionData> Partitions { get; set; } = new();

        public TopicData()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public TopicData(BufferReader reader, ApiVersions version)
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
    public class PartitionData: Message
    {
        /// <summary>
        /// The partition index
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The partition level error code
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The broker ID of the leader.
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        /// <summary>
        /// The in-sync replica IDs.
        /// </summary>
        public List<int> Isr { get; set; } = new();

        /// <summary>
        /// The current ISR version.
        /// </summary>
        public int CurrentIsrVersion { get; set; } = 0;

        public PartitionData()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public PartitionData(BufferReader reader, ApiVersions version)
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