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

public partial class ListOffsetsRequestMessage: RequestMessage
{
    /// <summary>
    /// The broker ID of the requestor, or -1 if this request is being made by a normal consumer.
    /// </summary>
    public int ReplicaId { get; set; }

    /// <summary>
    /// This setting controls the visibility of transactional records. Using READ_UNCOMMITTED (isolation_level = 0) makes all records visible. With READ_COMMITTED (isolation_level = 1), non-transactional and COMMITTED transactional records are visible. To be more concrete, READ_COMMITTED returns all data from offsets smaller than the current LSO (last stable offset), and enables the inclusion of the list of aborted transactions in the result, which allows consumers to discard ABORTED transactional records
    /// </summary>
    public sbyte IsolationLevel { get; set; }

    /// <summary>
    /// Each topic in the request.
    /// </summary>
    public IReadOnlyCollection<ListOffsetsTopicMessage> Topics { get; set; }

    public ListOffsetsRequestMessage()
    {
        ApiKey = ApiKeys.ListOffsets;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version6)
        {
        }
        else //no flexible version
        {
        }

    }

    public class ListOffsetsTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Each partition in the request.
        /// </summary>
        public IReadOnlyCollection<ListOffsetsPartitionMessage> Partitions { get; set; }

        public ListOffsetsTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version6)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class ListOffsetsPartitionMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The current leader epoch.
        /// </summary>
        public int? CurrentLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The current timestamp.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The maximum number of offsets to report.
        /// </summary>
        public int MaxNumOffsets { get; set; } = 1;

        public ListOffsetsPartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version6)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}