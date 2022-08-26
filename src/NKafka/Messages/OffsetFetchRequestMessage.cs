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

public sealed class OffsetFetchRequestMessage: RequestMessage
{
    /// <summary>
    /// The group to fetch offsets for.
    /// </summary>
    public string GroupId { get; set; } = "";

    /// <summary>
    /// Each topic we would like to fetch offsets for, or null to fetch offsets for all topics.
    /// </summary>
    public List<OffsetFetchRequestTopicMessage> Topics { get; set; } = new ();

    /// <summary>
    /// Each group we would like to fetch offsets for
    /// </summary>
    public List<OffsetFetchRequestGroupMessage> Groups { get; set; } = new ();

    /// <summary>
    /// Whether broker should hold on returning unstable offsets but set a retriable error code for the partitions.
    /// </summary>
    public bool RequireStable { get; set; } = false;

    public OffsetFetchRequestMessage()
    {
        ApiKey = ApiKeys.OffsetFetch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version8;
    }

    public OffsetFetchRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.OffsetFetch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version8;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed class OffsetFetchRequestTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The partition indexes we would like to fetch offsets for.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new ();

        public OffsetFetchRequestTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetFetchRequestTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class OffsetFetchRequestGroupMessage: Message
    {
        /// <summary>
        /// The group ID.
        /// </summary>
        public string groupId { get; set; } = "";

        /// <summary>
        /// Each topic we would like to fetch offsets for, or null to fetch offsets for all topics.
        /// </summary>
        public List<OffsetFetchRequestTopicsMessage> Topics { get; set; } = new ();

        public OffsetFetchRequestGroupMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetFetchRequestGroupMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class OffsetFetchRequestTopicsMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The partition indexes we would like to fetch offsets for.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new ();

        public OffsetFetchRequestTopicsMessage()
        {
            LowestSupportedVersion = ApiVersions.Version8;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        public OffsetFetchRequestTopicsMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version8;
            HighestSupportedVersion = ApiVersions.Version8;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}
