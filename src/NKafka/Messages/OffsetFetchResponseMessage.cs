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
public sealed partial class OffsetFetchResponseMessage: ResponseMessage
{
    /// <summary>
    /// The responses per topic.
    /// </summary>
    public List<OffsetFetchResponseTopic> Topics { get; set; } = new();

    /// <summary>
    /// The top-level error code, or 0 if there was no error.
    /// </summary>
    public short? ErrorCode { get; set; } = 0;

    /// <summary>
    /// The responses per group id.
    /// </summary>
    public List<OffsetFetchResponseGroup> Groups { get; set; } = new();

    public OffsetFetchResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version8;
    }

    public OffsetFetchResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version8;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class OffsetFetchResponseTopic: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The responses per partition
        /// </summary>
        public List<OffsetFetchResponsePartition> Partitions { get; set; } = new();

        public OffsetFetchResponseTopic()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetFetchResponseTopic(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class OffsetFetchResponsePartition: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The committed message offset.
        /// </summary>
        public long CommittedOffset { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int? CommittedLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The partition metadata.
        /// </summary>
        public string Metadata { get; set; } = null!;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        public OffsetFetchResponsePartition()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetFetchResponsePartition(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class OffsetFetchResponseGroup: Message
    {
        /// <summary>
        /// The group ID.
        /// </summary>
        public string groupId { get; set; } = null!;

        /// <summary>
        /// The responses per topic.
        /// </summary>
        public List<OffsetFetchResponseTopics> Topics { get; set; } = new();

        /// <summary>
        /// The group-level error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        public OffsetFetchResponseGroup()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetFetchResponseGroup(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class OffsetFetchResponseTopics: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The responses per partition
        /// </summary>
        public List<OffsetFetchResponsePartitions> Partitions { get; set; } = new();

        public OffsetFetchResponseTopics()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetFetchResponseTopics(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class OffsetFetchResponsePartitions: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The committed message offset.
        /// </summary>
        public long CommittedOffset { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int? CommittedLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The partition metadata.
        /// </summary>
        public string Metadata { get; set; } = null!;

        /// <summary>
        /// The partition-level error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        public OffsetFetchResponsePartitions()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetFetchResponsePartitions(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}