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

public partial class ProduceRequestMessage: RequestMessage
{
    /// <summary>
    /// The transactional ID, or null if the producer is not transactional.
    /// </summary>
    public string? TransactionalId { get; set; } = null;

    /// <summary>
    /// The number of acknowledgments the producer requires the leader to have received before considering a request complete. Allowed values: 0 for no acknowledgments, 1 for only the leader and -1 for the full ISR.
    /// </summary>
    public short Acks { get; set; }

    /// <summary>
    /// The timeout to await a response in milliseconds.
    /// </summary>
    public int TimeoutMs { get; set; }

    /// <summary>
    /// Each topic to produce to.
    /// </summary>
    public IReadOnlyCollection<TopicProduceDataMessage> TopicData { get; set; }

    public ProduceRequestMessage()
    {
        ApiKey = ApiKeys.Produce;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version9)
        {
        }
        else //no flexible version
        {
        }

    }

    public class TopicProduceDataMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Each partition to produce to.
        /// </summary>
        public IReadOnlyCollection<PartitionProduceDataMessage> PartitionData { get; set; }

        public TopicProduceDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version9)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class PartitionProduceDataMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The record data to be produced.
        /// </summary>
        public RecordBatch Records { get; set; }

        public PartitionProduceDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version9)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}