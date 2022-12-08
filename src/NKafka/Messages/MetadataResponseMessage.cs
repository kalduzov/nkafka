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

// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using System.Text;

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;

namespace NKafka.Messages;

public sealed class MetadataResponseMessage: IResponseMessage, IEquatable<MetadataResponseMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version12;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// Each broker in the response.
    /// </summary>
    public MetadataResponseBrokerCollection Brokers { get; set; } = new();

    /// <summary>
    /// The cluster ID that responding broker belongs to.
    /// </summary>
    public string? ClusterId { get; set; } = null;

    /// <summary>
    /// The ID of the controller broker.
    /// </summary>
    public int ControllerId { get; set; } = -1;

    /// <summary>
    /// Each topic in the response.
    /// </summary>
    public MetadataResponseTopicCollection Topics { get; set; } = new();

    /// <summary>
    /// 32-bit bitfield to represent authorized operations for this cluster.
    /// </summary>
    public int ClusterAuthorizedOperations { get; set; } = -2147483648;

    public MetadataResponseMessage()
    {
    }

    public MetadataResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        if (version >= ApiVersion.Version3)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        {
            if (version >= ApiVersion.Version9)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Brokers was serialized as null");
                }
                else
                {
                    var newCollection = new MetadataResponseBrokerCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new MetadataResponseBrokerMessage(reader, version));
                    }
                    Brokers = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Brokers was serialized as null");
                }
                else
                {
                    var newCollection = new MetadataResponseBrokerCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new MetadataResponseBrokerMessage(reader, version));
                    }
                    Brokers = newCollection;
                }
            }
        }
        if (version >= ApiVersion.Version2)
        {
            int length;
            if (version >= ApiVersion.Version9)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                ClusterId = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ClusterId had invalid length {length}");
            }
            else
            {
                ClusterId = reader.ReadString(length);
            }
        }
        else
        {
            ClusterId = null;
        }
        if (version >= ApiVersion.Version1)
        {
            ControllerId = reader.ReadInt();
        }
        else
        {
            ControllerId = -1;
        }
        {
            if (version >= ApiVersion.Version9)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new MetadataResponseTopicCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new MetadataResponseTopicMessage(reader, version));
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
                    var newCollection = new MetadataResponseTopicCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new MetadataResponseTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        if (version >= ApiVersion.Version8 && version <= ApiVersion.Version10)
        {
            ClusterAuthorizedOperations = reader.ReadInt();
        }
        else
        {
            ClusterAuthorizedOperations = -2147483648;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version9)
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

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersion.Version3)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersion.Version9)
        {
            writer.WriteVarUInt(Brokers.Count + 1);
            foreach (var element in Brokers)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Brokers.Count);
            foreach (var element in Brokers)
            {
                element.Write(writer, version);
            }
        }
        if (version >= ApiVersion.Version2)
        {
            if (ClusterId is null)
            {
                if (version >= ApiVersion.Version9)
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
                var stringBytes = Encoding.UTF8.GetBytes(ClusterId);
                if (version >= ApiVersion.Version9)
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
        if (version >= ApiVersion.Version1)
        {
            writer.WriteInt(ControllerId);
        }
        if (version >= ApiVersion.Version9)
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
        if (version >= ApiVersion.Version8 && version <= ApiVersion.Version10)
        {
            writer.WriteInt(ClusterAuthorizedOperations);
        }
        else
        {
            if (ClusterAuthorizedOperations != -2147483648)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ClusterAuthorizedOperations at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version9)
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
        return ReferenceEquals(this, obj) || obj is MetadataResponseMessage other && Equals(other);
    }

    public bool Equals(MetadataResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, Brokers, ClusterId, ControllerId, Topics, ClusterAuthorizedOperations);
        return hashCode;
    }

    public override string ToString()
    {
        return "MetadataResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ControllerId=" + ControllerId
            + ", ClusterAuthorizedOperations=" + ClusterAuthorizedOperations
            + ")";
    }

    public sealed class MetadataResponseBrokerMessage: IMessage, IEquatable<MetadataResponseBrokerMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version12;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The broker ID.
        /// </summary>
        public int NodeId { get; set; } = 0;

        /// <summary>
        /// The broker hostname.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The broker port.
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>
        /// The rack of the broker, or null if it has not been assigned to a rack.
        /// </summary>
        public string? Rack { get; set; } = null;

        public MetadataResponseBrokerMessage()
        {
        }

        public MetadataResponseBrokerMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version12)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of MetadataResponseBrokerMessage");
            }
            NodeId = reader.ReadInt();
            {
                int length;
                if (version >= ApiVersion.Version9)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Host was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Host had invalid length {length}");
                }
                else
                {
                    Host = reader.ReadString(length);
                }
            }
            Port = reader.ReadInt();
            if (version >= ApiVersion.Version1)
            {
                int length;
                if (version >= ApiVersion.Version9)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    Rack = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Rack had invalid length {length}");
                }
                else
                {
                    Rack = reader.ReadString(length);
                }
            }
            else
            {
                Rack = null;
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version9)
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(NodeId);
            {
                var stringBytes = Encoding.UTF8.GetBytes(Host);
                if (version >= ApiVersion.Version9)
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
            if (version >= ApiVersion.Version1)
            {
                if (Rack is null)
                {
                    if (version >= ApiVersion.Version9)
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
                    var stringBytes = Encoding.UTF8.GetBytes(Rack);
                    if (version >= ApiVersion.Version9)
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
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version9)
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
            return ReferenceEquals(this, obj) || obj is MetadataResponseBrokerMessage other && Equals(other);
        }

        public bool Equals(MetadataResponseBrokerMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, NodeId);
            return hashCode;
        }

        public override string ToString()
        {
            return "MetadataResponseBrokerMessage("
                + "NodeId=" + NodeId
                + ", Port=" + Port
                + ")";
        }
    }

    public sealed class MetadataResponseBrokerCollection: HashSet<MetadataResponseBrokerMessage>
    {
        public MetadataResponseBrokerCollection()
        {
        }

        public MetadataResponseBrokerCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class MetadataResponseTopicMessage: IMessage, IEquatable<MetadataResponseTopicMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version12;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic error, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The topic id.
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// True if the topic is internal.
        /// </summary>
        public bool IsInternal { get; set; } = false;

        /// <summary>
        /// Each partition in the topic.
        /// </summary>
        public List<MetadataResponsePartitionMessage> Partitions { get; set; } = new();

        /// <summary>
        /// 32-bit bitfield to represent authorized operations for this topic.
        /// </summary>
        public int TopicAuthorizedOperations { get; set; } = -2147483648;

        public MetadataResponseTopicMessage()
        {
        }

        public MetadataResponseTopicMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version12)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of MetadataResponseTopicMessage");
            }
            ErrorCode = reader.ReadShort();
            {
                int length;
                if (version >= ApiVersion.Version9)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    if (version >= ApiVersion.Version12)
                    {
                        Name = null;
                    }
                    else
                    {
                        throw new Exception("non-nullable field Name was serialized as null");
                    }
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
            if (version >= ApiVersion.Version10)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            if (version >= ApiVersion.Version1)
            {
                IsInternal = reader.ReadByte() != 0;
            }
            else
            {
                IsInternal = false;
            }
            {
                if (version >= ApiVersion.Version9)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<MetadataResponsePartitionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new MetadataResponsePartitionMessage(reader, version));
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
                        var newCollection = new List<MetadataResponsePartitionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new MetadataResponsePartitionMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
            }
            if (version >= ApiVersion.Version8)
            {
                TopicAuthorizedOperations = reader.ReadInt();
            }
            else
            {
                TopicAuthorizedOperations = -2147483648;
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version9)
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteShort((short)ErrorCode);
            if (Name is null)
            {
                if (version >= ApiVersion.Version12)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersion.Version9)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version10)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersion.Version1)
            {
                writer.WriteBool(IsInternal);
            }
            if (version >= ApiVersion.Version9)
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
            if (version >= ApiVersion.Version8)
            {
                writer.WriteInt(TopicAuthorizedOperations);
            }
            else
            {
                if (TopicAuthorizedOperations != -2147483648)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default TopicAuthorizedOperations at version {version}");
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version9)
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
            return ReferenceEquals(this, obj) || obj is MetadataResponseTopicMessage other && Equals(other);
        }

        public bool Equals(MetadataResponseTopicMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }

        public override string ToString()
        {
            return "MetadataResponseTopicMessage("
                + "ErrorCode=" + ErrorCode
                + ", TopicId=" + TopicId
                + ", IsInternal=" + (IsInternal ? "true" : "false")
                + ", TopicAuthorizedOperations=" + TopicAuthorizedOperations
                + ")";
        }
    }

    public sealed class MetadataResponsePartitionMessage: IMessage, IEquatable<MetadataResponsePartitionMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version12;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition error, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The ID of the leader broker.
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The leader epoch of this partition.
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The set of all nodes that host this partition.
        /// </summary>
        public List<int> ReplicaNodes { get; set; } = new();

        /// <summary>
        /// The set of nodes that are in sync with the leader for this partition.
        /// </summary>
        public List<int> IsrNodes { get; set; } = new();

        /// <summary>
        /// The set of offline replicas of this partition.
        /// </summary>
        public List<int> OfflineReplicas { get; set; } = new();

        public MetadataResponsePartitionMessage()
        {
        }

        public MetadataResponsePartitionMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version12)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of MetadataResponsePartitionMessage");
            }
            ErrorCode = reader.ReadShort();
            PartitionIndex = reader.ReadInt();
            LeaderId = reader.ReadInt();
            if (version >= ApiVersion.Version7)
            {
                LeaderEpoch = reader.ReadInt();
            }
            else
            {
                LeaderEpoch = -1;
            }
            {
                int arrayLength;
                if (version >= ApiVersion.Version9)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field ReplicaNodes was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    ReplicaNodes = newCollection;
                }
            }
            {
                int arrayLength;
                if (version >= ApiVersion.Version9)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field IsrNodes was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    IsrNodes = newCollection;
                }
            }
            if (version >= ApiVersion.Version5)
            {
                int arrayLength;
                if (version >= ApiVersion.Version9)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field OfflineReplicas was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    OfflineReplicas = newCollection;
                }
            }
            else
            {
                OfflineReplicas = new();
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version9)
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteShort((short)ErrorCode);
            writer.WriteInt(PartitionIndex);
            writer.WriteInt(LeaderId);
            if (version >= ApiVersion.Version7)
            {
                writer.WriteInt(LeaderEpoch);
            }
            if (version >= ApiVersion.Version9)
            {
                writer.WriteVarUInt(ReplicaNodes.Count + 1);
            }
            else
            {
                writer.WriteInt(ReplicaNodes.Count);
            }
            foreach (var element in ReplicaNodes)
            {
                writer.WriteInt(element);
            }
            if (version >= ApiVersion.Version9)
            {
                writer.WriteVarUInt(IsrNodes.Count + 1);
            }
            else
            {
                writer.WriteInt(IsrNodes.Count);
            }
            foreach (var element in IsrNodes)
            {
                writer.WriteInt(element);
            }
            if (version >= ApiVersion.Version5)
            {
                if (version >= ApiVersion.Version9)
                {
                    writer.WriteVarUInt(OfflineReplicas.Count + 1);
                }
                else
                {
                    writer.WriteInt(OfflineReplicas.Count);
                }
                foreach (var element in OfflineReplicas)
                {
                    writer.WriteInt(element);
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version9)
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
            return ReferenceEquals(this, obj) || obj is MetadataResponsePartitionMessage other && Equals(other);
        }

        public bool Equals(MetadataResponsePartitionMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ErrorCode, PartitionIndex, LeaderId, LeaderEpoch, ReplicaNodes, IsrNodes, OfflineReplicas);
            return hashCode;
        }

        public override string ToString()
        {
            return "MetadataResponsePartitionMessage("
                + "ErrorCode=" + ErrorCode
                + ", PartitionIndex=" + PartitionIndex
                + ", LeaderId=" + LeaderId
                + ", LeaderEpoch=" + LeaderEpoch
                + ")";
        }
    }

    public sealed class MetadataResponseTopicCollection: HashSet<MetadataResponseTopicMessage>
    {
        public MetadataResponseTopicCollection()
        {
        }

        public MetadataResponseTopicCollection(int capacity)
            : base(capacity)
        {
        }
    }
}