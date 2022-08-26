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

public sealed class CreateTopicsResponseMessage: ResponseMessage
{
    /// <summary>
    /// Results for each topic we tried to create.
    /// </summary>
    public List<CreatableTopicResultMessage> TopicsMessage { get; set; } = new ();

    public CreateTopicsResponseMessage()
    {
        ApiKey = ApiKeys.CreateTopics;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public CreateTopicsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.CreateTopics;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class CreatableTopicResultMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public Dictionary<string,> NameMessage { get; set; } = "";

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Dictionary<Guid,> TopicIdMessage { get; set; } = Guid.Empty;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public Dictionary<short,> ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The error message, or null if there was no error.
        /// </summary>
        public Dictionary<string,> ErrorMessageMessage { get; set; } = "";

        /// <summary>
        /// Optional topic config error returned if configs are not returned in the response.
        /// </summary>
        public Dictionary<short,> TopicConfigErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// Number of partitions of the topic.
        /// </summary>
        public Dictionary<int,> NumPartitionsMessage { get; set; } = -1;

        /// <summary>
        /// Replication factor of the topic.
        /// </summary>
        public Dictionary<short,> ReplicationFactorMessage { get; set; } = -1;

        /// <summary>
        /// Configuration of the topic.
        /// </summary>
        public List<CreatableTopicConfigsMessage> ConfigsMessage { get; set; } = new ();

        public CreatableTopicResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public CreatableTopicResultMessage(BufferReader reader, ApiVersions version)
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

    public sealed class CreatableTopicConfigsMessage: Message
    {
        /// <summary>
        /// The configuration name.
        /// </summary>
        public string NameMessage { get; set; } = "";

        /// <summary>
        /// The configuration value.
        /// </summary>
        public string ValueMessage { get; set; } = "";

        /// <summary>
        /// True if the configuration is read-only.
        /// </summary>
        public bool ReadOnlyMessage { get; set; } = false;

        /// <summary>
        /// The configuration source.
        /// </summary>
        public sbyte ConfigSourceMessage { get; set; } = -1;

        /// <summary>
        /// True if this configuration is sensitive.
        /// </summary>
        public bool IsSensitiveMessage { get; set; } = false;

        public CreatableTopicConfigsMessage()
        {
            LowestSupportedVersion = ApiVersions.Version5;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public CreatableTopicConfigsMessage(BufferReader reader, ApiVersions version)
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
}
