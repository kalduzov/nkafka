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

public sealed class LeaderAndIsrRequestMessage: IRequestMessage, IEquatable<LeaderAndIsrRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version6;

    public ApiKeys ApiKey => ApiKeys.LeaderAndIsr;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The current controller ID.
    /// </summary>
    public int ControllerId { get; set; } = 0;

    /// <summary>
    /// The current controller epoch.
    /// </summary>
    public int ControllerEpoch { get; set; } = 0;

    /// <summary>
    /// The current broker epoch.
    /// </summary>
    public long BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// The type that indicates whether all topics are included in the request
    /// </summary>
    public sbyte Type { get; set; } = 0;

    /// <summary>
    /// The state of each partition, in a v0 or v1 message.
    /// </summary>
    public List<LeaderAndIsrPartitionStateMessage> UngroupedPartitionStates { get; set; } = new ();

    /// <summary>
    /// Each topic.
    /// </summary>
    public List<LeaderAndIsrTopicStateMessage> TopicStates { get; set; } = new ();

    /// <summary>
    /// The current live leaders.
    /// </summary>
    public List<LeaderAndIsrLiveLeaderMessage> LiveLeaders { get; set; } = new ();

    public LeaderAndIsrRequestMessage()
    {
    }

    public LeaderAndIsrRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        ControllerId = reader.ReadInt();
        ControllerEpoch = reader.ReadInt();
        if (version >= ApiVersions.Version2)
        {
            BrokerEpoch = reader.ReadLong();
        }
        else
        {
            BrokerEpoch = -1;
        }
        if (version >= ApiVersions.Version5)
        {
            Type = reader.ReadSByte();
        }
        else
        {
            Type = 0;
        }
        if (version <= ApiVersions.Version1)
        {
            int arrayLength;
            arrayLength = reader.ReadInt();
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field UngroupedPartitionStates was serialized as null");
            }
            else
            {
                var newCollection = new List<LeaderAndIsrPartitionStateMessage>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new LeaderAndIsrPartitionStateMessage(reader, version));
                }
                UngroupedPartitionStates = newCollection;
            }
        }
        else
        {
            UngroupedPartitionStates = new ();
        }
        if (version >= ApiVersions.Version2)
        {
            if (version >= ApiVersions.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field TopicStates was serialized as null");
                }
                else
                {
                    var newCollection = new List<LeaderAndIsrTopicStateMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new LeaderAndIsrTopicStateMessage(reader, version));
                    }
                    TopicStates = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field TopicStates was serialized as null");
                }
                else
                {
                    var newCollection = new List<LeaderAndIsrTopicStateMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new LeaderAndIsrTopicStateMessage(reader, version));
                    }
                    TopicStates = newCollection;
                }
            }
        }
        else
        {
            TopicStates = new ();
        }
        {
            if (version >= ApiVersions.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field LiveLeaders was serialized as null");
                }
                else
                {
                    var newCollection = new List<LeaderAndIsrLiveLeaderMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new LeaderAndIsrLiveLeaderMessage(reader, version));
                    }
                    LiveLeaders = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field LiveLeaders was serialized as null");
                }
                else
                {
                    var newCollection = new List<LeaderAndIsrLiveLeaderMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new LeaderAndIsrLiveLeaderMessage(reader, version));
                    }
                    LiveLeaders = newCollection;
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
        writer.WriteInt(ControllerId);
        writer.WriteInt(ControllerEpoch);
        if (version >= ApiVersions.Version2)
        {
            writer.WriteLong(BrokerEpoch);
        }
        if (version >= ApiVersions.Version5)
        {
            writer.WriteSByte(Type);
        }
        else
        {
            if (Type != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Type at version {version}");
            }
        }
        if (version <= ApiVersions.Version1)
        {
            writer.WriteInt(UngroupedPartitionStates.Count);
            foreach (var element in UngroupedPartitionStates)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (UngroupedPartitionStates.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default UngroupedPartitionStates at version {version}");
            }
        }
        if (version >= ApiVersions.Version2)
        {
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(TopicStates.Count + 1);
                foreach (var element in TopicStates)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(TopicStates.Count);
                foreach (var element in TopicStates)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (TopicStates.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default TopicStates at version {version}");
            }
        }
        if (version >= ApiVersions.Version4)
        {
            writer.WriteVarUInt(LiveLeaders.Count + 1);
            foreach (var element in LiveLeaders)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(LiveLeaders.Count);
            foreach (var element in LiveLeaders)
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
        return ReferenceEquals(this, obj) || obj is LeaderAndIsrRequestMessage other && Equals(other);
    }

    public bool Equals(LeaderAndIsrRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ControllerId, ControllerEpoch, BrokerEpoch, Type, UngroupedPartitionStates, TopicStates, LiveLeaders);
        return hashCode;
    }

    public sealed class LeaderAndIsrTopicStateMessage: IMessage, IEquatable<LeaderAndIsrTopicStateMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version6;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The unique topic ID.
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The state of each partition
        /// </summary>
        public List<LeaderAndIsrPartitionStateMessage> PartitionStates { get; set; } = new ();

        public LeaderAndIsrTopicStateMessage()
        {
        }

        public LeaderAndIsrTopicStateMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version6)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of LeaderAndIsrTopicStateMessage");
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
            if (version >= ApiVersions.Version5)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            {
                if (version >= ApiVersions.Version4)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field PartitionStates was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<LeaderAndIsrPartitionStateMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new LeaderAndIsrPartitionStateMessage(reader, version));
                        }
                        PartitionStates = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field PartitionStates was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<LeaderAndIsrPartitionStateMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new LeaderAndIsrPartitionStateMessage(reader, version));
                        }
                        PartitionStates = newCollection;
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
            if (version < ApiVersions.Version2)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of LeaderAndIsrTopicStateMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(TopicName);
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
            if (version >= ApiVersions.Version5)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(PartitionStates.Count + 1);
                foreach (var element in PartitionStates)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(PartitionStates.Count);
                foreach (var element in PartitionStates)
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
            return ReferenceEquals(this, obj) || obj is LeaderAndIsrTopicStateMessage other && Equals(other);
        }

        public bool Equals(LeaderAndIsrTopicStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, TopicName, TopicId, PartitionStates);
            return hashCode;
        }
    }

    public sealed class LeaderAndIsrLiveLeaderMessage: IMessage, IEquatable<LeaderAndIsrLiveLeaderMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version6;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The leader's broker ID.
        /// </summary>
        public int BrokerId { get; set; } = 0;

        /// <summary>
        /// The leader's hostname.
        /// </summary>
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// The leader's port.
        /// </summary>
        public int Port { get; set; } = 0;

        public LeaderAndIsrLiveLeaderMessage()
        {
        }

        public LeaderAndIsrLiveLeaderMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version6)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of LeaderAndIsrLiveLeaderMessage");
            }
            BrokerId = reader.ReadInt();
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
                    throw new Exception("non-nullable field HostName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field HostName had invalid length {length}");
                }
                else
                {
                    HostName = reader.ReadString(length);
                }
            }
            Port = reader.ReadInt();
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
            writer.WriteInt(BrokerId);
            {
                var stringBytes = Encoding.UTF8.GetBytes(HostName);
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
            writer.WriteInt(Port);
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
            return ReferenceEquals(this, obj) || obj is LeaderAndIsrLiveLeaderMessage other && Equals(other);
        }

        public bool Equals(LeaderAndIsrLiveLeaderMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, BrokerId, HostName, Port);
            return hashCode;
        }
    }

    public sealed class LeaderAndIsrPartitionStateMessage: IMessage, IEquatable<LeaderAndIsrPartitionStateMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version6;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.  This is only present in v0 or v1.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The controller epoch.
        /// </summary>
        public int ControllerEpoch { get; set; } = 0;

        /// <summary>
        /// The broker ID of the leader.
        /// </summary>
        public int Leader { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        /// <summary>
        /// The in-sync replica IDs.
        /// </summary>
        public List<int> Isr { get; set; } = new ();

        /// <summary>
        /// The current epoch for the partition. The epoch is a monotonically increasing value which is incremented after every partition change. (Since the LeaderAndIsr request is only used by the legacy controller, this corresponds to the zkVersion)
        /// </summary>
        public int PartitionEpoch { get; set; } = 0;

        /// <summary>
        /// The replica IDs.
        /// </summary>
        public List<int> Replicas { get; set; } = new ();

        /// <summary>
        /// The replica IDs that we are adding this partition to, or null if no replicas are being added.
        /// </summary>
        public List<int> AddingReplicas { get; set; } = new ();

        /// <summary>
        /// The replica IDs that we are removing this partition from, or null if no replicas are being removed.
        /// </summary>
        public List<int> RemovingReplicas { get; set; } = new ();

        /// <summary>
        /// Whether the replica should have existed on the broker or not.
        /// </summary>
        public bool IsNew { get; set; } = false;

        /// <summary>
        /// 1 if the partition is recovering from an unclean leader election; 0 otherwise.
        /// </summary>
        public sbyte LeaderRecoveryState { get; set; } = 0;

        public LeaderAndIsrPartitionStateMessage()
        {
        }

        public LeaderAndIsrPartitionStateMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version6)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of LeaderAndIsrPartitionStateMessage");
            }
            if (version <= ApiVersions.Version1)
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
            else
            {
                TopicName = string.Empty;
            }
            PartitionIndex = reader.ReadInt();
            ControllerEpoch = reader.ReadInt();
            Leader = reader.ReadInt();
            LeaderEpoch = reader.ReadInt();
            {
                int arrayLength;
                if (version >= ApiVersions.Version4)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Isr was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Isr = newCollection;
                }
            }
            PartitionEpoch = reader.ReadInt();
            {
                int arrayLength;
                if (version >= ApiVersions.Version4)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Replicas was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Replicas = newCollection;
                }
            }
            if (version >= ApiVersions.Version3)
            {
                int arrayLength;
                if (version >= ApiVersions.Version4)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field AddingReplicas was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    AddingReplicas = newCollection;
                }
            }
            else
            {
                AddingReplicas = new ();
            }
            if (version >= ApiVersions.Version3)
            {
                int arrayLength;
                if (version >= ApiVersions.Version4)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field RemovingReplicas was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    RemovingReplicas = newCollection;
                }
            }
            else
            {
                RemovingReplicas = new ();
            }
            if (version >= ApiVersions.Version1)
            {
                IsNew = reader.ReadByte() != 0;
            }
            else
            {
                IsNew = false;
            }
            if (version >= ApiVersions.Version6)
            {
                LeaderRecoveryState = reader.ReadSByte();
            }
            else
            {
                LeaderRecoveryState = 0;
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
            if (version <= ApiVersions.Version1)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(TopicName);
                    writer.WriteShort((short)stringBytes.Length);
                    writer.WriteBytes(stringBytes);
                }
            }
            writer.WriteInt(PartitionIndex);
            writer.WriteInt(ControllerEpoch);
            writer.WriteInt(Leader);
            writer.WriteInt(LeaderEpoch);
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(Isr.Count + 1);
            }
            else
            {
                writer.WriteInt(Isr.Count);
            }
            foreach (var element in Isr)
            {
                writer.WriteInt(element);
            }
            writer.WriteInt(PartitionEpoch);
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(Replicas.Count + 1);
            }
            else
            {
                writer.WriteInt(Replicas.Count);
            }
            foreach (var element in Replicas)
            {
                writer.WriteInt(element);
            }
            if (version >= ApiVersions.Version3)
            {
                if (version >= ApiVersions.Version4)
                {
                    writer.WriteVarUInt(AddingReplicas.Count + 1);
                }
                else
                {
                    writer.WriteInt(AddingReplicas.Count);
                }
                foreach (var element in AddingReplicas)
                {
                    writer.WriteInt(element);
                }
            }
            if (version >= ApiVersions.Version3)
            {
                if (version >= ApiVersions.Version4)
                {
                    writer.WriteVarUInt(RemovingReplicas.Count + 1);
                }
                else
                {
                    writer.WriteInt(RemovingReplicas.Count);
                }
                foreach (var element in RemovingReplicas)
                {
                    writer.WriteInt(element);
                }
            }
            if (version >= ApiVersions.Version1)
            {
                writer.WriteBool(IsNew);
            }
            if (version >= ApiVersions.Version6)
            {
                writer.WriteSByte(LeaderRecoveryState);
            }
            else
            {
                if (LeaderRecoveryState != 0)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default LeaderRecoveryState at version {version}");
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
            return ReferenceEquals(this, obj) || obj is LeaderAndIsrPartitionStateMessage other && Equals(other);
        }

        public bool Equals(LeaderAndIsrPartitionStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, TopicName, PartitionIndex, ControllerEpoch, Leader, LeaderEpoch, Isr, PartitionEpoch);
            hashCode = HashCode.Combine(hashCode, Replicas, AddingReplicas, RemovingReplicas, IsNew, LeaderRecoveryState);
            return hashCode;
        }
    }
}
