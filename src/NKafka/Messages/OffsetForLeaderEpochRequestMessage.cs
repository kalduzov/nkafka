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

public sealed class OffsetForLeaderEpochRequestMessage: IRequestMessage, IEquatable<OffsetForLeaderEpochRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version4;

    public ApiKeys ApiKey => ApiKeys.OffsetForLeaderEpoch;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The broker ID of the follower, of -1 if this request is from a consumer.
    /// </summary>
    public int ReplicaId { get; set; } = -2;

    /// <summary>
    /// Each topic to get offsets for.
    /// </summary>
    public OffsetForLeaderTopicCollection Topics { get; set; } = new ();

    public OffsetForLeaderEpochRequestMessage()
    {
    }

    public OffsetForLeaderEpochRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        if (version >= ApiVersions.Version3)
        {
            ReplicaId = reader.ReadInt();
        }
        else
        {
            ReplicaId = -2;
        }
        {
            if (version >= ApiVersions.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    OffsetForLeaderTopicCollection newCollection = new(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetForLeaderTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    OffsetForLeaderTopicCollection newCollection = new(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetForLeaderTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version4)
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
        if (version >= ApiVersions.Version3)
        {
            writer.WriteInt(ReplicaId);
        }
        if (version >= ApiVersions.Version4)
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
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version4)
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
        return ReferenceEquals(this, obj) || obj is OffsetForLeaderEpochRequestMessage other && Equals(other);
    }

    public bool Equals(OffsetForLeaderEpochRequestMessage? other)
    {
        return true;
    }

    public sealed class OffsetForLeaderTopicMessage: IMessage, IEquatable<OffsetForLeaderTopicMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version4;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// Each partition to get offsets for.
        /// </summary>
        public List<OffsetForLeaderPartitionMessage> Partitions { get; set; } = new ();

        public OffsetForLeaderTopicMessage()
        {
        }

        public OffsetForLeaderTopicMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version4)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetForLeaderTopicMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version4)
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
                if (version >= ApiVersions.Version4)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<OffsetForLeaderPartitionMessage>(arrayLength);
                        for (var i = 0; i< arrayLength; i++)
                        {
                            newCollection.Add(new OffsetForLeaderPartitionMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<OffsetForLeaderPartitionMessage>(arrayLength);
                        for (var i = 0; i< arrayLength; i++)
                        {
                            newCollection.Add(new OffsetForLeaderPartitionMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version4)
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
                if (version >= ApiVersions.Version4)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version4)
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
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version4)
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
            return ReferenceEquals(this, obj) || obj is OffsetForLeaderTopicMessage other && Equals(other);
        }

        public bool Equals(OffsetForLeaderTopicMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetForLeaderPartitionMessage: IMessage, IEquatable<OffsetForLeaderPartitionMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version4;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int Partition { get; set; } = 0;

        /// <summary>
        /// An epoch used to fence consumers/replicas with old metadata. If the epoch provided by the client is larger than the current epoch known to the broker, then the UNKNOWN_LEADER_EPOCH error code will be returned. If the provided epoch is smaller, then the FENCED_LEADER_EPOCH error code will be returned.
        /// </summary>
        public int CurrentLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The epoch to look up an offset for.
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        public OffsetForLeaderPartitionMessage()
        {
        }

        public OffsetForLeaderPartitionMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version4)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetForLeaderPartitionMessage");
            }
            Partition = reader.ReadInt();
            if (version >= ApiVersions.Version2)
            {
                CurrentLeaderEpoch = reader.ReadInt();
            }
            else
            {
                CurrentLeaderEpoch = -1;
            }
            LeaderEpoch = reader.ReadInt();
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version4)
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
            writer.WriteInt(Partition);
            if (version >= ApiVersions.Version2)
            {
                writer.WriteInt(CurrentLeaderEpoch);
            }
            writer.WriteInt(LeaderEpoch);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version4)
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
            return ReferenceEquals(this, obj) || obj is OffsetForLeaderPartitionMessage other && Equals(other);
        }

        public bool Equals(OffsetForLeaderPartitionMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetForLeaderTopicCollection: HashSet<OffsetForLeaderTopicMessage>
    {
        public OffsetForLeaderTopicCollection()
        {
        }

        public OffsetForLeaderTopicCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
