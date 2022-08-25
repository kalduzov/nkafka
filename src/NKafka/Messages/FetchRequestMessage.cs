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
public sealed partial class FetchRequestMessage: RequestMessage
{
    /// <summary>
    /// The clusterId if known. This is used to validate metadata fetches prior to broker registration.
    /// </summary>
    public string? ClusterId { get; set; } = "null";

    /// <summary>
    /// The broker ID of the follower, of -1 if this request is from a consumer.
    /// </summary>
    public int ReplicaId { get; set; } = 0;

    /// <summary>
    /// The maximum time in milliseconds to wait for the response.
    /// </summary>
    public int MaxWaitMs { get; set; } = 0;

    /// <summary>
    /// The minimum bytes to accumulate in the response.
    /// </summary>
    public int MinBytes { get; set; } = 0;

    /// <summary>
    /// The maximum bytes to fetch.  See KIP-74 for cases where this limit may not be honored.
    /// </summary>
    public int? MaxBytes { get; set; } = 0x7fffffff;

    /// <summary>
    /// This setting controls the visibility of transactional records. Using READ_UNCOMMITTED (isolation_level = 0) makes all records visible. With READ_COMMITTED (isolation_level = 1), non-transactional and COMMITTED transactional records are visible. To be more concrete, READ_COMMITTED returns all data from offsets smaller than the current LSO (last stable offset), and enables the inclusion of the list of aborted transactions in the result, which allows consumers to discard ABORTED transactional records
    /// </summary>
    public sbyte? IsolationLevel { get; set; } = 0;

    /// <summary>
    /// The fetch session ID.
    /// </summary>
    public int? SessionId { get; set; } = 0;

    /// <summary>
    /// The fetch session epoch, which is used for ordering requests in a session.
    /// </summary>
    public int? SessionEpoch { get; set; } = -1;

    /// <summary>
    /// The topics to fetch.
    /// </summary>
    public List<FetchTopic> Topics { get; set; } = new();

    /// <summary>
    /// In an incremental fetch request, the partitions to remove.
    /// </summary>
    public List<ForgottenTopic> ForgottenTopicsData { get; set; } = new();

    /// <summary>
    /// Rack ID of the consumer making this request
    /// </summary>
    public string? RackId { get; set; } = null!;

    public FetchRequestMessage()
    {
        ApiKey = ApiKeys.Fetch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version13;
    }

    public FetchRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.Fetch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version13;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class FetchTopic: Message
    {
        /// <summary>
        /// The name of the topic to fetch.
        /// </summary>
        public string? Topic { get; set; } = null!;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid? TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The partitions to fetch.
        /// </summary>
        public List<FetchPartition> Partitions { get; set; } = new();

        public FetchTopic()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public FetchTopic(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class FetchPartition: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int Partition { get; set; } = 0;

        /// <summary>
        /// The current leader epoch of the partition.
        /// </summary>
        public int? CurrentLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The message offset.
        /// </summary>
        public long FetchOffset { get; set; } = 0;

        /// <summary>
        /// The epoch of the last fetched record or -1 if there is none
        /// </summary>
        public int LastFetchedEpoch { get; set; } = -1;

        /// <summary>
        /// The earliest available offset of the follower replica.  The field is only used when the request is sent by the follower.
        /// </summary>
        public long? LogStartOffset { get; set; } = -1;

        /// <summary>
        /// The maximum bytes to fetch from this partition.  See KIP-74 for cases where this limit may not be honored.
        /// </summary>
        public int PartitionMaxBytes { get; set; } = 0;

        public FetchPartition()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public FetchPartition(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class ForgottenTopic: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string? Topic { get; set; } = null!;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid? TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The partitions indexes to forget.
        /// </summary>
        public List<int> Partitions { get; set; } = new();

        public ForgottenTopic()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public ForgottenTopic(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}