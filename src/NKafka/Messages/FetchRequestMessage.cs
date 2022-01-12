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

public partial class FetchRequestMessage: RequestMessage
{
    /// <summary>
    /// The clusterId if known. This is used to validate metadata fetches prior to broker registration.
    /// </summary>
    public string? ClusterId { get; set; } = null;

    /// <summary>
    /// The broker ID of the follower, of -1 if this request is from a consumer.
    /// </summary>
    public int ReplicaId { get; set; }

    /// <summary>
    /// The maximum time in milliseconds to wait for the response.
    /// </summary>
    public int MaxWaitMs { get; set; }

    /// <summary>
    /// The minimum bytes to accumulate in the response.
    /// </summary>
    public int MinBytes { get; set; }

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
    public IReadOnlyCollection<FetchTopicMessage> Topics { get; set; }

    /// <summary>
    /// In an incremental fetch request, the partitions to remove.
    /// </summary>
    public IReadOnlyCollection<ForgottenTopicMessage> ForgottenTopicsData { get; set; }

    /// <summary>
    /// Rack ID of the consumer making this request
    /// </summary>
    public string? RackId { get; set; }

    public FetchRequestMessage()
    {
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

    public class FetchTopicMessage: Message
    {
        /// <summary>
        /// The name of the topic to fetch.
        /// </summary>
        public string? Topic { get; set; }

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid? TopicId { get; set; }

        /// <summary>
        /// The partitions to fetch.
        /// </summary>
        public IReadOnlyCollection<FetchPartitionMessage> Partitions { get; set; }

        public FetchTopicMessage()
        {
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
    public class FetchPartitionMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int Partition { get; set; }

        /// <summary>
        /// The current leader epoch of the partition.
        /// </summary>
        public int? CurrentLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The message offset.
        /// </summary>
        public long FetchOffset { get; set; }

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
        public int PartitionMaxBytes { get; set; }

        public FetchPartitionMessage()
        {
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
    public class ForgottenTopicMessage: Message
    {
        /// <summary>
        /// The partition name.
        /// </summary>
        public string? Topic { get; set; }

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid? TopicId { get; set; }

        /// <summary>
        /// The partitions indexes to forget.
        /// </summary>
        public IReadOnlyCollection<int> Partitions { get; set; }

        public ForgottenTopicMessage()
        {
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