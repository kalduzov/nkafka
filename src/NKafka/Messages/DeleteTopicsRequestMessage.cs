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

public sealed class DeleteTopicsRequestMessage: RequestMessage
{
    /// <summary>
    /// The name or topic ID of the topic
    /// </summary>
    public List<DeleteTopicStateMessage> TopicsMessage { get; set; } = new ();

    /// <summary>
    /// The names of the topics to delete
    /// </summary>
    public List<string> TopicNamesMessage { get; set; } = new ();

    /// <summary>
    /// The length of time in milliseconds to wait for the deletions to complete.
    /// </summary>
    public int TimeoutMsMessage { get; set; } = 0;

    public DeleteTopicsRequestMessage()
    {
        ApiKey = ApiKeys.DeleteTopics;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version6;
    }

    public DeleteTopicsRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DeleteTopics;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version6;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class DeleteTopicStateMessage: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string NameMessage { get; set; } = null;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicIdMessage { get; set; } = Guid.Empty;

        public DeleteTopicStateMessage()
        {
            LowestSupportedVersion = ApiVersions.Version6;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DeleteTopicStateMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version6;
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
