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

public partial class AlterReplicaLogDirsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The results for each topic.
    /// </summary>
    public IReadOnlyCollection<AlterReplicaLogDirTopicResultMessage> Results { get; set; }

    public AlterReplicaLogDirsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public AlterReplicaLogDirsResponseMessage(BufferReader reader, ApiVersions version)
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
        if (Version >= ApiVersions.Version2)
        {
            if (Results is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteVarUInt((uint)Results.Count + 1);
                foreach (var val in Results)
                {
                    writer.WriteVarInt(val);
                }
            }
        }
        else
        {
        }

    }

    public class AlterReplicaLogDirTopicResultMessage: Message
    {
        /// <summary>
        /// The name of the topic.
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// The results for each partition.
        /// </summary>
        public IReadOnlyCollection<AlterReplicaLogDirPartitionResultMessage> Partitions { get; set; }

        public AlterReplicaLogDirTopicResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public AlterReplicaLogDirTopicResultMessage(BufferReader reader, ApiVersions version)
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
            if (Version >= ApiVersions.Version2)
            {
                if (Partitions is null)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteVarUInt((uint)Partitions.Count + 1);
                    foreach (var val in Partitions)
                    {
                        writer.WriteVarInt(val);
                    }
                }
            }
            else
            {
            }

        }
    }
    public class AlterReplicaLogDirPartitionResultMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; }

        public AlterReplicaLogDirPartitionResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public AlterReplicaLogDirPartitionResultMessage(BufferReader reader, ApiVersions version)
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
        }
    }
}