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
public sealed partial class AlterIsrRequestMessage: RequestMessage
{
    /// <summary>
    /// The ID of the requesting broker
    /// </summary>
    public int BrokerId { get; set; } = 0;

    /// <summary>
    /// The epoch of the requesting broker
    /// </summary>
    public long BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// 
    /// </summary>
    public List<TopicData> Topics { get; set; } = new();

    public AlterIsrRequestMessage()
    {
        ApiKey = ApiKeys.AlterIsr;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public AlterIsrRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.AlterIsr;
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
        /// The name of the topic to alter ISRs for
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
        /// The leader epoch of this partition
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        /// <summary>
        /// The ISR for this partition
        /// </summary>
        public List<int> NewIsr { get; set; } = new();

        /// <summary>
        /// The expected version of ISR which is being updated
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