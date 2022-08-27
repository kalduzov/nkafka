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

using NKafka.Exceptions;
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
    public List<DeleteTopicStateMessage> Topics { get; set; } = new ();

    /// <summary>
    /// The names of the topics to delete
    /// </summary>
    public List<string> TopicNames { get; set; } = new ();

    /// <summary>
    /// The length of time in milliseconds to wait for the deletions to complete.
    /// </summary>
    public int TimeoutMs { get; set; } = 0;

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

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version6)
        {
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (Topics.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Topics at version {version}");
            }
        }
        if (version <= ApiVersions.Version5)
        {
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(TopicNames.Count + 1);
                foreach (var element in TopicNames)
                {
                    {
                        var stringBytes = Encoding.UTF8.GetBytes(element);
                        writer.WriteVarUInt(stringBytes.Length + 1);
                        writer.WriteBytes(stringBytes);
                    }
                }
            }
            else
            {
                writer.WriteInt(TopicNames.Count);
                foreach (var element in TopicNames)
                {
                    {
                        var stringBytes = Encoding.UTF8.GetBytes(element);
                        writer.WriteShort((short)stringBytes.Length);
                        writer.WriteBytes(stringBytes);
                    }
                }
            }
        }
        writer.WriteInt(TimeoutMs);
    }

    public sealed class DeleteTopicStateMessage: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string? Name { get; set; } = null;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        public DeleteTopicStateMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version6;
        }

        public DeleteTopicStateMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version6;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version6)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of DeleteTopicStateMessage");
            }
            var numTaggedFields = 0;
            if (Name is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteGuid(TopicId);
        }
    }
}
