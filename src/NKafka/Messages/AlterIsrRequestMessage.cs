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

public partial class AlterIsrRequestMessage: RequestMessage
{
    /// <summary>
    /// The ID of the requesting broker
    /// </summary>
    public int BrokerId { get; set; }

    /// <summary>
    /// The epoch of the requesting broker
    /// </summary>
    public long BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyCollection<TopicDataMessage> Topics { get; set; }

    public AlterIsrRequestMessage()
    {
        ApiKey = ApiKeys.AlterIsr;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        if (Version >= ApiVersions.Version0)
        {
            if (Topics is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteVarUInt((uint)Topics.Count + 1);
                foreach (var val in Topics)
                {
                    writer.WriteVarInt(val);
                }
            }
        }
        else
        {
        }

    }

    public class TopicDataMessage: Message
    {
        /// <summary>
        /// The name of the topic to alter ISRs for
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<PartitionDataMessage> Partitions { get; set; }

        public TopicDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            if (Version >= ApiVersions.Version0)
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
    public class PartitionDataMessage: Message
    {
        /// <summary>
        /// The partition index
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The leader epoch of this partition
        /// </summary>
        public int LeaderEpoch { get; set; }

        /// <summary>
        /// The ISR for this partition
        /// </summary>
        public IReadOnlyCollection<int> NewIsr { get; set; }

        /// <summary>
        /// The expected version of ISR which is being updated
        /// </summary>
        public int CurrentIsrVersion { get; set; }

        public PartitionDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            if (Version >= ApiVersions.Version0)
            {
                if (NewIsr is null)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteVarUInt((uint)NewIsr.Count + 1);
                    foreach (var val in NewIsr)
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
}