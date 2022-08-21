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

public partial class DescribeLogDirsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The log directories.
    /// </summary>
    public IReadOnlyCollection<DescribeLogDirsResultMessage> Results { get; set; }

    public DescribeLogDirsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public DescribeLogDirsResponseMessage(BufferReader reader, ApiVersions version)
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

    public class DescribeLogDirsResultMessage: Message
    {
        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// The absolute log directory path.
        /// </summary>
        public string LogDir { get; set; }

        /// <summary>
        /// Each topic.
        /// </summary>
        public IReadOnlyCollection<DescribeLogDirsTopicMessage> Topics { get; set; }

        public DescribeLogDirsResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public DescribeLogDirsResultMessage(BufferReader reader, ApiVersions version)
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
    public class DescribeLogDirsTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<DescribeLogDirsPartitionMessage> Partitions { get; set; }

        public DescribeLogDirsTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public DescribeLogDirsTopicMessage(BufferReader reader, ApiVersions version)
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
    public class DescribeLogDirsPartitionMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The size of the log segments in this partition in bytes.
        /// </summary>
        public long PartitionSize { get; set; }

        /// <summary>
        /// The lag of the log's LEO w.r.t. partition's HW (if it is the current log for the partition) or current replica's LEO (if it is the future log for the partition)
        /// </summary>
        public long OffsetLag { get; set; }

        /// <summary>
        /// True if this log is created by AlterReplicaLogDirsRequest and will replace the current log of the replica in the future.
        /// </summary>
        public bool IsFutureKey { get; set; }

        public DescribeLogDirsPartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public DescribeLogDirsPartitionMessage(BufferReader reader, ApiVersions version)
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