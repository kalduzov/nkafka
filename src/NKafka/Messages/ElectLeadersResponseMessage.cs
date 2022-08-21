﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

public partial class ElectLeadersResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top level response error code.
    /// </summary>
    public short ErrorCode { get; set; }

    /// <summary>
    /// The election results, or an empty array if the requester did not have permission and the request asks for all partitions.
    /// </summary>
    public IReadOnlyCollection<ReplicaElectionResultMessage> ReplicaElectionResults { get; set; }

    public ElectLeadersResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public ElectLeadersResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version2)
        {
        }
        else //no flexible version
        {
        }

    }

    public class ReplicaElectionResultMessage: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// The results for each partition
        /// </summary>
        public IReadOnlyCollection<PartitionResultMessage> PartitionResult { get; set; }

        public ReplicaElectionResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public ReplicaElectionResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version2)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class PartitionResultMessage: Message
    {
        /// <summary>
        /// The partition id
        /// </summary>
        public int PartitionId { get; set; }

        /// <summary>
        /// The result error, or zero if there was no error.
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// The result message, or null if there was no error.
        /// </summary>
        public string ErrorMessage { get; set; }

        public PartitionResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public PartitionResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version2)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}