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
public sealed partial class OffsetDeleteRequestMessage: RequestMessage
{
    /// <summary>
    /// The unique group identifier.
    /// </summary>
    public string GroupId { get; set; } = null!;

    /// <summary>
    /// The topics to delete offsets for
    /// </summary>
    public List<OffsetDeleteRequestTopic> Topics { get; set; } = new();

    public OffsetDeleteRequestMessage()
    {
        ApiKey = ApiKeys.OffsetDelete;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public OffsetDeleteRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.OffsetDelete;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class OffsetDeleteRequestTopic: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Each partition to delete offsets for.
        /// </summary>
        public List<OffsetDeleteRequestPartition> Partitions { get; set; } = new();

        public OffsetDeleteRequestTopic()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public OffsetDeleteRequestTopic(BufferReader reader, ApiVersions version)
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
    public class OffsetDeleteRequestPartition: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        public OffsetDeleteRequestPartition()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public OffsetDeleteRequestPartition(BufferReader reader, ApiVersions version)
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