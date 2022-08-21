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

public partial class ListOffsetsResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each topic in the response.
    /// </summary>
    public IReadOnlyCollection<ListOffsetsTopicResponseMessage> Topics { get; set; }

    public ListOffsetsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public ListOffsetsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
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

    public class ListOffsetsTopicResponseMessage: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Each partition in the response.
        /// </summary>
        public IReadOnlyCollection<ListOffsetsPartitionResponseMessage> Partitions { get; set; }

        public ListOffsetsTopicResponseMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public ListOffsetsTopicResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
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
    public class ListOffsetsPartitionResponseMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The partition error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// The result offsets.
        /// </summary>
        public IReadOnlyCollection<long> OldStyleOffsets { get; set; }

        /// <summary>
        /// The timestamp associated with the returned offset.
        /// </summary>
        public long Timestamp { get; set; } = -1;

        /// <summary>
        /// The returned offset.
        /// </summary>
        public long Offset { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        public ListOffsetsPartitionResponseMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public ListOffsetsPartitionResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
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