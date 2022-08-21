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

public partial class AddPartitionsToTxnResponseMessage: ResponseMessage
{
    /// <summary>
    /// The results for each topic.
    /// </summary>
    public IReadOnlyCollection<AddPartitionsToTxnTopicResultMessage> Results { get; set; }

    public AddPartitionsToTxnResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public AddPartitionsToTxnResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        if (Version >= ApiVersions.Version3)
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

    public class AddPartitionsToTxnTopicResultMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The results for each partition
        /// </summary>
        public IReadOnlyCollection<AddPartitionsToTxnPartitionResultMessage> Results { get; set; }

        public AddPartitionsToTxnTopicResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public AddPartitionsToTxnTopicResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            if (Version >= ApiVersions.Version3)
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
    }
    public class AddPartitionsToTxnPartitionResultMessage: Message
    {
        /// <summary>
        /// The partition indexes.
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The response error code.
        /// </summary>
        public short ErrorCode { get; set; }

        public AddPartitionsToTxnPartitionResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public AddPartitionsToTxnPartitionResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}