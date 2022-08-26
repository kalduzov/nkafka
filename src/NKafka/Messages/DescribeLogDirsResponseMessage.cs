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

public sealed class DescribeLogDirsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// The log directories.
    /// </summary>
    public List<DescribeLogDirsResultMessage> ResultsMessage { get; set; } = new ();

    public DescribeLogDirsResponseMessage()
    {
        ApiKey = ApiKeys.DescribeLogDirs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public DescribeLogDirsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeLogDirs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class DescribeLogDirsResultMessage: Message
    {
        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The absolute log directory path.
        /// </summary>
        public string LogDirMessage { get; set; } = "";

        /// <summary>
        /// Each topic.
        /// </summary>
        public List<DescribeLogDirsTopicMessage> TopicsMessage { get; set; } = new ();

        /// <summary>
        /// The total size in bytes of the volume the log directory is in.
        /// </summary>
        public long TotalBytesMessage { get; set; } = -1;

        /// <summary>
        /// The usable size in bytes of the volume the log directory is in.
        /// </summary>
        public long UsableBytesMessage { get; set; } = -1;

        public DescribeLogDirsResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DescribeLogDirsResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class DescribeLogDirsTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string NameMessage { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public List<DescribeLogDirsPartitionMessage> PartitionsMessage { get; set; } = new ();

        public DescribeLogDirsTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DescribeLogDirsTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class DescribeLogDirsPartitionMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndexMessage { get; set; } = 0;

        /// <summary>
        /// The size of the log segments in this partition in bytes.
        /// </summary>
        public long PartitionSizeMessage { get; set; } = 0;

        /// <summary>
        /// The lag of the log's LEO w.r.t. partition's HW (if it is the current log for the partition) or current replica's LEO (if it is the future log for the partition)
        /// </summary>
        public long OffsetLagMessage { get; set; } = 0;

        /// <summary>
        /// True if this log is created by AlterReplicaLogDirsRequest and will replace the current log of the replica in the future.
        /// </summary>
        public bool IsFutureKeyMessage { get; set; } = false;

        public DescribeLogDirsPartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DescribeLogDirsPartitionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
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
