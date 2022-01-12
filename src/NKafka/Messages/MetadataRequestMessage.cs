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

public partial class MetadataRequestMessage: RequestMessage
{
    /// <summary>
    /// The topics to fetch metadata for.
    /// </summary>
    public IReadOnlyCollection<MetadataRequestTopicMessage> Topics { get; set; }

    /// <summary>
    /// If this is true, the broker may auto-create topics that we requested which do not already exist, if it is configured to do so.
    /// </summary>
    public bool AllowAutoTopicCreation { get; set; } = true;

    /// <summary>
    /// Whether to include cluster authorized operations.
    /// </summary>
    public bool IncludeClusterAuthorizedOperations { get; set; }

    /// <summary>
    /// Whether to include topic authorized operations.
    /// </summary>
    public bool IncludeTopicAuthorizedOperations { get; set; }

    public MetadataRequestMessage()
    {
        ApiKey = ApiKeys.Metadata;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version12;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class MetadataRequestTopicMessage: Message
    {
        /// <summary>
        /// The topic id.
        /// </summary>
        public Guid? TopicId { get; set; }

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        public MetadataRequestTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}