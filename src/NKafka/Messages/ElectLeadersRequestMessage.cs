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

public sealed class ElectLeadersRequestMessage: IRequestMessage, IEquatable<ElectLeadersRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version2;

    public ApiKeys ApiKey => ApiKeys.ElectLeaders;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// Type of elections to conduct for the partition. A value of '0' elects the preferred replica. A value of '1' elects the first live replica if there are no in-sync replica.
    /// </summary>
    public sbyte ElectionType { get; set; } = 0;

    /// <summary>
    /// The topic partitions to elect leaders.
    /// </summary>
    public TopicPartitionsCollection TopicPartitions { get; set; } = new ();

    /// <summary>
    /// The time in ms to wait for the election to complete.
    /// </summary>
    public int TimeoutMs { get; set; } = 60000;

    public ElectLeadersRequestMessage()
    {
    }

    public ElectLeadersRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        if (version >= ApiVersions.Version1)
        {
            ElectionType = reader.ReadSByte();
        }
        else
        {
            ElectionType = 0;
        }
        {
            if (version >= ApiVersions.Version2)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    TopicPartitions = null;
                }
                else
                {
                    TopicPartitionsCollection newCollection = new(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new TopicPartitionsMessage(reader, version));
                    }
                    TopicPartitions = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    TopicPartitions = null;
                }
                else
                {
                    TopicPartitionsCollection newCollection = new(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new TopicPartitionsMessage(reader, version));
                    }
                    TopicPartitions = newCollection;
                }
            }
        }
        TimeoutMs = reader.ReadInt();
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version2)
        {
            var numTaggedFields = reader.ReadVarUInt();
            for (var t = 0; t < numTaggedFields; t++)
            {
                var tag = reader.ReadVarUInt();
                var size = reader.ReadVarUInt();
                switch (tag)
                {
                    default:
                        UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                        break;
                }
            }
        }
    }

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version1)
        {
            writer.WriteSByte(ElectionType);
        }
        else
        {
            if (ElectionType != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ElectionType at version {version}");
            }
        }
        if (version >= ApiVersions.Version2)
        {
            if (TopicPartitions is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteVarUInt(TopicPartitions.Count + 1);
                foreach (var element in TopicPartitions)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (TopicPartitions is null)
            {
                writer.WriteInt(-1);
            }
            else
            {
                writer.WriteInt(TopicPartitions.Count);
                foreach (var element in TopicPartitions)
                {
                    element.Write(writer, version);
                }
            }
        }
        writer.WriteInt(TimeoutMs);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version2)
        {
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }
        else
        {
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ElectLeadersRequestMessage other && Equals(other);
    }

    public bool Equals(ElectLeadersRequestMessage? other)
    {
        return true;
    }

    public sealed class TopicPartitionsMessage: IMessage, IEquatable<TopicPartitionsMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version2;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of a topic.
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// The partitions of this topic whose leader should be elected.
        /// </summary>
        public List<int> Partitions { get; set; } = new ();

        public TopicPartitionsMessage()
        {
        }

        public TopicPartitionsMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version2)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TopicPartitionsMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Topic was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Topic had invalid length {length}");
                }
                else
                {
                    Topic = reader.ReadString(length);
                }
            }
            {
                int arrayLength;
                if (version >= ApiVersions.Version2)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version2)
            {
                var numTaggedFields = reader.ReadVarUInt();
                for (var t = 0; t < numTaggedFields; t++)
                {
                    var tag = reader.ReadVarUInt();
                    var size = reader.ReadVarUInt();
                    switch (tag)
                    {
                        default:
                            UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                            break;
                    }
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Topic);
                if (version >= ApiVersions.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version2)
            {
                writer.WriteVarUInt(Partitions.Count + 1);
            }
            else
            {
                writer.WriteInt(Partitions.Count);
            }
            foreach (var element in Partitions)
            {
                writer.WriteInt(element);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version2)
            {
                writer.WriteVarUInt(numTaggedFields);
                rawWriter.WriteRawTags(writer, int.MaxValue);
            }
            else
            {
                if (numTaggedFields > 0)
                {
                    throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
                }
            }
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is TopicPartitionsMessage other && Equals(other);
        }

        public bool Equals(TopicPartitionsMessage? other)
        {
            return true;
        }
    }

    public sealed class TopicPartitionsCollection: HashSet<TopicPartitionsMessage>
    {
        public TopicPartitionsCollection()
        {
        }

        public TopicPartitionsCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
