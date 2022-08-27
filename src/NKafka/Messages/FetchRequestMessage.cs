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

public sealed class FetchRequestMessage: RequestMessage
{
    /// <summary>
    /// The clusterId if known. This is used to validate metadata fetches prior to broker registration.
    /// </summary>
    public string? ClusterId { get; set; } = null;

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
    public int MaxBytes { get; set; } = 2147483647;

    /// <summary>
    /// This setting controls the visibility of transactional records. Using READ_UNCOMMITTED (isolation_level = 0) makes all records visible. With READ_COMMITTED (isolation_level = 1), non-transactional and COMMITTED transactional records are visible. To be more concrete, READ_COMMITTED returns all data from offsets smaller than the current LSO (last stable offset), and enables the inclusion of the list of aborted transactions in the result, which allows consumers to discard ABORTED transactional records
    /// </summary>
    public sbyte IsolationLevel { get; set; } = 0;

    /// <summary>
    /// The fetch session ID.
    /// </summary>
    public int SessionId { get; set; } = 0;

    /// <summary>
    /// The fetch session epoch, which is used for ordering requests in a session.
    /// </summary>
    public int SessionEpoch { get; set; } = -1;

    /// <summary>
    /// The topics to fetch.
    /// </summary>
    public List<FetchTopicMessage> Topics { get; set; } = new ();

    /// <summary>
    /// In an incremental fetch request, the partitions to remove.
    /// </summary>
    public List<ForgottenTopicMessage> ForgottenTopicsData { get; set; } = new ();

    /// <summary>
    /// Rack ID of the consumer making this request
    /// </summary>
    public string RackId { get; set; } = "";

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

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version12)
        {
            if (ClusterId is not null)
            {
                numTaggedFields++;
            }
        }
        writer.WriteInt(ReplicaId);
        writer.WriteInt(MaxWaitMs);
        writer.WriteInt(MinBytes);
        if (version >= ApiVersions.Version3)
        {
            writer.WriteInt(MaxBytes);
        }
        if (version >= ApiVersions.Version4)
        {
            writer.WriteSByte(IsolationLevel);
        }
        if (version >= ApiVersions.Version7)
        {
            writer.WriteInt(SessionId);
        }
        if (version >= ApiVersions.Version7)
        {
            writer.WriteInt(SessionEpoch);
        }
        if (version >= ApiVersions.Version12)
        {
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Topics.Count);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        if (version >= ApiVersions.Version7)
        {
            if (version >= ApiVersions.Version12)
            {
                writer.WriteVarUInt(ForgottenTopicsData.Count + 1);
                foreach (var element in ForgottenTopicsData)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(ForgottenTopicsData.Count);
                foreach (var element in ForgottenTopicsData)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (ForgottenTopicsData.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ForgottenTopicsData at version {version}");
            }
        }
        if (version >= ApiVersions.Version11)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(RackId);
                if (version >= ApiVersions.Version12)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
        }
    }

    public sealed class FetchTopicMessage: Message
    {
        /// <summary>
        /// The name of the topic to fetch.
        /// </summary>
        public string Topic { get; set; } = "";

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The partitions to fetch.
        /// </summary>
        public List<FetchPartitionMessage> Partitions { get; set; } = new ();

        public FetchTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public FetchTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            if (version <= ApiVersions.Version12)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Topic);
                    if (version >= ApiVersions.Version12)
                    {
                        writer.WriteVarUInt(stringBytes.Length + 1);
                    }
                    else
                    {
                        writer.WriteShort((short)stringBytes.Length);
                    }
                    writer.WriteBytes(stringBytes);
                }
            }
            if (version >= ApiVersions.Version13)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersions.Version12)
            {
                writer.WriteVarUInt(Partitions.Count + 1);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Partitions.Count);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
        }
    }

    public sealed class FetchPartitionMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int Partition { get; set; } = 0;

        /// <summary>
        /// The current leader epoch of the partition.
        /// </summary>
        public int CurrentLeaderEpoch { get; set; } = -1;

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
        public long LogStartOffset { get; set; } = -1;

        /// <summary>
        /// The maximum bytes to fetch from this partition.  See KIP-74 for cases where this limit may not be honored.
        /// </summary>
        public int PartitionMaxBytes { get; set; } = 0;

        public FetchPartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public FetchPartitionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(Partition);
            if (version >= ApiVersions.Version9)
            {
                writer.WriteInt(CurrentLeaderEpoch);
            }
            writer.WriteLong(FetchOffset);
            if (version >= ApiVersions.Version12)
            {
                writer.WriteInt(LastFetchedEpoch);
            }
            else
            {
                if (LastFetchedEpoch != -1)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default LastFetchedEpoch at version {version}");
                }
            }
            if (version >= ApiVersions.Version5)
            {
                writer.WriteLong(LogStartOffset);
            }
            writer.WriteInt(PartitionMaxBytes);
        }
    }

    public sealed class ForgottenTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Topic { get; set; } = "";

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The partitions indexes to forget.
        /// </summary>
        public List<int> Partitions { get; set; } = new ();

        public ForgottenTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        public ForgottenTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version13;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of ForgottenTopicMessage");
            }
            var numTaggedFields = 0;
            if (version <= ApiVersions.Version12)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Topic);
                    if (version >= ApiVersions.Version12)
                    {
                        writer.WriteVarUInt(stringBytes.Length + 1);
                    }
                    else
                    {
                        writer.WriteShort((short)stringBytes.Length);
                    }
                    writer.WriteBytes(stringBytes);
                }
            }
            if (version >= ApiVersions.Version13)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersions.Version12)
            {
                writer.WriteVarUInt(Partitions.Count + 1);
            }
            else
            {
                writer.WriteInt(Partitions.Count);
            }
            foreach (var element in Partitions)
            {
                writer.WriteInt(element);
            }
        }
    }
}
