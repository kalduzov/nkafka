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

public partial class WriteTxnMarkersResponseMessage: ResponseMessage
{
    /// <summary>
    /// The results for writing makers.
    /// </summary>
    public IReadOnlyCollection<WritableTxnMarkerResultMessage> Markers { get; set; }

    public WriteTxnMarkersResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public WriteTxnMarkersResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version1)
        {
        }
        else //no flexible version
        {
        }

    }

    public class WritableTxnMarkerResultMessage: Message
    {
        /// <summary>
        /// The current producer ID in use by the transactional ID.
        /// </summary>
        public long ProducerId { get; set; }

        /// <summary>
        /// The results by topic.
        /// </summary>
        public IReadOnlyCollection<WritableTxnMarkerTopicResultMessage> Topics { get; set; }

        public WritableTxnMarkerResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public WritableTxnMarkerResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version1)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class WritableTxnMarkerTopicResultMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The results by partition.
        /// </summary>
        public IReadOnlyCollection<WritableTxnMarkerPartitionResultMessage> Partitions { get; set; }

        public WritableTxnMarkerTopicResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public WritableTxnMarkerTopicResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version1)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class WritableTxnMarkerPartitionResultMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; }

        public WritableTxnMarkerPartitionResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public WritableTxnMarkerPartitionResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version1)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}