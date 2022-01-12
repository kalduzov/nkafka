﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

public partial class ElectLeadersRequestMessage: RequestMessage
{
    /// <summary>
    /// Type of elections to conduct for the partition. A value of '0' elects the preferred replica. A value of '1' elects the first live replica if there are no in-sync replica.
    /// </summary>
    public sbyte ElectionType { get; set; }

    /// <summary>
    /// The topic partitions to elect leaders.
    /// </summary>
    public IReadOnlyCollection<TopicPartitionsMessage> TopicPartitions { get; set; }

    /// <summary>
    /// The time in ms to wait for the election to complete.
    /// </summary>
    public int TimeoutMs { get; set; } = 60000;

    public ElectLeadersRequestMessage()
    {
        ApiKey = ApiKeys.ElectLeaders;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class TopicPartitionsMessage: Message
    {
        /// <summary>
        /// The name of a topic.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// The partitions of this topic whose leader should be elected.
        /// </summary>
        public IReadOnlyCollection<int> Partitions { get; set; }

        public TopicPartitionsMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}