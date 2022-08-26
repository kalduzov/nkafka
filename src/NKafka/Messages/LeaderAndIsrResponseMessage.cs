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

public sealed class LeaderAndIsrResponseMessage: ResponseMessage
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// Each partition in v0 to v4 message.
    /// </summary>
    public List<LeaderAndIsrPartitionErrorMessage> PartitionErrorsMessage { get; set; } = new ();

    /// <summary>
    /// Each topic
    /// </summary>
    public List<LeaderAndIsrTopicErrorMessage> TopicsMessage { get; set; } = new ();

    public LeaderAndIsrResponseMessage()
    {
        ApiKey = ApiKeys.LeaderAndIsr;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version6;
    }

    public LeaderAndIsrResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.LeaderAndIsr;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version6;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class LeaderAndIsrTopicErrorMessage: Message
    {
        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Dictionary<Guid,> TopicIdMessage { get; set; } = Guid.Empty;

        /// <summary>
        /// Each partition.
        /// </summary>
        public List<LeaderAndIsrPartitionErrorMessage> PartitionErrorsMessage { get; set; } = new ();

        public LeaderAndIsrTopicErrorMessage()
        {
            LowestSupportedVersion = ApiVersions.Version5;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public LeaderAndIsrTopicErrorMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version5;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class LeaderAndIsrPartitionError: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicNameMessage { get; set; } = "";

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndexMessage { get; set; } = 0;

        /// <summary>
        /// The partition error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        public LeaderAndIsrPartitionError()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public LeaderAndIsrPartitionError(BufferReader reader, ApiVersions version)
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
