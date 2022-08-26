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

public sealed class OffsetFetchResponseMessage: ResponseMessage
{
    /// <summary>
    /// The responses per topic.
    /// </summary>
    public List<OffsetFetchResponseTopicMessage> TopicsMessage { get; set; } = new ();

    /// <summary>
    /// The top-level error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// The responses per group id.
    /// </summary>
    public List<OffsetFetchResponseGroupMessage> GroupsMessage { get; set; } = new ();

    public OffsetFetchResponseMessage()
    {
        ApiKey = ApiKeys.OffsetFetch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version8;
    }

    public OffsetFetchResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.OffsetFetch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version8;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class OffsetFetchResponseTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string NameMessage { get; set; } = "";

        /// <summary>
        /// The responses per partition
        /// </summary>
        public List<OffsetFetchResponsePartitionMessage> PartitionsMessage { get; set; } = new ();

        public OffsetFetchResponseTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public OffsetFetchResponseTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class OffsetFetchResponsePartitionMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndexMessage { get; set; } = 0;

        /// <summary>
        /// The committed message offset.
        /// </summary>
        public long CommittedOffsetMessage { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int CommittedLeaderEpochMessage { get; set; } = -1;

        /// <summary>
        /// The partition metadata.
        /// </summary>
        public string MetadataMessage { get; set; } = "";

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        public OffsetFetchResponsePartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public OffsetFetchResponsePartitionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class OffsetFetchResponseGroupMessage: Message
    {
        /// <summary>
        /// The group ID.
        /// </summary>
        public string groupIdMessage { get; set; } = "";

        /// <summary>
        /// The responses per topic.
        /// </summary>
        public List<OffsetFetchResponseTopicsMessage> TopicsMessage { get; set; } = new ();

        /// <summary>
        /// The group-level error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        public OffsetFetchResponseGroupMessage()
        {
            LowestSupportedVersion = ApiVersions.Version8;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public OffsetFetchResponseGroupMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version8;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class OffsetFetchResponseTopicsMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string NameMessage { get; set; } = "";

        /// <summary>
        /// The responses per partition
        /// </summary>
        public List<OffsetFetchResponsePartitionsMessage> PartitionsMessage { get; set; } = new ();

        public OffsetFetchResponseTopicsMessage()
        {
            LowestSupportedVersion = ApiVersions.Version8;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public OffsetFetchResponseTopicsMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version8;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class OffsetFetchResponsePartitionsMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndexMessage { get; set; } = 0;

        /// <summary>
        /// The committed message offset.
        /// </summary>
        public long CommittedOffsetMessage { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int CommittedLeaderEpochMessage { get; set; } = -1;

        /// <summary>
        /// The partition metadata.
        /// </summary>
        public string MetadataMessage { get; set; } = "";

        /// <summary>
        /// The partition-level error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        public OffsetFetchResponsePartitionsMessage()
        {
            LowestSupportedVersion = ApiVersions.Version8;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public OffsetFetchResponsePartitionsMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version8;
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
