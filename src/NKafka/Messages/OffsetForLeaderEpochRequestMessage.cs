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

public partial class OffsetForLeaderEpochRequestMessage: RequestMessage
{
    /// <summary>
    /// The broker ID of the follower, of -1 if this request is from a consumer.
    /// </summary>
    public int? ReplicaId { get; set; } = -2;

    /// <summary>
    /// Each topic to get offsets for.
    /// </summary>
    public IReadOnlyCollection<OffsetForLeaderTopicMessage> Topics { get; set; }

    public OffsetForLeaderEpochRequestMessage()
    {
        ApiKey = ApiKeys.OffsetForLeaderEpoch;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class OffsetForLeaderTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Each partition to get offsets for.
        /// </summary>
        public IReadOnlyCollection<OffsetForLeaderPartitionMessage> Partitions { get; set; }

        public OffsetForLeaderTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class OffsetForLeaderPartitionMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int Partition { get; set; }

        /// <summary>
        /// An epoch used to fence consumers/replicas with old metadata. If the epoch provided by the client is larger than the current epoch known to the broker, then the UNKNOWN_LEADER_EPOCH error code will be returned. If the provided epoch is smaller, then the FENCED_LEADER_EPOCH error code will be returned.
        /// </summary>
        public int? CurrentLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The epoch to look up an offset for.
        /// </summary>
        public int LeaderEpoch { get; set; }

        public OffsetForLeaderPartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}