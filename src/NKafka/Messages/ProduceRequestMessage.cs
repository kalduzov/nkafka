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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class ProduceRequestMessage: RequestMessage
{
    /// <summary>
    /// The transactional ID, or null if the producer is not transactional.
    /// </summary>
    public string? TransactionalId { get; set; } = null;

    /// <summary>
    /// The number of acknowledgments the producer requires the leader to have received before considering a request complete. Allowed values: 0 for no acknowledgments, 1 for only the leader and -1 for the full ISR.
    /// </summary>
    public short Acks { get; set; } = 0;

    /// <summary>
    /// The timeout to await a response in milliseconds.
    /// </summary>
    public int TimeoutMs { get; set; } = 0;

    /// <summary>
    /// Each topic to produce to.
    /// </summary>
    public TopicProduceDataCollection TopicData { get; set; } = new ();

    public ProduceRequestMessage()
    {
        ApiKey = ApiKeys.Produce;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    public ProduceRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.Produce;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version3)
        {
            if (TransactionalId is null)
            {
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteShort(-1);
                }
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(TransactionalId);
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
        }
        else
        {
            if (TransactionalId is not null)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default TransactionalId at version {version}");
            }
        }
        writer.WriteShort(Acks);
        writer.WriteInt(TimeoutMs);
        if (version >= ApiVersions.Version9)
        {
            writer.WriteVarUInt(TopicData.Count + 1);
            foreach (var element in TopicData)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(TopicData.Count);
            foreach (var element in TopicData)
            {
                element.Write(writer, version);
            }
        }
    }

    public sealed class TopicProduceDataMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Each partition to produce to.
        /// </summary>
        public List<PartitionProduceDataMessage> PartitionData { get; set; } = new ();

        public TopicProduceDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public TopicProduceDataMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version9)
            {
                writer.WriteVarUInt(PartitionData.Count + 1);
                foreach (var element in PartitionData)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(PartitionData.Count);
                foreach (var element in PartitionData)
                {
                    element.Write(writer, version);
                }
            }
        }
    }

    public sealed class PartitionProduceDataMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// The record data to be produced.
        /// </summary>
        public RecordBatch? Records { get; set; } = null;

        public PartitionProduceDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public PartitionProduceDataMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(Index);
            if (Records is null)
            {
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteInt(-1);
                }
            }
            else
            {
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(Records.Length + 1);
                }
                else
                {
                    writer.WriteInt(Records.Length);
                }
                writer.WriteRecords(Records);
            }
        }
    }

    public sealed class TopicProduceDataCollection: HashSet<TopicProduceDataMessage>
    {
        public TopicProduceDataCollection()
        {
        }

        public TopicProduceDataCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
