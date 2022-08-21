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

public partial class DeleteTopicsRequestMessage: RequestMessage
{
    /// <summary>
    /// The name or topic ID of the topic
    /// </summary>
    public IReadOnlyCollection<DeleteTopicStateMessage> Topics { get; set; }

    /// <summary>
    /// The names of the topics to delete
    /// </summary>
    public IReadOnlyCollection<string>? TopicNames { get; set; }

    /// <summary>
    /// The length of time in milliseconds to wait for the deletions to complete.
    /// </summary>
    public int TimeoutMs { get; set; }

    public DeleteTopicsRequestMessage()
    {
        ApiKey = ApiKeys.DeleteTopics;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version6;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version4)
        {
        }
        else //no flexible version
        {
        }

        if (Version >= ApiVersions.Version4)
        {
            if (TopicNames is null)
            {
                writer.WriteVarUInt(0);
                Size += 4;
            }
            else
            {
                writer.WriteVarUInt((uint)TopicNames.Count + 1);
                foreach (var val in TopicNames)
                {
                    writer.WriteVarInt(val);
                }
            }
        }
        else
        {
        }

    }

    public class DeleteTopicStateMessage: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string? Name { get; set; } = null;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; }

        public DeleteTopicStateMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version6;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version4)
            {
            }
            else //no flexible version
            {
            }

            if (Version >= ApiVersions.Version4)
            {
                if (TopicNames is null)
                {
                    writer.WriteVarUInt(0);
                    Size += 4;
                }
                else
                {
                    writer.WriteVarUInt((uint)TopicNames.Count + 1);
                    foreach (var val in TopicNames)
                    {
                        writer.WriteVarInt(val);
                    }
                }
            }
            else
            {
            }

        }
    }
}