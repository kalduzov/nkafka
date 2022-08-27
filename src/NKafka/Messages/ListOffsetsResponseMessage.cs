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

public sealed class ListOffsetsResponseMessage: ResponseMessage
{
    /// <summary>
    /// Each topic in the response.
    /// </summary>
    public List<ListOffsetsTopicResponseMessage> Topics { get; set; } = new ();

    public ListOffsetsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public ListOffsetsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version2)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersions.Version6)
        {
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Topics.Count);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
    }

    public sealed class ListOffsetsTopicResponseMessage: Message
    {
        /// <summary>
        /// The topic name
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Each partition in the response.
        /// </summary>
        public List<ListOffsetsPartitionResponseMessage> Partitions { get; set; } = new ();

        public ListOffsetsTopicResponseMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public ListOffsetsTopicResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersions.Version6)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(Partitions.Count + 1);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Partitions.Count);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
        }
    }

    public sealed class ListOffsetsPartitionResponseMessage: Message
    {
        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The partition error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The result offsets.
        /// </summary>
        public List<long> OldStyleOffsets { get; set; } = new ();

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
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteShort(ErrorCode);
            if (version <= ApiVersions.Version0)
            {
                writer.WriteInt(OldStyleOffsets.Count);
                foreach (var element in OldStyleOffsets)
                {
                    writer.WriteLong(element);
                }
            }
            else
            {
                if (OldStyleOffsets.Count != 0)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default OldStyleOffsets at version {version}");
                }
            }
            if (version >= ApiVersions.Version1)
            {
                writer.WriteLong(Timestamp);
            }
            else
            {
                if (Timestamp != -1)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default Timestamp at version {version}");
                }
            }
            if (version >= ApiVersions.Version1)
            {
                writer.WriteLong(Offset);
            }
            else
            {
                if (Offset != -1)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default Offset at version {version}");
                }
            }
            if (version >= ApiVersions.Version4)
            {
                writer.WriteInt(LeaderEpoch);
            }
            else
            {
                if (LeaderEpoch != -1)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default LeaderEpoch at version {version}");
                }
            }
        }
    }
}
