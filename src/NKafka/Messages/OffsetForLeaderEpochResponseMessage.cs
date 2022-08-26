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

public sealed class OffsetForLeaderEpochResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each topic we fetched offsets for.
    /// </summary>
    public List<OffsetForLeaderTopicResultMessage> TopicsMessage { get; set; } = new ();

    public OffsetForLeaderEpochResponseMessage()
    {
        ApiKey = ApiKeys.OffsetForLeaderEpoch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public OffsetForLeaderEpochResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.OffsetForLeaderEpoch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class OffsetForLeaderTopicResultMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public Dictionary<string,> TopicMessage { get; set; } = "";

        /// <summary>
        /// Each partition in the topic we fetched offsets for.
        /// </summary>
        public List<EpochEndOffsetMessage> PartitionsMessage { get; set; } = new ();

        public OffsetForLeaderTopicResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public OffsetForLeaderTopicResultMessage(BufferReader reader, ApiVersions version)
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

    public sealed class EpochEndOffsetMessage: Message
    {
        /// <summary>
        /// The error code 0, or if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionMessage { get; set; } = 0;

        /// <summary>
        /// The leader epoch of the partition.
        /// </summary>
        public int LeaderEpochMessage { get; set; } = -1;

        /// <summary>
        /// The end offset of the epoch.
        /// </summary>
        public long EndOffsetMessage { get; set; } = -1;

        public EpochEndOffsetMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public EpochEndOffsetMessage(BufferReader reader, ApiVersions version)
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
