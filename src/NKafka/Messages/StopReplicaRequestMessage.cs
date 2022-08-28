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

public sealed class StopReplicaRequestMessage: IRequestMessage, IEquatable<StopReplicaRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

    public ApiKeys ApiKey => ApiKeys.StopReplica;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The controller id.
    /// </summary>
    public int ControllerId { get; set; } = 0;

    /// <summary>
    /// The controller epoch.
    /// </summary>
    public int ControllerEpoch { get; set; } = 0;

    /// <summary>
    /// The broker epoch.
    /// </summary>
    public long BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// Whether these partitions should be deleted.
    /// </summary>
    public bool DeletePartitions { get; set; } = false;

    /// <summary>
    /// The partitions to stop.
    /// </summary>
    public List<StopReplicaPartitionV0Message> UngroupedPartitions { get; set; } = new ();

    /// <summary>
    /// The topics to stop.
    /// </summary>
    public List<StopReplicaTopicV1Message> Topics { get; set; } = new ();

    /// <summary>
    /// Each topic.
    /// </summary>
    public List<StopReplicaTopicStateMessage> TopicStates { get; set; } = new ();

    public StopReplicaRequestMessage()
    {
    }

    public StopReplicaRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        ControllerId = reader.ReadInt();
        ControllerEpoch = reader.ReadInt();
        if (version >= ApiVersions.Version1)
        {
            BrokerEpoch = reader.ReadLong();
        }
        else
        {
            BrokerEpoch = -1;
        }
        if (version <= ApiVersions.Version2)
        {
            DeletePartitions = reader.ReadByte() != 0;
        }
        else
        {
            DeletePartitions = false;
        }
        if (version <= ApiVersions.Version0)
        {
            int arrayLength;
            arrayLength = reader.ReadInt();
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field UngroupedPartitions was serialized as null");
            }
            else
            {
                var newCollection = new List<StopReplicaPartitionV0Message>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new StopReplicaPartitionV0Message(reader, version));
                }
                UngroupedPartitions = newCollection;
            }
        }
        else
        {
            UngroupedPartitions = new ();
        }
        if (version >= ApiVersions.Version1 && version <= ApiVersions.Version2)
        {
            if (version >= ApiVersions.Version2)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new List<StopReplicaTopicV1Message>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new StopReplicaTopicV1Message(reader, version));
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
                    var newCollection = new List<StopReplicaTopicV1Message>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new StopReplicaTopicV1Message(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        else
        {
            Topics = new ();
        }
        if (version >= ApiVersions.Version3)
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field TopicStates was serialized as null");
            }
            else
            {
                var newCollection = new List<StopReplicaTopicStateMessage>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new StopReplicaTopicStateMessage(reader, version));
                }
                TopicStates = newCollection;
            }
        }
        else
        {
            TopicStates = new ();
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
        writer.WriteInt(ControllerId);
        writer.WriteInt(ControllerEpoch);
        if (version >= ApiVersions.Version1)
        {
            writer.WriteLong(BrokerEpoch);
        }
        if (version <= ApiVersions.Version2)
        {
            writer.WriteBool(DeletePartitions);
        }
        else
        {
            if (DeletePartitions)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default DeletePartitions at version {version}");
            }
        }
        if (version <= ApiVersions.Version0)
        {
            writer.WriteInt(UngroupedPartitions.Count);
            foreach (var element in UngroupedPartitions)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (UngroupedPartitions.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default UngroupedPartitions at version {version}");
            }
        }
        if (version >= ApiVersions.Version1 && version <= ApiVersions.Version2)
        {
            if (version >= ApiVersions.Version2)
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
        else
        {
            if (Topics.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Topics at version {version}");
            }
        }
        if (version >= ApiVersions.Version3)
        {
            writer.WriteVarUInt(TopicStates.Count + 1);
            foreach (var element in TopicStates)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (TopicStates.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default TopicStates at version {version}");
            }
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
        return ReferenceEquals(this, obj) || obj is StopReplicaRequestMessage other && Equals(other);
    }

    public bool Equals(StopReplicaRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ControllerId, ControllerEpoch, BrokerEpoch, DeletePartitions, UngroupedPartitions, Topics, TopicStates);
        return hashCode;
    }

    public override string ToString()
    {
        return "StopReplicaRequestMessage("
            + "ControllerId=" + ControllerId
            + ", ControllerEpoch=" + ControllerEpoch
            + ", BrokerEpoch=" + BrokerEpoch
            + ", DeletePartitions=" + (DeletePartitions ? "true" : "false")
            + ")";
    }

    public sealed class StopReplicaPartitionV0Message: IMessage, IEquatable<StopReplicaPartitionV0Message>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        public StopReplicaPartitionV0Message()
        {
        }

        public StopReplicaPartitionV0Message(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            {
                int length;
                length = reader.ReadShort();
                if (length < 0)
                {
                    throw new Exception("non-nullable field TopicName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field TopicName had invalid length {length}");
                }
                else
                {
                    TopicName = reader.ReadString(length);
                }
            }
            PartitionIndex = reader.ReadInt();
            UnknownTaggedFields = null;
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of StopReplicaPartitionV0Message");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(TopicName);
                writer.WriteShort((short)stringBytes.Length);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteInt(PartitionIndex);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is StopReplicaPartitionV0Message other && Equals(other);
        }

        public bool Equals(StopReplicaPartitionV0Message? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, TopicName, PartitionIndex);
            return hashCode;
        }

        public override string ToString()
        {
            return "StopReplicaPartitionV0Message("
                + ", PartitionIndex=" + PartitionIndex
                + ")";
        }
    }

    public sealed class StopReplicaTopicV1Message: IMessage, IEquatable<StopReplicaTopicV1Message>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The partition indexes.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new ();

        public StopReplicaTopicV1Message()
        {
        }

        public StopReplicaTopicV1Message(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
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
                    throw new Exception("non-nullable field Name was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Name had invalid length {length}");
                }
                else
                {
                    Name = reader.ReadString(length);
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
                    throw new Exception("non-nullable field PartitionIndexes was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    PartitionIndexes = newCollection;
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
            if (version < ApiVersions.Version1 || version > ApiVersions.Version2)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of StopReplicaTopicV1Message");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
                writer.WriteVarUInt(PartitionIndexes.Count + 1);
            }
            else
            {
                writer.WriteInt(PartitionIndexes.Count);
            }
            foreach (var element in PartitionIndexes)
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
            return ReferenceEquals(this, obj) || obj is StopReplicaTopicV1Message other && Equals(other);
        }

        public bool Equals(StopReplicaTopicV1Message? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name, PartitionIndexes);
            return hashCode;
        }

        public override string ToString()
        {
            return "StopReplicaTopicV1Message("
                + ")";
        }
    }

    public sealed class StopReplicaTopicStateMessage: IMessage, IEquatable<StopReplicaTopicStateMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The state of each partition
        /// </summary>
        public List<StopReplicaPartitionStateMessage> PartitionStates { get; set; } = new ();

        public StopReplicaTopicStateMessage()
        {
        }

        public StopReplicaTopicStateMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of StopReplicaTopicStateMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field TopicName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field TopicName had invalid length {length}");
                }
                else
                {
                    TopicName = reader.ReadString(length);
                }
            }
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field PartitionStates was serialized as null");
                }
                else
                {
                    var newCollection = new List<StopReplicaPartitionStateMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new StopReplicaPartitionStateMessage(reader, version));
                    }
                    PartitionStates = newCollection;
                }
            }
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of StopReplicaTopicStateMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(TopicName);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteVarUInt(PartitionStates.Count + 1);
            foreach (var element in PartitionStates)
            {
                element.Write(writer, version);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is StopReplicaTopicStateMessage other && Equals(other);
        }

        public bool Equals(StopReplicaTopicStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, TopicName, PartitionStates);
            return hashCode;
        }

        public override string ToString()
        {
            return "StopReplicaTopicStateMessage("
                + ")";
        }
    }

    public sealed class StopReplicaPartitionStateMessage: IMessage, IEquatable<StopReplicaPartitionStateMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version3;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        /// <summary>
        /// Whether this partition should be deleted.
        /// </summary>
        public bool DeletePartition { get; set; } = false;

        public StopReplicaPartitionStateMessage()
        {
        }

        public StopReplicaPartitionStateMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of StopReplicaPartitionStateMessage");
            }
            PartitionIndex = reader.ReadInt();
            LeaderEpoch = reader.ReadInt();
            DeletePartition = reader.ReadByte() != 0;
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteInt(LeaderEpoch);
            writer.WriteBool(DeletePartition);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is StopReplicaPartitionStateMessage other && Equals(other);
        }

        public bool Equals(StopReplicaPartitionStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex, LeaderEpoch, DeletePartition);
            return hashCode;
        }

        public override string ToString()
        {
            return "StopReplicaPartitionStateMessage("
                + "PartitionIndex=" + PartitionIndex
                + ", LeaderEpoch=" + LeaderEpoch
                + ", DeletePartition=" + (DeletePartition ? "true" : "false")
                + ")";
        }
    }
}
